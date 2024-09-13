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

    string GetBaseColor(float r, float g, float b)
    {
        if (Mathf.Abs(r - g) <= 0.1 && r - b >= 0.1 && g - b >= 0.1) return "yellow";    // 红+绿=黄
        if (Mathf.Abs(r - b) <= 0.1 && r - g >= 0.1 && b - g >= 0.1) return "pink";   // 红+蓝=品红
        if (Mathf.Abs(g - b) <= 0.1 && g - r >= 0.1 && b - r >= 0.1) return "cyan";      // 绿+蓝=青

        if (r > g && r > b) return "red";
        if (g > r && g > b) return "green";
        if (b > r && b > g) return "blue";

        return "gray"; // 如果三者接近，输出灰色
    }

    Color GetMainColor(Texture2D tex)
    {

        Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();
        Color[] pixels = tex.GetPixels(); // Get all the pixels from the texture

        // Traverse all pixels
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];

            // Ignore fully transparent pixels
            if (pixel.a != 1.0) continue;
            // Quantize the RGB values to 17 discrete levels (0, 16, 32, ..., 240)
            Color quantizedColor = new Color(
                Mathf.Floor(pixel.r * 255 / 16) * 16 / 255f,
                Mathf.Floor(pixel.g * 255 / 16) * 16 / 255f,
                Mathf.Floor(pixel.b * 255 / 16) * 16 / 255f,
                pixel.a // Keep alpha as it is
            );

            // Count the occurrence of the quantized color
            if (colorCounts.ContainsKey(quantizedColor))
                colorCounts[quantizedColor]++;
            else
                colorCounts[quantizedColor] = 1;
        }

        // Find the most common color
        Color mainColor = Color.clear;
        int maxCount = 0;

        foreach (var colorCount in colorCounts)
        {
            if (colorCount.Value > maxCount)
            {
                maxCount = colorCount.Value;
                mainColor = colorCount.Key;
            }
        }

        return mainColor;
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
        string prefab_name = "BlackGoldenGolem";

        GameObject sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/iron_golem.prefab");
        Texture2D sourceTexture = LoadTexture("Assets/guard.png");
        if (enableRandomModel)
        {
            generatedAnimal = HumanoidGenerator.CreateHumaoid(prefab_name, sourceModel, sourceTexture);
            DataTransfer.messageToPass = "Assets/guard.prefab";
            DataTransfer.prefabName = prefab_name;
            DataTransfer.modelOffset = new Vector3(0, FindModelOffset(generatedAnimal.transform), 0);
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
        placer.like = "pea shooter";
        placer.color = "green";
        placer.desc = "";
        placer.dir = "top";

        subtitles.Add(AddModelSub(comment_all_start.ToArray(), placer));
        Debug.Log(" first comment " + subtitles[subtitles.Count - 1]);

        List<string> added_part = new List<string>();

        for (int j = 0; j < parts.Count; j++)
        {
            GameObject part = parts[j];
            if (part.transform.childCount == 0)
            {
                part.SetActive(false);
            }
            int count = CountDirectChildCubes(part.transform);
            if (count > 0)
            {
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
                                    Debug.Log("added = " + added + " nowed " + nowed);
                                    du = 4;
                                    break;
                                }
                            }

                            int r = Random.Range(0, 6 + du);
                            placer.name = part.transform.name;
                            if (r < 6)
                            {
                                subtitles.Add(AddModelSub(comment_base.ToArray(), placer));
                            }
                            else
                            {

                                subtitles.Add(AddModelSub(comment_duplicate.ToArray(), placer));
                            }


                            first_cube = false;
                        }
                        else if (i == count - 1)
                        {

                            placer.name = "cube";
                            subtitles.Add(AddModelSub(comment_finish.Concat(comment).ToArray(), placer));
                        }
                        else
                        {
                            placer.name = part.name;
                            subtitles.Add(AddModelSub(comment.ToArray(), placer));
                        }

                        Debug.Log("name = " + part.transform.name + " child index = " + i + " comment === " + subtitles[subtitles.Count - 1]);

                    }
                }

                placer.name = part.name;
                Color mainColor = GetMainColor((Texture2D)part.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture);
                placer.color = GetBaseColor(mainColor.r, mainColor.g, mainColor.b);
                subtitles.Add(AddModelSub(comment_color.ToArray(), placer)); 
                Debug.Log(" parent name =" + part.transform.name + " color special comment === " + subtitles[subtitles.Count - 1]);

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
            string pythonScriptPath = "F:/DaisyDay/test.py";

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

    private string audioClipPath = "Audio/model"; // 声音文件的路径，假设在Assets/Resources/Audio/mySound.wav

    private AudioSource audioSource;

    private AudioClip modelClip;

    private int Random3;
    void Start()
    {
        audioClipPath = "Audio/model_3275";

        Random3 = Random.Range(1000, 9999);
        Debug.Log("r 3 = " +  Random3);

        parts = new List<GameObject>();
        activeParts = new List<ActivePart>();
        int[] build_times = GenerateComments();
        int build_times_index = 0;
        int build_time = 0;

        start_time = Time.time;
        LoadAnimation("D:\\GameDe\\GLTFmodl\\magnet_shroom.animation.json");
        //mushMaterial = AddMaterial("Assets/Characters/Plants/split_pea.png");
        mushMaterial = AddMaterial("Assets/Animations/Materials/uv1.png");
        audioSource = gameObject.AddComponent<AudioSource>();


        GameObject emptyObject2 = new GameObject("MyEmptyObject");
        emptyObject2.transform.position = new Vector3(0, 1, 0);

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
                    activeParts.Add(new ActivePart(part.transform.name,
                    part.transform.GetChild(i).gameObject, activeCount - build_time, build_time, size, j));
                    GameObject emptyObject = new GameObject("MyEmptyObject");
                    Vector3 bound = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                    Vector3 ssize = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                    emptyObject.transform.position = bound;

                    // 根据bound.x和bound.z判断象限
                    float baseAngle = 0f;

                    if (bound.x > 0 && bound.z > 0)
                    {
                        baseAngle = 45f;  // 第一象限
                    }
                    else if (bound.x < 0 && bound.z > 0)
                    {
                        baseAngle = 135f;  // 第二象限
                    }
                    else if (bound.x < 0 && bound.z < 0)
                    {
                        baseAngle = 225f;  // 第三象限
                    }
                    else if (bound.x > 0 && bound.z < 0)
                    {
                        baseAngle = 315f;  // 第四象限
                    }
                    else if (Mathf.Abs(bound.x) < 0.1f)
                    {
                        if (bound.z > 0)
                        {
                            baseAngle = Random.Range(0, 2) == 0 ? 45f : 135f;  // 第一或第二象限
                        }
                        else
                        {
                            baseAngle = Random.Range(0, 2) == 0 ? 225f : 315f; // 第三或第四象限
                        }

                    }
                    else if (Mathf.Abs(bound.z) < 0.1f)
                    {
                        if (bound.x > 0)
                        {
                            baseAngle = Random.Range(0, 2) == 0 ? 45f : 315f;  // 第一或第四象限
                        }
                        else
                        {
                            baseAngle = Random.Range(0, 2) == 0 ? 135f : 225f; // 第二或第三象限
                        }


                    
                    }

                    float randomAngle = baseAngle + Random.Range(-10f, 10f);  // 在±10度范围内偏移



                    // 随机生成距离（10到12之间）
                    float randomDistance = ssize.magnitude * Random.Range(2f, 4f);

                    // 计算x和z轴上的坐标（y轴保持为0）
                    float rx = bound.x + randomDistance * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
                    float ry = Mathf.Abs(bound.y) + ssize.magnitude * Random.Range(1f, 3f);
                    if (bound.y < 0) ry = -ry;
                    float rz = bound.z + randomDistance * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
                    Vector3 bound2 = new Vector3(rx, ry, rz);
                    cameras.Add(addCameraMove(activeCount - build_time, activeCount, bound2, bound2, emptyObject, new Vector3(0, 0, 0)));

                    remianPos0 = bound;
                    remainPos2 = bound2;
                }

                build_time = build_times[build_times_index++];
                activeCount += build_time;
                for (int i = 0; i < count; i++)
                {
                    activeParts.Add(new ActivePart(part.transform.GetChild(i).gameObject, activeCount - build_time, build_time));
                }

                GameObject emptyObject3 = new GameObject("MyEmptyObject");
                emptyObject3.transform.position = remianPos0;
                Quaternion rotation = Quaternion.AngleAxis(Random.Range(-60f,60f), Vector3.up);
                Vector3 newCameraPosition = remianPos0 + rotation * (remainPos2 - remianPos0);
                cameras.Add(addCameraMove(activeCount - build_time, activeCount, remainPos2, newCameraPosition, emptyObject3, new Vector3(0, 0, 0)));
                //cameras[cameras.Count - 1].AddBuildTime(build_time);
            }
        }

        generatedAnimal.SetActive(true);

        build_time = build_times[build_times_index++];
        cameras.Add(addCameraMove(activeCount, activeCount + build_time, new Vector3(-2, 1, -2), new Vector3(2, 1, -2), emptyObject2, new Vector3(0, 0, 0)));
    }

    public bool checkScene = false;
    private int FrameCount = 0;
    private int CheckAudioCount = 0;
    private bool CheckAudio = false;

    // Update is called once per frame
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
                if (activeParts[i].modelMode)
                {
                    SetChildActive(generatedAnimal.transform, activeParts[i].name);
                    SetChildMaterial(generatedAnimal.transform, activeParts[i].name, mushMaterial);
                }
                else
                {
                    SetChildMaterial(generatedAnimal.transform, activeParts[i].name, activeParts[i].material);
                }
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

        if(FrameCount > 1 && checkScene)
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



    string[] comment_all_start = { "all right I'm already here to start making our _part mob",
                                    "the first thing I'm going to do to make our _part is to create a standard base",
                                    "and the _part is going to be quite funny",
        };

    // 由于是第一，所以必须说明_part
    // 每组中的第一个正方体  
    string[] comment_base = { "i am going to create a base for his _part， i`ll make a _part like this",
        "let's add the arm and as I mentioned the arm is a thousand times bigger than the body",
        "now for the _prepart it has a _desc _part",
                                "we'll start by adding a cube here and stretching it to form the _part",
                                "he has a somewhat _desc _part, so I would have to make it like this",
                                "so I will create here and pull a cube down to make the _part",
                                "i`ll put a _part to be able to make a very cool animation",
                                "to make the _part I think it has to be another color",
                                "i`ll made his _part and then added all the details",
                                " then i`ll add a base to start making his _part",
                                "I'll start by making this _desc piece of _part",
                                "these _part will be _desc ones like these",
                                "I adjusted _part to make it more pointed",
                                "then i went to the part of the _part",
                                "and on _dir of it, the _part part",
                                "this part here is the _part",
                                "now i`m going to add _part",
                            };
    // 必须是 first cube
    string[] comment_duplicate = { "I duplicated the _part to the other side",
                                    "select _part and duplicate to the other side",
                                    "then I'll duplicate here down",
                                    "i wanted another _part, so pull and one more",
                                "before I create the rest of the body I need to make the other _part",
                                ""};


    string[] comment = { " we need a large _part",
                        "more or less like this",
                        " and _dir next a slightly a _desc _part",
                        "he has very _desc _part by the way",
                        "something like this, see ?",
                        " and the _part !!!",
                        "pull one more",
                        "add a cube here",
                        "we will bring a _part",
                        "then add some realistic _part to it",
                         "so we will make it a _part",
                         "and stretch one more time",
                         "just stretch the _desc one",
                         "and another bigger cube",
                         "one more",
                         " pull another _part _dir here", " pull one more here",
                          };

    //https://youtu.be/DcDNgGLpJgs?t=344
    string[] comment_appedn = { "that later I can animate and rotate any way I want look" };

    string[] comment_finish = { " it`s already looks like a _part", 
                                "the shape is gonna be totally different, it gonna be super _desc", 
                                "ok, cool",
                                "it's already taking a bit of shape", 
                                "and look our nightmare _part is practically ready",
                                "let me get back to Minecraft and well",
                                "so let's see how this turned out in mind", 
                                "and our catnap is ready look how it turned out"};

    string[] comment_color = { "I'm going to start doing the texture.",
                                "i start painting him the color he is. in this case it's _color",
                                "i colored it _color",
                                "I'll start by adding a texture to it about this color which is _color",
                                "okay finishing the texture it looks like this",
                                "his _part will be a slightly different _color color",
                                "I will also start texturing her _part", 
                                "I use a very good style to make _color texture",
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