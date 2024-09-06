using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Structure
{
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
        public MinecraftAnimation.Animation animation;
        public Vector3 posStart;
        public Vector3 posEnd;
        public Quaternion rotationStart;
        public Quaternion rotationEnd;
        public bool active;
        public ActorSettings(int frameStart, int frameEnd, GameObject actor, MinecraftFighter.Animation animation, Vector3 pos, Quaternion rotation, bool active)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.actor = actor;
            this.animation = MinecraftAnimation.Animation.Wait;
            this.posStart = pos;
            this.posEnd = pos;
            this.rotationStart = rotation;
            this.rotationEnd = rotation;
            this.active = active;
        }

        public ActorSettings(int frameStart, int frameEnd, GameObject actor, MinecraftAnimation.Animation animation, 
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd, bool active)
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
        }

        public bool Run(int frameCount)
        {
            if (frameCount >= this.frameStart && frameCount < this.frameEnd)
            {
                if(this.active == false)
                {
                    Debug.Log("set false");
                    actor.SetActive(false);
                    return true;
                }
                if(actor.GetComponent<MinecraftAnimation>() != null)
                {
                    actor.GetComponent<MinecraftAnimation>().SetAnimation(animation); 
                    float ratio = (frameCount - this.frameStart) * 1.0f / (this.frameEnd - this.frameStart);
                    Vector3 pos = Vector3.Lerp(posStart, posEnd, ratio);
                    Quaternion rot = Quaternion.Lerp(rotationStart, rotationEnd, ratio);
                    actor.GetComponent<MinecraftAnimation>().SetTransform(pos, rot);
                }


                return true;
            }
            return false;
        }
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, MinecraftAnimation.Animation animation,
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, rotationStart, rotationEnd, true);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, bool active)
    {
        return new ActorSettings(frameStart, frameEnd, actor, MinecraftAnimation.Animation.Wait, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity, active);
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
        // 检查当前Transform的名称是否是我们要找的
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

        // 递归遍历所有子物体
        foreach (Transform child in current)
        {
            RecursiveFindAndModify(targetName, child, rotation, local);
        }
    }

    public static float RecursiveFindAndLookat(string targetName, Transform current, Vector3 lookat)
    {
        // 检查当前Transform的名称是否是我们要找的
        if (current.name == targetName)
        {
            Vector3 directionToLookAt = lookat - current.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLookAt) * Quaternion.Euler(0, 180, 0);

            // 计算当前旋转和目标旋转之间的角度差
            float angleDifference = Quaternion.Angle(Quaternion.identity, targetRotation);
           // Debug.Log("name " + targetName + " angles = " + angleDifference);
            // 限制旋转角度
            if (angleDifference > 60)
            {
                // 按最大角度旋转
                current.rotation = Quaternion.RotateTowards(Quaternion.identity, targetRotation, 60);

                return angleDifference;
            }
            else
            {
                // 直接朝向目标
                current.rotation = targetRotation;
                return angleDifference;
            }
        }

        // 递归遍历所有子物体
        foreach (Transform child in current)
        {

            return RecursiveFindAndLookat(targetName, child, lookat);
        }
        return 0;
    }

    public static int[] ReadNumbersFromFile(string filePath)
    {
        // 读取文件内容并去除前后空白
        string line = File.ReadAllText(filePath).Trim();

        // 根据空格拆分成字符串数组
        string[] parts = line.Split(' ');

        // 将字符串数组转换成整型数组
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
        // 去掉不需要的字符，只保留数字和逗号
        line = line.Replace("\"", "")   // 去掉双引号
                   .Replace(":", "")    // 去掉冒号
                   .Replace("[", "")    // 去掉左括号
                   .Replace("]", "")    // 去掉右括号
                   .Replace(",", " ");

        // 按空格分隔字符串并解析为浮点数
        string[] parts = line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        float firstValue = float.Parse(parts[0]);
        float x = float.Parse(parts[1]);
        float y = float.Parse(parts[2]);
        float z = float.Parse(parts[3]);

        // 返回Vector4
        return new Vector4(firstValue, x, y, z);
    }

    public static int[] ReadNumbersFromFile2(string filePath)
    {
        // 读取文件内容并去除前后空白
        string line = File.ReadAllText(filePath).Trim();

        // 根据空格拆分成字符串数组
        string[] parts = line.Split(' ');

        // 将字符串数组转换成整型数组
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
    public static void AddBoxCollider(GameObject obj)
    {
        // 获取所有的 MeshRenderer 组件
        MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();

        if (meshRenderers.Length == 0)
        {
            Debug.LogWarning("没有找到任何MeshRenderer组件。");
            return;
        }

        // 初始化第一个 bounds 为合并的起点
        Bounds combinedBounds = meshRenderers[0].bounds;

        // 遍历所有的 MeshRenderer 并合并它们的 bounds
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            combinedBounds.Encapsulate(meshRenderer.bounds);
        }

        // 获取 BoxCollider，如果不存在则添加一个
        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = obj.AddComponent<BoxCollider>();
        }

        // 设置 BoxCollider 的中心和大小
        boxCollider.center = combinedBounds.center - obj.transform.position;
        boxCollider.size = combinedBounds.size;
        boxCollider.isTrigger = true;
    }
}
