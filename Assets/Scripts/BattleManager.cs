using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public GameObject ragdoll; // Ragdoll的根对象
    public Vector3 forceDirection = new Vector3(0, 10, 0); // 力的方向和大小
    public ForceMode forceMode = ForceMode.Impulse; // 力的模式
    public string targetBoneName = "Hips"; // 施加力的骨骼名称

    public GameObject bomb;
    public List<GameObject> Cannons;
    public int ShootTime = 20;

    private string folderPath = "Assets/Characters/RagdollPrefabs"; // 资源所在文件夹的路径
    void Start()
    {
        if (ragdoll == null)
        {
            Debug.LogError("No Ragdoll GameObject assigned.");
            return;
        }

        // 在Ragdoll中找到目标骨骼
        Transform targetBone = ragdoll.transform.Find(targetBoneName);

        if (targetBone == null)
        {
            Debug.LogError($"No bone named {targetBoneName} found in the Ragdoll.");
            return;
        }

        Rigidbody rb = targetBone.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody component found on the target bone.");
            return;
        }

        // 向骨骼施加力
        //rb.AddForce(forceDirection, forceMode);
        //SetAllRigidbodiesKinematic(ragdoll);
    }

    int insframe = 0;
    private void FixedUpdate()
    {
        insframe += 1;
        if (insframe % ShootTime == 0)
        {
            for(int i = 0; i < Cannons.Count; i++)
            {
                Instantiate(bomb, Cannons[i].transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
            }
            
        }
        
    }

    void SetAllRigidbodiesKinematic(GameObject obj)
    {
        // 获取当前物体的Rigidbody组件（如果有的话）
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 递归查找并处理子物体中的Rigidbody
        foreach (Transform child in obj.transform)
        {
            SetAllRigidbodiesKinematic(child.gameObject);
        }
    }
}
