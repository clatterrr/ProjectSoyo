using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform[] targets; // ��Inspector������Ŀ���������������
    public float distance = 1.0f; // ׷������Ŀ��֮��ľ���
    public float height = 1.0f; // ׷������Ŀ��֮��ľ���

    private int targetIndex; // ��ǰĿ�������
    private float stayTimer; // ͣ���ڵ�ǰĿ��ļ�ʱ��
    private float switchTimer; // ƽ���л�����һ��Ŀ��ļ�ʱ��

    private Vector3 lerpTargetPosition; // ���ڻ�����Ŀ��λ��
    private Vector3 StarterPosition;
    private Quaternion StarterRotation;
    private Quaternion lerpTargetRotation; // ���ڻ�����Ŀ����ת

    void Start()
    {
        // ��ʼ��Ŀ�������ͼ�ʱ��
        targetIndex = 0;
        stayTimer = 8.0f; // ��ʼͣ��ʱ���趨Ϊ5��
        switchTimer = 1.0f; // ��ʼ��ƽ���л���ʱ��

        // ��ʼ������Ŀ��λ�ú���תΪ��ǰĿ��
        lerpTargetPosition = targets[targetIndex].position + targets[targetIndex].forward * distance + targets[targetIndex].up * height;
        lerpTargetRotation = Quaternion.LookRotation(targets[targetIndex].forward);
    }

    void Update()
    {
        if (targets.Length > 0)
        {
            // ����ͣ����ʱ��
            stayTimer -= Time.deltaTime;

            // ����Ƿ���Ҫ��ʼ�л�Ŀ��
            if (stayTimer <= 0)
            {
                // ����ͣ����ʱ������ʼƽ���л�
                stayTimer = 8.0f;
                switchTimer = 1.0f; // ����ƽ���л�����ʱ��Ϊ1��

                StarterPosition = lerpTargetPosition;
                StarterRotation = lerpTargetRotation;
                // �ƶ�����һ��Ŀ��
                targetIndex = (targetIndex + 1) % targets.Length;

                // ���»���Ŀ��λ�ú���תΪ��Ŀ��
                lerpTargetPosition = targets[targetIndex].position + targets[targetIndex].forward * distance + targets[targetIndex].up * height;
                lerpTargetRotation = Quaternion.LookRotation(targets[targetIndex].forward);

            }

            // ����ƽ���л���ʱ��
            if (switchTimer > 0)
            {
                switchTimer -= Time.deltaTime;

                // ���㵱ǰλ�ú���ת�Ĳ�ֵ
                transform.position = Vector3.Lerp(StarterPosition, lerpTargetPosition, 1.0f - switchTimer / 1.0f);
                transform.rotation = Quaternion.Slerp(StarterRotation, lerpTargetRotation, 1.0f - switchTimer / 1.0f);
                var tar = Vector3.Lerp(targets[targetIndex-1].position, targets[targetIndex].position, 1.0f - switchTimer / 1.0f) + targets[targetIndex].up * height;
                transform.LookAt(tar);
            }
            else
            {
                // �������л�������ʱ�������ڵ�ǰĿ�����ǰ��
                transform.position = lerpTargetPosition;
                transform.rotation = lerpTargetRotation;
                transform.LookAt(targets[targetIndex].position + targets[targetIndex].up * height);
            }


        }
    }
}