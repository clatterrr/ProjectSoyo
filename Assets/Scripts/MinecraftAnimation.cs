using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Structure;
using static UnityEditor.SceneView;

public class AnimationSystem : MonoBehaviour
{
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        switch (anim)
        {
            case Animation.Walk:
                {
                    VoxelWalkAnimation();
                    break;
                }
            default: break;
        }

        //SimpleLookCamera();
       // VoxelWalkAnimation();

    }

    private int DeadCount = 0;
    void VoxelDeadAnim()
    {
        float deadHalfCycle = 60;
        if (DeadCount < deadHalfCycle)
        {
            float ratio = DeadCount / deadHalfCycle;
            float r = -89 * ratio + 1;
            gameObject.transform.localRotation = Quaternion.Euler(r, 90, 0);
        }
        DeadCount++;
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
        }
        else
        {
            frameCount = 0;
        }
    }

    private void VoxelAttackAnimation()
    {
        
    }

    void SimpleIdleAnimation()
    {
        Vector3 baseScale = Vector3.zero;
        float happyCycle = 6;
        if (frameCount < happyCycle)
        {
            float ration = frameCount / happyCycle;
            transform.localScale = new Vector3(1, 400 * (1.1f - ration * 0.2f), 1) * baseScale.x;
        }
        else if (frameCount >= happyCycle && frameCount < happyCycle * 2)
        {

            float ration = (frameCount - happyCycle) / happyCycle;
            transform.localScale = new Vector3(1, 400 * (0.9f + ration * 0.2f), 1) * baseScale.x;
        }
        else if (frameCount >= happyCycle * 2)
        {
            frameCount = 0;
        }
    }
}
