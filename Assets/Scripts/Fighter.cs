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

    public GameObject healthBarPrefab; // ��ק���䵽 HealthBarPrefab Ԥ�Ƽ�
    public Canvas uiCanvas; // ��ק���䵽�����е� Canvas
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // Ѫ��������������λ��ƫ��

    private GameObject healthBarInstance;
    private RectTransform redBarRectTransform; // ��ɫѪ������ RectTransform
    private Image healthBarImage;
    private float maxHealth = 100f;
    private float currentHealth;

    void SelectRandomClip()
    {
        // ���� Animations �ļ����е����� .anim �ļ�
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Animations" });

        if (guids.Length > 0)
        {
            // ���ѡ��һ�� .anim �ļ�
            int randomIndex = Random.Range(0, guids.Length);
            string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
            path = "Assets/Animations/BeastWalk.anim";
            // ���� AnimationClip ����ֵ
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
        // ��̬����һ���µ� AnimatorController
        var animatorController = new AnimatorController();

        // ����һ��Ĭ�ϵĲ��״̬��
        animatorController.AddLayer("Base Layer");
        var rootStateMachine = animatorController.layers[0].stateMachine;

        // ����һ���յ�Ĭ��״̬
        var idleState = rootStateMachine.AddState("Idle");

        // ����������Ϊ Idle ״̬���ö�����������ѡ��
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

        // ��ȡ Animator �� Controller
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            animatorController = CreateDefaultAnimatorController();
            animator.runtimeAnimatorController = animatorController;
            Debug.Log("A new AnimatorController has been created and assigned.");
        }

        // ��鵱ǰ AnimationClip �Ƿ���Ч
        if (clip == null)
        {
            Debug.LogError("No AnimationClip assigned.");
            return;
        }
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // ���һ򴴽��뵱ǰ AnimationClip ��Ӧ�� AnimatorState
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
            // ���δ�ҵ���Ӧ��״̬���򴴽�һ���µ�״̬
            newState = animatorController.layers[0].stateMachine.AddState(clip.name);
            newState.motion = clip;
        }

        // ����ΪĬ��״̬
        animatorController.layers[0].stateMachine.defaultState = newState;
    }
    void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;

        // ���ݽ�������������ɫѪ�����Ŀ��
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
        // ��ȡ��ǰ�����Rigidbody���������еĻ���
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = state;
        }

        // �ݹ���Ҳ������������е�Rigidbody
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
        // �� Canvas ������ HealthBar ʵ��
        healthBarInstance = Instantiate(healthBarPrefab, uiCanvas.transform);

        healthBarInstance.GetComponent<Image>().color = Color.white;
        healthBarInstance.transform.Find("RedBar").GetComponent<Image>().color = Color.red;
        redBarPos = healthBarInstance.transform.Find("RedBar").localPosition;
        redBarRectTransform = healthBarInstance.transform.Find("RedBar").GetComponent<RectTransform>();

        UpdateHealthBar();
    }

    public float moveSpeed = 5f;

    public Vector3 moveDirection = Vector3.right; // �ƶ��ķ���Ĭ��Ϊǰ����

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
            // ���ٽ�����ʵ��
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

            // �ƶ�����
            transform.Translate(movement, Space.World);
        }
        // ���� TextMeshPro ��λ�ã�ʹ��ʼ��λ�� GameObject ��ͷ��


        // ʾ�������¿ո���۳�����ֵ
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

    // �������������еݹ����ָ�����Ƶ�����
    Transform FindChildRecursive(Transform parent, string targetBoneName)
    {
        // ��鵱ǰ�����Ƿ�ƥ��Ŀ������
        if (parent.name == targetBoneName)
        {
            return parent;
        }

        // ������ǰ���������������
        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, targetBoneName);
            if (result != null)
            {
                return result;
            }
        }

        // ���û���ҵ�ƥ��������壬���� null
        return null;
    }
}
