using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Structure;
using static UnityEditor.SceneView;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ShowPlants : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject hand;
    public GameObject emptyObject;

    private List<GeoAnim> animList = new List<GeoAnim>();
    Vector4 ParseLineToVector4(string line)
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
                        RotateChild(theMob.transform, part.name, rot * GetBasicRot(part.name));
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
                        TranslateChild(theMob.transform, part.name, GetBasicPos(part.name) + pos * 0.08f);
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
    private List<CameraSetting> cameraSettings = new List<CameraSetting>();
    private List<ActorSettings> actorSettings = new List<ActorSettings>();
    private GameObject theMob;
    private float start_time;


    struct BasicInfo
    {
        public string name;
        public Vector3 pos;
        public Quaternion rot;

        public BasicInfo(string name, Vector3 pos, Quaternion rot)
        {
            this.name = name;
            this.pos = pos;
            this.rot = rot;
        }
    }

    private List<BasicInfo> baseInfo = new List<BasicInfo>();

    Vector3 GetBasicPos(string name)
    {
        foreach (var info in baseInfo)
        {
            if (info.name == name)
            {
                return info.pos;
            }
        }
        return Vector3.zero;
    }

    Quaternion GetBasicRot(string name)
    {
        foreach (var info in baseInfo)
        {
            if (info.name == name)
            {
                return info.rot;
            }
        }
        return Quaternion.identity;
    }
    void TraverseChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            TraverseChildren(child);
        }
        if (parent.name != "All")
        {
            parent.transform.localRotation = Quaternion.identity;
        }
        else
        {

        }

        baseInfo.Add(new BasicInfo(parent.transform.name, parent.transform.localPosition, parent.transform.localRotation));
    }

    string RandomString(string[] s)
    {
        return s[UnityEngine.Random.Range(0, s.Length)];
    }

    public bool generateStory;
    private Vector3 handb;

    private Vector3 headOffset = new Vector3(0, 1, 0);
    void CameraIdle(int startFrame, int endFrame, Vector3 offset, GameObject lookat, int loops)
    {
        float len = offset.magnitude * 0.01f;
        Vector3 v0 = new Vector3(0, 0, -len);
        Vector3 v1 = new Vector3(0, 0, len);
        Vector3 v2 = new Vector3(0, len / 2, 0);
        Vector3 v3 = new Vector3(0, -len, -len / 2);
        Vector3 v4 = new Vector3(0, -len, len / 2);
        int oneLoops = (endFrame - startFrame) / loops;
        int oneFive = oneLoops / 5;

        for (int i = 0; i < loops; i++)
        {
            int frame = startFrame + i * oneLoops;
            cameraSettings.Add(addCameraMove(frame, frame + oneFive, offset + v0, offset + v1, lookat, headOffset));
            cameraSettings.Add(addCameraMove(frame + oneFive, frame + oneFive * 2, offset + v1, offset + v2, lookat, headOffset));
            cameraSettings.Add(addCameraMove(frame + oneFive * 2, frame + oneFive * 3, offset + v2, offset + v3, lookat, headOffset));
            cameraSettings.Add(addCameraMove(frame + oneFive * 3, frame + oneFive * 4, offset + v3, offset + v4, lookat, headOffset));
            cameraSettings.Add(addCameraMove(frame + oneFive * 4, frame + oneFive * 5, offset + v4, offset + v0, lookat, headOffset));
        }

    }

    void CameraSharpMovement(int startFrame, int endFrame, GameObject lookat)
    {
        float startDegree = Random.Range(0.0f, 360.0f);
        float nextDegree = Random.Range(-30.0f, 30.0f);
        float finalDegree = Random.Range(0.5f, 2.0f) * nextDegree;
        nextDegree = startDegree + nextDegree;
        finalDegree = nextDegree - startDegree;

        float firstDistance = Random.Range(2, 5f);
        float secondDistance = Random.Range(2, 5f);

        Vector3 pos0 = new Vector3(Mathf.Cos(startDegree) * firstDistance, 4, Mathf.Sin(startDegree) * firstDistance);
        Vector3 pos1 = new Vector3(Mathf.Cos(nextDegree) * secondDistance, 4, Mathf.Sin(nextDegree) * secondDistance);
        Vector3 pos2 = new Vector3(Mathf.Cos(finalDegree) * firstDistance, 4, Mathf.Sin(finalDegree) * firstDistance);

        int oneTwo = (endFrame - startFrame) / 2;

        cameraSettings.Add(addCameraMove(startFrame, startFrame + oneTwo, pos0, pos1, lookat, headOffset));
        cameraSettings.Add(addCameraMove(startFrame + oneTwo, endFrame, pos1, pos2, lookat, headOffset));

    }

    void RandomCamera(int startFrame, int endFrame)
    {
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 0: CameraIdle(startFrame, endFrame, new Vector3(3, 3, 0), theMob, 2); break;
            case 1: cameraSettings.Add(addCameraMove(startFrame, endFrame, new Vector3(5, 5, 0), new Vector3(5, 5, 0) * 0.3f, theMob, headOffset)); break;
            case 2: CameraSharpMovement(startFrame, endFrame, theMob); break;
            case 3: CameraSharpMovement(startFrame, endFrame, player); break;
            default: break;
        }
    }
    //��Զ����Զ�ĵط���һֱ�ߵ����︽��������
    void CameraFarToCloseMovement(int startFrame, int endFrame, GameObject lookat)
    {
        float degree0 = Random.Range(0f, 3.14f);
        float degree1 = Random.Range(-1.0f, 1.0f);
        float degree2 = degree0 + degree1;
        degree1 = degree0 - degree1;

        float distance0 = Random.Range(10, 20f);
        float distance1 = Random.Range(5, 10f);
        float distance2 = Random.Range(3, 5f);

        Vector3 pos0 = new Vector3(Mathf.Cos(degree0) * distance0, 1, Mathf.Sin(degree0) * distance0);
        Vector3 pos1 = new Vector3(Mathf.Cos(degree1) * distance1, 1, Mathf.Sin(degree1) * distance1);
        Vector3 pos2 = new Vector3(Mathf.Cos(degree2) * distance2, 1, Mathf.Sin(degree2) * distance2);
        int oneTwo = (endFrame - startFrame) / 2;

        cameraSettings.Add(addCameraMove(startFrame, startFrame + oneTwo, pos0, pos1, lookat, headOffset));
        cameraSettings.Add(addCameraMove(startFrame + oneTwo, endFrame, pos1, pos2, lookat, headOffset));
        actorSettings.Add(addActorMove(startFrame, endFrame, player, false));
        actorSettings.Add(addActorMove(startFrame, endFrame, hand, false));

    }
    // �����Ŵ󿴣�������
    // ����������
    void CameraCloseLookMovement(int startFrame, int endFrame, GameObject lookat)
    {
        float degree0 = Random.Range(0f, 3.14f);

        MeshRenderer[] meshRenderers = lookat.GetComponentsInChildren<MeshRenderer>();
        Bounds cb = meshRenderers[0].bounds;
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            cb.Encapsulate(meshRenderer.bounds);
        }
        float distance = Mathf.Sqrt(cb.size.x * cb.size.x + cb.size.z * cb.size.z);
        float distance2 = Random.Range(2f, 5f) * distance;
        Vector3 pos0 = new Vector3(Mathf.Cos(degree0) * distance2, 1, Mathf.Sin(degree0) * distance2);
        CameraIdle(startFrame, endFrame, pos0, lookat, 1);
        //cameraSettings.Add(addCameraMove(startFrame, endFrame, pos0, pos0, lookat, new Vector3(0, cb.size.y * 0.8f, 0)));

    }

    void CameraStaticLookMovement(int startFrame, int endFrame, GameObject lookat)
    {
        Vector3 pos0 = new Vector3(-5, DataTransfer.mobEyeOffset, 0);
        CameraIdle(startFrame, endFrame, pos0, lookat, 1);

    }
    void CameraCloseLook_WithPlayerLook(int startFrame, int endFrame)
    {
        float degree = Random.Range(0f, 3.14f);
        float distance = Random.Range(2, 5f);
        Vector3 pos = theMob.transform.position + new Vector3(Mathf.Cos(degree) * distance, 0, Mathf.Sin(degree) * distance);

        Vector3 playerPos = new Vector3(pos.x, 0.5f, pos.z);
        actorSettings.Add(addActorMove(startFrame, endFrame, player, AnimationSystem.Animation.Wait, playerPos, playerPos, Quaternion.identity, Quaternion.identity));
        actorSettings.Add(addActorMove(startFrame, endFrame, hand, false));
        CameraCloseLookMovement(startFrame, endFrame, player);
    }

    public static void AssignMaterial(Transform current)
    {
        if (current.name.Contains("cube"))
        {
            string indexStr = current.name.Replace("cube_", "");
            string texturePath = "Assets/Temp/" + DataTransfer.prefabName + "_" + indexStr + ".png";
            string meshPath = "Assets/Temp/" + DataTransfer.prefabName + "_" + indexStr + ".mesh";
            Material material = new(Shader.Find("Standard"));
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);


            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.textureFormat = TextureImporterFormat.RGBA32;
                textureImporter.SaveAndReimport();  // 保存并重新导入纹理

            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            texture.filterMode = FilterMode.Point;
            material.mainTexture = texture;
            current.GetComponent<MeshRenderer>().material = material;
            current.GetComponent<MeshFilter>().sharedMesh = mesh;

            return;
        }
        foreach (Transform child in current)
        {
            AssignMaterial(child);
        }
    }



    private bool enableVoice = true;
    int[] GenerateComments(List<MPS> subtitles)
    {
        string writeto = "";
        foreach (MPS sub in subtitles)
        {
            Debug.Log("comment = " + sub.content[0]);
            writeto += sub.content[0] + "\n";
        }
        Random3 = Random.Range(1000, 9999);
        Debug.Log("show part random number = " + Random3);
        
        if (enableVoice)
        {
            string pythonScriptPath = "D:/pr2023/test.py";

            string stringArg1 = writeto;
            string stringArg2 = Random3.ToString();
            Process pythonProcess = new Process();
            pythonProcess.StartInfo.FileName = "python";
            pythonProcess.StartInfo.Arguments = $"{pythonScriptPath} \"{stringArg1}\" \"{stringArg2}\"";
            pythonProcess.StartInfo.UseShellExecute = false;
            pythonProcess.StartInfo.RedirectStandardOutput = true;
            pythonProcess.StartInfo.RedirectStandardError = true;
            pythonProcess.StartInfo.CreateNoWindow = true;
            pythonProcess.Start();
            pythonProcess.WaitForExit();
            string output = pythonProcess.StandardOutput.ReadToEnd();
            string error = pythonProcess.StandardError.ReadToEnd();

            Debug.Log("build time = " + output);
            string[] line_part = output.Split(' ');
            int[] numbers = new int[line_part.Length - 1];
            for (int i = 0; i < line_part.Length - 1; i++)
            {
                numbers[i] = (int)(int.Parse(line_part[i]) * 0.05);
            }

            return numbers;
        }
        else
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < subtitles.Count; i++)
            {
                numbers.Add(100);
            }
            return numbers.ToArray();
        }
    }

    MFA GetMFA(MCH ch, List<MFA> mfas)
    {
        for (int i = 0; i < mfas.Count; i++)
        {
            if (mfas[i].ch == ch)
            {
                return mfas[i];
            }
        }
        Debug.LogError(" not find " + ch.ToString());
        return new MFA(ch);
    }
    Vector3 CirlcePos(float distance, float angle)
    {
        // 确保0是眼镜正前方，可能需要乘上 DataTransfer.MobBaseRotation
        return new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);
    }

    void Start()
    {
        emptyObject = new GameObject("MyEmptyObject");
        emptyObject.transform.position = new Vector3(0, 0, 0);
        emptyObject.AddComponent<AnimationSystem>();
        string[] envs = { "grass lands", "desert" };
        string env_string = RandomString(envs);

        SubReplacer replacer = new SubReplacer(DataTransfer.prefabName, env_string, "husk", "air bubbles", "nine");

        List<MSC> subEnum = new List<MSC>() { MSC.ENV, MSC.Nice, MSC.Character, MSC.Attack_Summary, MSC.Attacking };

        List<MPS> possibleSubtitles = Prepare();
        List<MPS> subtitles = new List<MPS>();
        for (int i = 0; i < subEnum.Count; i++)
        {
            for (int j = 0; j < possibleSubtitles.Count; j++)
            {
                if (possibleSubtitles[j].sc == subEnum[i])
                {
                    MPS current = possibleSubtitles[j];
                    current.SelectOne(replacer);
                    subtitles.Add(current);
                    break;
                }
            }
        }

        int[] build_times = GenerateComments(subtitles);

        audioSource = gameObject.AddComponent<AudioSource>();
        handb = hand.transform.localPosition;

        bool loadZombieYeti = false;
        if (loadZombieYeti == true)
        {
            string prefab_name = "ZombieYeti";
            string path = "Assets/Characters/Plants/Prefab/" + prefab_name + ".prefab";
            Debug.Log("data transfrer " + DataTransfer.messageToPass);
            GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            DataTransfer.mobEyeOffset = 2;
            theMob = Instantiate(selectedPrefab, new Vector3(0, 0.5f + DataTransfer.mobFootOffset, 0), Quaternion.Euler(0, 190, 0));
        }
        else
        {
            GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DataTransfer.messageToPass);
            DataTransfer.mobEyeOffset = 2;
            theMob = Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            AssignMaterial(theMob.transform);
        }

        // ��һ��SimpleAnimation
        //
        theMob.AddComponent<AnimationSystem>();
        //LoadAnimation("D:\\GameDe\\GLTFmodl\\split_pea.animation.json");

        TraverseChildren(theMob.transform);
        //cameraSettings.Add(addCameraMove(0, 100, new Vector3(-10, 1, 10), new Vector3(10, 1, 10), theMob));


        int startFrame = 1;
        int endFrame = 1;
        for (int i = 0; i < subtitles.Count; i++)
        {
            MPS sub = subtitles[i];
            int travel_time = build_times[i];
            startFrame = endFrame;
            endFrame += travel_time;

            bool handIsPlayer = false;

            switch (sub.ca)
            {
                case MCA.NoHand:
                    {
                        actorSettings.Add(addActorMove(startFrame, endFrame, player, false));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, false));
                        break;
                    };
                case MCA.RotateAround:
                    {
                        int midFrame = (startFrame + endFrame) / 2;
                        Vector3 startPos = CirlcePos(2, -30);
                        Vector3 midPos = CirlcePos(2, 0);
                        Vector3 endPos = CirlcePos(2, 30);
                        cameraSettings.Add(addCameraMove(startFrame, midFrame, startPos, midPos, theMob, headOffset));
                        cameraSettings.Add(addCameraMove(startFrame, midFrame, midPos, endPos, theMob, headOffset));
                        break;
                    }
                case MCA.HandFollowPlayer:
                    {
                        handIsPlayer = true;
                        MFA fa = GetMFA(MCH.I, sub.fa);
                        Vector3 offset = (GetPos(fa.sp0) - GetPos(fa.sp1)).normalized * 2f + new Vector3(0, 2, 0);
                        GameObject lookat = new GameObject("MyEmptyObject");
                        lookat.transform.position = GetPos(fa.sp0) + (GetPos(fa.sp1) - GetPos(fa.sp0));
                        cameraSettings.Add(addCameraMove(startFrame, endFrame, offset, offset, lookat, headOffset, emptyObject));
                        actorSettings.Add(addActorMove(startFrame, endFrame, emptyObject, true));
                        actorSettings.Add(addActorMove(startFrame, endFrame, player, false));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, true));
                        break;
                    }
                case MCA.FollowEmptyPlayerLookMob:
                    {
                        handIsPlayer = true;
                        MFA fa = GetMFA(MCH.I, sub.fa);
                        Vector3 offset = (GetPos(fa.sp0) - GetPos(fa.sp1)).normalized * 2f + new Vector3(0, 2, 0);
                        cameraSettings.Add(addCameraMove(startFrame, endFrame, offset, offset, theMob, headOffset, emptyObject));
                        actorSettings.Add(addActorMove(startFrame, endFrame, emptyObject, true));
                        actorSettings.Add(addActorMove(startFrame, endFrame, player, false));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, true));
                        break;
                    }
                case MCA.FollowPlayerForward:
                    {
                        MFA fa = GetMFA(MCH.I, sub.fa);
                        Vector3 offset = -(GetPos(fa.sp0) - GetPos(fa.sp1)).normalized * 4f + new Vector3(0, 3, 0);
                        cameraSettings.Add(addCameraMove(startFrame, endFrame, offset, offset, player, headOffset));
                        actorSettings.Add(addActorMove(startFrame, endFrame, player, true));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, false));
                        break;
                    }
                case MCA.HandFollowMob:
                    {
                        cameraSettings.Add(addCameraMove(startFrame, endFrame, Vector3.zero, Vector3.zero, theMob, headOffset));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, true));
                        break;
                    }
                case MCA.EmptyLookingTwo:
                    {
                        // player from -z to z
                        MFA fa = GetMFA(MCH.I, sub.fa);
                        Vector3 offset = (GetPos(fa.sp1) - GetPos(fa.sp0)).normalized * 4f + new Vector3(0, 3, 0);
                        cameraSettings.Add(addCameraMove(startFrame, endFrame, Vector3.zero, Vector3.zero, player, headOffset));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, false));

                        break;
                    }
                case MCA.SelfLooking:
                    {
                        // player from -z to z
                        float playerEyeHeight = 0f;
                        Vector3 offset = new Vector3(0, playerEyeHeight, 0) * 1.2f;
                        cameraSettings.Add(addCameraMove(startFrame, endFrame, Vector3.zero, Vector3.zero, theMob, headOffset));
                        actorSettings.Add(addActorMove(startFrame, endFrame, hand, true));

                        break;
                    }
                default: { Debug.LogError(sub.ca.ToString() + " not implement"); break; }
            }

            for (int k = 0; k < sub.fa.Count; k++)
            {
                MFA fa = sub.fa[k];
                GameObject mainActor = fa.ch == MCH.I ? player : theMob;
                GameObject subActor = fa.ch == MCH.I ? theMob : player;
                DataTransfer.mobFootOffset = 0.5f;
                DataTransfer.playerFootOffset = 0.5f;
                Vector3 posOffset = new Vector3(0, fa.ch == MCH.I ? DataTransfer.playerFootOffset : DataTransfer.mobFootOffset, 0);
                if (mainActor == player && handIsPlayer) mainActor = emptyObject;
                switch (fa.ac)
                {
                    case MAC.Idle:
                        {
                            actorSettings.Add(addActorMove(startFrame, endFrame, mainActor, AnimationSystem.Animation.Wait, GetPos(fa.sp0), GetPos(fa.sp1)));
                            break;
                        };
                    case MAC.LookingActor:
                        {

                            break;
                        }
                    case MAC.Walk:
                        {
                            actorSettings.Add(addActorMove(startFrame, endFrame, mainActor, AnimationSystem.Animation.Walk, GetPos(fa.sp0), GetPos(fa.sp1)));
                            break;
                        }
                    case MAC.WalkInvisible:
                        {
                            actorSettings.Add(addActorMove(startFrame, endFrame, mainActor, AnimationSystem.Animation.Walk, GetPos(fa.sp0), GetPos(fa.sp1)));
                            break;
                        }
                    case MAC.WalkingToMob:
                        {
                            actorSettings.Add(addActorMove(startFrame, endFrame, theMob, AnimationSystem.Animation.Walk, GetPos(fa.sp0), GetPos(fa.sp1)));
                            break;
                        }
                    case MAC.WalkingAroundSub_r:
                        {
                            Vector3 pos0 = CirlcePos(5, -10) + subActor.transform.position;
                            Vector3 pos1 = CirlcePos(5, 10) + subActor.transform.position;
                            actorSettings.Add(addActorMove(startFrame, endFrame, mainActor, AnimationSystem.Animation.Walk, pos0, pos1));
                            break;
                        }
                    default: { Debug.LogError(fa.ac.ToString() + " anim not implement"); break; }
                }
            }

        }
        /*
            switch (cameraMode[i])
            {
                case CameraMode.FarToClose: CameraFarToCloseMovement(startFrame, endFrame, theMob); break;
                case CameraMode.CloseWithoutAnything: CameraCloseLook_WithNothing(startFrame, endFrame); break;
                case CameraMode.CloseWithPlayer: CameraCloseLook_WithPlayerLook(startFrame, endFrame); break;
                case CameraMode.CloseWithHand: CameraCloseLook_WithHand(startFrame, endFrame); break;
                case CameraMode.CloseWithPlayerWalk: CameraCloseLook_WithPlayerWalk(startFrame, endFrame); break;
                case CameraMode.FiveStarWithHand: CameraSharpMovement(startFrame, endFrame, theMob); break;
        }
        */

    }

    Vector3 GetPos(MSP spot)
    {
        switch (spot)
        {
            case MSP.HabitatPlace: return new Vector3(-10, 0, -10);
            case MSP.HabitatPlaceClose: return new Vector3(-8, 0, -8);
            case MSP.HabitatPlaceFar: return new Vector3(2, 0, 2);
            case MSP.MobChaserStart: return new Vector3(2, 0, -10);
            case MSP.MobChaserEnd: return new Vector3(2, 0, 0);
            case MSP.PlayerChaseStart: return new Vector3(5, 0, -7);
            case MSP.PlayerChaseEnd: return new Vector3(5, 0, 3);
            case MSP.ShowLeft: return new Vector3(2, 0, -5);
            case MSP.ShowRight: return new Vector3(2, 0, 5);
            case MSP.WatchShow: return new Vector3(-2, 0, 0);
        }
        return Vector3.zero;
    }





    private int GlobalFrameCount = 0;
    // Update is called once per frame
    private int FrameCount = 0;
    private int CheckAudioCount = 0;
    private bool CheckAudio = false;

    // Update is called once per frame

    private string audioClipPath = "Audio/model"; // �����ļ���·����������Assets/Resources/Audio/mySound.wav

    private AudioSource audioSource;
    private int Random3;
    private AudioClip modelClip;
    void FixedUpdate()
    {

        if (enableVoice)
        {
            audioClipPath = "Audio/model_" + Random3.ToString();
            CheckAudioCount++;
            if (!CheckAudio && CheckAudioCount % 50 == 0 && CheckAudioCount > 200)
            {
                AssetDatabase.Refresh();
                FrameCount = -1;
                Debug.Log("Check Audio Failed " + CheckAudioCount);
                if (File.Exists("Assets/Resources/" + audioClipPath + ".wav"))
                {
                    Debug.Log("file exists");
                    modelClip = Resources.Load<AudioClip>(audioClipPath);
                    if (modelClip != null)
                    {

                        Debug.Log("clip exists");
                        audioSource.clip = modelClip;
                        audioSource.Play();
                        CheckAudio = true;
                    }
                }


            }
            if (!CheckAudio)
            {
                return;
            }
        }


        for (int i = 0; i < cameraSettings.Count; i++)
        {
            if (cameraSettings[i].Run(GlobalFrameCount))
            {
                break;
            }
        }
        for (int i = 0; i < actorSettings.Count; i++)
        {
            if (actorSettings[i].Run(GlobalFrameCount))
            {

            }
        }

        int handCountCycle = 20;
        float handCycleScale = 0.01f;
        float handCycleScaleY = 0.005f;
        int handCount = GlobalFrameCount % (handCountCycle * 2);
        if (handCount < handCountCycle)
        {
            float x = (handCount - handCountCycle / 2) * 2.0f / handCountCycle;
            Vector3 offset = new Vector3(x * handCycleScale, x * x * handCycleScaleY, 0);
            hand.transform.localPosition = handb + offset;
        }
        else
        {
            float x = (handCount - handCountCycle - handCountCycle / 2) * 2.0f / handCountCycle;
            Vector3 offset = new Vector3(-x * handCycleScale, x * x * handCycleScaleY, 0);
            hand.transform.localPosition = handb + offset;
        }

        GlobalFrameCount++;
        if (GlobalFrameCount > 200)
        {
            //SceneManager.LoadScene("BlockBench");
        }

    }

    public enum MSC
    {
        None,
        ENV,
        Nice,
        Animation,
        Habit,
        Character,
        Attack_Summary,
        Attacking,
    }

    public enum MCA
    {
        None,
        NoHand,
        RotateAround,
        Env_Hand,
        HandFollowPlayer,
        HandFollowMob,
        HandAttack,
        EmptyLookingTwo,
        EmptyLookingAround,
        FollowEmptyPlayerLookMob,
        AirLooking,
        SelfLooking,
        FollowPlayerForward,
        FarToClose,
        CloseWithoutAnything,
        CloseWithPlayer,
        CloseWithHand,
        CloseWithPlayerWalk,
        FiveStarWithHand,
        //https://youtu.be/KYf70U6i6Ak?t=37
        HeWalkCameraAhead,
        // https://youtu.be/KlP_jAbpygg?t=162 
        // б�ţ�����
        // yes in this case, _name is a monster, he`s attacking us
        HeWalkPlayerAhead,
        HeIdleCameraRotate,
        //https://youtu.be/QpVAD2zn3kY?t=41
        HeWlakCrossI,
        //https://youtu.be/QpVAD2zn3kY?t=42
        HeIdelPlayerRotate,
        //https://youtu.be/f81byheKWoQ?t=109
        HeRunawayILook,
        //https://youtu.be/f81byheKWoQ?t=262
        // ��ռ80%������
        HeIdleFace,
        // https://youtu.be/f81byheKWoQ?t=292
        PlayerRunaway,
        // Two ������վ�ţ����������
        CameraZoomIn,

        HeFight,

    }

    public enum MAC
    {
        None,
        LookingActor,
        LookingActorFar,
        Idle,
        Walk,
        WalkInvisible,
        WalkingRandomSearch,
        WalkingAroundSub_r,
        WalkingToPlayer,
        WalkingToMob,
        AttackCircle,
        RunAway,
        AttackMob,
        AttackPlayer,
        ChasePlayer,
        WalkingBackWard,
        BeingAttackNoDefend,
        AttackEverything,
    }

    public enum MCH
    {
        None,
        Mob,
        I,
    }

    public struct MFA
    {
        public MCH ch;
        public MAC ac;
        public MSP sp0;
        public MSP sp1;

        public void set(MAC ac, MSP sp0, MSP sp1)
        {
            this.ac = ac;
            this.sp0 = sp0;
            this.sp1 = sp1;
        }

        public void set(MAC ac, MSP sp0)
        {
            this.ac = ac;
            this.sp0 = sp0;
            this.sp1 = sp0;
        }

        public MFA(MCH ch)
        {
            this.ch = ch;
            ac = MAC.None;
            sp0 = MSP.None;
            sp1 = MSP.None;
        }
    }

    public enum MSP
    {
        None,
        Left,
        Right,
        Flat,
        Attack0,
        Attack1,
        MobAttackFar,
        MobAttackClose,
        PlayerAttackFar,
        PlayerAttackClose,
        HabitatPlace,
        HabitatPlaceFar,
        HabitatPlaceClose,
        MobChaserStart,
        MobChaserEnd,
        PlayerChaseStart,
        PlayerChaseEnd,
        FarRunnerStart,
        FarRunnerEnd,
        CloseRunnerStart,
        CloseRunnerEnd,
        ShowLeft,
        ShowRight,
        WatchShow,
    }

    struct MPS
    {
        string link;
        public MSC sc;
        public MCA ca;
        public List<MFA> fa;
        public List<string> content;

        public MPS(string link, MSC sc, MCA ca, MFA fa, string content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.fa = new List<MFA>() { fa };
            this.content = new List<string>() { content };
        }

        public MPS(string link, MSC sc, MCA ca, MFA fa0, MFA fa1, string content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.fa = new List<MFA>() { fa0, fa1, };
            this.content = new List<string>() { content };
        }

        public MPS(string link, MSC sc, MCA ca, MFA fa, List<String> content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.fa = new List<MFA>() { fa };
            this.content = content;
        }

        public MPS(string link, MSC sc, MCA ca, MFA fa0, MFA fa1, List<String> content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.fa = new List<MFA>() { fa0, fa1, };
            this.content = content;
        }
        public void SelectOne(SubReplacer replacer)
        {
            string sub = AddSub3(this.content, replacer);
            this.content = new List<string>() { sub };


            string str = " Scene= " + sc.ToString() + " Camera= " + ca.ToString();
            for (int i = 0; i < fa.Count; i++)
            {
                str += " actor= " + fa[i].ch.ToString() + " anim= " + fa[i].ac.ToString() + " start= " + fa[i].sp0.ToString() + " end= " + fa[i].sp1.ToString();
            }
            Debug.Log(str);
            Debug.Log(" content = " + content[0].ToString());

        }

        public string Content()
        {
            if (this.content.Count > 0)
            {
                return this.content[0];
            }
            return "";
        }


    }

    static List<MPS> Prepare()
    {
        List<MPS> pres = new List<MPS>();

        MFA fame = new MFA(MCH.I);
        MFA famob = new MFA(MCH.Mob);

        fame.set(MAC.Idle, MSP.PlayerAttackClose);
        famob.set(MAC.Walk, MSP.MobAttackFar, MSP.MobAttackClose);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=434", MSC.None, MCA.HandFollowPlayer, fame, famob, "he is quite big let me try to fight with him"));

        fame.set(MAC.AttackCircle, MSP.PlayerAttackClose);
        famob.set(MAC.Walk, MSP.MobAttackClose);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.HandFollowPlayer, fame, famob,
            "I need to try to defeat him and the problem is that these arms are long"));

        famob.set(MAC.Idle, MSP.Flat);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.RotateAround, famob,
            "you can see all the details properly. but it turned out like this, the model of him here in minecraft"));

        famob.set(MAC.RunAway, MSP.FarRunnerStart, MSP.FarRunnerEnd);
        fame.set(MAC.ChasePlayer, MSP.FarRunnerEnd, MSP.FarRunnerEnd);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.HandFollowPlayer, fame, famob,
            "only there's a small problem he is slightly fast huh"));

        fame.set(MAC.WalkingRandomSearch, MSP.Flat);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.HandFollowPlayer, fame,
            "because he is strong and the problem is that I think if I try to fight with him it's almost impossible dude"));

        famob.set(MAC.AttackPlayer, MSP.Attack0);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.HandAttack, famob,
            "dude no way no way wait I hit my God he keeps circling dude stop a"));

        famob.set(MAC.AttackPlayer, MSP.Attack0);
        fame.set(MAC.BeingAttackNoDefend, MSP.Attack1);
        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.HandAttack, fame, famob,
        "okay I can't. defeat me. I give up. I give. congratulations. I can't hit him"));

        /*
            pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.EmptyLookingTwo, MCH.Mob, MAC.ChasePlayer, MSP.Attack0, MCH.I, MAC.RunAway, MSP.Attack1,
            "i think I have a plan come it's right here in front of the house hold on"),

                        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=445", MSC.None, MCA.HandAttack, MCH.I, MAC.AttackMob, MSP.Attack0,
            "I'm not going to waste too much time with him, so I'm going to try to defeat him without much delay"),



            pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=755", MSC.None, MCA.Hand, MCH.Mob, MAC.ChasePlayer, MSP.Attack0, MCH.I, MAC.WalkingBackWard, MSP.Attack1,
            "come here my dear come look how his animation turned out it was very good"),

                        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=755", MSC.None, MCA.Hand, MCH.Mob, MAC.Idle, MSP.Attack0, MCH.I, MAC.LookingActorFar, MSP.Attack1,
            "I think it's better to leave _mob alone there in his place I thought he had taken me to bad place"),


                        pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=755", MSC.None, MCA.Hand, MCH.Mob, MAC.WalkingToPlayer, MSP.Attack0, MCH.I, MAC.LookingActor, MSP.Attack1,
            "so here in front is our first character which is _mob"),

            pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=755", MSC.None, MCA.EmptyLookingTwo, MCH.Mob, MAC.ChasePlayer, MSP.Attack0, MCH.I, MAC.RunAway, MSP.Attack1,
            "the problem is that I forgot for a detail that he is _mob and he releases _weapons"),


            pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=21", MSC.None, MCA.SelfLooking, MCH.Mob, MAC.Walk, MSP.Attack0,
            "the problem is that he will probably _weapon_destory_way this entire _spot"),


            pres.Add(new MPS("https://youtu.be/XN6B7vK4Yrw?t=755", MSC.None, MCA.AirLooking, MCH.Mob, MAC.AttackEverything, MSP.Attack0,
            "okay I managed to defeat _mob, wow he is very strong"),


            pres.Add(new MPS("https://youtu.be/LwjvP2qF9vQ?t=344", MSC.None, MCA.EmptyLookingAround, MCH.Mob, MAC.Idle, MSP.Attack0, 
            "let's analyze all the details of this _mob. first that I put a reflective hat on him it already looked very beautiful"),


            pres.Add(new MPS("https://youtu.be/LwjvP2qF9vQ?t=367", MSC.None, MCA.EmptyLookingTwo, MCH.Mob, MAC.Idle, MSP.Attack0, MCH.I, MAC.Idle, MSP.Attack1, //near by
            "look let me tell you Halloween is also my favorite holiday like of all so yeah"),
            */

        famob.set(MAC.Idle, MSP.HabitatPlace);
        fame.set(MAC.WalkInvisible, MSP.HabitatPlaceFar, MSP.HabitatPlaceClose);
        pres.Add(new MPS("", MSC.ENV, MCA.HandFollowPlayer, fame, famob,
         new List<String>() { "anywhere in a _env we could find _name", "we've spotted some _env and that means we can find a _name",
             "if we sneak up to the _env we may be able to Feast Our Eyes Upon a wild baby _name",
             "we have _name all over the _env and it matched a lot because his  color matched a lot with the surroundings"}));

        famob.set(MAC.Idle, MSP.Flat);
        fame.set(MAC.WalkingAroundSub_r, MSP.Flat);
        pres.Add(new MPS("", MSC.Nice, MCA.HandFollowPlayer, fame, famob,
            new List<String>() { " the models themselves turned out very good so I think they they deserve a grade _score ",
                            "in this case she's quite small, because like she's small in the game",
                            "look at this she's a little bigger than just a block but turned out very good",
                              "for now the character will be nice anyway",
                            "look at this they turned out really nice. I think it matched the style" }));



        pres.Add(new MPS("", MSC.Nice, MCA.HandFollowPlayer, fame, famob,
         new List<String>() {"looking very cute in its Youth and Hungry",
                          "the _name looks a little bit scary but don't be thrown off the _name is here to be your friend",
                          "and these guys are so cute. look at the face on this little guy. oh my goodness buddy I just want to come give you a pet",
                          "well the normal _name turned out like this in Minecraft",
                          "the _name just came out adorable all at all he's so cute",
                          "we've got _name looking super cute extra furry"}));

        famob.set(MAC.Walk, MSP.MobChaserStart, MSP.MobChaserEnd);
        fame.set(MAC.Walk, MSP.PlayerChaseStart, MSP.PlayerChaseEnd);
        pres.Add(new MPS("", MSC.Character, MCA.FollowPlayerForward, fame, famob,

        new List<String>() { "but there's a difference now yes he is aggressive", "in his baby form he'll run and hide",
                                "and this little guy looks happy all of the time"}));

        famob.set(MAC.Walk, MSP.MobChaserStart, MSP.MobChaserEnd);
        fame.set(MAC.WalkInvisible, MSP.PlayerChaseStart, MSP.PlayerChaseEnd);
        pres.Add(new MPS("", MSC.Attack_Summary, MCA.FollowEmptyPlayerLookMob, fame, famob,

            new List<String>() {"there's a small important detail I forgot to mention, he releases a _attack_weapon " +
            "               which is the Sleep _attack_weapon so that makes me want to sleep", "this is the power of _name he can make his enemies fall asleep",
                            "she's peaceful so she's not going to hit me unless I annoy her",
                             "he is attack me and i was running out for my life, let`s defeat her here",
                            "_name is going to do everything he can to defend Us",
                            "_name will defend the birch trees with all its might. " +
                            "growing little birch trees underneath the threat and launching it into the air",
                            "the _name spots anything threatening nearby by ready to take it down instantly right"}));

        famob.set(MAC.Walk, MSP.ShowLeft, MSP.ShowRight);
        fame.set(MAC.WalkInvisible, MSP.WatchShow);
        pres.Add(new MPS("", MSC.Attacking, MCA.FollowEmptyPlayerLookMob, fame, famob,
         new List<String>() {"head towards a _enemy and send the adult _name on it." +
            "the adult _name is going to send out _attack_weapon completely surrounding the",
            "the _name who will send out a _attack_weapon, full of allergic reaction." +
            "the _name defends us shoots out the _attack_weapon." +
            "one _enemy down after the second _enemy and this _enemy doesn't stand a chance against those allergies",
            "the _enemy came out ! he's trying to tackle the _enemy ! he got the _enemy knocked down",
            //https://youtu.be/KYf70U6i6Ak?t=16
            "oh, wait, he`s attacking _enemy",
            //https://youtu.be/Se0oM5liI-4?t=646
            "around the edge we can see a few _enemy coming in and now _name is preparing defenses as soon as a _enemy threatens us",
            "if any _enemy tries to come nearby, our _name is going to defend us hopefully ",
            //https://youtu.be/hewT7YXbOhY?t=70
            "bring in a _enemy here and let's see our _name go to work. do the rest of your work here. here you go. I think it was four hits"}));

        pres.Add(new MPS("", MSC.Animation, MCA.HandFollowPlayer, fame, famob,
         new List<String>() {"oh and if anyone wants to know the animation of _name turned out like this " +
            "look walking very smoothly also bending the legs standing and walking he bends the legs very smoothly",
        "I tried to make a very smooth animation with his leg look they move as if they were a spider",
        "I really like this animation to be honest",
        // Scary 09:51 
        "I don't know his name he got the best animation I will hit him for you to see and look how his arm moves it's very fluid",
        // Scary 12:07 he chase i run backward / he chase i run foreward
        "I want to show you how his walking animation turned out did it go a little further here",
        "seriously I dedicated seriously I dedicated perfect",
                            "their animation also turned out very good", "also this walking animation turned out very good"}));

        return pres;
    }
    // ����ʵս
    // https://youtu.be/Se0oM5liI-4?t=749
    // https://youtu.be/Se0oM5liI-4?t=422
    string[] desc_attack_time = { "head towards a _enemy and send the adult _name on it." +
            "the adult _name is going to send out _attack_weapon completely surrounding the",
            "the _name who will send out a _attack_weapon, full of allergic reaction." +
            "the _name defends us shoots out the _attack_weapon." +
            "one _enemy down after the second _enemy and this _enemy doesn't stand a chance against those allergies",
            "the _enemy came out ! he's trying to tackle the _enemy ! he got the _enemy knocked down",
            //https://youtu.be/KYf70U6i6Ak?t=16
            "oh, wait, he`s attacking _enemy",
            //https://youtu.be/Se0oM5liI-4?t=646
            "around the edge we can see a few _enemy coming in and now _name is preparing defenses as soon as a _enemy threatens us",
            "if any _enemy tries to come nearby, our _name is going to defend us hopefully ",
            //https://youtu.be/hewT7YXbOhY?t=70
            "bring in a _enemy here and let's see our _name go to work. do the rest of your work here. here you go. I think it was four hits"};


    string[] drop_desc = { "if we defeat the Sheep yes it will drop green wool as I had imagined and all right" };




    string[] desc_grade = { "and we'll go to the next character. but before I want you guys to comment a grade for _name, " +
                         "Below in my opinion definitely it's _score. out of 10 because he's very beautiful well",
                           "but as I made more just for the sake of making I'll give a grade eight out of _score"};



    // ����
    string[] desc_play = { "all we have to do is get into the _env and make friends with our baby _name" };

    // ��
    string[] desc_eat = { "but if we take some kelp and tame our little baby corvis he becomes stronger and stronger until he's ready to take down any illers in sight",
                            "all we have to do is of course feed it a little bit of _food to tame it",
                        "all you have to do is keep _food on hand to keep him happy. so he doesn't send any _attack_weapon after you"};

    // ����
    string[] desc_look = { "where did you go buddy don't worry I'm not here to harm you I'm here to be your friend" };

    // ��
    string[] desc_walk = { "looks like he's venturing to the top of the temple exploring his home biome" };

    //HeFightI
    // Scary 2.55
    string[] lets_fight = { " how are you let me try to defeat  him here", 
        // 7.03
        "I configured a little detail in the mod, that he can yes attack let me see him" };

    // I Search
    // Scary 5.48
    string[] grab = { " obviously I will use the rabbits to transform into _name" };

    // Scary 9:55
    string[] heAttackMeIFightBack = { "I really like this but okay my dear you can stop" };


    //Scary 11:55
    string[] MultipleView = { "look at the size of this cat he got huge and seriously tell me if it wasn't the best mob in this video surely" };
}
