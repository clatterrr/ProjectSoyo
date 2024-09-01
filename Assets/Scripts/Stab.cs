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
            // 0 �� 100 ֡�������� -8 �� -2
            r = Mathf.Lerp(-14, -2, frameCount / targetFrame);
        }
        else if (frameCount <= targetFrame * 2)
        {
            // 100 �� 200 ֡�������� -2 �� -8
            r = Mathf.Lerp(-2, -14, (frameCount - targetFrame) / targetFrame);
        }
        else
        {
            // ���� frameCount ����������������
            frameCount = 0;
            r = -8; // ����Ϊ��ʼֵ
        }

        transform.position = new Vector3(transform.position.x, r, transform.position.z);

        frameCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����ײ�Ķ����Ƿ��� RealFighter ���
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
