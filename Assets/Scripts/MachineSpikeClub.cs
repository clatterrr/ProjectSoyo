using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSpikeClub : MonoBehaviour
{
    private float rotationSpeed = 120f; // 旋转速度

    private Transform centerTransform;

    void Start()
    {
        // 找到名为 "center" 的子物体
        centerTransform = transform.Find("Center");
        rotationSpeed = Random.Range(100, 200);
        if (centerTransform == null)
        {
            Debug.LogError("No child object named 'center' found!");
        }
    }

    void Update()
    {
        if (centerTransform != null)
        {
            // 绕着 center 子物体旋转
            transform.RotateAround(centerTransform.position, Vector3.left, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的对象是否有 RealFighter 组件
        Walker realFighter = other.GetComponent<Walker>();
        if (realFighter != null)
        {
            Debug.Log("hitted");
            realFighter.TakeDamage(70);
        }

        MinecraftFighter walker = other.GetComponent<MinecraftFighter>();
        if (walker != null)
        {
            walker.TakeDamage(70);
        }
    }
}
