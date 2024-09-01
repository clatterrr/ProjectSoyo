using UnityEngine;
using UnityEditor;

public class RagdollCreator : EditorWindow
{
    private GameObject selectedObject;

    [MenuItem("Tools/Ragdoll Creator")]
    public static void ShowWindow()
    {
        GetWindow<RagdollCreator>("Ragdoll Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Ragdoll Creator", EditorStyles.boldLabel);

        selectedObject = (GameObject)EditorGUILayout.ObjectField("Humanoid Model", selectedObject, typeof(GameObject), true);

        if (GUILayout.Button("Create Ragdoll"))
        {
            if (selectedObject != null)
            {
                CreateRagdoll(selectedObject);
            }
            else
            {
                Debug.LogError("No Humanoid Model selected!");
            }
        }
    }

    private void CreateRagdoll(GameObject model)
    {
        Animator animator = model.GetComponent<Animator>();
        if (animator == null || !animator.isHuman)
        {
            Debug.LogError("The selected model is not a Humanoid or doesn't have an Animator component.");
            return;
        }

        Transform Pelvis = FindChildByName(model.transform, "mixamorig:Hips");
        Transform LeftHips = FindChildByName(model.transform, "mixamorig:LeftUpLeg");
        Transform LeftKnee = FindChildByName(model.transform, "mixamorig:LeftLeg");
        Transform LeftFoot = FindChildByName(model.transform, "mixamorig:LeftFoot");
        Transform RightHips = FindChildByName(model.transform, "mixamorig:RightUpLeg");
        Transform RightKnee = FindChildByName(model.transform, "mixamorig:RightLeg");
        Transform RightFoot = FindChildByName(model.transform, "mixamorig:RightFoot");
        Transform LeftArm = FindChildByName(model.transform, "mixamorig:LeftArm");
        Transform LeftElbow = FindChildByName(model.transform, "mixamorig:LeftForeArm");
        Transform RightArm = FindChildByName(model.transform, "mixamorig:RightArm");
        Transform RightElbow = FindChildByName(model.transform, "mixamorig:RightForeArm");
        Transform MiddleSpine = FindChildByName(model.transform, "mixamorig:Spine");
        Transform Head = FindChildByName(model.transform, "mixamorig:Head");

        CreateRagdollPart(Pelvis, null, 3.125f);
        CreateRagdollPart(LeftHips, Pelvis, 1.875f);
        CreateRagdollPart(LeftKnee, LeftHips, 1.875f);
        CreateRagdollPart(LeftFoot, LeftKnee, 1.875f);
        CreateRagdollPart(RightHips, Pelvis, 1.875f);
        CreateRagdollPart(RightKnee, RightHips, 1.875f);
        CreateRagdollPart(RightFoot, RightKnee, 1.875f);
        CreateRagdollPart(MiddleSpine, Pelvis, 3.125f);
        CreateRagdollPart(LeftArm, Pelvis, 1.25f);
        CreateRagdollPart(LeftElbow, LeftArm, 1.25f);
        CreateRagdollPart(RightArm, Pelvis, 1.25f);
        CreateRagdollPart(RightElbow, RightArm, 1.25f);
        CreateRagdollPart(Head, MiddleSpine, 1.25f);



        Debug.Log("Ragdoll created successfully.");
    }

    private void CreateRagdollPart(Transform bone, Transform parent, float mass)
    {
        if (bone == null) return;

        Rigidbody rb = bone.gameObject.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.isKinematic = true;

        CapsuleCollider collider = bone.gameObject.AddComponent<CapsuleCollider>();
        collider.radius = 0.1f;
        collider.height = Vector3.Distance(bone.position, bone.parent.position);

        if (parent != null)
        {
            CharacterJoint joint = bone.gameObject.AddComponent<CharacterJoint>();
            joint.connectedBody = parent.GetComponent<Rigidbody>();
        }
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        if (parent.name == name)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            Transform result = FindChildByName(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

}
