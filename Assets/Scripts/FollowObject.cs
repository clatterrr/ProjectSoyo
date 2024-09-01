using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform[] targets; // 在Inspector中设置目标物体的数组引用
    public float distance = 1.0f; // 追随者与目标之间的距离
    public float height = 1.0f; // 追随者与目标之间的距离

    private int targetIndex; // 当前目标的索引
    private float stayTimer; // 停留在当前目标的计时器
    private float switchTimer; // 平滑切换到下一个目标的计时器

    private Vector3 lerpTargetPosition; // 用于缓动的目标位置
    private Vector3 StarterPosition;
    private Quaternion StarterRotation;
    private Quaternion lerpTargetRotation; // 用于缓动的目标旋转

    void Start()
    {
        // 初始化目标索引和计时器
        targetIndex = 0;
        stayTimer = 8.0f; // 初始停留时间设定为5秒
        switchTimer = 1.0f; // 初始化平滑切换计时器

        // 初始化缓动目标位置和旋转为当前目标
        lerpTargetPosition = targets[targetIndex].position + targets[targetIndex].forward * distance + targets[targetIndex].up * height;
        lerpTargetRotation = Quaternion.LookRotation(targets[targetIndex].forward);
    }

    void Update()
    {
        if (targets.Length > 0)
        {
            // 减少停留计时器
            stayTimer -= Time.deltaTime;

            // 检查是否需要开始切换目标
            if (stayTimer <= 0)
            {
                // 重置停留计时器并开始平滑切换
                stayTimer = 8.0f;
                switchTimer = 1.0f; // 设置平滑切换持续时间为1秒

                StarterPosition = lerpTargetPosition;
                StarterRotation = lerpTargetRotation;
                // 移动到下一个目标
                targetIndex = (targetIndex + 1) % targets.Length;

                // 更新缓动目标位置和旋转为新目标
                lerpTargetPosition = targets[targetIndex].position + targets[targetIndex].forward * distance + targets[targetIndex].up * height;
                lerpTargetRotation = Quaternion.LookRotation(targets[targetIndex].forward);

            }

            // 减少平滑切换计时器
            if (switchTimer > 0)
            {
                switchTimer -= Time.deltaTime;

                // 计算当前位置和旋转的插值
                transform.position = Vector3.Lerp(StarterPosition, lerpTargetPosition, 1.0f - switchTimer / 1.0f);
                transform.rotation = Quaternion.Slerp(StarterRotation, lerpTargetRotation, 1.0f - switchTimer / 1.0f);
                var tar = Vector3.Lerp(targets[targetIndex-1].position, targets[targetIndex].position, 1.0f - switchTimer / 1.0f) + targets[targetIndex].up * height;
                transform.LookAt(tar);
            }
            else
            {
                // 当不在切换过程中时，保持在当前目标的正前方
                transform.position = lerpTargetPosition;
                transform.rotation = lerpTargetRotation;
                transform.LookAt(targets[targetIndex].position + targets[targetIndex].up * height);
            }


        }
    }
}