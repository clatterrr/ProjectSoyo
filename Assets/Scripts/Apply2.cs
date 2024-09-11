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
        public Transform transform;
        public Material material;
        public bool modelMode;

        public ActivePart(string parentName, GameObject child, int activeStartFrame, int buildFrame, Vector3 size, int index)
        {
            this.name  = child.name;
            this.activeStartFrame = activeStartFrame;
            this.parentName = parentName;
            this.index = index;
            this.buildFrame = buildFrame;
            this.size = size;
            this.transform =child.transform;
            if(child.GetComponent<MeshRenderer>() != null)
            {
                material = child.GetComponent<MeshRenderer>().material;
            }
            else
            {
                material = null;
            }
            this.modelMode = true;
        }

        public ActivePart(GameObject pa, int activeStartFrame, int buildFrame)
        {
            this.name = pa.name;
            this.activeStartFrame = activeStartFrame;
            this.parentName = pa.name;
            this.index = 0;
            this.buildFrame = buildFrame;
            this.size = Vector3.zero;
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
            // �ҵ����� "0.0": [0, 0, 0], ����
            if (line.Contains(": ["))
            {
                Vector4 vector = ParseLineToVector4(line.Trim());
                vectorList.Add(vector);
            }
        }
    }

    string GetBaseColor(float r, float g, float b)
    {
        if (Mathf.Abs(r - g) <= 0.1 && r - b >= 0.1 && g - b >= 0.1) return "yellow";    // ��+��=��
        if (Mathf.Abs(r - b) <= 0.1 && r - g >= 0.1 && b - g >= 0.1) return "pink";   // ��+��=Ʒ��
        if (Mathf.Abs(g - b) <= 0.1 && g - r >= 0.1 && b - r >= 0.1) return "cyan";      // ��+��=��

        if (r > g && r > b) return "red";
        if (g > r && g > b) return "green";
        if (b > r && b > g) return "blue";

        return "gray"; // ������߽ӽ��������ɫ
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
                            placer.name = part.transform.name;
                            subtitles.Add(AddModelSub(comment_base.ToArray(), placer));
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
            // ����Python�ű�·��
            string pythonScriptPath = "F:/DaisyDay/test.py";

            // Ҫ���ݸ�Python���ַ���
            string stringArg1 = writeto;
            string stringArg2 = Random3.ToString();
            Debug.Log(stringArg2);
            // ����һ���µĽ���
            Process pythonProcess = new Process();

            // ����Python������·��
            pythonProcess.StartInfo.FileName = "python";

            // ���ݽű�·�����ַ�������
            pythonProcess.StartInfo.Arguments = $"{pythonScriptPath} \"{stringArg1}\" \"{stringArg2}\"";

            // ������������������Ϣ
            pythonProcess.StartInfo.UseShellExecute = false;
            pythonProcess.StartInfo.RedirectStandardOutput = true;
            pythonProcess.StartInfo.RedirectStandardError = true;
            pythonProcess.StartInfo.CreateNoWindow = true;

            // ����Python����
            pythonProcess.Start();

            // �ȴ�Python�ű�ִ�����
            pythonProcess.WaitForExit();

            // ��ȡPython���������У�
            string output = pythonProcess.StandardOutput.ReadToEnd();
            string error = pythonProcess.StandardError.ReadToEnd();

            Debug.Log("build time = " + output);
            // ���ݿո��ֳ��ַ�������
            string[] line_part = output.Split(' ');

            // ���ַ�������ת������������
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
                numbers.Add(20);
            }
            return numbers.ToArray();
        }

    }

    private string audioClipPath = "Audio/model"; // �����ļ���·����������Assets/Resources/Audio/mySound.wav

    private AudioSource audioSource;

    private AudioClip modelClip;

    private int Random3;
    void Start()
    {
        Random3 = Random.Range(1000, 9999);
        Debug.Log("r 3 = " +  Random3);

        parts = new List<GameObject>();
        ActiveParts = new List<ActivePart>();
        int[] build_times = GenerateComments();
        int build_times_index = 0;
        int build_time = 0;

        start_time = Time.time;
        LoadAnimation("D:\\GameDe\\GLTFmodl\\magnet_shroom.animation.json");
        mushMaterial = AddMaterial("Assets/Characters/Plants/split_pea.png");

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
            // ������Ļ
            int count = CountDirectChildCubes(part.transform);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < 4)
                    {
                        build_time = build_times[build_times_index++];
                        activeCount += build_time;
                    }

                    Vector3 size = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                    ActiveParts.Add(new ActivePart(part.transform.name,
                    part.transform.GetChild(i).gameObject, activeCount - build_time, build_time, size, j));
                    GameObject emptyObject = new GameObject("MyEmptyObject");
                    Vector3 bound = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                    Vector3 ssize = part.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
                    emptyObject.transform.position = bound;
                    float mag = ssize.magnitude * 10;
                    float bx = bound.x * Random.Range(1.5f, 2.5f) + mag * Random.Range(-0.2f, 0.2f);
                    float by = bound.y * Random.Range(0.8f, 1.2f) + mag * Random.Range(-0.2f, 0.2f);
                    float bz = bound.z * Random.Range(1.5f, 2.5f) + mag * Random.Range(-0.2f, 0.2f);
                    Vector3 bound2 = new Vector3(bx, by, bz);
                    cameras.Add(addCameraMove(activeCount - build_time, activeCount, bound2, bound2, emptyObject, new Vector3(0, 0, 0)));
                }

                build_time = build_times[build_times_index++];
                activeCount += build_time;
                ActiveParts.Add(new ActivePart(part.transform.GetChild(0).gameObject, activeCount - build_time, build_time));
                cameras[cameras.Count - 1].AddBuildTime(build_time);
            }
        }

        generatedAnimal.SetActive(true);

        build_time = build_times[build_times_index++];
        cameras.Add(addCameraMove(activeCount, activeCount + build_time, new Vector3(-2, 1, -2), new Vector3(2, 1, -2), emptyObject2, new Vector3(0, 0, 0)));
    }


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
                FrameCount = -1;
                Debug.Log("Check Audio Failed " + CheckAudioCount);
                modelClip = Resources.Load<AudioClip>(audioClipPath);
                if (modelClip != null)
                {
                    // һ�����سɹ���������ƵƬ�β���ʼ����
                    audioSource.clip = modelClip;
                    audioSource.Play();
                    CheckAudio = true;
                }
            }
        }



        for (int i = 0; i < ActiveParts.Count; i++)
        {
            int frame0 = ActiveParts[i].activeStartFrame;
            int frame1 = ActiveParts[i].activeStartFrame + ActiveParts[i].buildFrame;
            if (FrameCount == frame0)
            {
                if (ActiveParts[i].modelMode)
                {
                    SetChildActive(generatedAnimal.transform, ActiveParts[i].name);
                    SetChildMaterial(generatedAnimal.transform, ActiveParts[i].name, mushMaterial);
                }
                else
                {
                    SetChildMaterial(generatedAnimal.transform, ActiveParts[i].name, ActiveParts[i].material);
                }
            }
            if (FrameCount >= frame0 && FrameCount < frame1)
            {
                if (ActiveParts[i].modelMode)
                {
                    float ratio = (FrameCount - frame0) * 1.2f / ActiveParts[i].buildFrame;
                    if (ratio > 1.0f) ratio = 1.0f;
                    Vector3 size = ActiveParts[i].size;
                    Vector3 scale = ActiveParts[i].transform.localScale;
                    float[] localScale = new float[] { size.x, size.y, size.z };
                    int maxIndex = 0;
                    if (size.y > size.x && size.y > size.z) maxIndex = 1;
                    if (size.z > size.y && size.z > size.x) maxIndex = 2;
                    localScale[maxIndex] = ratio * localScale[maxIndex];
                    SetChildLocalScale(generatedAnimal.transform, ActiveParts[i].name, new Vector3(localScale[0], localScale[1], localScale[2]));
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

        if(FrameCount > 1)
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



    string[] comment_all_start = { "all right I'm already here to start making our _part mob" };

    // �����ǵ�һ�����Ա���˵��_part
    // ÿ���еĵ�һ��������  
    string[] comment_base = { "i am going to create a base for his _part�� i`ll make a _part like this",
                                " then i`ll add a base to start making his _part",
                                "now i`m going to add _part",
                                "to make the _part I think it has to be another color",
                                "he has a somewhat _desc _part, so I would have to make it like this",
                                "these _part will be _desc ones like these",
                                "I adjusted _part to make it more pointed",
                                //"so I created with scarier _part much _desc_adj for _name looks like _desc_noun",
                                "i`ll put a _part to be able to make a very cool animation",
                                "i`ll made his _part and then added all the details",
                                "then i went to the part of the _part",
                                "and on _dir of it, the _part part",
                                "so I will create here and pull a cube down to make the _part",
                                "this part here is the _part",
                                "I'll start by making this _desc piece of _part"};

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

    string[] comment_color = { "I'm going to start doing the texture.",
                                "i start painting him the color he is. in this case it's _color",
                                "i colored it _color",
                                "his _part will be a slightly different _color color",
                                "I will also start texturing her _part", 
                                "I use a very good style to make _color texture",
                                };
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
    # ����Ƶ�ļ�
    with wave.open(file_path, 'rb') as audio_file:
        # ��ȡ��Ƶ�ļ���֡��
        num_frames = audio_file.getnframes()
        # ��ȡ��Ƶ�ļ���֡�ʣ�֡��/�룩
        frame_rate = audio_file.getframerate()
        # ������Ƶ�ļ��ĳ��ȣ��룩
        audio_length = num_frames / frame_rate
        return round(audio_length, 2)
    

lines = sys.argv[1].splitlines()  # ��һ���ַ�������
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
    current_duration = len(audio)  # ��ǰ��Ƶ��ʱ�������룩
    time_str += str(current_duration) + " "
    silent_audio += audio

silent_audio.export("E:/software/unityproject/gudu/ddd/Assets/Resources/Audio/model.wav", format="wav")
print(time_str)
            
 
 */