using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using static Structure;
using static UnityEditor.SceneView;

public class ShowPlants : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject hand;

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
        public List<uint> pos_time;
        public List<uint> rot_time;
        public List<Vector3> pos;
        public List<Quaternion> rot;

        public GeoAnimPart(string name)
        {
            this.name=name;
            this.pos_time = new List<uint>();
            this.rot_time = new List<uint>();
            this.pos = new List<Vector3>();
            this.rot = new List<Quaternion>();
        }
    }

    void RunAnimation(int frameCount)
    {
        string animation_name = "animation.normal_zombie.idle";
        int animation_index = 0;
        for(int i = 0; i < animList.Count; i++)
        {
            if (animList[i].name == animation_name)
            {
                animation_index = i;
            }
        }
        
        for(int i = 0;i < animList[animation_index].animaPart.Count; i++) { 
            GeoAnimPart part = animList[animation_index].animaPart[i];
            if(part.rot_time.Count > 0)
            {
                int max_rot_time = (int)part.rot_time[part.rot_time.Count - 1];
                int now_time = frameCount % max_rot_time;
                for (int j = 0; j < part.rot_time.Count - 1; j++)
                {
                    if(now_time >= part.rot_time[j] && now_time < part.rot_time[j + 1])
                    {
                        float r = (now_time - part.rot_time[j]) * 1.0f / (part.rot_time[j + 1] - part.rot_time[j]);
                        Quaternion rot = Quaternion.Lerp(part.rot[j], part.rot[j + 1], r);
                        RotateChild(generatedPlant.transform, part.name, rot * GetBasicRot(part.name));
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
                        TranslateChild(generatedPlant.transform, part.name, GetBasicPos(part.name) +  pos * 0.08f);
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
            if(line.Contains("}"))
            {
                brackets_count--;
                brackets_add = false;
                this_line_add = true;
            }
            if(plog) Debug.Log("brackets num" + brackets_count + " line = " + line);
            if(brackets_count == 3 && brackets_add && this_line_add) {
                if(anim.length > 0)
                {
                    if (plog) Debug.Log("anim list added = " + anim.name + " count " + anim.animaPart.Count);
                    animList.Add(anim);
                }
                string anim_name = line.Split(':')[0].Replace('"',' ');
                anim_name = anim_name.Trim();
                if (plog) Debug.Log(" anim name = " + anim_name);
                anim = new GeoAnim(anim_name, true, 1);
            }
            if(brackets_count == 5 && brackets_add && this_line_add)
            {
                string anim_part = line.Split(':')[0].Replace('"',' ');
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
                float t = float.Parse(line.Split(':')[0].Replace('"',' ').Replace(" ", ""));
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

                    // 将字符串分割为数字字符串，并转换为浮点数数组
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
                        animPart.rot.Add(Quaternion.Euler(-vector[0], -vector[1], -vector[2]));
                    }
                }
            }

            if(brackets_count == 4 && brackets_add == false && this_line_add)
            {
                if (plog) Debug.Log(" added anima part = name " + animPart.name + " count = " + animPart.pos_time.Count);
                anim.animaPart.Add(animPart);
            }
        }
        foreach(var animx in animList)
        {
            if (plog) Debug.Log("animx name" + animx.name);
            foreach(var animy in animx.animaPart)
            {

                if (plog) Debug.Log("animy name" + animy.name);
            }
        }
    }
    private List<CameraSetting> cameraSettings = new List<CameraSetting>();
    private List<ActorSettings> actorSettings = new List<ActorSettings>();
    private List<string> subtitles = new List<string>();
    private GameObject generatedPlant;
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
        foreach(var info  in baseInfo)
        {
            if(info.name == name)
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
        baseInfo.Add(new BasicInfo(parent.transform.name, parent.transform.localPosition, parent.transform.localRotation));
    }

    string RandomString(string[] s)
    {
        return s[UnityEngine.Random.Range(0, s.Length)];
    }

    public bool generateStory;
    private Vector3 handb;

    void CameraIdle(int startFrame, int endFrame, Vector3 offset, GameObject lookat, int loops)
    {
        float len = offset.magnitude * 0.03f;
        Vector3 v0 = new Vector3(0, 0, -len);
        Vector3 v1 = new Vector3(0, 0, len);
        Vector3 v2 = new Vector3(0, len / 2, 0);
        Vector3 v3 = new Vector3(0, -len, - len / 2);
        Vector3 v4 = new Vector3(0, -len, len / 2);
        int oneLoops = (endFrame - startFrame) / loops;
        int oneFive = oneLoops / 5;

        for(int i = 0; i < loops; i++)
        {
            int frame = startFrame + i * oneLoops;
            Debug.Log("start frame = " + startFrame + " end frame " + endFrame + " one five = " + oneFive + "len " + len);
            cameraSettings.Add(addCameraMove(frame, frame + oneFive,                    offset + v0, offset + v1, lookat));
            cameraSettings.Add(addCameraMove(frame + oneFive , frame + oneFive * 2,     offset + v1, offset + v2, lookat));
            cameraSettings.Add(addCameraMove(frame + oneFive * 2, frame + oneFive * 3,  offset + v2, offset + v3, lookat));
            cameraSettings.Add(addCameraMove(frame + oneFive * 3, frame + oneFive * 4,  offset + v3, offset + v4, lookat));
            cameraSettings.Add(addCameraMove(frame + oneFive * 4, frame + oneFive * 5,  offset + v4, offset + v0, lookat));
        }

    }

    void Start()
    {
        handb = hand.transform.localPosition;
        start_time = Time.time;
        string prefab_name = "ZombieYeti";
        string path = "Assets/Characters/Plants/Prefab/" + prefab_name + ".prefab";
        GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        generatedPlant = Instantiate(selectedPrefab, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
        generatedPlant.transform.rotation = Quaternion.Euler(0, 270, 0);
        LoadAnimation("D:\\GameDe\\GLTFmodl\\zombie.animation.json");

        TraverseChildren(generatedPlant.transform);
        //cameraSettings.Add(addCameraMove(0, 100, new Vector3(-10, 1, 10), new Vector3(10, 1, 10), generatedPlant));

        string[] envs = { "grass lands", "desert" };
        string env_string = RandomString(envs);

        SubReplacer replacer = new SubReplacer(prefab_name, env_string, "husk", "air bubbles", "nine");

        subtitles.Add(AddSub2(desc_env, replacer));
        subtitles.Add(AddSub2(desc_nice, replacer));
        subtitles.Add(AddSub2(desc_cute, replacer));
        subtitles.Add(AddSub2(desc_character, replacer));
        subtitles.Add(AddSub2(desc_attack, replacer));
        subtitles.Add(AddSub2(desc_attack_time, replacer));
        subtitles.Add(AddSub2(desc_grade, replacer));

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
            CameraIdle(start_frame, end_frame, new Vector3(3, 3, 0), generatedPlant, 2);


             start_frame = numbers[0];
            end_frame = numbers[0] + numbers[1];
            actorSettings.Add(addActorMove(start_frame, end_frame, player, MinecraftAnimation.Animation.Wait, new Vector3(-3, 0.5f, -3), new Vector3(3, 0.5f, -3), Quaternion.Euler(0, 270, 0), Quaternion.Euler(0, 270, 0)));
            cameraSettings.Add(addCameraMove(start_frame, end_frame, new Vector3(5, 3, 0), new Vector3(5, 3, 0) * 0.3f, generatedPlant));

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
        RunAnimation(GlobalFrameCount);

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

        int handCountCycle = 20;
        float handCycleScale = 0.1f;
        float handCycleScaleY = 0.02f;
        int handCount = GlobalFrameCount % (handCountCycle * 2);
        if( handCount < handCountCycle ) {
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

    }
    // 第一步：在哪儿能找到他

    string[] desc_env = { "anywhere in a _env we could find _name" ,
            "we've spotted some _env and that means we can find a _name",
            "if we sneak up to the _env we may be able to Feast Our Eyes Upon a wild baby _name",
            "we have _name all over the _env and it matched a lot because his  color matched a lot with the surroundings"};

    // 评价外形，好看，恐怖

    string[] desc_nice = { " the models themselves turned out very good so I think they they deserve a grade _score ",
                            "in this case she's quite small, because like she's small in the game",
                            "look at this she's a little bigger than just a block but turned out very good",
                              "for now the character will be nice anyway",
                            "look at this they turned out really nice. I think it matched the style" };

    string[] desc_cute = {"looking very cute in its Youth and Hungry",
                          "the _name looks a little bit scary but don't be thrown off the _name is here to be your friend",
                          "and these guys are so cute. look at the face on this little guy. oh my goodness buddy I just want to come give you a pet",
                          "well the normal _name turned out like this in Minecraft",
                          "the _name just came out adorable all at all he's so cute",
                          "we've got _name looking super cute extra furry"};

    // 非攻击性格描述


    string[] desc_character = { "but there's a difference now yes he is aggressive", "in his baby form he'll run and hide",
                                "and this little guy looks happy all of the time"};

    // 习性描述
    string[] desc_habit = { "paparo goes and grabs all the bones nearby as" };

    // 攻击性格描述

    string[] desc_attack = { "there's a small important detail I forgot to mention, he releases a _attack_weapon " +
            "               which is the Sleep _attack_weapon so that makes me want to sleep", "this is the power of _name he can make his enemies fall asleep",
                            "she's peaceful so she's not going to hit me unless I annoy her",
                             "he is attack me and i was running out for my life, let`s defeat her here",
                            "_name is going to do everything he can to defend Us",
                            "_name will defend the birch trees with all its might. " +
                            "growing little birch trees underneath the threat and launching it into the air",
                            "the _name spots anything threatening nearby by ready to take it down instantly right"};


    string[] sur = { " in order to make sure we can witness all of his defense mechanisms, I just have to work on staying alive myself" };
    // 攻击实战
    // https://youtu.be/Se0oM5liI-4?t=749
    // https://youtu.be/Se0oM5liI-4?t=422
    string[] desc_attack_time = { "head towards a _enemy and send the adult _name on it." +
            "the adult _name is going to send out _attack_weapon completely surrounding the",
            "the _name who will send out a _attack_weapon, full of allergic reaction." +
            "the _name defends us shoots out the _attack_weapon." +
            "one _enemy down after the second _enemy and this _enemy doesn't stand a chance against those allergies",
            "the _enemy came out ! he's trying to tackle the _enemy ! he got the _enemy knocked down",
            //https://youtu.be/Se0oM5liI-4?t=646
            "around the edge we can see a few _enemy coming in and now _name is preparing defenses as soon as a _enemy threatens us",
            "if any _enemy tries to come nearby, our _name is going to defend us hopefully "};
    // 动画系统

    string[] desc_anim = { "oh and if anyone wants to know the animation of _name turned out like this " +
            "look walking very smoothly also bending the legs standing and walking he bends the legs very smoothly",
        "I tried to make a very smooth animation with his leg look they move as if they were a spider",
                            "their animation also turned out very good", "also this walking animation turned out very good"};

    // 掉落描述


    string[] drop_desc = { "if we defeat the Sheep yes it will drop green wool as I had imagined and all right" };

    // 最后一步：打分


    string[] desc_grade = { "and we'll go to the next character. but before I want you guys to comment a grade for _name, " +
                         "Below in my opinion definitely it's _score. out of 10 because he's very beautiful well",
                           "but as I made more just for the sake of making I'll give a grade eight out of _score"};
    

   
    // 互动
    string[] desc_play = { "all we have to do is get into the _env and make friends with our baby _name" };

    // 吃
    string[] desc_eat = { "but if we take some kelp and tame our little baby corvis he becomes stronger and stronger until he's ready to take down any illers in sight",
                            "all we have to do is of course feed it a little bit of _food to tame it",
                        "all you have to do is keep _food on hand to keep him happy. so he doesn't send any _attack_weapon after you"};

    // 别跑
    string[] desc_look = { "where did you go buddy don't worry I'm not here to harm you I'm here to be your friend" };

    // 走
    string[] desc_walk = { "looks like he's venturing to the top of the temple exploring his home biome" };
}
