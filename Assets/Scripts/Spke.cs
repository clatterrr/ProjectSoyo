using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spke : MonoBehaviour
{
    private float startTime;
    private float duration;
    private float amplitude = 4f; // 正弦波的振幅，即从 -10 到 10

    void Start()
    {
        startTime = Time.time;
        duration = Random.Range(4f, 6f); // 随机选择一个在 4 到 6 秒之间的时间
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float normalizedTime = elapsedTime / duration; // 归一化时间，使得它从 0 到 1
        float yPosition = Mathf.Sin(normalizedTime * Mathf.PI * 2) * amplitude - 25f;

        // 更新物体的 y 位置
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

        // 如果动画完成，重新随机化时间并重置开始时间
        if (elapsedTime >= duration)
        {
            startTime = Time.time;
            duration = Random.Range(4f, 6f);

            couldTakeDamage = true;
        }
    }
    private Vector3 forceDirection = new Vector3(-10, 0, 0); // 力的方向和大小
    public ForceMode forceMode = ForceMode.Impulse; // 力的模式
    public string targetBoneName = "mixamorig:Hips"; // 施加力的骨骼名称
    bool couldTakeDamage = true;


    void OnTriggerStay(Collider other)
    {
        if (couldTakeDamage == false)
        {
            return;
        }
        // 检查碰撞到的物体是否带有Fighter组件
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
