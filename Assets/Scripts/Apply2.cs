using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;
using static UnityEditor.PlayerSettings;
using File = System.IO.File;
using static Structure;
using Random = UnityEngine.Random;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using static HumanoidGenerator;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Apply2 : MonoBehaviour
{
    private List<GameObject> parts;
    private Material mushMaterial;
    private float start_time;

    public bool enableRandomModel;
    public bool enableVoice;
    public struct ActivePart
    {
        public string parentName;
        public string name;
        public int index;
        public int activeStartFrame;
        public int buildFrame;
        public Vector3 size;
        public Vector3 pos;
        public Transform transform;
        public Material material;
        public bool modelMode;
        public int strechMode;//0 center 1 edge
        public bool materialStartGray;

        public ActivePart(string parentName, GameObject child, int activeStartFrame, int buildFrame, Vector3 size, int index)
        {
            this.name  = child.name;
            
            this.activeStartFrame = activeStartFrame;
            this.parentName = parentName;
            this.index = index;
            this.buildFrame = buildFrame;
            this.size = size;
            pos = child.transform.localPosition;
            this.transform = child.transform;
            if(child.GetComponent<MeshRenderer>() != null)
            {
                material = child.GetComponent<MeshRenderer>().material;
            }
            else
            {
                material = null;
            }
            this.modelMode = true;
            strechMode = Random.Range(0, 2);
            this.materialStartGray = false;
        }

        public ActivePart(GameObject pa, int activeStartFrame, int buildFrame)
        {
            this.name = pa.name;
            this.activeStartFrame = activeStartFrame;
            this.parentName = pa.name;
            this.index = 0;
            this.buildFrame = buildFrame;
            this.size = Vector3.zero;
            pos = Vector3.zero;
            this.transform = pa.transform;
            if (pa.GetComponent<MeshRenderer>() != null)
            {
                material = pa.GetComponent<MeshRenderer>().material;
            }
            else
            {
                material = null;
            }
            this.modelMode = false;
            strechMode = 0;
            this.materialStartGray = false;
        }
    }

    int activeCount = 0;
    private List<ActivePart> activeParts;
    private List<Vector4> vectorList = new List<Vector4>();
    private List<string> subtitles = new List<string>();
    private List<CameraSetting> cameras = new List<CameraSetting>();



    void LoadAnimation(string path)
    {
        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            // 找到类似 "0.0": [0, 0, 0], 的行
            if (line.Contains(": ["))
            {
                Vector4 vector = ParseLineToVector4(line.Trim());
                vectorList.Add(vector);
            }
        }
    }

    private Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
    {
        { "Black", Color.black },
        { "White", Color.white },
        { "Red", Color.red },
        { "Green", Color.green },
        { "Blue", Color.blue },
        { "Yellow", new Color(1f, 1f, 0f) },
        { "Cyan", Color.cyan },
        { "Magenta", Color.magenta },
        { "Gray", Color.gray },
        { "Orange", new Color(1f, 0.65f, 0f) },
        { "Pink", new Color(1f, 0.75f, 0.8f) },
        { "Brown", new Color(0.65f, 0.16f, 0.16f) },
        { "Purple", new Color(0.5f, 0f, 0.5f) },
        { "Violet", new Color(0.93f, 0.51f, 0.93f) },
        { "Olive", new Color(0.5f, 0.5f, 0f) },
        { "Teal", new Color(0f, 0.5f, 0.5f) },
        { "Navy", new Color(0f, 0f, 0.5f) }
    };

    private float ColorDistance(Color a, Color b)
    {
        float r = a.r - b.r;
        float g = a.g - b.g;
        float bDist = a.b - b.b;
        return Mathf.Sqrt(r * r + g * g + bDist * bDist);
    }
    string GetMainColor(Texture2D tex)
    {

        Color[] pixels = tex.GetPixels(); // Get all the pixels from the texture
        Dictionary<string, int> colorCount = new Dictionary<string, int>();
        // Traverse all pixels
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Color pixel = tex.GetPixel(x, y);

                // Ignore fully transparent pixels
                if (pixel.a != 1.0) continue;
                //if (x == 0 || y == 0 || x == tex.width - 1 || y == tex.height - 1) continue; ;


                string closestColorName = null;
                float shortestDistance = Mathf.Infinity;

                foreach (var colorPair in colorDict)
                {
                    Color knownColor = colorPair.Value;
                    float distance = ColorDistance(pixel, knownColor);

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestColorName = colorPair.Key;
                    }
                }
                // Count the occurrence of the quantized color
                if (colorCount.ContainsKey(closestColorName)) colorCount[closestColorName]++;
                else colorCount[closestColorName] = 1;
            }
        }

        string mostFrequentColor = null;
        int maxCount = 0;

        foreach (var pair in colorCount)
        {
            if (pair.Value > maxCount)
            {
                mostFrequentColor = pair.Key;
                maxCount = pair.Value;
            }
        }

        return mostFrequentColor;
    }

    GameObject generatedAnimal;
    public void AssignTexture(Transform current)
    {
        if (current.name.Contains("cube"))
        {
            string parent = current.parent.name;
            
            

            return;
        }
        foreach (Transform child in current)
        {
            AssignTexture(child);
        }
    }
    int[] GenerateComments()
    {
        string prefab_name = "DarkDragonPeaShooter";

        GameObject sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/zombie.prefab");
        Texture2D sourceTexture = LoadTexture("Assets/dragon.png");
        if (enableRandomModel)
        {
            generatedAnimal = HumanoidGenerator.CreateHumaoid(prefab_name, sourceModel, sourceTexture, true);
            DataTransfer.messageToPass = "Assets/guard.prefab";
            DataTransfer.prefabName = prefab_name;
            DataTransfer.mobFootOffset = FindModelOffset(generatedAnimal.transform);
            PrefabUtility.SaveAsPrefabAsset(generatedAnimal, DataTransfer.messageToPass);
            TraverseChildren(generatedAnimal.transform);
            
        }
        else
        {

            string path = "Assets/Characters/Plants/Prefab/" + prefab_name + ".prefab";
            GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            generatedAnimal = Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            TraverseChildren(generatedAnimal.transform);
        }

        

        SubModelReplacer placer = new SubModelReplacer();
        placer.name = prefab_name;
        placer.like = "villager";
        placer.color = "green";
        placer.desc = "";
        placer.dir = "top";
        placer.feel = "scary";

            string[] whats = { "cursed", "dumb", "adorable", "crazy", "earthy", "comfy" , "cute"};
        subtitles.Add(AddModelSub(comment_all_start.ToArray(), placer));
        Debug.Log(" first comment " + subtitles[subtitles.Count - 1]);

        List<string> added_part = new List<string>();

        for (int j = 0; j < parts.Count; j++)
        {
            float typer = Random.Range(0.0f, 1.0f);
            if (typer < 0.3f) subTypes.Add(SubtitleType.OnlyBase);
            else if (typer < 0.7f) subTypes.Add(SubtitleType.BaseColor);
            else subTypes.Add(SubtitleType.ColoredBase);
         //   else if (typer < 0.93f) subTypes.Add(SubtitleType.BaseTwo);
         //   else if (typer < 0.96f) subTypes.Add(SubtitleType.BaseTwoColor);
          //  else subTypes.Add(SubtitleType.ColoredBaseTwo);


            GameObject part = parts[j];
            if (part.transform.childCount == 0)
            {
                part.SetActive(false);
            }
            int count = CountDirectChildCubes(part.transform);
            if (count > 0)
            {

                placer.color = GetMainColor((Texture2D)part.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture);
                Debug.Log(" j = " + j + " partname " + part.name +  " subtype = " + subTypes[j] + " main color = " + placer.color);

                bool first_cube = true;
                for (int i = 0; i < count; i++)
                {

                    if (i < 4)
                    {
                        placer.desc = "longer";
                        if (part.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                        {
                            Vector3 childPart = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                            if (childPart.y > 3)
                            {
                                placer.desc = "longer";
                            }
                            else
                            {
                                placer.desc = "shorter";
                            }
                        }

                        if (first_cube)
                        {
                            int du = 0;
                            for(int idx = 0; idx < added_part.Count; idx++)
                            {
                                string added = added_part[idx];
                                added = added.Replace("Left", "").Replace("Right", "").Replace("Right", "").Replace("left", "");
                                string nowed = part.name.Replace("Left", "").Replace("Right", "").Replace("Right", "").Replace("left", "");
                                if(nowed == added)
                                {
                                    //Debug.Log("added = " + added + " nowed " + nowed);
                                    du = 4;
                                    break;
                                }
                            }

                            int r = Random.Range(0, 6 + du);
                            placer.part = part.name;
                            if (r < 6)
                            {
                                switch (subTypes[subTypes.Count - 1])
                                {
                                    case SubtitleType.OnlyBase:
                                    case SubtitleType.BaseColor:
                                        {
                                            subtitles.Add(AddModelSub(comment_base_fact.ToArray(), placer));
                                            break;
                                        }
                                    case SubtitleType.ColoredBase:
                                        {
                                            placer.part = placer.color + " " + part.name;
                                            subtitles.Add(AddModelSub(comment_base_fact.ToArray(), placer));
                                            break;
                                        }
                                }

                                if(subTypes[subTypes.Count - 1] == SubtitleType.OnlyBase)
                                {
                                }

                            }
                            else
                            {

                                subtitles.Add(AddModelSub(comment_duplicate.ToArray(), placer));
                            }


                            first_cube = false;
                        }
                        else if (i == count - 1)
                        {

                            placer.part = "cube";
                            subtitles.Add(AddModelSub(comment_finish.Concat(comment).ToArray(), placer));
                        }
                        else
                        {
                            placer.part = part.name;
                            subtitles.Add(AddModelSub(comment.ToArray(), placer));
                        }

                        Debug.Log("name = " + part.transform.name + " child index = " + i + " comment === " + subtitles[subtitles.Count - 1]);

                    }
                }

                if(subTypes[subTypes.Count - 1] == SubtitleType.BaseColor)
                {
                    subtitles.Add(AddModelSub(comment_color.ToArray(), placer));
                    Debug.Log(" parent name =" + part.transform.name + " color special comment === " + subtitles[subtitles.Count - 1]);
                }
                

                added_part.Add(part.name);
            }
        }

        subtitles.Add(AddModelSub(comment_finish.ToArray(), placer));
        Debug.Log(" final comment " + subtitles[subtitles.Count - 1]);

        string writeto = "";
        foreach (string sub in subtitles)
        {
            writeto += sub + "\n";
        }

        if (enableVoice)
        {
            // 设置Python脚本路径
            string pythonScriptPath = "D:/pr2023/test.py";

            // 要传递给Python的字符串
            string stringArg1 = writeto;
            string stringArg2 = Random3.ToString();
            Debug.Log(stringArg2);
            // 创建一个新的进程
            Process pythonProcess = new Process();

            // 设置Python解释器路径
            pythonProcess.StartInfo.FileName = "python";

            // 传递脚本路径和字符串参数
            pythonProcess.StartInfo.Arguments = $"{pythonScriptPath} \"{stringArg1}\" \"{stringArg2}\"";

            // 配置其他进程启动信息
            pythonProcess.StartInfo.UseShellExecute = false;
            pythonProcess.StartInfo.RedirectStandardOutput = true;
            pythonProcess.StartInfo.RedirectStandardError = true;
            pythonProcess.StartInfo.CreateNoWindow = true;

            // 启动Python进程
            pythonProcess.Start();

            // 等待Python脚本执行完成
            pythonProcess.WaitForExit();

            // 获取Python输出（如果有）
            string output = pythonProcess.StandardOutput.ReadToEnd();
            string error = pythonProcess.StandardError.ReadToEnd();

            Debug.Log("build time = " + output);
            // 根据空格拆分成字符串数组
            string[] line_part = output.Split(' ');

            // 将字符串数组转换成整型数组
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
                numbers.Add(60);
            }
            return numbers.ToArray();
        }

    }

    enum SubtitleType
    {
        OnlyBase, // 50 %
        BaseTwo, // 10%
        BaseColor, // 10%
        BaseTwoColor, // 10%
        ColoredBase, // 10%
        ColoredBaseTwo, // 10%
    }

    private string audioClipPath = "Audio/model"; // 声音文件的路径，假设在Assets/Resources/Audio/mySound.wav

    private AudioSource audioSource;

    private AudioClip modelClip;

    private List<SubtitleType> subTypes ;

    private int Random3;
    void Start()
    {
        // step 1: delete temp files
        DirectoryInfo directory = new DirectoryInfo(Path.Combine(Application.dataPath, "Temp"));
        foreach (FileInfo file in directory.GetFiles()) file.Delete(); 

        Random3 = Random.Range(1000, 9999);
        Debug.Log("r 3 = " +  Random3);

        parts = new List<GameObject>();
        activeParts = new List<ActivePart>();
        subTypes = new List<SubtitleType>();
        int[] build_times = GenerateComments();
        int build_times_index = 0;
        int build_time = 0;

        start_time = Time.time;
        //LoadAnimation("D:\\GameDe\\GLTFmodl\\magnet_shroom.animation.json");
        //mushMaterial = AddMaterial("Assets/Characters/Plants/split_pea.png");
        mushMaterial = AddMaterial("Assets/Animations/Materials/uv1.png");
        audioSource = gameObject.AddComponent<AudioSource>();


        GameObject emptyObject2 = new GameObject("MyEmptyObject");
        emptyObject2.transform.position = new Vector3(0, 3, 0);

        activeCount = 100;
        build_time = build_times[build_times_index++];
        cameras.Add(addCameraMove(activeCount, activeCount + build_time, new Vector3(-2, 1, -2), new Vector3(-2, 1, -2), emptyObject2, new Vector3(0, 0, 0)));
        activeCount += build_time;


        for (int j = 0; j < parts.Count; j++)
        {
            GameObject part = parts[j];
            // 处理字幕
            int count = CountDirectChildCubes(part.transform);
            if (count > 0)
            {
                Vector3 remianPos0 = Vector3.zero; ;
                Vector3 remainPos2 = Vector3.zero;
                for (int i = 0; i < count; i++)
                {
                    if (i < 4)
                    {
                        build_time = build_times[build_times_index++];
                        activeCount += build_time;
                    }

                    Vector3 size = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                    ActivePart ap = new ActivePart(part.transform.name,
                    part.transform.GetChild(i).gameObject, activeCount - build_time, build_time, size, j);
                    if(subTypes[j] == SubtitleType.BaseColor) ap.materialStartGray = true;
                    activeParts.Add(ap);
                    GameObject emptyObject = new GameObject("MyEmptyObject");
                    Vector3 bound = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                    Vector3 ssize = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                    emptyObject.transform.position = bound;

                    // 根据bound.x和bound.z判断象限
                    float baseAngle = 0f;
                    if (bound.x > 0 && bound.z > 0) baseAngle = 45f;  // 第一象限
                    else if (bound.x < 0 && bound.z > 0) baseAngle = 135f;  // 第二象限
                    else if (bound.x < 0 && bound.z < 0) baseAngle = 225f;  // 第三象限
                    else if (bound.x > 0 && bound.z < 0) baseAngle = 315f;  // 第四象限
                    else if (Mathf.Abs(bound.x) < 0.1f)
                    {
                        if (bound.z > 0) baseAngle = Random.Range(0, 2) == 0 ? 45f : 135f;  // 第一或第二象限
                        else baseAngle = Random.Range(0, 2) == 0 ? 225f : 315f; // 第三或第四象限
                    }
                    else if (Mathf.Abs(bound.z) < 0.1f)
                    {
                        if (bound.x > 0) baseAngle = Random.Range(0, 2) == 0 ? 45f : 315f;  // 第一或第四象限
                        else baseAngle = Random.Range(0, 2) == 0 ? 135f : 225f; // 第二或第三象限
                    }

                    float randomAngle = baseAngle + Random.Range(-10f, 10f);  // 在±10度范围内偏移
                    float randomDistance = ssize.magnitude * Random.Range(1.5f,3.0f);
                    float rx = bound.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
                    float ry = bound.y * Random.Range(-0.2f, 0.2f) + ssize.magnitude * Random.Range(-0.2f, 0.2f);
                    if (bound.y < 0) ry = -ry;
                    float rz = bound.z + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
                    Vector3 bound2 = new Vector3(rx, ry, rz);
                    cameras.Add(addCameraMove(activeCount - build_time, activeCount, bound2, bound2, emptyObject, new Vector3(0, 0, 0)));

                    remianPos0 = bound;
                    remainPos2 = bound2;
                }



                if(subTypes[j] == SubtitleType.BaseColor)
                {
                    build_time = build_times[build_times_index++];
                    activeCount += build_time;
                    for (int i = 0; i < count; i++)
                    {
                        activeParts.Add(new ActivePart(part.transform.GetChild(i).gameObject, activeCount - build_time, build_time));
                    }

                    float colorr = Random.Range(0f, 1.0f);
                    if (colorr < 0.3)
                    {
                        cameras[cameras.Count - 1].AddBuildTime(build_time);
                    }
                    else
                    {

                        GameObject emptyObject3 = new GameObject("MyEmptyObject");
                        emptyObject3.transform.position = remianPos0;
                        remainPos2 = remianPos0 + (remainPos2 - remianPos0) * Random.Range(0.8f, 1.2f);
                        Quaternion rotation = Quaternion.AngleAxis(Random.Range(-60f, 60f), Vector3.up);
                        Vector3 newCameraPosition = remianPos0 + rotation * (remainPos2 - remianPos0);
                        cameras.Add(addCameraMove(activeCount - build_time, activeCount, remainPos2, newCameraPosition, emptyObject3, new Vector3(0, 0, 0)));
                    }
                }

              
            }
        }

        generatedAnimal.SetActive(true);

        build_time = build_times[build_times_index++];
        cameras.Add(addCameraMove(activeCount, activeCount + build_time, new Vector3(-3, 1, -3), new Vector3(3, 1, -3), emptyObject2, new Vector3(0, 0, 0)));
        activeCount += build_time;
    }

    public bool checkScene = false;
    public bool fastCheckScene = false;
    private int FrameCount = 0;
    private int CheckAudioCount = 0;
    private bool CheckAudio = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(activeCount < 100)
        {
            Debug.LogError(" active count too small");
            return;
        }

        if (enableVoice)
        {
            audioClipPath = "Audio/model_" + Random3.ToString();
            CheckAudioCount++;
            if (!CheckAudio && CheckAudioCount % 50 == 0 && CheckAudioCount > 200)
            {
                AssetDatabase.Refresh();
                FrameCount = -1;
                Debug.Log("Check Audio Failed " + CheckAudioCount);
                modelClip = Resources.Load<AudioClip>(audioClipPath);
                if (modelClip != null)
                {
                    // 一旦加载成功，设置音频片段并开始播放
                    audioSource.clip = modelClip;
                    audioSource.Play();
                    CheckAudio = true;
                }
            }
        }



        for (int i = 0; i < activeParts.Count; i++)
        {
            int frame0 = activeParts[i].activeStartFrame;
            int frame1 = activeParts[i].activeStartFrame + activeParts[i].buildFrame;
            if (FrameCount == frame0)
            {
                if (activeParts[i].modelMode) SetChildActive(generatedAnimal.transform, activeParts[i].name);
                if(activeParts[i].modelMode && activeParts[i].materialStartGray) SetChildMaterial(generatedAnimal.transform, activeParts[i].name, mushMaterial);
                else SetChildMaterial(generatedAnimal.transform, activeParts[i].name, activeParts[i].material);
           
            }
            if (FrameCount >= frame0 && FrameCount < frame1)
            {
                if (activeParts[i].modelMode)
                {
                    float ratio = (FrameCount - frame0) * 1.2f / activeParts[i].buildFrame;
                    if (ratio > 1.0f) ratio = 1.0f;
                    Vector3 size = activeParts[i].size;
                    Vector3 scale = activeParts[i].transform.localScale;
                    float[] localScale = new float[] { size.x, size.y, size.z };
                    int maxIndex = 0;
                    if (size.y > size.x && size.y > size.z) maxIndex = 1;
                    if (size.z > size.y && size.z > size.x) maxIndex = 2;
                    localScale[maxIndex] = ratio * localScale[maxIndex];
                    SetChildLocalScale(generatedAnimal.transform, activeParts[i].name, new Vector3(localScale[0], localScale[1], localScale[2]));

                    if (activeParts[i].strechMode == 1)
                    {

                        Vector3 lp = activeParts[i].pos;
                        float[] localPos = new float[] { lp.x, lp.y, lp.z };
                        localPos[maxIndex] = localPos[maxIndex] - 0.5f * (1.0f - ratio) * localPos[maxIndex];
                        SetChildLocalPos(generatedAnimal.transform, activeParts[i].name, new Vector3(localPos[0], localPos[1], localPos[2]));
                    }

                }
            }
        }

        for(int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i].Run(FrameCount))
            {
                break;
            }
        }

        FrameCount++;
        if((FrameCount > activeCount + 5 && checkScene) || fastCheckScene)
        {
            SceneManager.LoadScene("ShowTime");
        }
    }

    void TraverseChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            TraverseChildren(child);
        }
        parts.Add(parent.gameObject);
    }

    string[] comment_turn = { "first I will be making the Villager better by turning it into a villager Knight", 
                            "we want to make the _prename better",
    "_prename looks kinds of boring, so let`s make it way better"};

    string[] comment_all_start = { "all right I'm already here to start making our _part mob",
                                    "the first thing I'm going to do to make our _part is to create a standard base",
                                    "and the _part is going to be quite funny",
                                    
        };


    // fact + feel
    string[] comment_base_dir = { " for" };
    string[] comment_base_feel = { " these one will be _feel, trust me",
                                "my goal is to see if i can make this _name almost _feel",
                                "these guy is looking _feel", "to make it really get that _like look",
                                " i will make a _part like this",
                                "as i mentioned before the _part is _desc than the regular one",
                                "because i want ot make it super _feel _name",
                                "the real work is going to making its _part look much much better",
                                "to make it really cool"};

    // if has part add comment_dir
    // add _color to _part ahead,
    // base1 and 50% base2 , 50% no
    //_part may be cube, things when coloring

    // 由于是第一，所以必须说明_part
    // 每组中的第一个正方体  
    string[] comment_base_fact = { "i am going to create a base for his _part", 
                               // "now for the _prepart it has a _desc _part",
                                "i have to turn these into _part",
                                "a _part would work for here",
                                "i will even give this _part",
                                "i will make these piece the new _part",
                                "some _desc _part just like these",
                                "tis _part is going to have to get a lot _desc",
                                "i will attach _part to the _name",
                                " i made it _part like this",
                                "i also wanted to make the _part extremely _feel looking",
                                "with these _feel _part ",
                                "adding these _part are definitely going to help",
                                "when i think of _name i think of _feel, so i am going to add these _aprt",
                                "the real _feel _part is going to be this _desc _part for the bdoy",
                                "we will start by adding a cube here and stretching it to form the _part",
                                "he has a somewhat _desc _part, so I would have to make it like this",
                                "so I will create here and pull a cube down to make the _part",
                                "i will put a _part to be able to make a very cool animation",
                                "to make the _part I think it has to be another color",
                                "i will made his _part and then added all the details",
                                "i also thought it would be cool to add this _desc _part",
                                "give this guy some super _desc _part",
                                "i will need those _desc _part though",
                                "these _part are going to magically attached to the _name",
                                "i want to make it look like there`s a _feel _part",
                                " then i will add a base to start making his _part",
                                "I will start by making this _desc piece of _part",
                                "and this part will becone _part",
                                "but i also can`t forget to give him a _part",
                                "this _part looks so dumb right now, but we will fix later",
                                "it`s time to get a total _part",
                                "we will come back to this _desc _axe later",
                                "these _part will be _desc ones like these",
                                "I adjusted _part to make it more pointed",
                                "then i went to the part of the _part",
                                "and on _dir of it, the _part part",
                                "i will be using all parts of the _name to make this _part look totally _feel",
                                "i will start by with a regular _part, but it going to have to get a lot _desc",
                                "now for th awesome part, the _part",
                                "this part here is the _part",
                                "i use these part to make some _part",
                                "let`s start to give it those _name with some _desc _part",
                                "let`s get to work the _part, it`s looks amazing",
                                "we will going to need some _part in there just like that",
                                "can you image if we made a _name without _part?",
                                "i nearly forgot to give it a _part",
                                "i am going to use this as the _part",
                                "once i got the _part part complete",
                                "i decided to give it _part like this",
                                "make some room for this _part",
                                "the regular _part is _desc, so i started editing to create some of these _part",
                                "unlike the _prename, we will be making all these _part parts way _desc",
                                "now i am going to add _part",

                                // chatgpt generated
                                "i decided to color the _part to match the overall look",
                                "he's going to need a better _part for balance",
                                "i'll add a _desc _part here to give it more personality",
                                "the next step is to tweak this _part so it fits better",
                                "i'm thinking of making this _part more _feel",
                                "we can adjust the size of the _part to get the right proportion",
                                "this character definitely needs a stronger _part",
                                "i will add a _part to the _name to complete the design",
                                "this _desc _part is going to make a huge difference",
                                "i'll work on this _part first, then focus on the details",
                                "it's time to add a custom _part to really make it stand out",
                                "i'm going to try a _desc _part here to see how it looks",
                                "once we finalize the _part, everything else will come together",
                                "i might make this _part a little more _feel to match the theme",
                                "this _part is crucial for the overall aesthetic",
                                "i'm planning to blend this _part with a subtle gradient",
                                "adding a _feel _part here will give it a more dynamic look",
                                "this section needs a totally new _part",
                                "i think i'll switch the _part to something more _desc",
                                "now i'll focus on adding the _part before moving on",
                                "let's enhance the _part with some detailed textures",
                                "i'll create a _desc _part to give it more visual interest",
                                "the next _part will be the final piece of the puzzle",
                                "this _feel _part should balance out the overall design",
                                "i'll replace this old _part with something more modern",
                                "this _part needs to be _desc so it fits with the rest",
                                "we can't forget to adjust the _part to match the _name",
                                "i'll try a different color scheme for the _part",
                                "let's add some shine to the _part for a polished finish",
                                "i need to tweak the size of the _part to make it work",
                                "we're going to need a stronger _part for support",
                                "this _part should be more _feel to complete the vibe",
                                "i'll make sure this _part is perfectly aligned",
                                "i'll sculpt the _part so it blends in seamlessly",
                                "we can duplicate this _part and adjust the position",
                                "i'll attach this _desc _part to the _name",
                                "i need to add a _part that's more _desc for contrast",
                                "let's focus on perfecting the _part before we move on",
                                "i'll make sure the _part is sized just right",
                                "this _feel _part will give it that unique touch",
                                "i'll finish up this _part and then work on the next one",
                                "let's highlight this _part to draw more attention",
                                "i think this _part needs to be a little more _desc",
                                "once the _part is in place, the design will feel complete",
                                "i'll layer a _desc _part on top for extra depth",
                                "this _part needs to complement the overall structure",
                                "we'll add some fine details to the _part to enhance it",
                                "let's smooth out the edges of the _part to soften the look",
                                "i'll experiment with a different material for the _part",
                                "this _desc _part should add more contrast to the design",
                            };
    // 必须是 first cube
    string[] comment_duplicate = { "I duplicated the _part to the other side",
                                    "select _part and duplicate to the other side",
                                    "then I will duplicate here down",
                                    "i create another piece",
                                    "duplicate that over again",
                                    "i wanted another _part, so pull and one more",
                                "before I create the rest of the body I need to make the other _part",
                                ""};


    string[] comment = { " we need a large _part",
                        "more or less like this",
                        " and _dir next a slightly a _desc _part",
                        "he has very _desc _part by the way",
                        "something like this, see ?",
                        " and the _part !!!",
                        "look at this little _part",
                        "pull one more",
                        "add a cube here",
                        "we will bring a _part",
                        "then add some realistic _part to it",
                         "so we will make it a _part",
                         "and stretch one more time",
                         "just stretch the _desc one",
                         "and another bigger cube",
                         "shrink it down",
                         "one more",
                         " pull another _part _dir here", " pull one more here",
                          };

    //https://youtu.be/DcDNgGLpJgs?t=344
    string[] comment_appedn = { "that later I can animate and rotate any way I want look" };

    string[] comment_finish = { " it`s already looks like a _name", 
                                "the shape is gonna be totally different, it gonna be super _desc", 
                                "ok, cool",
                                "let`s see how _feel it really is in minecraft",
                                "i would not want to mess with these _name",
                                "it's already taking a bit of shape", 
                                "after some work i am pretty happy with how it came together",
                                "and look our nightmare _name is practically ready",
                                "let`s find _name for sure in game",
                                "i feel pretty good about what the _name has turned into",
                                "let me get back to Minecraft and well",
                                "so let's see how this _name turned out in mind", 
                                "and our _name is ready look how it turned out"};

    string[] comment_color = { "I'm going to start doing the texture.",
                                "i start painting him the color he is. in this case it's _color",
                                "i colored it _color",
                                "attach it and paint it all _color",
                                "these _part one will be this _color",
                                " and there will be _color too",
                                "let`s give it the sense that it has some _feel",
                                " there are some _color for his _part",
                                "turn it all _part color",
                                "let`s grab this _color and use it on _part",
                                "it absolutely no details, so i paint it all _color",
                                "a _color _part would be perfect",
                                "the start to recolor the whole thing",
                                "i will change it _part to _color",
                                "now for the _color _part on the _name",
                                "let`s recolor it so it will be more _feel",
                                "it`s time to retexturing _part _color",
                                "i will recolor it _color to give it these _part",
                                "i did some subtle texturing all around",
                                "i created this _part texture and will use for those",
                                "_name also have these _color colors in the _part",
                                "these _color colors works perfectly for the _part",
                                "i put this pattern on its _part",
                                "I will start by adding a texture to it about this color which is _color",
                                "okay finishing the texture it looks like this",
                                "his _part will be a slightly different _color color",
                                "make him _color",
                                "I will also start texturing her _part", 
                                "I use a very good style to make _color texture",
                                "i added this _color design to the _part",
                                "and now look at those _color _part",
                                "this _color is absolutely ridiculous right now",
                                "now it`s time to _color _part",
                                "get the basic coloring done",
                                "you know we had to give it the _color, it look like kinds cursed",
                                "let`s make it more look like _name",
                                };


    string[] details = { "I just thought of adding a little extra detail" };
    string[] comment_legs = { "create some legs for our friends to walk on" };

}

/*
 
import wave
import random
from VoicGenerator import TextToSpeech
from pydub import AudioSegment
import sys

def get_audio_length(file_path):
    # 打开音频文件
    with wave.open(file_path, 'rb') as audio_file:
        # 获取音频文件的帧数
        num_frames = audio_file.getnframes()
        # 获取音频文件的帧率（帧数/秒）
        frame_rate = audio_file.getframerate()
        # 计算音频文件的长度（秒）
        audio_length = num_frames / frame_rate
        return round(audio_length, 2)
    

lines = sys.argv[1].splitlines()  # 第一个字符串参数
asset_path = sys.argv[2]
line_count = 0
speech = TextToSpeech()
for line in lines:
    # speech.save_audio(line.strip())
    line_count += 1
    
time_str = ""
silent_audio = AudioSegment.silent(duration=2000)
for t in range(1, 1+ line_count):
    audio_file_name = "index_" + str(t) + ".wav"
    audio = AudioSegment.from_file(audio_file_name)
    current_duration = len(audio)  # 当前音频的时长（毫秒）
    time_str += str(current_duration) + " "
    silent_audio += audio

silent_audio.export("E:/software/unityproject/gudu/ddd/Assets/Resources/Audio/model.wav", format="wav")
print(time_str)
            
 
 */