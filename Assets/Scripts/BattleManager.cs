using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public GameObject ragdoll; // Ragdoll�ĸ�����
    public Vector3 forceDirection = new Vector3(0, 10, 0); // ���ķ���ʹ�С
    public ForceMode forceMode = ForceMode.Impulse; // ����ģʽ
    public string targetBoneName = "Hips"; // ʩ�����Ĺ�������

    public GameObject bomb;
    public List<GameObject> Cannons;
    public int ShootTime = 20;

    private string folderPath = "Assets/Characters/RagdollPrefabs"; // ��Դ�����ļ��е�·��
    void Start()
    {
        if (ragdoll == null)
        {
            Debug.LogError("No Ragdoll GameObject assigned.");
            return;
        }

        // ��Ragdoll���ҵ�Ŀ�����
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

        // �����ʩ����
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
        // ��ȡ��ǰ�����Rigidbody���������еĻ���
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // �ݹ���Ҳ������������е�Rigidbody
        foreach (Transform child in obj.transform)
        {
            SetAllRigidbodiesKinematic(child.gameObject);
        }
    }
}
