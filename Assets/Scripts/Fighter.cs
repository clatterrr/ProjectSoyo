using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    
    private AnimationClip clip;

    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件
    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // 血量条相对于物体的位置偏移

    private GameObject healthBarInstance;
    private RectTransform redBarRectTransform; // 红色血量条的 RectTransform
    private Image healthBarImage;
    private float maxHealth = 100f;
    private float currentHealth;

    void SelectRandomClip()
    {
        // 查找 Animations 文件夹中的所有 .anim 文件
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Animations" });

        if (guids.Length > 0)
        {
            // 随机选择一个 .anim 文件
            int randomIndex = Random.Range(0, guids.Length);
            string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
            path = "Assets/Animations/BeastWalk.anim";
            // 加载 AnimationClip 并赋值
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);


            if (clip != null)
            {
                Debug.Log($"Assigned AnimationClip: {clip.name} from {path}");
            }
            else
            {
                Debug.LogWarning("Failed to load AnimationClip.");
            }
        }
        else
        {
            Debug.LogWarning("No .anim files found in the Animations folder.");
        }
    }

    AnimatorController CreateDefaultAnimatorController()
    {
        // 动态创建一个新的 AnimatorController
        var animatorController = new AnimatorController();

        // 创建一个默认的层和状态机
        animatorController.AddLayer("Base Layer");
        var rootStateMachine = animatorController.layers[0].stateMachine;

        // 创建一个空的默认状态
        var idleState = rootStateMachine.AddState("Idle");

        // 可以在这里为 Idle 状态设置动画剪辑（可选）
        // AnimationClip idleClip = new AnimationClip();
        // idleState.motion = idleClip;

        return animatorController;
    }

    void ConnectClip()
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on this GameObject.");
            return;
        }

        // 获取 Animator 的 Controller
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            animatorController = CreateDefaultAnimatorController();
            animator.runtimeAnimatorController = animatorController;
            Debug.Log("A new AnimatorController has been created and assigned.");
        }

        // 检查当前 AnimationClip 是否有效
        if (clip == null)
        {
            Debug.LogError("No AnimationClip assigned.");
            return;
        }
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // 查找或创建与当前 AnimationClip 对应的 AnimatorState
        AnimatorState newState = null;
        foreach (var layer in animatorController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.motion == clip)
                {
                    newState = state.state;
                    break;
                }
            }
            if (newState != null) break;
        }

        if (newState == null)
        {
            // 如果未找到对应的状态，则创建一个新的状态
            newState = animatorController.layers[0].stateMachine.AddState(clip.name);
            newState.motion = clip;
        }

        // 设置为默认状态
        animatorController.layers[0].stateMachine.defaultState = newState;
    }
    void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;

        // 根据健康比例调整红色血量条的宽度
        if(redBarRectTransform != null)
        {
            redBarRectTransform.localScale = new Vector3(healthPercentage, 1f, 1f);
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
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

    private Vector3 redBarPos;
    void Start()
    {

        SelectRandomClip();
        ConnectClip();
        SetAllRigidbodiesKinematic(gameObject, true);

        currentHealth = maxHealth;
        // 在 Canvas 上生成 HealthBar 实例
        healthBarInstance = Instantiate(healthBarPrefab, uiCanvas.transform);

        healthBarInstance.GetComponent<Image>().color = Color.white;
        healthBarInstance.transform.Find("RedBar").GetComponent<Image>().color = Color.red;
        redBarPos = healthBarInstance.transform.Find("RedBar").localPosition;
        redBarRectTransform = healthBarInstance.transform.Find("RedBar").GetComponent<RectTransform>();

        UpdateHealthBar();
    }

    public float moveSpeed = 5f;

    public Vector3 moveDirection = Vector3.right; // 移动的方向，默认为前方向

    public bool isDead()
    {
        return currentHealth <= 0;
    }

    bool FirstDead = true;
    void UpdateHealth()
    {
        if(!isDead())
        {

            float ratio = currentHealth / maxHealth;
            if (healthBarInstance != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
                healthBarInstance.transform.position = screenPosition;
            }
        }

        if(currentHealth <= 0f && FirstDead)
        {
            FirstDead = false;
            // 销毁健康条实例
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }
            gameObject.GetComponent<Animator>().enabled = false;
            SetAllRigidbodiesKinematic(gameObject, false);
            AddForce(new Vector3(0,300,0), "mixamorig:Hips", ForceMode.Impulse);
        }

    }
    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        //FindFighter();
        transform.rotation = Quaternion.Euler(0, 90, 0);
        if (!isDead())
        {

            Vector3 movement = moveDirection.normalized * moveSpeed * Time.deltaTime;

            // 移动物体
            transform.Translate(movement, Space.World);
        }
        // 更新 TextMeshPro 的位置，使其始终位于 GameObject 的头顶


        // 示例：按下空格键扣除生命值
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10f);

        }
        if(transform.position.x > 110)
        {
            TakeDamage(100f);
        }
    }

    public void AddForce(Vector3 forceDirection, string targetBoneName, ForceMode forceMode)
    {
        Transform targetBone = FindChildRecursive(gameObject.transform, targetBoneName);
        Rigidbody rb = targetBone.GetComponent<Rigidbody>();
        rb.AddForce(forceDirection, forceMode);
    }

    // 在所有子物体中递归查找指定名称的物体
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
}
