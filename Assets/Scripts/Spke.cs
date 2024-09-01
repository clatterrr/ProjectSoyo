using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spke : MonoBehaviour
{
    private float startTime;
    private float duration;
    private float amplitude = 4f; // ���Ҳ������������ -10 �� 10

    void Start()
    {
        startTime = Time.time;
        duration = Random.Range(4f, 6f); // ���ѡ��һ���� 4 �� 6 ��֮���ʱ��
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float normalizedTime = elapsedTime / duration; // ��һ��ʱ�䣬ʹ������ 0 �� 1
        float yPosition = Mathf.Sin(normalizedTime * Mathf.PI * 2) * amplitude - 25f;

        // ��������� y λ��
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

        // ���������ɣ����������ʱ�䲢���ÿ�ʼʱ��
        if (elapsedTime >= duration)
        {
            startTime = Time.time;
            duration = Random.Range(4f, 6f);

            couldTakeDamage = true;
        }
    }
    private Vector3 forceDirection = new Vector3(-10, 0, 0); // ���ķ���ʹ�С
    public ForceMode forceMode = ForceMode.Impulse; // ����ģʽ
    public string targetBoneName = "mixamorig:Hips"; // ʩ�����Ĺ�������
    bool couldTakeDamage = true;


    void OnTriggerStay(Collider other)
    {
        if (couldTakeDamage == false)
        {
            return;
        }
        // �����ײ���������Ƿ����Fighter���
        Fighter fighter = other.GetComponent<Fighter>();
        if (fighter != null && fighter.isActiveAndEnabled)
        {
            Debug.Log("hitted");


            fighter.AddForce(forceDirection, targetBoneName, forceMode);
            fighter.TakeDamage(25f);

            couldTakeDamage = false;
           
        }
    }

}
