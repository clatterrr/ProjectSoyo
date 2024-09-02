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
    public GameObject hand;

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

    public bool generateStory;
    private Vector3 handb;
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
            Debug.Log("x = " + x);
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
                             "he is attack me and i was running out for my life, let`s defeat her here",
                            "paparo is going to do everything he can to defend Us"};
    string[] sur = { " in order to make sure we can witness all of his defense mechanisms, I just have to work on staying alive myself" };
    string[] drop_desc = { "if we defeat the Sheep yes it will drop green wool as I had imagined and all right" };

    string[] desc_got = { "we've got _name looking super cute extra furry" };

    string[] desc_happy = { "and this little guy looks happy all of the time" };

}
