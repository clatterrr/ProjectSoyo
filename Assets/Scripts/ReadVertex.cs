using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadVertex : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh mesh;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        mesh = skinnedMeshRenderer.sharedMesh;

        if (mesh != null)
        {
            // ��ȡ��������
            Vector3[] vertices = mesh.vertices;
            // ��ȡ�����Ȩ������
            BoneWeight[] boneWeights = mesh.boneWeights;

            for (int i = 0; i < vertices.Length; i++)
            {
                // ��ÿ��������д���
                Vector3 vertex = vertices[i];
                BoneWeight boneWeight = boneWeights[i];

                // ��������Ȩ����Ϣ
                //Debug.Log("Vertex " + i + ": " + vertex.x.ToString("F6") + ", Bone Index: " + boneWeight.boneIndex0 + ", Weight: " + boneWeight.weight0);
            }
        }
    }
}
