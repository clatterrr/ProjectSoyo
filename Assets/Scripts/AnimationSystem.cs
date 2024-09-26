using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Structure;
using static UnityEditor.SceneView;

public class AnimationSystem : MonoBehaviour
{
    public enum theAnim
    {
        Idle,
        Idle2,
        Walk,
        Superise,
        Wait,
        Happy,
        Dead,
        Attack_ShootPea,

        //https://youtu.be/jCgRV9bRBt8?t=169
        SelfTalk,

        // Walk / Run
        ChargeIn,

    }

    public enum AnimationActor
    {
        None,
        Human,
        Villager,
        Plants,
    }

    public AnimationActor animActor;

    public theAnim anim;
    public void SetAnimation(theAnim anim)
    {
        this.anim = anim;
    }

    public void SetTransform(Vector3 t, Quaternion r)
    {
        transform.position = t;
        transform.rotation = r;
    }
    private string animationName = "";

    private int frameCount = 0;

    void SimpleWalkAnimation()
    {
        float moveSpeed = 3.0f;
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
        else if (frameCount < walkHalfCycle * 2)
        {

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

    void SimpleLookCamera()
    {
        Vector3 directionToLookAt = Camera.main.transform.position - gameObject.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLookAt) ;
        Quaternion bodyRotation = targetRotation;
        float angleDifference = Quaternion.Angle(Quaternion.identity, targetRotation);
        if (angleDifference > 20)
        {
            angleDifference = (int)(angleDifference / 20) * 20;
            bodyRotation = Quaternion.RotateTowards(Quaternion.identity, targetRotation, angleDifference);
        }
        Vector3 euler = bodyRotation.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        bodyRotation = Quaternion.Euler(euler);

        //RotateChild(gameObject.transform, "All", bodyRotation);
        RotateChild(gameObject.transform, "head", targetRotation * Quaternion.Euler(0, 180, 0));
        RotateChild(gameObject.transform, "headwear", targetRotation * Quaternion.Euler(0, 180, 0));
    }

    private GameObject pea_prefab;
    void Start()
    {
        animActor = AnimationActor.Plants;
        string path = "Assets/Characters/Plants/Prefab/pea.prefab";
        pea_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    // Update is called once per frame
    private int GlobalFrameCount = 0;
    void Update()
    {
        GlobalFrameCount++;
        frameCount++;
        switch (anim)
        {
            case theAnim.Walk:
                {
                    VoxelWalkAnimation();
                    break;
                }
            case theAnim.Dead:
                {
                    SimpleDeadAnim();
                    break;
                }
            case theAnim.Attack_ShootPea:
                {
                    SimpleAttackShoot();
                    break;
                }
            case theAnim.Wait:
                {
                    SimpleIdleAnimation();
                    break;
                }
            case theAnim.SelfTalk:
                {
                    VoxelNodTalkAnimation();
                    break;
                }
            default: break;
        }

        //SimpleLookCamera();
       // VoxelWalkAnimation();

    }

    private int DeadCount = 0;
    void SimpleDeadAnim()
    {
        float deadHalfCycle = 60;
        if (DeadCount < deadHalfCycle)
        {
            float ratio = DeadCount / deadHalfCycle;
            float r = -89 * ratio + 1;
            gameObject.transform.localRotation = Quaternion.Euler(r, 90, 0);
        }
        else
        {

            gameObject.transform.localRotation = Quaternion.Euler(-88, 90, 0);
        }
        DeadCount++;
    }

    void SimpleAttackShoot()
    {
        int shootCycle = 60;
        if(GlobalFrameCount % shootCycle == 0)
        {
            Instantiate(pea_prefab, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    float moveSpeed = 1.0f;
    void VoxelWalkAnimation()
    {
        float walkHalfCycle = 200 / moveSpeed;
        float startRotaion = 20;
        Quaternion rotation = Quaternion.identity;
        Quaternion rotation1 = Quaternion.identity;
        if (frameCount < walkHalfCycle)
        {
            float ratio = frameCount / walkHalfCycle;
            float r = startRotaion - 2 * ratio * startRotaion;
            rotation = Quaternion.Euler(r, 0, 0);
            rotation1 = Quaternion.Euler(-r, 0, 0);
            RecursiveFindAndModify("LeftLeg", gameObject.transform, rotation, true);
            RecursiveFindAndModify("RightLeg", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("LeftArm", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("RightArm", gameObject.transform, rotation, true);
            RecursiveFindAndModify("left_leg", gameObject.transform, rotation, true);
            RecursiveFindAndModify("right_leg", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("left_arm", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("right_arm", gameObject.transform, rotation, true);
            if(animActor == AnimationActor.Plants)
            {
                rotation = Quaternion.Euler(0, r, r);
                rotation1 = Quaternion.Euler(0, 0, -r);
                RecursiveFindAndModify("LeftRootLeaf", gameObject.transform, rotation, true);
                RecursiveFindAndModify("RightRootLeaf", gameObject.transform, rotation, true);
            }
        }
        else if (frameCount < walkHalfCycle * 2)
        {

            float ratio = (frameCount - walkHalfCycle) / walkHalfCycle;
            float r = startRotaion - 2 * ratio * startRotaion;
            rotation = Quaternion.Euler(-r, 0, 0);
            rotation1 = Quaternion.Euler(r, 0, 0);
            RecursiveFindAndModify("LeftLeg", gameObject.transform, rotation, true);
            RecursiveFindAndModify("RightLeg", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("LeftArm", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("RightArm", gameObject.transform, rotation, true);
            RecursiveFindAndModify("left_leg", gameObject.transform, rotation, true);
            RecursiveFindAndModify("right_leg", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("left_arm", gameObject.transform, rotation1, true);
            RecursiveFindAndModify("right_arm", gameObject.transform, rotation, true);
            if (animActor == AnimationActor.Plants)
            {
                rotation = Quaternion.Euler(0, -r, r);
                rotation1 = Quaternion.Euler(0, 0, r);
                RecursiveFindAndModify("LeftRootLeaf", gameObject.transform, rotation, true);
                RecursiveFindAndModify("RightRootLeaf", gameObject.transform, rotation, true);
            }
        }
        else
        {
            frameCount = 0;
        }
    }

    float targetDegree = 0.0f;
    float startDegree = 0.0f;
    private void VoxelAttackAnimation()
    {
        
    }

    private void VoxelTalkToSelfAnimation()
    {
        int cycle = 200;
        //https://youtu.be/jCgRV9bRBt8?t=171
        // ͷ
        if (GlobalFrameCount % cycle == 0)
        {
            startDegree = targetDegree;
            targetDegree = Random.Range(-30f, 30f);
        }
        float ratio = (GlobalFrameCount % cycle) / (cycle * 1.0f);
        float currentDegree = Mathf.Lerp(startDegree, targetDegree, ratio);
        RecursiveFindAndModify("head", gameObject.transform, Quaternion.Euler(0, currentDegree, 0), true);
    }

    private void VoxelNodTalkAnimation()
    {
        int cycle = 200;
        //https://youtu.be/jCgRV9bRBt8?t=171
        // ͷ
        if (GlobalFrameCount % cycle == 0)
        {
            startDegree = targetDegree;
            targetDegree = Random.Range(-30f, 30f);
        }
        float ratio = (GlobalFrameCount % cycle) / (cycle * 1.0f);
        float currentDegree = Mathf.Lerp(startDegree, targetDegree, ratio);
        RecursiveFindAndModify("head", gameObject.transform, Quaternion.Euler(0, 0, currentDegree), true);
    }
    void SimpleIdleAnimation()
    {
        Vector3 baseScale = Vector3.zero;
        float happyCycle = 60;
        if (frameCount < happyCycle)
        {
            float ration = frameCount / happyCycle;
            Vector3 scale = new Vector3(1, (1.1f - ration * 0.2f), 1);

            RecursiveFindAndModifyScale("all", gameObject.transform, scale);
        }
        else if (frameCount >= happyCycle && frameCount < happyCycle * 2)
        {

            float ration = (frameCount - happyCycle) / happyCycle;
            Vector3 scale = new Vector3(1,  (0.9f + ration * 0.2f), 1) ;
            RecursiveFindAndModifyScale("all", gameObject.transform, scale);
        }
        else if (frameCount >= happyCycle * 2)
        {
            frameCount = 0;
        }
    }
}
