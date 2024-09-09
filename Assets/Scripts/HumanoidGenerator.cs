using Palmmedia.ReportGenerator.Core;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Structure;
using static TextureEditor;
public class HumanoidGenerator : MonoBehaviour
{
    public enum BodyPartName
    {
        Head,
        Body,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
        HatBottom,
        HatTop,
        LeftEye,
        RightEye,
        Mouth,   
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

        public Vector3 BottomCenterConnector()
        {

            float x = actor.transform.position.x;
            float y = actor.transform.position.y - actor.transform.localScale.y / 2;
            float z = actor.transform.position.z;

            return new Vector3(x, y, z);
        }

        public Vector3 FrontLeftConntector()
        {

            float x = actor.transform.position.x + actor.transform.localScale.y / 4;
            float y = actor.transform.position.y + actor.transform.localScale.y / 4;
            float z = actor.transform.position.z + actor.transform.localScale.z / 2;

            return new Vector3(x, y, z);
        }

        public Vector3 HeadMouthConntector()
        {

            float x = actor.transform.position.x;
            float y = actor.transform.position.y - actor.transform.localScale.y / 4;
            float z = actor.transform.position.z + actor.transform.localScale.z / 2;

            return new Vector3(x, y, z);
        }

        public Vector3 FrontRightEyeConntector()
        {

            float x = actor.transform.position.x - actor.transform.localScale.y / 4;
            float y = actor.transform.position.y + actor.transform.localScale.y / 4;
            float z = actor.transform.position.z + actor.transform.localScale.z / 2;
            return new Vector3(x, y, z);
        }

        public Vector3 BackCenterConntector()
        {

            float x = actor.transform.position.x;
            float y = actor.transform.position.y;
            float z = actor.transform.position.z - actor.transform.localScale.z / 2;
            return new Vector3(x, y, z);
        }
    }


    public static BodyPart FindBodyPart(List<BodyPart> parts,BodyPartName name)
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

    private static GameObject sourceModel;
    private static Texture2D sourceTexture;

    private void Start()
    {
        
        sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/iron_golem.prefab");
        sourceTexture = LoadTexture("Assets/iron_golem.png");
        CreateHumaoid();

        // ��״�� �����壬Բ�Σ�����״����������пգ�������ȫ�пգ���˼⣬��˱�ƽ
        // ���⸽��������ʾ�����а�ť�������ߣ����������л���

        // �۾�����ݣ�ͻ�������࣬��

        // �·���Ԫ�� ����
        // ���
        // �ǣ� ����


        string[] wooden = { "leaves", "branches" };
    }

    public static GameObject CreateHumaoid()
    {
        List<BodyPart> parts = new List<BodyPart>();

        
        // ������ɳ����
        float scale = 0.2f;
        float width = 0; 
        float height = 0; 
        float depth = 0; 

        width = Random.Range(4, 6) * scale;
        height = Random.Range(4, 12) * scale;
        depth = Random.Range(4, 6) * scale;
        CreatePart(parts, BodyPartName.Body, Vector3.zero, width, height, depth);
      
        width = Random.Range(2, 4) * scale;
        height = Random.Range(2, 6) * scale;
        depth = Random.Range(2, 4) * scale;
        CreatePart(parts, BodyPartName.Head, Vector3.zero, width, height, depth);  
        
        width = Random.Range(2, 4) * scale;
        height = Random.Range(6, 10) * scale;
        depth = Random.Range(2, 4) * scale;
        CreatePart(parts, BodyPartName.LeftArm, Vector3.zero, width, height, depth);
        CreatePart(parts, BodyPartName.RightArm, Vector3.zero, width, height, depth);
        width = Random.Range(2, 4) * scale;
        height = Random.Range(6, 10) * scale;
        depth = Random.Range(2, 4) * scale;
        CreatePart(parts, BodyPartName.LeftLeg, Vector3.zero, width, height, depth);
        CreatePart(parts, BodyPartName.RightLeg, Vector3.zero, width, height, depth);
        /*
        CreatePart(parts, BodyPartName.HatBottom, Vector3.zero, 0.1f, 0.1f, 0.1f);
        CreatePart(parts, BodyPartName.HatTop, Vector3.zero, 0.05f, 0.05f, 0.05f);


        CreatePart(parts, BodyPartName.LeftEye, Vector3.zero, scale, scale, 0.1f * scale);
        CreatePart(parts, BodyPartName.RightEye, Vector3.zero, scale, scale, 0.1f *  scale);
        */

        // ����һ���µĿյ� GameObject ��Ϊ������
        GameObject parentObject = new GameObject("PartsParent");

        // �������� part�������ǵĸ�������Ϊ�´����ĸ�����
        int cube_index = 0;
        foreach (var part in parts)
        {
            GameObject second = new GameObject("SPartsParent");

            string parent_name = "parent";
            switch (part.name)
            {
                case BodyPartName.Head:
                    parent_name = "Head";
                    break;
                case BodyPartName.Body:
                    parent_name = "Body";
                    break;
                case BodyPartName.LeftArm:
                    parent_name = "LeftArm";
                    break;
                case BodyPartName.RightArm:
                    parent_name = "RightArm";
                    break;
                case BodyPartName.LeftLeg:
                    parent_name = "LeftLeg";
                    break;
                case BodyPartName.RightLeg:
                    parent_name = "RightLeg";
                    break;
                case BodyPartName.HatBottom:
                    parent_name = "HatBottom";
                    break;
                case BodyPartName.HatTop:
                    parent_name = "HatTop";
                    break;
            }
            second.name = parent_name;
            part.actor.name = "cube_" + cube_index.ToString();
            cube_index ++;
            part.actor.transform.SetParent(second.transform);
            second.transform.SetParent(parentObject.transform);
        }

        // ��������µĸ�����
        return parentObject;

    }

    public static GameObject GenerateCircle(int radius, float heightOffset, float scale)
    {

        // �������ںϲ��� CombineInstance ����
        List<CombineInstance> combineList = new List<CombineInstance>();

        List<int> heights = new List<int>
        {
            0
        };

        for (int i = -radius; i <= radius; i++)
        {
            bool finded = false;
            int start = 0;
            int end = 0;

            for(int j = -radius; j <= radius; j++)
            {

                if((i+0.5)*(i+0.5) + (j+0.5)*(j+0.5) <= radius * radius)
                {
                    if(finded == false)
                    {
                        start = j;
                        finded = true;
                    }
                }
                else
                {
                    if(finded == true)
                    {
                        end = j;
                        finded = false;
                        break;
                    }
                }
            }
            // ����������
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0, i) * scale;
            cube.transform.localScale = new Vector3(end * 2, 1, 1) * scale; // �����������С

            // ��ȡ������� MeshFilter
            MeshFilter meshFilter = cube.GetComponent<MeshFilter>();

            // ���� CombineInstance ������ List
            CombineInstance combine = new CombineInstance();
            combine.mesh = meshFilter.mesh;
            combine.transform = cube.transform.localToWorldMatrix;
            combineList.Add(combine);

            // ɾ����ʱ������
            Destroy(cube);
        }

        // �ϲ� Mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineList.ToArray());

        // ����һ���µ� GameObject ������ϲ��� Mesh
        GameObject combinedObject = new GameObject("CombinedCircleMesh");
        combinedObject.AddComponent<MeshFilter>().mesh = combinedMesh;
        combinedObject.AddComponent<MeshRenderer>();

        // ���ϲ����������ϲ���
        combinedObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        return combinedObject;
    }

    public static Material generateMaterial()
    {
        // ����һ���µ� 2x2 ��С�� Texture2D
        Texture2D greenTexture = new Texture2D(2, 2);

        // �����������Ϊ����ɫ
        for (int x = 0; x < greenTexture.width; x++)
        {
            for (int y = 0; y < greenTexture.height; y++)
            {
                greenTexture.SetPixel(x, y, Color.green); // ����Ϊ��ɫ
            }
        }

        greenTexture.SetPixel(0, 0, Color.black);
        greenTexture.SetPixel(1, 0, Color.white);
        greenTexture.SetPixel(0, 1, Color.black);
        greenTexture.SetPixel(1, 1, Color.black);

        // Ӧ�ø��ģ�ʹ�����õ�������Ч
        greenTexture.Apply();
        greenTexture.filterMode = FilterMode.Point;
        //greenTexture = GeneratePoissonNoise();

        // ����һ���µĲ��ʣ�ʹ�� Unity Ĭ�ϵ� Shader
        Material greenMaterial = new Material(Shader.Find("Standard"));

        // ���մ�������ɫ���������ʵ� mainTexture
        greenMaterial.mainTexture = greenTexture;

        return greenMaterial;
    }

    public static GameObject CreateCube()
    {

        // ����һ���յ� GameObject
        GameObject part = new GameObject("CustomCube");

        // ������� MeshFilter �� MeshRenderer ���
        MeshFilter meshFilter = part.AddComponent<MeshFilter>(); 
        MeshRenderer renderer =  part.AddComponent<MeshRenderer>();
        //renderer.material = GeneratePoissonNoise();


        // �����µ� Mesh
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // ����24�����㣨ÿ������4�����㣩
        Vector3[] vertices = new Vector3[24]
        {
            // Back face
            new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1), 
            // Front face
            new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1),
            // Left face
            new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1),
            // Right face
            new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1),
            // Top face
            new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1),
            // Bottom face
            new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(1, -1, 1), new Vector3(-1, -1, 1)
        };
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= 0.5f;
        }

        // ����������������ÿ�������������Σ�
        int[] triangles = new int[36]
        {
            // Back face
            0, 2, 1, 0, 3, 2,
            // Front face
            4, 5, 6, 4, 6, 7,
            // Left face
            8, 10, 9, 8, 11, 10,
            // Right face
            12, 13, 14, 12, 14, 15,
            // Top face
            16, 18, 17, 16, 19, 18,
            // Bottom face
            20, 21, 22, 20, 22, 23
        };

        // ���� UV ���꣨ÿ����4��UV��
        Vector2[] uvs = new Vector2[24]
        {
            // Back face
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            // Front face
            new Vector2(1f, 0f), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1),
            // Left face
            new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0),
            // Right face
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
            // Top face
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            // Bottom face
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)
        };

        // ���� Mesh �Ķ��㡢�����κ� UV
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // ���¼��㷨���Ա�֤����Ч��
        mesh.RecalculateNormals();

        return part;
    }


    public static void RecomputeUV2(List<Vector2> uvs, int[] index, Uint2 start, Uint2 size, Uint2 textureSize)
    {
        /*
         3 --- 2
               
         0 --- 1
         */
        float invx = 1.0f / textureSize.x;
        float invy = 1.0f / textureSize.y;
        Vector2 uv0 = new Vector2(start.x * invx, start.y * invy);
        Vector2 uv1 = new Vector2((start.x + size.x) * invx, start.y * invy);
        Vector2 uv2 = new Vector2((start.x + size.x) * invx, (start.y + size.y) * invy);
        Vector2 uv3 = new Vector2(start.x * invx, (start.y + size.y) * invy);
        uvs[index[0]] = uv0;
        uvs[index[1]] = uv1;
        uvs[index[2]] = uv2;
        uvs[index[3]] = uv3; 

    }

    public static List<Vector2> ComputeUVs(Uint3 size)
    {
        Uint2 tSize = new Uint2((size.x + size.z) * 2, size.y + size.z);
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < 24; i++)
        {
            uvs.Add(new Vector2(0, 0));
        }
        RecomputeUV2(uvs, new int[] { 0, 1, 2, 3 }, new Uint2(size.z, 0), new Uint2(size.x, size.y), tSize);
        RecomputeUV2(uvs, new int[] { 4, 5, 6, 7 }, new Uint2(size.z + size.z + size.x, 0), new Uint2(size.x, size.y), tSize);
        RecomputeUV2(uvs, new int[] { 8, 9, 10, 11 }, new Uint2(0, 0), new Uint2(size.z, size.y), tSize);
        RecomputeUV2(uvs, new int[] { 12, 13, 14, 15 }, new Uint2(size.x + size.z, 0), new Uint2(size.z, size.y), tSize);
        RecomputeUV2(uvs, new int[] { 16, 17, 18, 19 }, new Uint2(size.z, size.y), new Uint2(size.z, size.x), tSize);
        RecomputeUV2(uvs, new int[] { 20, 21, 22, 23 }, new Uint2(size.x + size.z, size.y), new Uint2(size.z, size.x), tSize);
        return uvs;
    }
    public static void CreatePart(List<BodyPart> parts, BodyPartName partName, Vector3 position, float width, float height, float depth)
    {

        GameObject part ;
        BodyPart thePart;

        switch (partName)
        {
            case BodyPartName.Head:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);
                    Uint3 size = new Uint3(8, 8, 8);
                    part.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                    part.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, BodyPartName.Head, size);

                    thePart = new BodyPart(part, partName);
                    BodyPart body = FindBodyPart(parts, BodyPartName.Body);
                    Vector3 offset = body.TopCenterConnector() - thePart.BottomCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.LeftArm:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);

                    Uint3 size = new Uint3(8, 8, 8);
                    part.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                    part.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, BodyPartName.LeftArm, size);

                    thePart = new BodyPart(part, partName);
                    BodyPart body = FindBodyPart(parts, BodyPartName.Body);
                    Vector3 offset = body.RightHandConnector() - thePart.LeftHandConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.RightArm:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);

                    Uint3 size = new Uint3(8, 8, 8);
                    part.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                    part.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, BodyPartName.RightArm, size);

                    thePart = new BodyPart(part, partName);
                    BodyPart body = FindBodyPart(parts, BodyPartName.Body);
                    Vector3 offset = body.LeftHandConnector() - thePart.RightHandConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.LeftLeg:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);

                    Uint3 size = new Uint3(8, 8, 8);
                    part.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                    part.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, BodyPartName.LeftLeg, size);

                    thePart = new BodyPart(part, partName);
                    BodyPart body = FindBodyPart(parts, BodyPartName.Body);
                    Vector3 offset = body.BottomLeftConnector() - thePart.TopCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.RightLeg:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);

                    Uint3 size = new Uint3(8, 8, 8);
                    part.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                    part.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, BodyPartName.RightLeg, size);

                    thePart = new BodyPart(part, partName);
                    BodyPart body = FindBodyPart(parts, BodyPartName.Body);
                    Vector3 offset = body.BottomRightConnector() - thePart.TopCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.HatBottom:
                {
                    part = GenerateCircle(4,0,1);
                    part.transform.localPosition = position;
                    Vector3 ls = part.transform.localScale;
                    part.transform.localScale =  new Vector3(width * ls.x, height * ls.y, depth * ls.z);
                    thePart = new BodyPart(part, partName);
                    Vector3 offset = FindBodyPart(parts, BodyPartName.Head).TopCenterConnector() - thePart.BottomCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    Debug.Log(" bot = " + thePart.BottomCenterConnector());
                    Debug.Log(" top = " + FindBodyPart(parts, BodyPartName.Head).TopCenterConnector());
                    Debug.Log(" local = " + thePart.actor.transform.localPosition);

                    break;
                }
            case BodyPartName.HatTop:
                {
                    part = GenerateCircle(4, 0, 1);
                    part.transform.localPosition = position;
                    Vector3 ls = part.transform.localScale;
                    part.transform.localScale = new Vector3(width * ls.x, height * ls.y, depth * ls.z);
                    thePart = new BodyPart(part, partName);
                    Vector3 offset = FindBodyPart(parts, BodyPartName.HatBottom).TopCenterConnector() - thePart.BottomCenterConnector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.LeftEye:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);
                    part.GetComponent<MeshRenderer>().material = generateMaterial();
                    thePart = new BodyPart(part, partName);
                    Vector3 offset = FindBodyPart(parts, BodyPartName.Head).FrontLeftConntector() - thePart.BackCenterConntector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.RightEye:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);
                    part.GetComponent<MeshRenderer>().material = generateMaterial();



                    thePart = new BodyPart(part, partName);
                    Vector3 offset = FindBodyPart(parts, BodyPartName.Head).FrontRightEyeConntector() - thePart.BackCenterConntector();
                    thePart.actor.transform.localPosition += offset;
                    break;
                }
            case BodyPartName.Body:
                {
                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);
                    Uint3 size = new Uint3(8,8,8);
                    part.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                    part.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, BodyPartName.Body, size);
                    thePart = new BodyPart(part, partName);
                    break;
                }
            default:
                {

                    part = CreateCube();
                    part.transform.localPosition = position;
                    part.transform.localScale = new Vector3(width, height, depth);
                    thePart = new BodyPart(part, partName);
                    break;
                };
        }
        

        parts.Add(thePart);

    }
}
