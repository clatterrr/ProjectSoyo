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
   void Start()
   {
       start_time = Time.time;
       LoadAnimation("D:\\GameDe\\GLTFmodl\\magnet_shroom.animation.json");
       mushMaterial = AddMaterial("Assets/Characters/Plants/zombie_yeti.png");
       parts = new List<GameObject>();
       ActiveParts = new List<ActivePart>();
        string prefab_name = "ZombieYeti";
       string path = "Assets/Characters/Plants/Prefab/ZombieYeti.prefab";
       GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        generatedAnimal = Instantiate(selectedPrefab, new Vector3(0,0,0), Quaternion.Euler(0,0,0));

        int[] numbers = ReadNumbersFromFile2("D:/example.txt");

        GameObject emptyObject2 = new GameObject("MyEmptyObject");
        emptyObject2.transform.position = new Vector3(0, 1, 0);
        int continous_time = 0;
        continous_time = 30;
        if (!generateStory)
        {
            continous_time = (int)(numbers[subtitles.Count] * 0.05f);
        }
        cameras.Add(addCameraMove(activeCount, activeCount + continous_time, new Vector3(-2, 1, -2), new Vector3(-2, 1, -2), emptyObject2));
        subtitles.Add(AddSub(comment_all_start.ToArray(), prefab_name, "", "", ""));
        Debug.Log(" first comment " + subtitles[subtitles.Count - 1]);
        activeCount += continous_time;


       Material greenMaterial = new Material(Shader.Find("Standard"));
       greenMaterial.color = Color.green;

       List<Material> materials = new List<Material>();
       for(int i = 0; i < 6;i++)
       {
           materials.Add(new Material(Shader.Find("Standard")));
       }
       materials[0].color = new Color(180 / 256.0f, 212 / 256.0f ,225 / 256.0f);
       materials[1].color = new Color(198 / 256.0f, 177 / 256.0f, 91 / 256.0f);
       materials[2].color = new Color(40 / 256.0f, 139 / 256.0f, 85 / 256.0f);
       materials[3].color = new Color(73 / 256.0f, 190 / 256.0f, 195 / 256.0f);
       materials[4].color = new Color(146 / 256.0f, 80 / 256.0f, 80 / 256.0f);
       materials[5].color = new Color(41 / 256.0f, 48 / 256.0f, 58 / 256.0f);


       TraverseChildren(generatedAnimal.transform);

       string writeto = "";
        
       for(int j = 0; j < parts.Count; j++)
       {
           GameObject part = parts[j];
           if(part.GetComponent<MeshRenderer>() != null )
           {
               //part.GetComponent<MeshRenderer>().material = materials[UnityEngine.Random.Range(0,6)];
           }
           if(part.transform.childCount == 0)
           {
               part.SetActive(false);
           }

           // 处理字幕
           int count = CountDirectChildCubes(part.transform);
           if(count > 0)
           {
               bool first_cube = true;
               for (int i = 0; i  < count; i++)
               {
                   continous_time = 30; 
                   if (!generateStory)
                   {
                       continous_time = (int)(numbers[subtitles.Count] * 0.05f);
                   }

                   if(i < 4)
                   {
                       Debug.Log(" name = " + part.transform.name + " time " + continous_time);
                       activeCount += continous_time;
                       string desc = "";
                       desc = "higher";
                       if(part.transform.GetChild(i).transform.localScale.y < 3)
                       {
                           desc = "shorter";
                       }

                       if (i == count - 1)
                       {
                         subtitles.Add(AddSub(comment_finish.Concat(comment).ToArray(), part.transform.name, desc, "", ""));
                       }
                       else if (first_cube)
                       {
                          subtitles.Add(AddSub(comment_base.ToArray(), part.transform.name, desc, "", ""));
                       }
                       else
                       {
                           subtitles.Add(AddSub(comment.ToArray(), "cube",desc, "", ""));
                       }

                       if (first_cube)
                       {
                           first_cube = false;
                       }
                       Debug.Log(" parent name =" + part.transform.name + " child index = " + i + " comment " + subtitles[subtitles.Count - 1]) ;
                        
                   }

                   Vector3 size = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                   ActiveParts.Add(new ActivePart(part.transform.name, 
                   part.transform.GetChild(i).transform.name, activeCount - continous_time, continous_time, size,j, part.transform.GetChild(i).transform));
                   GameObject emptyObject = new GameObject("MyEmptyObject");
                   emptyObject.transform.position = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                   cameras.Add(addCameraMove(activeCount - continous_time, activeCount, new Vector3(1, 1, -1), new Vector3(1, 1, -1), emptyObject));
               }
           }
       }
       
       generatedAnimal.SetActive(true);



        continous_time = 30;
        if (!generateStory)
        {
            continous_time = (int)(numbers[subtitles.Count] * 0.05f);
        }
        cameras.Add(addCameraMove(activeCount, activeCount + continous_time, new Vector3(-2, 1, -2), new Vector3(2, 1, -2), emptyObject2));
        subtitles.Add(AddSub(comment_finish.ToArray(), prefab_name, "", "", ""));
        Debug.Log(" final comment " + subtitles[subtitles.Count - 1]);

        if (generateStory)
        {
            foreach (string sub in subtitles)
            {
                writeto += sub + "\n";
            }
            File.WriteAllText("D://sub.txt", writeto);
        }
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
           float ratio = (FrameCount - ActiveParts[i].activeStartFrame)  * 1.0f / ActiveParts[i].continueFrame;
           Vector3 size= ActiveParts[i].size;
           float[] localScale = new float[] { 1, 1, 1};
           int maxIndex = 0;
           if (size.y > size.x && size.y > size.z) maxIndex = 1;
           if(size.z > size.y && size.z > size.x) maxIndex = 2;
           localScale[maxIndex] = ratio;
           SetChildLocalScale(generatedAnimal.transform, ActiveParts[i].name, new Vector3(localScale[0], localScale[1], localScale[2]));
            
           /*
           Vector3 part_pos = ActiveParts[i].transform.GetComponent<MeshRenderer>().bounds.center;
           Camera.main.transform.position = part_pos + new Vector3(1,1,-1);
           Camera.main.transform.LookAt(part_pos);
           */

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
    // 每组中的第一个正方体
    string[] comment_base = { "i am going to create a base for his _part， i`ll make a _part like this", 
                                " then i`ll add a base to start making his _part",
                                "now i`m going to add _part",
                                "to make the head I think it has to be another color",
                                "he has a somewhat smiling head, so I would have to make it like this",
                                "these _part will be _desc ones like these",
                                "so I adjusted hugs head to make it more pointed",
                                "so I created with scarier _part much _desc_adj for _name looks like _desc_noun",
                                "put a tail to be able to make a very cool animation",
                                "i made her dress and then added all the details",
                                "then i went to the part of the _part",
                                "and on _dir of it, the _part part",
                                "so I will create here and pull a cube down to make the legs",
                                "this part here is the _part",
                                    };

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
                     

}

/*
 
import wave
import random
from VoicGenerator import TextToSpeech
from pydub import AudioSegment

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

def contains_letter(s):
    return any(char.isalpha() for char in s)

line_count = 0
speech = TextToSpeech()
time = []
with open("D://sub.txt", 'r', encoding='utf-8') as file:
    for line in file:
        if contains_letter(line):
            speech.save_audio(line.strip())
            line_count += 1
        else:
            time.append(float(line)  * 20)
                
                
strr = ""
for t in range(1, 1+ line_count):
    audio_file_name = "index_" + str(t) + ".wav"
    audio = AudioSegment.from_file(audio_file_name)
    current_duration = len(audio)  # 当前音频的时长（毫秒）
    strr += str(current_duration) + " "

with open("D:/example.txt", "w") as file:
    file.write(strr)
    
            
 
 */