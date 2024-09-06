using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Structure;
using static UnityEditor.SceneView;

public class MinecraftAnimation : MonoBehaviour
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
        SimpleLookCamera();

    }
}
