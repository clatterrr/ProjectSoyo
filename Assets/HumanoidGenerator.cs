using System.Collections.Generic;
using UnityEngine;

public class HumanoidGenerator : MonoBehaviour
{
    // 每个部位的父对象，方便调整整体位置
    public Transform parentTransform;



    public enum BodyPartName
    {
        Head,
        Body,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
    }

    public struct BodyPart
    {
        public GameObject actor;
        public BodyPartName name;

        public BodyPart(GameObject actor, BodyPartName name)
        {
            this.actor = actor;
            this.name = name;
        }

        public float Top()
        {
            return actor.transform.position.y + actor.transform.localScale.y / 2;
        }

        public Vector3 LeftHandConnector()
        {
            float x = actor.transform.position.x - actor.transform.localScale.x / 2;
            float y = actor.transform.position.y + actor.transform.localScale.y / 2 / 4 * 3;
            float z = actor.transform.position.z;
            return new Vector3(x, y, z);
        }

        public Vector3 RightHandConnector()
        {
            float x = actor.transform.position.x + actor.transform.localScale.x / 2;
            float y = actor.transform.position.y + actor.transform.localScale.y / 2 / 4 * 3;
            float z = actor.transform.position.z;
            return new Vector3(x, y, z);
        }

        public Vector3 BottomLeftConnector()
        {
            float x = actor.transform.position.x - actor.transform.localScale.x / 4;
            float y = actor.transform.position.y - actor.transform.localScale.y / 2;
            float z = actor.transform.position.z;
            return new Vector3(x, y, z);
        }

        public Vector3 BottomRightConnector()
        {
            float x = actor.transform.position.x + actor.transform.localScale.x / 4;
            float y = actor.transform.position.y - actor.transform.localScale.y / 2;
            float z = actor.transform.position.z;
            return new Vector3(x, y, z);
        }

        public Vector3 TopCenterConnector()
        {
            float x = actor.transform.position.x;
            float y = actor.transform.position.y + actor.transform.localScale.y / 2;
            float z = actor.transform.position.z;
            return new Vector3(x, y, z);
        }
    }

    BodyPart FindBodyPart(BodyPartName name)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].name == name)
            {
                return parts[i];
            }
        }
        return parts[0];
    }

    private List<BodyPart> parts;
    void Start()
    {
        parts = new List<BodyPart>();

        // 随机生成长宽高
        float scale = 0.2f;
        float width = 0; 
        float height = 0; 
        float depth = 0; 

        width = Random.Range(4, 6) * scale;
        height = Random.Range(4, 12) * scale;
        depth = Random.Range(4, 6) * scale;
        CreatePart(BodyPartName.Body, Vector3.zero, width, height, depth);
        /*
        width = Random.Range(2, 4) * scale;
        height = Random.Range(2, 6) * scale;
        depth = Random.Range(2, 4) * scale;
        CreatePart(BodyPartName.Head, Vector3.zero, width, height, depth);
        width = Random.Range(2, 4) * scale;
        height = Random.Range(6, 10) * scale;
        depth = Random.Range(2, 4) * scale;
        CreatePart(BodyPartName.LeftArm, Vector3.zero, width, height, depth);
        CreatePart(BodyPartName.RightArm, Vector3.zero, width, height, depth);
        width = Random.Range(2, 4) * scale;
        height = Random.Range(6, 10) * scale;
        depth = Random.Range(2, 4) * scale;
        CreatePart(BodyPartName.LeftLeg, Vector3.zero, width, height, depth);
        CreatePart(BodyPartName.RightLeg, Vector3.zero, width, height, depth);
        */
    }

    void CreatePart(BodyPartName partName, Vector3 position, float width, float height, float depth)
    {

        // 创建立方体
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // 获取立方体的Mesh
        Mesh mesh = part.GetComponent<MeshFilter>().mesh;

        // 自定义UV数组
        Vector2[] newUVs = new Vector2[mesh.vertices.Length];

        // 设置每个顶点的 UV 坐标（这个例子中只是给定简单的默认UV）
        // Unity 的立方体有 24 个顶点，因为每个面有独立的 UV 坐标
        // 你可以根据实际的需求来设置这些 UV

        // 顶部
        mesh.vertices[0] = new Vector3(-1, 1, -1);
        mesh.vertices[1] = new Vector3(1, 1, -1);
        mesh.vertices[2] = new Vector3(1, 1, 1);
        mesh.vertices[3] = new Vector3(-1, 1, 1);

        // 底部
        mesh.vertices[4] = new Vector3(-1, -1, -1);
        mesh.vertices[5] = new Vector3(1, -1, -1);
        mesh.vertices[6] = new Vector3(1, -1, 1);
        mesh.vertices[7] = new Vector3(-1, -1, 1);

        // 前面
        mesh.vertices[8] = new Vector3(-1, -1, 1);
        mesh.vertices[9] = new Vector3(1, -1, 1);
        mesh.vertices[10] = new Vector3(1, 1, 1);
        mesh.vertices[11] = new Vector3(-1, 1, 1);

        // 后面
        mesh.vertices[12] = new Vector3(-1, -1, -1);
        mesh.vertices[13] = new Vector3(1, -1, -1);
        mesh.vertices[14] = new Vector3(1, 1, -1);
        mesh.vertices[15] = new Vector3(-1, 1, -1);

        // 左面
        mesh.vertices[16] = new Vector3(-1, -1, -1);
        mesh.vertices[17] = new Vector3(-1, -1, 1);
        mesh.vertices[18] = new Vector3(-1, 1, 1);
        mesh.vertices[19] = new Vector3(-1, 1, -1);

        // 右面
        mesh.vertices[20] = new Vector3(1, -1, -1);
        mesh.vertices[21] = new Vector3(1, -1, 1);
        mesh.vertices[22] = new Vector3(1, 1, 1);
        mesh.vertices[23] = new Vector3(1, 1, -1);


        for (int i = 0; i < mesh.uv.Length; i++)
        {
            Debug.Log(" uv = " + mesh.uv[i]);
            newUVs[i] = mesh.uv[i];
        }

        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            Debug.Log(" tir = " + mesh.triangles[i]);
            
        }
        for(int i = 0; i < 6; i++)
        {
            mesh.triangles[i * 6 + 0] = i * 4 + 0;
            mesh.triangles[i * 6 + 1] = i * 4 + 2;
            mesh.triangles[i * 6 + 2] = i * 4 + 3;

            mesh.triangles[i * 6 + 3] = i * 4 + 0;
            mesh.triangles[i * 6 + 4] = i * 4 + 3;
            mesh.triangles[i * 6 + 5] = i * 4 + 1;

        }

        // front top

        newUVs[0] = new Vector2(0, 0);
        newUVs[1] = new Vector2(1, 0);
        newUVs[2] = new Vector2(0, 1);
        newUVs[3] = new Vector2(1, 1);

        int uvi = 2;
      newUVs[uvi * 4 + 0] = new Vector2(0.5f, 0.5f);
      newUVs[uvi * 4 + 1] = new Vector2(1, 0.0f);
      newUVs[uvi * 4 + 2] = new Vector2(0, 1f);
      newUVs[uvi * 4 + 3] = new Vector2(1, 1f);
        /*
     newUVs[8] = new Vector2(0, 0);
     newUVs[9] = new Vector2(1, 0);
     newUVs[10] = new Vector2(0, 1);
     newUVs[11] = new Vector2(1, 1);

     newUVs[12] = new Vector2(0, 0);
     newUVs[13] = new Vector2(1, 0);
     newUVs[14] = new Vector2(0, 1);
     newUVs[15] = new Vector2(1, 1);

     newUVs[16] = new Vector2(0, 0);
     newUVs[17] = new Vector2(1, 0);
     newUVs[18] = new Vector2(0, 1);
     newUVs[19] = new Vector2(1, 1);

     newUVs[20] = new Vector2(0, 0);
     newUVs[21] = new Vector2(1, 0);
     newUVs[22] = new Vector2(0, 1);
     newUVs[23] = new Vector2(1, 1);
     */

        // 设置自定义 UV 到 Mesh
        mesh.uv = newUVs;

        // 设置大小
        part.transform.localScale = new Vector3(width, height, depth);

        // 设置相对位置
        part.transform.localPosition = position;
        BodyPart thePart = new BodyPart(part, partName);

        switch (partName)
        {
            case BodyPartName.Head:
                {
                    BodyPart body = FindBodyPart(BodyPartName.Body);
                    thePart.actor.transform.localPosition += new Vector3(0, body.Top() + height / 2, 0);
                    break;
                }
            case BodyPartName.LeftArm:
                {
                    BodyPart body = FindBodyPart(BodyPartName.Body);
                    Vector3 offset = body.RightHandConnector() - thePart.LeftHandConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.RightArm:
                {
                    BodyPart body = FindBodyPart(BodyPartName.Body);
                    Vector3 offset = body.LeftHandConnector() - thePart.RightHandConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.LeftLeg:
                {
                    BodyPart body = FindBodyPart(BodyPartName.Body);
                    Vector3 offset = body.BottomLeftConnector() - thePart.TopCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.RightLeg:
                {
                    BodyPart body = FindBodyPart(BodyPartName.Body);
                    Vector3 offset = body.BottomRightConnector() - thePart.TopCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            default: break;
        }
        

        parts.Add(thePart);
    }
}
