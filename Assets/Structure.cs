using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Structure
{
    public struct CameraSetting
    {
        public int frameStart;
        public int frameEnd;
        public GameObject lookat;
        public Vector3 posOffsetStart;
        public Vector3 posOffsetEnd;
        public bool detectDead;

        public CameraSetting(int frameStart, int frameEnd, Vector3 posOffset, GameObject lookat, bool detectDead)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.posOffsetStart = posOffset;
            this.posOffsetEnd = posOffset;
            this.lookat = lookat;
            this.detectDead = detectDead;
        }

        public CameraSetting(int frameStart, int frameEnd, Vector3 posOffset, Vector3 posOffset2, GameObject lookat)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
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
                Camera.main.transform.LookAt(lookat.transform.position);

                return true;
            }
            return false;
        }
    }

    public static CameraSetting addCameraMove(int frameStart, int frameEnd, Vector3 pos, Vector3 pos2, GameObject lookat)
    {
        return new CameraSetting(frameStart, frameEnd, pos, pos2, lookat);
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
        public ActorSettings(int frameStart, int frameEnd, GameObject actor, MinecraftFighter.Animation animation, Vector3 pos, Quaternion rotation)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.actor = actor;
            this.animation = MinecraftAnimation.Animation.Wait;
            this.posStart = pos;
            this.posEnd = pos;
            this.rotationStart = rotation;
            this.rotationEnd = rotation;
        }

        public ActorSettings(int frameStart, int frameEnd, GameObject actor, MinecraftAnimation.Animation animation, 
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.actor = actor;
            this.animation = animation;
            this.posStart = posStart;
            this.posEnd = posEnd;
            this.rotationStart = rotationStart;
            this.rotationEnd = rotationEnd;
        }

        public bool Run(int frameCount)
        {
            if (frameCount >= this.frameStart && frameCount < this.frameEnd)
            {
                actor.GetComponent<MinecraftAnimation>().SetAnimation(animation);
                float ratio = (frameCount - this.frameStart) * 1.0f / (this.frameEnd - this.frameStart);
                Vector3 pos = Vector3.Lerp(posStart, posEnd, ratio);
                Quaternion rot = Quaternion.Lerp(rotationStart, rotationEnd, ratio);
                actor.GetComponent<MinecraftAnimation>().SetTransform(pos, rot);

                return true;
            }
            return false;
        }
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, MinecraftAnimation.Animation animation,
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, rotationStart, rotationEnd);
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
        if (parent.name == targetName)
        {
            parent.localRotation = rotation;
            return;
        }
        foreach (Transform child in parent)
        {
            RotateChild(child, targetName, rotation);
        }
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

    public static string AddSub(string[] sentences, string part_name, string desc, string dir, string color)
    {
        int r = UnityEngine.Random.Range(0, sentences.Length);
        string subtitle = sentences[r];
        subtitle = subtitle.Replace("_part", part_name);
        subtitle = subtitle.Replace("_desc", desc);
        subtitle = subtitle.Replace("_dir", dir);
        subtitle = subtitle.Replace("_color", color);
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

}
