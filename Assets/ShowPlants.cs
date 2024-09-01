using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using static Structure;
using static UnityEditor.SceneView;

public class ShowPlants : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    private List<GeoAnim> animList = new List<GeoAnim>();
    Vector4 ParseLineToVector4(string line)
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
        public List<float> pos_time;
        public List<float> rot_time;
        public List<Vector3> pos;
        public List<Quaternion> rot;

        public GeoAnimPart(string name)
        {
            this.name=name;
            this.pos_time = new List<float>();
            this.rot_time = new List<float>();
            this.pos = new List<Vector3>();
            this.rot = new List<Quaternion>();
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
        foreach (string line in lines)
        {
            this_line_add = false;
            if (line.Contains("{"))
            {
                brackets_count++;
                brackets_add = true;
                this_line_add = true;
            }
            if(line.Contains("}"))
            {
                brackets_count--;
                brackets_add = false;
                this_line_add = true;
            }

            if(brackets_count == 3 && brackets_add && this_line_add) {
                if(anim.name != "")
                {
                    animList.Add(anim);
                }
                anim = new GeoAnim(line.Split(':')[0].Trim('"'), true, 0);
            }
            if(brackets_count == 5 && brackets_add && this_line_add)
            {
                animPart = new GeoAnimPart(line.Split(':')[0].Trim('"'));
            }
            if (brackets_count == 6 && brackets_add && line.Contains("position"))
            {
                mode_pos = true;
            }
            else if (brackets_count == 6 && brackets_add && line.Contains("rotation"))
            {
                mode_pos = false;
            }
            if (brackets_count == 7 && brackets_add && line.Contains("vector") && this_line_add)
            {
                float t = float.Parse(line.Split(':')[0].Trim('"'));
                if (mode_pos)
                {
                    animPart.pos_time.Add(t);
                }
                else
                {
                    animPart.rot_time.Add(t);
                }
                
            }

            if (brackets_count == 7 && brackets_add && line.Contains("vector"))
            {
                var match = Regex.Match(line, @"\[(.*?)\]");
                if (match.Success)
                {
                    string numbersStr = match.Groups[1].Value;

                    // 将字符串分割为数字字符串，并转换为浮点数数组
                    float[] vector = numbersStr
                        .Split(',')
                        .Select(s => float.Parse(s.Trim()))
                        .ToArray();

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

            if(brackets_count == 4 && brackets_add == false && this_line_add)
            {
                anim.animaPart.Add(animPart);
            }
        }
    }
    private List<CameraSetting> cameraSettings = new List<CameraSetting>();
    private List<ActorSettings> actorSettings = new List<ActorSettings>();
    private List<string> subtitles = new List<string>();
    private GameObject generatedAnimal;
    private float start_time;

    public bool generateStory;
    void Start()
    {
        start_time = Time.time;
        string prefab_name = "ZombieYeti";
        string path = "Assets/Characters/Plants/Prefab/" + prefab_name + ".prefab";
        GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        generatedAnimal = Instantiate(selectedPrefab, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
        generatedAnimal.transform.rotation = Quaternion.Euler(0, 270, 0);
        LoadAnimation("D:\\GameDe\\GLTFmodl\\zombie.animation.json");

        //cameraSettings.Add(addCameraMove(0, 100, new Vector3(-10, 1, 10), new Vector3(10, 1, 10), generatedAnimal));

        subtitles.Add(AddSub(shape_desc, prefab_name, "", "", ""));
        subtitles.Add(AddSub(desc_character, prefab_name, "", "", ""));
        subtitles.Add(AddSub(desc_attack, prefab_name, "", "", ""));

        if (!generateStory)
        {

            int[] numbers = ReadNumbersFromFile("D:/example.txt");
            for(int i = 0; i < numbers.Length;i++)
            {
                numbers[i] /= 20;
            }
            int start_frame = 0;
            int end_frame = numbers[0];
            actorSettings.Add(addActorMove(start_frame, end_frame, player, MinecraftAnimation.Animation.Wait, new Vector3(-3, 0.5f, -3), new Vector3(3, 0.5f, -3), Quaternion.Euler(0, 270, 0), Quaternion.Euler(0, 270, 0)));
            cameraSettings.Add(addCameraMove(start_frame, end_frame, new Vector3(5, 3, 0), new Vector3(5, 3, 0), player));


            start_frame = numbers[0];
            end_frame = numbers[0] + numbers[1];
            actorSettings.Add(addActorMove(start_frame, end_frame, player, MinecraftAnimation.Animation.Wait, new Vector3(-3, 0.5f, -3), new Vector3(3, 0.5f, -3), Quaternion.Euler(0, 270, 0), Quaternion.Euler(0, 270, 0)));
            cameraSettings.Add(addCameraMove(start_frame, end_frame, new Vector3(5, 3, 0), new Vector3(5, 3, 0), player));

            start_frame = numbers[0] + numbers[1];
            end_frame = numbers[0] + numbers[1] + numbers[2];
            actorSettings.Add(addActorMove(start_frame, end_frame, player, MinecraftAnimation.Animation.Wait, new Vector3(-3, 0.5f, -3), new Vector3(3, 0.5f, -3), Quaternion.Euler(0, 270, 0), Quaternion.Euler(0, 270, 0)));
            cameraSettings.Add(addCameraMove(start_frame, end_frame, new Vector3(5, 3, 0), new Vector3(5, 3, 0), player));

        }
        else
        {
            string writeto = "";
            foreach (string sub in subtitles)
            {
                writeto += sub + "\n";
            }
            File.WriteAllText("D://sub.txt", writeto);
        }
    }


    private int GlobalFrameCount = 0;   
    // Update is called once per frame
    void FixedUpdate()
    {
        GlobalFrameCount++;


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
                break;
            }
        }

    }

    string[] common = { "for now the character will be nice anyway",
                        "well the normal _name turned out like this in Minecraft"};

    string[] color_change = { "the only thing that changed between between one and the other is the color" };

    string[] grade = { "and we'll go to the next character. but before I want you guys to comment a grade for _name, " +
                         "Below in my opinion definitely it's 10. out of 10 because he's very beautiful well",
                           "but as I made more just for the sake of making I'll give a grade eight out of 10"};
    string[] spawn_desc = { "we have _name all over the lands and it matched a lot because his  color matched a lot with the surroundings" };
    
    string[] shape_desc = { " the models themselves turned out very good so I think they they deserve a grade nine ",
                            "in this case she's quite small, because like she's small in the game",
                            "look at this she's a little bigger than just a block but turned out very good",
                            "look at this they turned out really nice. I think it matched the style"};
    string[] anim_desc = { "oh and if anyone wants to know the animation of _name turned out like this " +
            "look walking very smoothly also bending the legs standing and walking he bends the legs very smoothly",
        "I tried to make a very smooth animation with his leg look they move as if they were a spider",
                            "their animation also turned out very good", "also this walking animation turned out very good"};

    string[] desc_character = { "but there's a difference now yes he is aggressive" };
    string[] desc_attack = { "there's a small important detail I forgot to mention, he releases a smoke. " +
            "               which is the Sleep smoke so that makes me want to sleep", "this is the power of catnap he can make his enemies fall asleep",
                            "she's peaceful so she's not going to hit me unless I annoy her",
                             "he is attack me and i was running out for my life, let`s defeat her here"};
    string[] drop_desc = { "if we defeat the Sheep yes it will drop green wool as I had imagined and all right" };

}
