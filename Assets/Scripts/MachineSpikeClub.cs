using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSpikeClub : MonoBehaviour
{
    private float rotationSpeed = 120f; // ��ת�ٶ�

    private Transform centerTransform;

    void Start()
    {
        // �ҵ���Ϊ "center" ��������
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
            // ���� center ��������ת
            transform.RotateAround(centerTransform.position, Vector3.left, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����ײ�Ķ����Ƿ��� RealFighter ���
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
