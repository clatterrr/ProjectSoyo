using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Walker : MonoBehaviour
{
    // Start is called before the first frame update

    private float moveSpeed = 5.0f;
    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件
    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // 血量条相对于物体的位置偏移

    private GameObject healthBarInstance;
    private RectTransform redBarRectTransform; // 红色血量条的 RectTransform
    private float maxHealth = 100f;
    private float currentHealth;

    public bool positive = true;
    private bool FirstDead = true;

    void Start()
    {
        currentHealth = maxHealth;
        // 在 Canvas 上生成 HealthBar 实例
        healthBarInstance = Instantiate(healthBarPrefab, uiCanvas.transform);

        healthBarInstance.GetComponent<Image>().color = Color.white;
        healthBarInstance.transform.Find("RedBar").GetComponent<Image>().color = Color.red;
        redBarRectTransform = healthBarInstance.transform.Find("RedBar").GetComponent<RectTransform>();
        moveSpeed = Random.Range(3, 15);
        UpdateHealthBar();
        SetAllRigidbodiesKinematic(gameObject, true);
        SelectAnimation("CrouchedWalking");
        FirstDead = true;

    }

    void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;
        // 根据健康比例调整红色血量条的宽度
        if (redBarRectTransform != null)
        {
            redBarRectTransform.localScale = new Vector3(healthPercentage, 1f, 1f);
            redBarRectTransform.localPosition = new Vector3(-50f * (1 - healthPercentage), 0, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private GameObject weapon;
    public void TakeWeapon(GameObject weaponPrefab)
    {
        Transform rightHand = FindChildRecursive(gameObject.transform, "mixamorig:RightHand");
        weapon = Instantiate(weaponPrefab, rightHand.position,  Quaternion.Euler(14,-6,154));
        weapon.transform.localScale = transform.localScale * 50;
        
    }

    // Update is called once per frame
    public bool isDead()
    {
        return currentHealth <= 0;
    }

    public bool isAlive()
    {
        return currentHealth > 0;
    }
    void UpdateHealth()
    {
        if (!isDead())
        {
            float ratio = currentHealth / maxHealth;
            if (healthBarInstance != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
                healthBarInstance.transform.position = screenPosition;
            }
        }

        if (currentHealth <= 0f && FirstDead)
        {
            FirstDead = false;
            // 销毁健康条实例
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }
            gameObject.GetComponent<Animator>().enabled = false;
            SetAllRigidbodiesKinematic(gameObject, false);
            AddForce(new Vector3(0, 300, 0), "mixamorig:Hips", ForceMode.Impulse);
        }

    }

    public void AddForce(Vector3 forceDirection, string targetBoneName, ForceMode forceMode)
    {
        Transform targetBone = FindChildRecursive(gameObject.transform, targetBoneName);
        Rigidbody rb = targetBone.GetComponent<Rigidbody>();
        rb.AddForce(forceDirection, forceMode);
    }

    Transform FindChildRecursive(Transform parent, string targetBoneName)
    {
        // 检查当前物体是否匹配目标名称
        if (parent.name == targetBoneName)
        {
            return parent;
        }

        // 遍历当前物体的所有子物体
        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, targetBoneName);
            if (result != null)
            {
                return result;
            }
        }

        // 如果没有找到匹配的子物体，返回 null
        return null;
    }

    void SetAllRigidbodiesKinematic(GameObject obj, bool state)
    {
        // 获取当前物体的Rigidbody组件（如果有的话）
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = state;
        }

        // 递归查找并处理子物体中的Rigidbody
        foreach (Transform child in obj.transform)
        {
            SetAllRigidbodiesKinematic(child.gameObject, state);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateHealth();
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        if(transform.position.x > 100)
        {
            TakeDamage(100);
        }
    }

    private string animationName;
    void SelectAnimation(string path)
    {
        if (path != animationName)
        {
            animationName = path;
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/FightAnimations/" + path + ".fbx");
            AnimatorController controller = new AnimatorController();
            controller.AddLayer("Base Layer");
            AnimatorState state = controller.layers[0].stateMachine.AddState("Default State");
            state.motion = clip;
            GetComponent<Animator>().runtimeAnimatorController = controller;
        }

    }

}
