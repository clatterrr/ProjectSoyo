using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stab : MonoBehaviour
{
    private int frameCount = 0;

    private float targetFrame = 0;
    private int tragetHeight = 0;

    private void Start()
    {
        targetFrame = Random.Range(50, 200);
    }
    private void FixedUpdate()
    {
        float r;

        if (frameCount <= targetFrame)
        {
            // 0 到 100 帧，数字由 -8 到 -2
            r = Mathf.Lerp(-14, -2, frameCount / targetFrame);
        }
        else if (frameCount <= targetFrame * 2)
        {
            // 100 到 200 帧，数字由 -2 到 -8
            r = Mathf.Lerp(-2, -14, (frameCount - targetFrame) / targetFrame);
        }
        else
        {
            // 重置 frameCount 或根据需求继续处理
            frameCount = 0;
            r = -8; // 重置为初始值
        }

        transform.position = new Vector3(transform.position.x, r, transform.position.z);

        frameCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的对象是否有 RealFighter 组件
        Walker realFighter = other.GetComponent<Walker>();

        if (realFighter != null)
        {
            Debug.Log("hitted");
            realFighter.TakeDamage(20);
        }


        MinecraftFighter walker = other.GetComponent<MinecraftFighter>();
        if (walker != null)
        {
            walker.TakeDamage(20);
        }
    }
}
