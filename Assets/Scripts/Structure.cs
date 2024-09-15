using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Structure
{
    public static class DataTransfer
    {
        public static string messageToPass;
        public static string prefabName;
        public static GameObject actor;
        public static float mobFootOffset;
        public static float mobEyeOffset;
        public static float playerFootOffset;
        public static float playerEyeOffset;
    }
    public struct Uint2
    {
        public uint x;
        public uint y;

        public Uint2(uint x, uint y)
        {
            this.x = x;
            this.y = y;
        }

        public Uint2(int width, int height) 
        {
            this.x = (uint)width;
            this.y = (uint)height;
        }

        // ��д ToString ���������ڵ���ʱ���
        public override string ToString()
        {
            return $"Uint2({x}, {y})";
        }

        // ���� + �����
        public static Uint2 operator +(Uint2 a, Uint2 b)
        {
            return new Uint2(a.x + b.x, a.y + b.y);
        }

        // ��ѡ: ���� - �����
        public static Uint2 operator -(Uint2 a, Uint2 b)
        {
            return new Uint2(a.x - b.x, a.y - b.y);
        }

        // ��ѡ: ���� * �����
        public static Uint2 operator *(Uint2 a, uint scalar)
        {
            return new Uint2(a.x * scalar, a.y * scalar);
        }
    }
    public struct Uint3
    {
        public uint x;
        public uint y;
        public uint z;

        public Uint3(uint x, uint y, uint z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // ��д ToString ���������ڵ���ʱ���
        public override string ToString()
        {
            return $"Uint3({x}, {y}, {z})";
        }
    }

    public struct Uint4
    {
        public uint x;
        public uint y;
        public uint z;
        public uint w;

        public Uint4(uint x, uint y, uint z, uint w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        // ��д ToString ���������ڵ���ʱ���
        public override string ToString()
        {
            return $"Uint4({x}, {y}, {z}, {w})";
        }
    }
    public struct CameraSetting
    {
        public int frameStart;
        public int frameEnd;
        public GameObject lookat;
        public Vector3 lookatOffset;
        public Vector3 posOffsetStart;
        public Vector3 posOffsetEnd;
        public bool detectDead;

        public CameraSetting(int frameStart, int frameEnd,  Vector3 posOffset, GameObject lookat, Vector3 lookatOffset, bool detectDead)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.lookatOffset = lookatOffset;
            this.posOffsetStart = posOffset;
            this.posOffsetEnd = posOffset;
            this.lookat = lookat;
            this.detectDead = detectDead;
        }

        public CameraSetting(int frameStart, int frameEnd,  Vector3 posOffset, Vector3 posOffset2, GameObject lookat, Vector3 lookatOffset)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd; 
            this.lookatOffset = lookatOffset;
            this.posOffsetStart = posOffset;
            this.posOffsetEnd = posOffset2;
            this.lookat = lookat;
            this.detectDead = false;
        }

        public bool Run(int frameCount)
        {
            if(frameCount >= frameStart && frameCount < frameEnd) { 
                float ratio = (frameCount - frameStart) * 1.0f/ (frameEnd - frameStart);
                Vector3 realPosOffset = Vector3.Lerp(posOffsetStart, posOffsetEnd, ratio);
                Camera.main.transform.position = lookat.transform.position + realPosOffset;
                Camera.main.transform.LookAt(lookat.transform.position + lookatOffset);

                return true;
            }
            return false;
        }

        public void AddBuildTime(int count)
        {
            this.frameEnd += count;
        }
    }



    public static CameraSetting addCameraMove(int frameStart, int frameEnd,  Vector3 pos, Vector3 pos2, GameObject lookat, Vector3 lookatOffset)
    {

        return new CameraSetting(frameStart, frameEnd, pos, pos2, lookat, lookatOffset);
    }

    public struct ActorSettings
    {
        public int frameStart;
        public int frameEnd;
        public GameObject actor;
        public AnimationSystem.Animation animation;
        public Vector3 posStart;
        public Vector3 posEnd;
        public Quaternion rotationStart;
        public Quaternion rotationEnd;
        public GameObject lookat;
        public bool active;
        public ActorSettings(int frameStart, int frameEnd, GameObject actor, MinecraftFighter.Animation animation, Vector3 pos, Quaternion rotation, bool active, GameObject lookat)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.actor = actor;
            this.animation = AnimationSystem.Animation.Wait;
            this.posStart = pos;
            this.posEnd = pos;
            this.rotationStart = rotation;
            this.rotationEnd = rotation;
            this.active = active;
            this.lookat = lookat;
        }

        public ActorSettings(int frameStart, int frameEnd, GameObject actor, AnimationSystem.Animation animation, 
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd, bool active, GameObject lookat)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.actor = actor;
            this.animation = animation;
            this.posStart = posStart;
            this.posEnd = posEnd;
            this.rotationStart = rotationStart;
            this.rotationEnd = rotationEnd;
            this.active = active;
            this.lookat = lookat;
        }

        public bool Run(int frameCount)
        {
            SetMeshRenderer(actor.transform, true);
            if (frameCount >= this.frameStart && frameCount < this.frameEnd)
            {
                if(this.active == false)
                {

                    SetMeshRenderer(actor.transform, false);
                    return true;
                }
                if(actor.GetComponent<AnimationSystem>() != null)
                {
                    actor.GetComponent<AnimationSystem>().SetAnimation(animation); 
                    float ratio = (frameCount - this.frameStart) * 1.0f / (this.frameEnd - this.frameStart);
                    Vector3 pos = Vector3.Lerp(posStart, posEnd, ratio);
                    Quaternion rot = Quaternion.Lerp(rotationStart, rotationEnd, ratio);
                    
                    actor.GetComponent<AnimationSystem>().SetTransform(pos, rot);
                }


                return true;
            }
            return false;
        }
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, AnimationSystem.Animation animation,
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, rotationStart, rotationEnd, true, null);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, AnimationSystem.Animation animation,
        Vector3 posStart, Vector3 posEnd)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, Quaternion.identity, Quaternion.identity, true, null);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, AnimationSystem.Animation animation,
    Vector3 posStart, Vector3 posEnd, GameObject lookat)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, Quaternion.identity, Quaternion.identity, true, lookat);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, bool active)
    {
        return new ActorSettings(frameStart, frameEnd, actor, AnimationSystem.Animation.Wait, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity, active, null);
    }

    public struct ObjectSettings
    {
        public int frameStart;
        public int frameEnd;
        public GameObject thing;
        public Vector3 posStart;
        public Vector3 posEnd;
        public Quaternion rotationStart;
        public Quaternion rotationEnd;
        public ObjectSettings(int frameStart, int frameEnd, GameObject thing, Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.thing = thing;
            this.posStart = posStart;
            this.posEnd = posEnd;
            this.rotationStart = rotationStart;
            this.rotationEnd = rotationEnd;
        }
    }

    public static void RotateChild(Transform parent, string targetName, Quaternion rotation)
    {
        if (parent.name == targetName || targetName == "Main")
        {
            parent.localRotation = rotation;
            return;
        }
        foreach (Transform child in parent)
        {
            RotateChild(child, targetName, rotation);
        }
    }

    public static void RotateChildGlobal(Transform parent, string targetName, Quaternion rotation)
    {
        if (parent.name == targetName)
        {
            parent.rotation = rotation;
            return;
        }
        foreach (Transform child in parent)
        {
            RotateChildGlobal(child, targetName, rotation);
        }
    }

    public static void TranslateChild(Transform parent, string targetName, Vector3 pos)
    {
        

        if (parent.name == targetName || targetName == "Main")
        {
            parent.localPosition = pos;
            return;
        }
        foreach (Transform child in parent)
        {
            TranslateChild(child, targetName, pos);
        }
    }

    public static Vector3 GetChildPos(Transform parent, string targetName)
    {
        if (parent.name == targetName)
        {
            return parent.position;
        }
        foreach (Transform child in parent)
        {
            return GetChildPos(child, targetName);
        }
        return Vector3.zero;
    }

    public static void RecursiveFindAndModify(string targetName, Transform current, Quaternion rotation, bool local)
    {
        // ��鵱ǰTransform�������Ƿ�������Ҫ�ҵ�
        if (current.name == targetName)
        {
            if (!local)
            {

                current.rotation = rotation;
            }
            else
            {
                current.localRotation = rotation;
            }
            return;
        }

        // �ݹ��������������
        foreach (Transform child in current)
        {
            RecursiveFindAndModify(targetName, child, rotation, local);
        }
    }

    public static float RecursiveFindAndLookat(string targetName, Transform current, Vector3 lookat)
    {
        // ��鵱ǰTransform�������Ƿ�������Ҫ�ҵ�
        if (current.name == targetName)
        {
            Vector3 directionToLookAt = lookat - current.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLookAt) * Quaternion.Euler(0, 180, 0);

            // ���㵱ǰ��ת��Ŀ����ת֮��ĽǶȲ�
            float angleDifference = Quaternion.Angle(Quaternion.identity, targetRotation);
           // Debug.Log("name " + targetName + " angles = " + angleDifference);
            // ������ת�Ƕ�
            if (angleDifference > 60)
            {
                // �����Ƕ���ת
                current.rotation = Quaternion.RotateTowards(Quaternion.identity, targetRotation, 60);

                return angleDifference;
            }
            else
            {
                // ֱ�ӳ���Ŀ��
                current.rotation = targetRotation;
                return angleDifference;
            }
        }

        // �ݹ��������������
        foreach (Transform child in current)
        {

            return RecursiveFindAndLookat(targetName, child, lookat);
        }
        return 0;
    }

    public static int[] ReadNumbersFromFile(string filePath)
    {
        // ��ȡ�ļ����ݲ�ȥ��ǰ��հ�
        string line = File.ReadAllText(filePath).Trim();

        // ���ݿո��ֳ��ַ�������
        string[] parts = line.Split(' ');

        // ���ַ�������ת������������
        int[] numbers = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            numbers[i] = int.Parse(parts[i]);
        }

        return numbers;
    }

    public struct SubModelReplacer
    {
        public string name;
        public string desc;
        public string dir;
        public string color;
        public string like;

        public SubModelReplacer(string name, string des, string dir, string color, string like)
        {
            this.name = name;
            this.desc = des;
            this.dir = dir;
            this.color = color;
            this.like = like;
        }
    }
    public static string AddModelSub(string[] sentences, SubModelReplacer replacer)
    {
        int r = UnityEngine.Random.Range(0, sentences.Length);
        string subtitle = sentences[r];
        subtitle = subtitle.Replace("_part", replacer.name);
        subtitle = subtitle.Replace("_desc", replacer.desc);
        subtitle = subtitle.Replace("_dir", replacer.dir);
        subtitle = subtitle.Replace("_color", replacer.color);
        subtitle = subtitle.Replace("_like", replacer.like);
        subtitle = System.Text.RegularExpressions.Regex.Replace(subtitle, @"\d", "");
        return subtitle;
    }

    public struct SubReplacer
    {
        public string name;
        public string env;
        public string enemy;
        public string attack_weapon;
        public string score;

        public SubReplacer(string name, string env, string enemy, string attack_weapon, string score)
        {
            this.name = name;
            this.env = env;
            this.enemy = enemy;
            this.attack_weapon = attack_weapon;
            this.score = score;
        }
    }

    public static string AddSub2(string[] sentences, SubReplacer replacer)
    {
        int r = UnityEngine.Random.Range(0, sentences.Length);
        string subtitle = sentences[r];
        subtitle = subtitle.Replace("_env", replacer.env);
        subtitle = subtitle.Replace("_name", replacer.name);
        subtitle = subtitle.Replace("_enemy", replacer.enemy);
        subtitle = subtitle.Replace("_attack_weapon", replacer.attack_weapon);
        subtitle = subtitle.Replace("_score", replacer.score);
        subtitle = System.Text.RegularExpressions.Regex.Replace(subtitle, @"\d", "");
        return subtitle;
    }

    public static Texture2D LoadTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }

    public static Material AddMaterial(string path)
    {
        Texture2D grass_texture = LoadTexture(path);
        grass_texture.filterMode = FilterMode.Point;
        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = grass_texture;
        return mat;
    }

    public static Vector4 ParseLineToVector4(string line)
    {
        // ȥ������Ҫ���ַ���ֻ�������ֺͶ���
        line = line.Replace("\"", "")   // ȥ��˫����
                   .Replace(":", "")    // ȥ��ð��
                   .Replace("[", "")    // ȥ��������
                   .Replace("]", "")    // ȥ��������
                   .Replace(",", " ");

        // ���ո�ָ��ַ���������Ϊ������
        string[] parts = line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        float firstValue = float.Parse(parts[0]);
        float x = float.Parse(parts[1]);
        float y = float.Parse(parts[2]);
        float z = float.Parse(parts[3]);

        // ����Vector4
        return new Vector4(firstValue, x, y, z);
    }

    public static int[] ReadNumbersFromFile2(string filePath)
    {
        // ��ȡ�ļ����ݲ�ȥ��ǰ��հ�
        string line = File.ReadAllText(filePath).Trim();

        // ���ݿո��ֳ��ַ�������
        string[] parts = line.Split(' ');

        // ���ַ�������ת������������
        int[] numbers = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            numbers[i] = int.Parse(parts[i]);
        }

        return numbers;
    }

    public static int CountDirectChildCubes(Transform parent)
    {
        int cubeCount = 0;

        // Iterate through all direct child objects
        foreach (Transform child in parent)
        {
            if (child.name.Contains("cube"))
            {
                cubeCount++;
            }
        }

        return cubeCount;
    }


    public static void SetChildLocalScale(Transform parent, string targetName, Vector3 scale)
    {
        if (parent.name == targetName)
        {
            parent.localScale = scale;
            return;
        }
        foreach (Transform child in parent)
        {
            SetChildLocalScale(child, targetName, scale);
        }
    }

    public static void SetChildLocalPos(Transform parent, string targetName, Vector3 pos)
    {
        if (parent.name == targetName)
        {
            parent.localPosition = pos;
            return;
        }
        foreach (Transform child in parent)
        {
            SetChildLocalPos(child, targetName, pos);
        }
    }
    public static void SetChildActive(Transform parent, string targetName)
    {
        if (parent.name == targetName)
        {
            parent.gameObject.SetActive(true);
            return;
        }
        foreach (Transform child in parent)
        {
            SetChildActive(child, targetName);
        }
    }

    public static Vector3 GetChildSize(Transform parent, string targetName)
    {
        if (parent.name == targetName)
        {
            return parent.gameObject.GetComponent<MeshRenderer>().bounds.size;
        }
        foreach (Transform child in parent)
        {
            return GetChildSize(child, targetName);
        }
        return Vector3.zero;
    }
    public static float FindModelOffset(Transform current)
    {
        if (current.parent != null && current.parent.name == "LeftLeg")
        {
            return -current.transform.position.y + current.transform.localScale.y / 2;
        }
        foreach (Transform child in current)
        {
            float v = FindModelOffset(child);
            if(v != 0)
            {
                return v;
            }
        }
        return 0;
    }
    public static void SetChildMaterial(Transform parent, string targetName, Material material)
    {
        if (parent.name == targetName)
        {
            if (parent.GetComponent<MeshRenderer>() != null)
            {
                parent.GetComponent<MeshRenderer>().material = material;
            }
            return;
        }
        foreach (Transform child in parent)
        {
            SetChildMaterial(child, targetName, material);
        }
    }

    public static void SetMeshRenderer(Transform parent,  bool active)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            parent.GetComponent<MeshRenderer>().enabled = active;
        }
        foreach (Transform child in parent)
        {
            SetMeshRenderer(child, active);
        }
    }
    public static void AddBoxCollider(GameObject obj)
    {
        // ��ȡ���е� MeshRenderer ���
        MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();

        if (meshRenderers.Length == 0)
        {
            Debug.LogWarning("û���ҵ��κ�MeshRenderer�����");
            return;
        }

        // ��ʼ����һ�� bounds Ϊ�ϲ������
        Bounds combinedBounds = meshRenderers[0].bounds;

        // �������е� MeshRenderer ���ϲ����ǵ� bounds
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            combinedBounds.Encapsulate(meshRenderer.bounds);
        }

        // ��ȡ BoxCollider����������������һ��
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = obj.AddComponent<BoxCollider>();
        }

        // ���� BoxCollider �����ĺʹ�С
        boxCollider.center = combinedBounds.center - obj.transform.position;
        boxCollider.size = combinedBounds.size;
        boxCollider.isTrigger = true;
    }

    public static GameObject CreateCube()
    {

        // ����һ���յ� GameObject
        GameObject part = new GameObject("CustomCube");

        // ������� MeshFilter �� MeshRenderer ���
        MeshFilter meshFilter = part.AddComponent<MeshFilter>();
        MeshRenderer renderer = part.AddComponent<MeshRenderer>();
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
        RecomputeUV2(uvs, new int[] { 11, 8, 9, 10 }, new Uint2(0, 0), new Uint2(size.z, size.y), tSize);
        RecomputeUV2(uvs, new int[] { 12, 15, 14, 13 }, new Uint2(size.x + size.z, 0), new Uint2(size.z, size.y), tSize);
        RecomputeUV2(uvs, new int[] { 16, 17, 18, 19 }, new Uint2(size.z, size.y), new Uint2(size.z, size.x), tSize);
        RecomputeUV2(uvs, new int[] { 20, 21, 22, 23 }, new Uint2(size.x + size.z, size.y), new Uint2(size.z, size.x), tSize);
        return uvs;
    }
}
