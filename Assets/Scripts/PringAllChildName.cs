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
            // 递归调用，打印子对象的子对象
            PrintAllChildrenNames(child);

            // 然后输出关键帧的位置
        }
    }
}