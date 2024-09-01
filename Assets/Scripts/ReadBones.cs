using UnityEngine;
using UnityEditor;

public class ReadBoneNames : MonoBehaviour
{
    public GameObject fbxModel;

    // 此方法将在按钮点击时被调用
    public void RenameBones()
    {
        if (fbxModel != null)
        {
            // 获取模型的骨骼层次
            Transform[] boneTransforms = fbxModel.GetComponentsInChildren<Transform>();

            // 遍历所有骨骼并打印它们的名字
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
