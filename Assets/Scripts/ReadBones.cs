using UnityEngine;
using UnityEditor;

public class ReadBoneNames : MonoBehaviour
{
    public GameObject fbxModel;

    // �˷������ڰ�ť���ʱ������
    public void RenameBones()
    {
        if (fbxModel != null)
        {
            // ��ȡģ�͵Ĺ������
            Transform[] boneTransforms = fbxModel.GetComponentsInChildren<Transform>();

            // �������й�������ӡ���ǵ�����
            foreach (Transform bone in boneTransforms)
            {
                bone.name = bone.name.Replace("mixamorig:", "");
                Debug.Log("Bone Name: " + bone.name);
            }
        }
        else
        {
            Debug.LogError("FBX Model is not assigned!");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ReadBoneNames))]
public class ReadBoneNamesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ReadBoneNames script = (ReadBoneNames)target;
        if (GUILayout.Button("Rename Bones"))
        {
            script.RenameBones();
        }
    }
}
#endif
