using UnityEngine;

public class PringAllChildName : MonoBehaviour
{
    void Start()
    {
        PrintAllChildrenNames(this.transform);
    }

    void PrintAllChildrenNames(Transform parent)
    {
        foreach (Transform child in parent)
        {
           // Debug.Log("Child Name: " + child.name);
            // �ݹ���ã���ӡ�Ӷ�����Ӷ���
            PrintAllChildrenNames(child);

            // Ȼ������ؼ�֡��λ��
        }
    }
}