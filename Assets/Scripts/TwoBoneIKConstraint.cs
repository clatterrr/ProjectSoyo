using UnityEngine;

[RequireComponent(typeof(Animator))] // ȷ������Animator���
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
        // ��鵱ǰTransform�������Ƿ�������Ҫ�ҵ�
        if (current.name == targetName)
        {
            Vector3 towardObjectFromHead = target.position - current.position;
            current.rotation =  Quaternion.LookRotation(towardObjectFromHead, transform.up);

            // ʾ������ӡ�ҵ���Transform�����ƺ�λ��
            Debug.Log($"Found '{targetName}' at position {current.position}");
        }

        // �ݹ��������������
        foreach (Transform child in current)
        {
            RecursiveFindAndModify(targetName, child);
        }
    }

    void Start()
    {
        // �ӵ�ǰTransform��ʼ�ݹ�����
        //SetKinematicRecursive(transform);
    }

    void SetKinematicRecursive(Transform currentTransform)
    {
        // ���ҵ�ǰTransform�ϵ�Rigidbody���
        Rigidbody rb = currentTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // �����ǰTransform��������"Spine"��������isKinematicΪfalse
            rb.isKinematic = true;// (currentTransform.name != "mixamorig:RightForeArm");
        }

        // ������ǰTransform��������Transform�����ݹ����
        foreach (Transform child in currentTransform)
        {
            SetKinematicRecursive(child);
        }
    }
}