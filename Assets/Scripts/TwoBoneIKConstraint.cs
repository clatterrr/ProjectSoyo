using UnityEngine;

[RequireComponent(typeof(Animator))] // 确保你有Animator组件
public class TwoBoneIKConstraint : MonoBehaviour
{
    // The target we are going to track
    [SerializeField] Transform target;

    // We will put all our animation code in LateUpdate.
    // This allows other systems to update the environment first, 
    // allowing the animation system to adapt to it before the frame is drawn.
    void LateUpdate()
    {
        RecursiveFindAndModify("mixamorig:RightForeArm", gameObject.transform);
    }

    void RecursiveFindAndModify(string targetName, Transform current)
    {
        // 检查当前Transform的名称是否是我们要找的
        if (current.name == targetName)
        {
            Vector3 towardObjectFromHead = target.position - current.position;
            current.rotation =  Quaternion.LookRotation(towardObjectFromHead, transform.up);

            // 示例：打印找到的Transform的名称和位置
            Debug.Log($"Found '{targetName}' at position {current.position}");
        }

        // 递归遍历所有子物体
        foreach (Transform child in current)
        {
            RecursiveFindAndModify(targetName, child);
        }
    }

    void Start()
    {
        // 从当前Transform开始递归设置
        //SetKinematicRecursive(transform);
    }

    void SetKinematicRecursive(Transform currentTransform)
    {
        // 查找当前Transform上的Rigidbody组件
        Rigidbody rb = currentTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 如果当前Transform的名字是"Spine"，则设置isKinematic为false
            rb.isKinematic = true;// (currentTransform.name != "mixamorig:RightForeArm");
        }

        // 遍历当前Transform的所有子Transform，并递归调用
        foreach (Transform child in currentTransform)
        {
            SetKinematicRecursive(child);
        }
    }
}