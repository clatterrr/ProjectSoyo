using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static AnimationSystem;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

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

        public static string featureDesc;
        public static string featurePart;
        public static List<int> indexToIndex;
        public static int startIndex = 0;
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

        // 重写 ToString 方法，便于调试时输出
        public override string ToString()
        {
            return $"Uint2({x}, {y})";
        }

        // 重载 + 运算符
        public static Uint2 operator +(Uint2 a, Uint2 b)
        {
            return new Uint2(a.x + b.x, a.y + b.y);
        }

        // 可选: 重载 - 运算符
        public static Uint2 operator -(Uint2 a, Uint2 b)
        {
            return new Uint2(a.x - b.x, a.y - b.y);
        }

        // 可选: 重载 * 运算符
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

        // 重写 ToString 方法，便于调试时输出
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

        // 重写 ToString 方法，便于调试时输出
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
        public GameObject follow;
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
            this.follow = null;
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
            this.follow = null;
        }

        public CameraSetting(int frameStart, int frameEnd, Vector3 posOffset, Vector3 posOffset2, GameObject lookat, Vector3 lookatOffset, GameObject follow)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.lookatOffset = lookatOffset;
            this.posOffsetStart = posOffset;
            this.posOffsetEnd = posOffset2;
            this.lookat = lookat;
            this.detectDead = false;
            this.follow = follow;
        }

        public bool Run(int frameCount)
        {
            if(frameCount >= frameStart && frameCount < frameEnd)
            {
                float ratio = (frameCount - frameStart) * 1.0f / (frameEnd - frameStart);
                Vector3 realPosOffset = Vector3.Lerp(posOffsetStart, posOffsetEnd, ratio);
                if (follow == null)
                {
                    Camera.main.transform.position = lookat.transform.position + realPosOffset;
                }
                else
                {
                    Camera.main.transform.position = follow.transform.position + realPosOffset;
                }
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

    public static CameraSetting addCameraMove(int frameStart, int frameEnd, Vector3 pos,  GameObject lookat, Vector3 lookatOffset)
    {

        return new CameraSetting(frameStart, frameEnd, pos, pos, lookat, lookatOffset);
    }

    public static CameraSetting addCameraMove(int frameStart, int frameEnd,  Vector3 pos, Vector3 pos2, GameObject lookat, Vector3 lookatOffset)
    {

        return new CameraSetting(frameStart, frameEnd, pos, pos2, lookat, lookatOffset);
    }

    public static CameraSetting addCameraMove(int frameStart, int frameEnd, Vector3 pos, Vector3 pos2, GameObject lookat, Vector3 lookatOffset, GameObject follow)
    {

        return new CameraSetting(frameStart, frameEnd, pos, pos2, lookat, lookatOffset, follow);
    }

    enum AnimPart
    {
        Body,
        Leg,
        Arm,
        Head, // 主要是看向哪儿
        FullBody,
        None,
    }

    enum AnimObject
    {
        Ground,
        Lookat,
    }

    enum RotationStrategy
    {
        LookAtSomething,
        Forward,
    }

    // 我觉得还是段落比较好
    public struct AnimFrameKey
    {
        AnimPart animPart;
        theAnim anim; // 是否有动画从这帧开始
        bool animStart; // 动画是开始还是结束 还有过渡呢，边跑步边受伤 动画结束后，是应该Idle 还是 转入下一个动画
        AnimObject animObject;
        public int frameStart;
        public int frameEnd;
        public Vector3 posStart;
        public Vector3 posEnd;
       // public RotationStrategy rotationStrategy;
        public Quaternion rotStart;
        public Quaternion rotEnd;
    }

    // Actor Type 可能是复数
    public enum ActorType
    {
        None,

        Player,
        Friend,
        Enemy,

        CameraFollow,
        CameraLookat,

        Stranger1,
        Stranger2,
        Camera,
    }
    public struct ActorSettings
    {
        public int frameStart;
        public int frameEnd;
        public GameObject actor;
        public theAnim animation;
        public Vector3 posStart;
        public Vector3 posEnd;
        public Quaternion rotationStart;
        public Quaternion rotationEnd;
        public GameObject lookat;
        public bool active;
        public ActorType type;
        public ActorSettings(int frameStart, int frameEnd, GameObject actor, MinecraftFighter.Animation animation, Vector3 pos, Quaternion rotation, bool active, GameObject lookat)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
            this.actor = actor;
            this.animation = theAnim.Wait;
            this.posStart = pos;
            this.posEnd = pos;
            this.rotationStart = rotation;
            this.rotationEnd = rotation;
            this.active = active;
            this.lookat = lookat;
            type = ActorType.Player;
        }

        public ActorSettings(int frameStart, int frameEnd, GameObject actor, theAnim animation, 
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
            type    =ActorType.Player;
        }

        public bool Run(int frameCount)
        {
            if (frameCount >= this.frameStart && frameCount < this.frameEnd)
            {
                if(this.active == false)
                {
                    this.actor.gameObject.SetActive(false);
                    return true;
                }
                else
                {

                    this.actor.gameObject.SetActive(true);
                }
                if(actor.GetComponent<AnimationSystem>() != null)
                {
                    actor.GetComponent<AnimationSystem>().SetAnimation(animation); 
                    float ratio = (frameCount - this.frameStart) * 1.0f / (this.frameEnd - this.frameStart);
                    Vector3 pos = Vector3.Lerp(posStart, posEnd, ratio);

                    // walk _customRoate

                    Vector3 speed = posEnd - posStart;
                    if(speed.magnitude < 0.01f)
                    {

                        actor.GetComponent<AnimationSystem>().SetTransform(pos, Quaternion.identity);
                    }
                    else
                    {

                        Quaternion rot = Quaternion.LookRotation(speed.normalized);
                        actor.GetComponent<AnimationSystem>().SetTransform(pos, rot);
                    }


                }


                return true;
            }
            return false;
        }

        public bool RunWithHeight(int frameCount, Dictionary<Vector2Int, int> heights, ActorType type)
        {
            if (frameCount >= this.frameStart && frameCount < this.frameEnd)
            {
                if (actor.GetComponent<AnimationSystem>() != null)
                {
                    actor.GetComponent<AnimationSystem>().SetAnimation(animation);
                    float ratio = (frameCount - this.frameStart) * 1.0f / (this.frameEnd - this.frameStart);
                    Vector3 pos = Vector3.Lerp(posStart, posEnd, ratio);
                    if(type == ActorType.CameraFollow || type == ActorType.CameraLookat) { }else
                    pos.y = heights[new Vector2Int((int)(pos.x + 0.5f), (int)(pos.z + 0.5f))];
                    // walk _customRoate
                    if (lookat != null)
                    {
                        // 计算方向
                        Vector3 direction = lookat.transform.position - actor.transform.position;
                        direction.y = 0; 
                        actor.GetComponent<AnimationSystem>().SetTransform(pos, Quaternion.LookRotation(direction));
                    }
                    else
                    {

                        Vector3 speed = posEnd - posStart;
                        if (speed.magnitude < 0.01f) actor.GetComponent<AnimationSystem>().SetTransform(pos, Quaternion.identity);
                        else actor.GetComponent<AnimationSystem>().SetTransform(pos, Quaternion.LookRotation(speed.normalized));
                    }
                }
                return true;
            }
            return false;
        }
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, theAnim animation,
            Vector3 posStart, Vector3 posEnd, Quaternion rotationStart, Quaternion rotationEnd)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, rotationStart, rotationEnd, true, null);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, theAnim animation,
        Vector3 posStart, Vector3 posEnd, bool active)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, Quaternion.identity, Quaternion.identity, active, null);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, theAnim animation,
        Vector3 posStart, Vector3 posEnd)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, Quaternion.identity, Quaternion.identity, true, null);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, theAnim animation,
    Vector3 posStart, Vector3 posEnd, GameObject lookat)
    {
        return new ActorSettings(frameStart, frameEnd, actor, animation, posStart, posEnd, Quaternion.identity, Quaternion.identity, true, lookat);
    }

    public static ActorSettings addActorMove(int frameStart, int frameEnd, GameObject actor, bool active)
    {
        return new ActorSettings(frameStart, frameEnd, actor, theAnim.Wait, Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity, active, null);
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
        if (current.name.ToLower().Contains(targetName.ToLower()))
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

    public static void RecursiveFindAndModifyScale(string targetName, Transform current, Vector3 scale)
    {
        if (current.name.ToLower().Contains(targetName.ToLower())) current.localScale = scale;
        else foreach (Transform child in current) RecursiveFindAndModifyScale(targetName, child, scale);
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
        public string part;
        public string desc;
        public string dir;
        public string color;
        public string like;
        public string feel;

        public SubModelReplacer(string name, string part, string des, string dir, string color, string like, string feel)
        {
            this.name = name;
            this.part = part;
            this.desc = des;
            this.dir = dir;
            this.color = color;
            this.like = like;
            this.feel = feel;
        }
    }
    public static string AddModelSub(string[] sentences, SubModelReplacer replacer)
    {
        int r = UnityEngine.Random.Range(0, sentences.Length);
        string subtitle = sentences[r];
        subtitle = subtitle.Replace("_part", replacer.part);
        subtitle = subtitle.Replace("_name", replacer.name);
        subtitle = subtitle.Replace("_desc", replacer.desc);
        subtitle = subtitle.Replace("_dir", replacer.dir);
        subtitle = subtitle.Replace("_color", replacer.color);
        subtitle = subtitle.Replace("_like", replacer.like);
        subtitle = subtitle.Replace("_feel", replacer.feel);
        subtitle = subtitle.Replace("PH_", "");
        subtitle = subtitle.Replace("P_", "");
        subtitle = Regex.Replace(subtitle, "(\\B[A-Z])", " $1").ToLower();
        subtitle = System.Text.RegularExpressions.Regex.Replace(subtitle, @"\d", "");
        return subtitle;
    }

    public static string AddModelSubTwo(string[] sentences, string[] sentences2, SubModelReplacer replacer)
    {
        int r = UnityEngine.Random.Range(0, sentences.Length);
        int r2 = UnityEngine.Random.Range(0, sentences2.Length);
        string subtitle = sentences[r] + "," + sentences2[r2];
        subtitle = subtitle.Replace("_part", replacer.name);
        subtitle = subtitle.Replace("_desc", replacer.desc);
        subtitle = subtitle.Replace("_dir", replacer.dir);
        subtitle = subtitle.Replace("_color", replacer.color);
        subtitle = subtitle.Replace("_like", replacer.like);
        subtitle = subtitle.Replace("_feel", replacer.feel);
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
        public string featureDesc;
        public string featurePart;
       

        public SubReplacer(string name, string env, string enemy, string attack_weapon, string score, string desc, string part)
        {
            this.name = name;
            this.env = env;
            this.enemy = enemy;
            this.attack_weapon = attack_weapon;
            this.score = score;
            this.featureDesc = desc;
            this.featurePart = part;
        }
    }
    public static string AddSub3(List<string> sentences, SubReplacer replacer)
    {
        int r = UnityEngine.Random.Range(0, sentences.Count);
        string subtitle = sentences[r];
        subtitle = subtitle.Replace("_env", replacer.env);
        subtitle = subtitle.Replace("_name", replacer.name);
        subtitle = subtitle.Replace("_enemy", replacer.enemy);
        subtitle = subtitle.Replace("_desc", replacer.featureDesc);
        subtitle = subtitle.Replace("_part", replacer.featurePart);
        subtitle = subtitle.Replace("_attack_weapon", replacer.attack_weapon);
        subtitle = subtitle.Replace("_score", replacer.score);
        subtitle = System.Text.RegularExpressions.Regex.Replace(subtitle, @"\d", "");
        return subtitle;
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

    public static void SetChildRotation(Transform parent, string targetName, Quaternion r, bool local)
    {
        if (parent.name == targetName)
        {
            if (local) parent.localRotation = r;
            else parent.rotation = r;
            return;
        }
        foreach (Transform child in parent)
        {
            SetChildRotation(child, targetName, r, local);
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


    public static void SetChildHurt(Transform parent, bool hurt)
    {
        float rscale = 20;
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            Debug.Log(" hurt = " + hurt + " name = " + parent.name);
            Texture2D originTexture = parent.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            if (originTexture != null) // 如果存在mainTexture
            {
                for (int x = 0; x < originTexture.width; x++)
                {
                    for (int y = 0; y < originTexture.height; y++)
                    {
                        Color pixelColor = originTexture.GetPixel(x, y);
                        pixelColor.r = hurt ? Mathf.Clamp01(pixelColor.r + rscale) : Mathf.Clamp01(pixelColor.r - rscale); // 调整红色通道
                        originTexture.SetPixel(x, y, pixelColor);
                    }
                }
                originTexture.Apply();
                parent.GetComponent<MeshRenderer>().material.mainTexture = originTexture;
            }
            else // 如果没有mainTexture
            {
                Material material = parent.GetComponent<MeshRenderer>().material;
                Color materialColor = material.color;
                materialColor.r = hurt ? Mathf.Clamp01(materialColor.r + rscale) : Mathf.Clamp01(materialColor.r - rscale); // 调整材质的红色通道
                material.color = materialColor;
            }
            return;
        }
        foreach (Transform child in parent)
        {
            SetChildHurt(child, hurt);
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
        if (current.parent != null && (current.parent.name == "LeftLeg" || current.parent.name.Contains("Root")))
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

    public static void SetAllRotate(Transform parent, Quaternion r)
    {
        if (parent.name == "All")
        {
           // parent.transform.rotation = r;
            return;
        }
        foreach (Transform child in parent)
        {
            SetAllRotate(child, r);
        }
    }

    public static void SetMeshRenderer(Transform parent,  bool active)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            //parent.GetComponent<MeshRenderer>().enabled = active;
        }
        foreach (Transform child in parent)
        {
            SetMeshRenderer(child, active);
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

    public static GameObject CreateCube()
    {

        // 创建一个空的 GameObject
        GameObject part = new GameObject("CustomCube");

        // 给它添加 MeshFilter 和 MeshRenderer 组件
        MeshFilter meshFilter = part.AddComponent<MeshFilter>();
        MeshRenderer renderer = part.AddComponent<MeshRenderer>();
        //renderer.material = GeneratePoissonNoise();


        // 创建新的 Mesh
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // 定义24个顶点（每个面有4个顶点）
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

        // 定义三角形索引（每个面两个三角形）
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

        // 定义 UV 坐标（每个面4个UV）
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

        // 设置 Mesh 的顶点、三角形和 UV
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // 重新计算法线以保证光照效果
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


    private List<GeoAnim> animList = new List<GeoAnim>();
    Vector4 ParseLineToVector42(string line)
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

    struct GeoAnim
    {
        public string name;
        public bool loop;
        public float length;
        public List<GeoAnimPart> animaPart;

        public GeoAnim(string name, bool loop, float length)
        {
            this.name = name;
            this.loop = loop;
            this.length = length;
            this.animaPart = new List<GeoAnimPart>();
        }
    }

    struct GeoAnimPart
    {
        public string name;
        public List<uint> pos_time;
        public List<uint> rot_time;
        public List<Vector3> pos;
        public List<Quaternion> rot;

        public GeoAnimPart(string name)
        {
            this.name = name;
            this.pos_time = new List<uint>();
            this.rot_time = new List<uint>();
            this.pos = new List<Vector3>();
            this.rot = new List<Quaternion>();
        }
    }

    void RunAnimation(int frameCount)
    {
        string animation_name = "animation.split_pea.idle";
        int animation_index = 0;
        for (int i = 0; i < animList.Count; i++)
        {
            if (animList[i].name == animation_name)
            {
                animation_index = i;
            }
        }

        for (int i = 0; i < animList[animation_index].animaPart.Count; i++)
        {
            GeoAnimPart part = animList[animation_index].animaPart[i];
            if (part.rot_time.Count > 0)
            {
                int max_rot_time = (int)part.rot_time[part.rot_time.Count - 1];
                int now_time = frameCount % max_rot_time;
                for (int j = 0; j < part.rot_time.Count - 1; j++)
                {
                    if (now_time >= part.rot_time[j] && now_time < part.rot_time[j + 1])
                    {
                        float r = (now_time - part.rot_time[j]) * 1.0f / (part.rot_time[j + 1] - part.rot_time[j]);
                        Quaternion rot = Quaternion.Lerp(part.rot[j], part.rot[j + 1], r);
                       // RotateChild(theMob.transform, part.name, rot * GetBasicRot(part.name));
                        break;
                    }
                }
            }
            if (part.pos_time.Count > 0)
            {
                int max_pos_time = (int)part.pos_time[part.pos_time.Count - 1];
                int now_time = frameCount % max_pos_time;
                for (int j = 0; j < part.pos_time.Count - 1; j++)
                {
                    if (now_time >= part.pos_time[j] && now_time < part.pos_time[j + 1])
                    {
                        float r = (now_time - part.pos_time[j]) * 1.0f / (part.pos_time[j + 1] - part.pos_time[j]);
                        Vector3 pos = Vector3.Lerp(part.pos[j], part.pos[j + 1], r);
                      //  TranslateChild(theMob.transform, part.name, GetBasicPos(part.name) + pos * 0.08f);
                        break;
                    }
                }
            }
        }
    }
    void LoadAnimation(string path)
    {
        string[] lines = File.ReadAllLines(path);
        GeoAnimPart animPart = new GeoAnimPart("");
        GeoAnim anim = new GeoAnim();
        bool mode_pos = true;
        bool this_line_add = false;
        int brackets_count = 0;
        bool brackets_add = false;
        bool plog = false;
        foreach (string line in lines)
        {
            this_line_add = false;
            if (line.Contains("{"))
            {
                brackets_count++;
                brackets_add = true;
                this_line_add = true;
            }
            if (line.Contains("}"))
            {
                brackets_count--;
                brackets_add = false;
                this_line_add = true;
            }
            if (plog) Debug.Log("brackets num" + brackets_count + " line = " + line);
            if (brackets_count == 3 && this_line_add)
            {
                if (anim.length > 0)
                {
                    if (plog) Debug.Log("anim list added = " + anim.name + " count " + anim.animaPart.Count);
                    animList.Add(anim);
                }
                string anim_name = line.Split(':')[0].Replace('"', ' ');
                anim_name = anim_name.Trim();
                if (plog) Debug.Log(" anim name = " + anim_name);
                anim = new GeoAnim(anim_name, true, 1);
            }
            if (brackets_count == 5 && brackets_add && this_line_add)
            {
                string anim_part = line.Split(':')[0].Replace('"', ' ');
                anim_part = anim_part.Trim();
                if (plog) Debug.Log("anim part = " + anim_part);
                animPart = new GeoAnimPart(anim_part);
            }
            if (brackets_count == 6 && brackets_add && line.Contains("position"))
            {
                if (plog) Debug.Log("mode pos");
                mode_pos = true;
            }
            else if (brackets_count == 6 && brackets_add && line.Contains("rotation"))
            {
                if (plog) Debug.Log("mode rot");
                mode_pos = false;
            }
            if (brackets_count == 7 && brackets_add && this_line_add)
            {
                float t = float.Parse(line.Split(':')[0].Replace('"', ' ').Replace(" ", ""));
                uint tint = (uint)(t * 50);
                if (plog) Debug.Log("mode pos t = " + t);
                if (mode_pos)
                {
                    animPart.pos_time.Add(tint);
                }
                else
                {
                    animPart.rot_time.Add(tint);
                }

            }

            if (brackets_count == 7 && brackets_add && line.Contains("vector"))
            {
                var match = Regex.Match(line, @"\[(.*?)\]");
                if (match.Success)
                {
                    string numbersStr = match.Groups[1].Value;

                    // ���ַ����ָ�Ϊ�����ַ�������ת��Ϊ����������
                    float[] vector = numbersStr
                        .Split(',')
                        .Select(s => float.Parse(s.Trim()))
                        .ToArray();
                    if (plog) Debug.Log("mode pos vec = " + new Vector3(vector[0], vector[1], vector[2]));
                    if (mode_pos)
                    {
                        animPart.pos.Add(new Vector3(vector[0], vector[1], vector[2]));
                    }
                    else
                    {
                        animPart.rot.Add(Quaternion.Euler(vector[0], vector[1], vector[2]));
                    }
                }
            }

            if (brackets_count == 4 && brackets_add == false && this_line_add)
            {
                if (plog) Debug.Log(" added anima part = name " + animPart.name + " count = " + animPart.pos_time.Count);
                anim.animaPart.Add(animPart);
            }
        }
        foreach (var animx in animList)
        {
            if (plog) Debug.Log("animx name" + animx.name);
            foreach (var animy in animx.animaPart)
            {

                if (plog) Debug.Log("animy name" + animy.name);
            }
        }
    }

    public static List<int> GetCircle(int start, int radius)
    {
        List<int> result = new List<int>();
        for (int circlex = start; circlex < radius; circlex++)
        {
            int pDis = 0;
            for (int circlez = 0; circlez < radius; circlez++)
            {
                float dis = (float)((circlex + 0.5) * (circlex + 0.5) + (circlez + 0.5) * (circlez + 0.5));
                if (dis < radius * radius) { pDis = circlez; }
            }
            int startX = circlex;
            while (true)
            {
                float dis = (float)((circlex + 1.5) * (circlex + 1.5) + (pDis + 0.5) * (pDis + 0.5));
                if (dis < radius * radius) { circlex += 1; }

                else
                {
                    Vector3 basePos = new Vector3((circlex + 1 + startX) * 0.05f, 0f, 0f);
                    Vector3 baseSize = new Vector3((circlex + 1 - startX) * 0.1f, 0.1f, 0.1f);
                    break;
                }
            }
            result.AddRange(new int[3] { startX, circlex - startX, pDis });
        }
        return result;
    }

    public enum ShuffleRuleOrder
    {
        MustPreOne,
        MustPostOne,
        Pre,
        Post,
    }
    public struct ShuffleRule
    {
        public int index0;
        public int index1;
        public ShuffleRuleOrder order;

        public ShuffleRule(int index0, int index1, ShuffleRuleOrder order)
        {
            this.index0 = index0;
            this.index1 = index1;
            this.order = order;
        }

        public bool Compare(List<int> tempList)
        {
            int index0Pos = -1;
            int index1Pos = -1;
            for (int w = 0; w < tempList.Count; w++)
            {
                if (tempList[w] == index0) index0Pos = w;
                if (tempList[w] == index1) index1Pos = w;
            }
            if (index0Pos == -1 || index1Pos == -1) return true;
            switch (order)
            {
                case ShuffleRuleOrder.Pre: return index0Pos < index1Pos;
                case ShuffleRuleOrder.Post: return index0Pos > index1Pos;
                case ShuffleRuleOrder.MustPreOne: return index0Pos == index1Pos - 1;
                case ShuffleRuleOrder.MustPostOne: return index0Pos == index1Pos + 1;
            }
            return false;
        }
    }

    public static List<int> ShuffleTheList(int listCount, List<ShuffleRule> rules)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < listCount; i++) list.Add(i);

        for (int i = 0; i < list.Count; i++)
        {
            // while(true)
            for (int j = 0; j < list.Count; j++)
            {
                int rindex = Random.Range(0, list.Count);
                List<int> tempList = list;
                tempList[rindex] = i;
                tempList[i] = rindex;

                bool MatchRules = true;

                for (int k = 0; k < rules.Count; k++)
                {
                    if (!rules[k].Compare(tempList))
                    {
                        MatchRules = false;
                        break;
                    }
                }
                if (MatchRules)
                {
                    list = tempList;
                    break;
                }
            }
        }
        return list;

    }
    // TODO DEBUG SPHERE LENGHT
    // TODO REVERSE GET FRAME WORK
    // TODO GENERATE MESH
    // TODO MORE SHOW PLANTS
    // TODO MORE CURVE FOR STEM
    // TODO MORE WIDTH FOR ROOT LEAVES
    // TODO IDLE ANIAMTION FOR PLANTS
    public static T RandomList<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogError("The list is either null or empty.");
            return default;
        }

        // 使用Random.Range随机选择一个索引
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static List<int> GetCirlceFull(int baseX, int radius)
    {
        List<int> result = new List<int>();
        for (int px = baseX; px < radius; px++)
        {
            int pz = Mathf.FloorToInt(Mathf.Sqrt(radius * radius - (px + 0.5f) * (px + 0.5f)) + 0.5f);
            result.Add(pz);
        }
        return result;
    }

    public struct CircleStruct
    {
        public int startPx;
        public int contiPx;
        public int length;
        public CircleStruct(int startPx, int contiPx, int length)
        {
            this.startPx = startPx;
            this.contiPx = contiPx;
            this.length = length;
        }
    }

    public static List<CircleStruct> GetCirlceFull2(int baseX, int radius)
    {
        List<CircleStruct> result = new List<CircleStruct>();
        for (int px = baseX; px < radius; px++)
        {
            int pz = Mathf.FloorToInt(Mathf.Sqrt(radius * radius - (px + 0.5f) * (px + 0.5f)) + 0.5f);
            int startPx = px;
            while (true)
            {
                int pzz = Mathf.FloorToInt(Mathf.Sqrt(radius * radius - (px + 1.5f) * (px + 1.5f)) + 0.5f);
                if (pz == pzz) px += 1;
                else
                {
                    result.Add(new CircleStruct(startPx, px + 1 - startPx, pz));
                    break;
                }
            }

        }
        return result;
    }

    public struct XZvalues
    {
        public float x;
        public float z;
        public XZvalues(float x, float z)
        {
            this.x = x;
            this.z = z;
        }

    }

    public struct TheEvent
    {
        public string bigEvent;
        public List<string> middleEvent;
        public List<string> smallEvent;

        public TheEvent(string big)
        {
            bigEvent = big;
            middleEvent = new List<string>();
            smallEvent = new List<string>();
        }
    }

    public static List<TheEvent> GetStory()
    {
        List<string> bigEvent = SelectBigEvent();
        bigEvent = new List<string>() { "Fight" };
        List<TheEvent> theEvents = new List<TheEvent>();
        for (int i = 0; i < bigEvent.Count; i++)
        {
            TheEvent theEvent = new TheEvent(bigEvent[i]);
            theEvent.middleEvent = SelectMiddleEvent(bigEvent[i]);
            theEvents.Add(theEvent);
        }
        for(int i = 0; i < theEvents.Count; i++)
        {
            for(int j = 0; j < theEvents[i].middleEvent.Count; j++)
            {
                theEvents[i].smallEvent.Add(SelectSmallEvent(theEvents[i].middleEvent[j]));
            }
            
        }
        return theEvents;
    }

    public static string SelectSmallEvent(string middleEvent)
    {
        string path = "Assets/csv/output.csv";
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            int rowCount = lines.Length;

            if (rowCount > 0)
            {
                List<string> possibleString = new List<string>();

                for (int lineIndex = 0; lineIndex < rowCount; lineIndex++)
                {
                    string[] strs = lines[lineIndex].Split(',');
                    if (strs[0].ToLower() == middleEvent.ToLower())
                    {
                        possibleString.Add(strs[1]);
                    }

                }
                if(possibleString.Count > 0)
                {
                    int randomIndex = Random.Range(0, possibleString.Count);
                    randomIndex = 0;
                    string selectString = possibleString[randomIndex];
                    Debug.Log("Select String = " + selectString);
                    return selectString;
                }
            }
        }
        return "";
    }

    public static List<string> SelectMiddleEvent(string BigEvent)
    {
        string path = "Assets/csv/big.csv";
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            int rowCount = lines.Length;

            if (rowCount > 0)
            {
                for(int lineIndex = 0; lineIndex < rowCount; lineIndex++)
                {
                    string[] strs = lines[lineIndex].Split(',');
                    if (strs[0].ToLower() == BigEvent.ToLower())
                    {
                        List<string> middleEvent = new List<string>();
                        for (int i = 1; i < strs.Length; i++)
                        {
                            middleEvent.Add(strs[i]);
                        }
                        Debug.Log("select middle event " + lines[lineIndex]);
                        return middleEvent;
                    }
                }
            }
        }
        return new List<string>();
    }

    public static List<string> SelectBigEvent()
    {
        string path = "Assets/csv/day.csv";
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            int rowCount = lines.Length;

            if (rowCount > 0)
            {
                System.Random random = new System.Random();
                int randomIndex = random.Next(rowCount);
                string[] strs = lines[randomIndex].Split(',');
                List<string> bigEvent = new List<string>();
                for(int i = 0; i < strs.Length; i++)
                {
                    bigEvent.Add(strs[i]);
                }
                Debug.Log("select big event: " + lines[randomIndex]);
                return bigEvent;
            }
        }
        return new List<string>();
    }

}
