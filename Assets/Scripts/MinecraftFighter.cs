using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using System;
using UnityEditor.Animations;
using UnityEditor;
using static Structure;

public class MinecraftFighter : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件
    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // 血量条相对于物体的位置偏移

    private GameObject healthBarInstance;
    private RectTransform redBarRectTransform; // 红色血量条的 RectTransform
    private float maxHealth = 100f;
    private float currentHealth;

    public GameObject eyes;

    public enum AnimationMode
    {
        Normal,
        Human,
        Minecraft,
    }

    public AnimationMode animationMode = AnimationMode.Normal;

    private float targetMoveSpeed = 5.0f;

    public enum Animation
    {
        Walk,
        Superise,
        Wait,
        Happy,
        Dead,

    }

    public Animation anim;
    public void SetAnimation(Animation anim)
    {
        this.anim = anim;
    }

    public void SetTransform(Vector3 t, Quaternion r)
    {
        transform.position = t;
        transform.rotation = r;
    }
    private string animationName = "";
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
            state.cycleOffset = 0;
            GetComponent<Animator>().runtimeAnimatorController = controller;
        }

    }
    void initHealth()
    {
        currentHealth = maxHealth;
        // 在 Canvas 上生成 HealthBar 实例
        healthBarInstance = Instantiate(healthBarPrefab, uiCanvas.transform);
        FirstDead = true;
        healthBarInstance.GetComponent<Image>().color = Color.white;
        healthBarInstance.transform.Find("RedBar").GetComponent<Image>().color = Color.red;
        redBarRectTransform = healthBarInstance.transform.Find("RedBar").GetComponent<RectTransform>();
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

    public void SelfKill()
    {
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }
    public bool isDead()
    {
        return currentHealth <= 0;
    }

    public bool isAlive()
    {
        return currentHealth > 0;
    }

    private bool FirstDead = true;
    void UpdateHealth()
    {
        if (!isDead())
        {
            if (healthBarInstance != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
                healthBarInstance.transform.position = screenPosition;
            }
        }

        if (currentHealth <= 0f && FirstDead)
        {
            FirstDead = false;
            if(healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }
            frameCount = 0;
        }

    }

    private Vector3 baseScale;

    private string supriseAniamtion;
    private string idleAnim;
    private string walkAnim;
    void Start()
    {
        supriseAniamtion = RandomSelectAnimation(new string[] { "Rejected", "Disappointed", "RumbaDancing" });
        walkAnim = RandomSelectAnimation(new string[] { "CrouchedWalking", "Walking2", "Right Strafe Walking", "Scary Clown Start Walking", "Injured Walking" });
        idleAnim = RandomSelectAnimation(new string[] { "idleAiming", "RifleIdle", "NinjaIdle" });
        float r = UnityEngine.Random.Range(0.6f, 1.0f);
        baseScale = new Vector3(r, r, r);
        transform.localScale *= r;
        targetMoveSpeed = UnityEngine.Random.Range(6, 15);
        moveSpeed = 0;
        initHealth();
        UpdateHealthBar();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        UpdateHealth();
        if (isAlive())
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
        else
        {
            moveSpeed = 0;
            if(animationMode == AnimationMode.Human)
            {
                SelectAnimation("Falling Back Death");
            }
            else
            {
               // SimpleDeadAnimation();
            }
            
            return;
        }
        
        if (transform.position.x > 250)
        {
            TakeDamage(100);
        }
        if(transform.position.z > 7)
        {

            transform.position = new Vector3(transform.position.x, transform.position.y, 7);
        }
        if(transform.position.z < -20)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -20);
        }
        frameCount++;
        switch (anim)
        {
            case Animation.Wait:
                {

                    
                    if (animationMode == AnimationMode.Minecraft)
                    {
                        SimpleHappyXAnimation();
                    }
                    else if (animationMode == AnimationMode.Normal)
                    {
                        SimpleSupriseXAnimation(transform, 4);
                    }
                    else
                    {
                        SelectAnimation(idleAnim);
                    }
                    break;
                }
            case Animation.Happy:
                {
                    SimpleHappyYAnimation(); break;
                }
            case Animation.Walk:
                {

                    moveSpeed = targetMoveSpeed;
                    if (animationMode == AnimationMode.Minecraft)
                    {
                        SimpleWalkAnimation();
                    }
                    else if(animationMode == AnimationMode.Normal)
                    {
                        SimpleSupriseXAnimation(transform, 4);
                    }
                    else
                    {
                        SelectAnimation(walkAnim);
                    }
                     break;
                }
            case Animation.Superise:
                {
                    if (animationMode == AnimationMode.Minecraft)
                    {

                        Transform head = FindChildByName(gameObject.transform, "head");
                        SimpleSupriseAnimation(head, 40);
                    }
                    else if (animationMode == AnimationMode.Normal)
                    {
                        SimpleSupriseAnimation(transform, 40);
                    }
                    else
                    {
                        SelectAnimation(supriseAniamtion);
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    int frameCount = 0;

    string RandomSelectAnimation(string[] name)
    {
        int r = UnityEngine.Random.Range(0, name.Length);
        return name[r];
    }
    void SimpleWalkAnimation()
    {
        float walkHalfCycle = 200 / moveSpeed;
        float startRotaion = 40;
        Quaternion rotation = Quaternion.identity;
        Quaternion rotation1 = Quaternion.identity;
        if (frameCount < walkHalfCycle)
        {
            float ratio = frameCount / walkHalfCycle;
            float r = startRotaion - 2 * ratio * startRotaion;
            rotation = Quaternion.Euler(r, 0, 0);
            rotation1 = Quaternion.Euler(-r, 0, 0);
            RecursiveFindAndModify("left_leg", gameObject.transform, rotation, true);
            RecursiveFindAndModify("right_leg", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("left_arm", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("right_arm", gameObject.transform, rotation, true);
        }
        else if(frameCount < walkHalfCycle * 2) {

            float ratio = (frameCount - walkHalfCycle) / walkHalfCycle;
            float r = startRotaion - 2 * ratio * startRotaion;
            rotation = Quaternion.Euler(-r, 0, 0);
            rotation1 = Quaternion.Euler(r, 0, 0);
            RecursiveFindAndModify("left_leg", gameObject.transform, rotation, true);
            RecursiveFindAndModify("right_leg", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("left_arm", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("right_arm", gameObject.transform, rotation, true);
        }
        else
        {
            frameCount = 0;
        }
    }


    bool GenerateEyes = false;

    void SimpleSupriseAnimation(Transform transform, float supriseScale)
    {
        if(GenerateEyes == false && animationMode == AnimationMode.Minecraft)
        {
            GenerateEyes = true;
            Transform leftEye = FindChildByName(transform, "LeftEyePos");
            Vector3 position = leftEye.position;
            Quaternion rotation = leftEye.rotation;
            GameObject eye0 = Instantiate(eyes, position, Quaternion.identity);
            eye0.transform.parent = transform;
            eye0.transform.localRotation = Quaternion.identity;

            Transform rightEye = FindChildByName(gameObject.transform, "RightEyePos");
            position = rightEye.position;
            rotation = rightEye.rotation;
            GameObject eye1 = Instantiate(eyes, position, Quaternion.identity);
            eye1.transform.parent = transform;
            eye1.transform.rotation = rotation;


        }
        if (frameCount <= 20)
        {
            // 0~20帧往左转40度
            float ratio = frameCount / 20f;
            float r = supriseScale * ratio;
            transform.localRotation = Quaternion.Euler(0, 0, r) * Quaternion.Euler(0,90,0);
        }
        else if (frameCount >= 80 && frameCount <= 100)
        {
            // 80~100帧往右转80度
            float ratio = (frameCount - 80) / 20f;
            float r = supriseScale - supriseScale * 2 * ratio;
            transform.localRotation = Quaternion.Euler(0, 0, r) * Quaternion.Euler(0, 90, 0);
        }
        else if (frameCount >= 160 && frameCount <= 180)
        {
            // 160~180帧往左转40度
            float ratio = (frameCount - 160) / 20f;
            float r = -supriseScale + supriseScale * ratio;
            transform.localRotation = Quaternion.Euler(0, 0, r) * Quaternion.Euler(0, 90, 0); ;
        }
        else if(frameCount >=180)
        {
            frameCount = 0;
        }
    }

    void SimpleSupriseXAnimation(Transform transform, float supriseScale)
    {
        if (frameCount <= 20)
        {
            // 0~20帧往左转40度
            float ratio = frameCount / 20f;
            float r = supriseScale * ratio;
            transform.localRotation = Quaternion.Euler(r, 0, 0) * Quaternion.Euler(0, 90, 0);
        }
        else if (frameCount >= 80 && frameCount <= 100)
        {
            // 80~100帧往右转80度
            float ratio = (frameCount - 80) / 20f;
            float r = supriseScale - supriseScale * 2 * ratio;
            transform.localRotation = Quaternion.Euler(r, 0, 0) * Quaternion.Euler(0, 90, 0);
        }
        else if (frameCount >= 160 && frameCount <= 180)
        {
            // 160~180帧往左转40度
            float ratio = (frameCount - 160) / 20f;
            float r = -supriseScale + supriseScale * ratio;
            transform.localRotation = Quaternion.Euler(r, 0, 0) * Quaternion.Euler(0, 90, 0); ;
        }
        else if (frameCount >= 180)
        {
            frameCount = 0;
        }
    }

    void SimpleHappyXAnimation()
    {
        float happyCycle = 6;
        float scale = 0.05f;
        if(frameCount < happyCycle)
        {
            float ration = frameCount / happyCycle;
            transform.localScale = new Vector3( ((1.0f + scale) - ration * scale * 2), 1, 1) * baseScale.x;

        }
        else if(frameCount >= happyCycle && frameCount < happyCycle * 2)
        {

            float ration = (frameCount - happyCycle) / happyCycle;
            transform.localScale = new Vector3( ((1.0f - scale) + ration * scale * 2), 1, 1) * baseScale.x;
        }
        else if(frameCount >= happyCycle * 2)
        {
            frameCount = 0;
        }
    }

    void SimpleHappyYAnimation()
    {
        float happyCycle = 6;
        if (frameCount < happyCycle)
        {
            float ration = frameCount / happyCycle;
            transform.localScale = new Vector3(1, 400 * (1.1f - ration * 0.2f),  1) * baseScale.x;
        }
        else if (frameCount >= happyCycle && frameCount < happyCycle * 2)
        {

            float ration = (frameCount - happyCycle) / happyCycle;
            transform.localScale = new Vector3(1, 400 * (0.9f + ration * 0.2f),  1) * baseScale.x;
        }
        else if (frameCount >= happyCycle * 2)
        {
            frameCount = 0;
        }
    }





    Transform FindChildByName(Transform parent, string name)
    {
        // Check if the current object has the desired name
        if (parent.name == name)
        {
            return parent;
        }

        // Recursively search through all children
        foreach (Transform child in parent)
        {
            Transform result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }

        // Return null if not found
        return null;
    }
}
