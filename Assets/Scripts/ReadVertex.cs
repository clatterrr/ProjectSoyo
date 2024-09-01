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
            // 获取顶点数据
            Vector3[] vertices = mesh.vertices;
            // 获取顶点的权重数据
            BoneWeight[] boneWeights = mesh.boneWeights;

            for (int i = 0; i < vertices.Length; i++)
            {
                // 对每个顶点进行处理
                Vector3 vertex = vertices[i];
                BoneWeight boneWeight = boneWeights[i];

                // 输出顶点和权重信息
                //Debug.Log("Vertex " + i + ": " + vertex.x.ToString("F6") + ", Bone Index: " + boneWeight.boneIndex0 + ", Weight: " + boneWeight.weight0);
            }
        }
    }
}
