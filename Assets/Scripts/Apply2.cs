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

public class Apply2 : MonoBehaviour
{
    private List<GameObject> parts;
    private Material mushMaterial;
    private float start_time;

    public bool generateStory;
    public struct ActivePart
    {
        public string parentName;
        public string name;
        public int index;
        public int activeStartFrame;
        public int continueFrame;
        public Vector3 size;
        public Transform transform;

        public ActivePart(string parentName, string name, int activeStartFrame, int continueFrame, Vector3 size, int index, Transform transform)
        {
            this.name
                = name;
            this.activeStartFrame = activeStartFrame;
            this.parentName = parentName;
            this.index = index;
            this.continueFrame = continueFrame;
            this.size = size;
            this.transform = transform;
        }
    }

    int activeCount = 0;
    private List<ActivePart> ActiveParts;
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


   GameObject generatedAnimal;

   int[] GenerateComments()
   {
        string prefab_name = "SplitPea";
        string path = "Assets/Characters/Plants/Prefab/" + prefab_name + ".prefab";
        GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        generatedAnimal = Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        TraverseChildren(generatedAnimal.transform);

        SubModelReplacer placer = new SubModelReplacer();
        placer.name = prefab_name;
        placer.like = "pea shooter";
        placer.color = "green";
        placer.desc = "";
        placer.dir = "top";

        subtitles.Add(AddModelSub(comment_all_start.ToArray(), placer));
        Debug.Log(" first comment " + subtitles[subtitles.Count - 1]);

        

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
                            if(childPart.y > 3)
                            {
                                placer.desc = "longer";
                            }
                            else
                            {
                                placer.desc = "shorter";
                            }
                        }
                        
                        if (i == count - 1)
                        {

                            placer.name = "cube";
                            subtitles.Add(AddModelSub(comment_finish.Concat(comment).ToArray(), placer));
                        }
                        else if (first_cube)
                        {
                            placer.name = part.name;
                            subtitles.Add(AddModelSub(comment_base.ToArray(), placer));
                        }
                        else
                        {
                            placer.name = part.name;
                            subtitles.Add(AddModelSub(comment.ToArray(), placer));
                        }

                        if (first_cube)
                        {
                            first_cube = false;
                        }
                        Debug.Log(" parent name =" + part.transform.name + " child index = " + i + " comment " + subtitles[subtitles.Count - 1]);

                    }
                }
            }
        }

        subtitles.Add(AddModelSub(comment_finish.ToArray(), placer));
        Debug.Log(" final comment " + subtitles[subtitles.Count - 1]);

        string writeto = "";
        foreach (string sub in subtitles)
        {
            writeto += sub + "\n";
        }
        // 设置Python脚本路径
        string pythonScriptPath = "F:/DaisyDay/test.py";

        // 要传递给Python的字符串
        string stringArg1 = writeto;
        string stringArg2 = Application.dataPath + "/Resources/Audio/";
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

    private string audioClipPath = "Audio/model"; // 声音文件的路径，假设在Assets/Resources/Audio/mySound.wav

    private AudioSource audioSource;

    private AudioClip modelClip;
    void Start()
    {
        parts = new List<GameObject>();
        ActiveParts = new List<ActivePart>();
       int[] build_times =  GenerateComments();
        int build_times_index = 0;
        int build_time = 0;

       start_time = Time.time;
       LoadAnimation("D:\\GameDe\\GLTFmodl\\magnet_shroom.animation.json");
       mushMaterial = AddMaterial("Assets/Characters/Plants/split_pea.png");

        audioSource = gameObject.AddComponent<AudioSource>();
        modelClip = Resources.Load<AudioClip>(audioClipPath);
        if(modelClip != null)
        {
            audioSource.clip = modelClip;
            audioSource.Play();
        }

        GameObject emptyObject2 = new GameObject("MyEmptyObject");
        emptyObject2.transform.position = new Vector3(0, 1, 0);

        activeCount = 100;
        build_time = build_times[build_times_index++];
        cameras.Add(addCameraMove(activeCount, activeCount + build_time, new Vector3(-2, 1, -2), new Vector3(-2, 1, -2), emptyObject2, new Vector3(0, 0, 0)));
        activeCount += build_time;

        
       for(int j = 0; j < parts.Count; j++)
       {
           GameObject part = parts[j];



           // 处理字幕
           int count = CountDirectChildCubes(part.transform);
           if(count > 0)
           {
               for (int i = 0; i  < count; i++)
               {
                   if(i < 4)
                   {
                       build_time = build_times[build_times_index++];
                       activeCount += build_time;
                   }

                   Vector3 size = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                   ActiveParts.Add(new ActivePart(part.transform.name, 
                   part.transform.GetChild(i).transform.name, activeCount - build_time, build_time, size,j, part.transform.GetChild(i).transform));
                   GameObject emptyObject = new GameObject("MyEmptyObject");
                   Vector3 bound = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                   Vector3 ssize = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                   emptyObject.transform.position = bound;
                   float mag = bound.magnitude;
                   float bx = bound.x * Random.Range(1.5f, 2.5f) + mag * Random.Range(-0.2f, 0.2f);
                   float by = bound.y * Random.Range(0.8f, 1.2f) + mag * Random.Range(-0.2f, 0.2f);
                    float bz = bound.z * Random.Range(1.5f, 2.5f) + mag * Random.Range(-0.2f, 0.2f);
                    Vector3 bound2 = new Vector3(bx, by, bz);
                    cameras.Add(addCameraMove(activeCount - build_time, activeCount, bound2, bound2, emptyObject, new Vector3(0, 0, 0)));
               }
           }
       }
       
       generatedAnimal.SetActive(true);

       build_time = build_times[build_times_index++];
       cameras.Add(addCameraMove(activeCount, activeCount + build_time, new Vector3(-2, 1, -2), new Vector3(2, 1, -2), emptyObject2, new Vector3(0, 0, 0)));
 }
    

private int FrameCount = 0;

// Update is called once per frame
void FixedUpdate()
{
   for (int i = 0; i < ActiveParts.Count; i++)
   {
       //Debug.Log(" active name = " + ActiveParts[i].name + " count " + ActiveParts[i].activeCount);
       if(FrameCount == ActiveParts[i].activeStartFrame)
       {
           SetChildActive(generatedAnimal.transform, ActiveParts[i].name);
       }
       if(FrameCount >= ActiveParts[i].activeStartFrame && FrameCount < ActiveParts[i].activeStartFrame + ActiveParts[i].continueFrame)
       {
           float ratio = (FrameCount - ActiveParts[i].activeStartFrame)  * 1.2f / ActiveParts[i].continueFrame;
           if(ratio > 1.0f) ratio = 1.0f;
           Vector3 size= ActiveParts[i].size;
           float[] localScale = new float[] { 1, 1, 1};
           int maxIndex = 0;
           if (size.y > size.x && size.y > size.z) maxIndex = 1;
           if(size.z > size.y && size.z > size.x) maxIndex = 2;
           localScale[maxIndex] = ratio;
           SetChildLocalScale(generatedAnimal.transform, ActiveParts[i].name, new Vector3(localScale[0], localScale[1], localScale[2]));
            
       }
       if (FrameCount == ActiveParts[i].activeStartFrame + activeCount)
       {
           SetChildMaterial(generatedAnimal.transform, ActiveParts[i].name, mushMaterial);
       }
   }

   for(int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i].Run(FrameCount))
            {
                break;
            }
        }
   
   float e_time = Time.time - start_time;
   while(e_time >= vectorList[vectorList.Count - 1].x)
   {
       e_time -= vectorList[vectorList.Count - 1].x;
   }
   for (int i = 0;i < vectorList.Count - 1; i++)
   {
       if(e_time >= vectorList[i].x && e_time < vectorList[i + 1].x)
       {
           float r = (e_time - vectorList[i].x) / (vectorList[i + 1].x - vectorList[i].x);
           float rx = (vectorList[i+1].y - vectorList[i].y)*r + vectorList[i].y  ;
           float ry = (vectorList[i+1].z - vectorList[i].z)*r + vectorList[i].z ;
           float rz = (vectorList[i+1].w - vectorList[i].w)*r + vectorList[i].w;
          // RotateChild(generatedAnimal.transform, "01UpperBody", Quaternion.Euler(rx, ry, rz));
       }
   }
   
   FrameCount++;
}

void TraverseChildren(Transform parent)
{
   foreach (Transform child in parent)
   {
       TraverseChildren(child);
   }
   parts.Add(parent.gameObject);
}



    string[] comment_all_start = { "all right I'm already here to start making our _part mob" };

    // 由于是第一，所以必须说明_part
    // 每组中的第一个正方体  
    string[] comment_base = { "i am going to create a base for his _part， i`ll make a _part like this", 
                                " then i`ll add a base to start making his _part",
                                "now i`m going to add _part",
                                "to make the head I think it has to be another color",
                                "he has a somewhat smiling head, so I would have to make it like this",
                                "these _part will be _desc ones like these",
                                "I adjusted _part to make it more pointed",
                                //"so I created with scarier _part much _desc_adj for _name looks like _desc_noun",
                                "i`ll put a _part to be able to make a very cool animation",
                                "i`ll made his _part and then added all the details",
                                "then i went to the part of the _part",
                                "and on _dir of it, the _part part",
                                "so I will create here and pull a cube down to make the legs",
                                "this part here is the _part",
                                "I'll start by making this _desc piece of _like"};

    string[] comment = { " we need a large _part",
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

    string[] comment_finish = { " it`s already looks like a _part", "the shape is gonna be totally different, it gonna be super _desc", "ok, cool",
                                "it's already taking a bit of shape", "and look our nightmare _part is practically ready",
                                "so let's see how this turned out in mind", "and our catnap is ready look how it turned out"};

    string[] comment_color = { "I'm going to start doing the texture. and start painting him the color he is. in this case it's orange",
                                "his belly will be a slightly different color", 
                                "I will also start texturing her body", "I use a very good style to make texture",};
    string[] comment_duplicate = { "I duplicated the _part to the other side",
    "select and duplicate to the other side"};

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