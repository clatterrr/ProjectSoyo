using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEditor.UI;

public class MonsterSchoolManager : MonoBehaviour
{
    public bool ReadMode = true;
    public GameObject[] actors;
    public Vector3[] c0Offset;

    List<HumanFace> faces = new List<HumanFace>();

    const float IMAGE_HEIGHT = 1080;
    const float IMAGE_WIDTH = 1920;

    List<SceneInfo> sceneInfos = new List<SceneInfo>();
    List<int> startFrames = new List<int>();
    List<int> endFrames = new List<int>();
    struct ActorInfo
    {
        public int actorIndex;
        public Vector3 pos;
        public Vector3 relativeLookat;

        public ActorInfo(int actor, Vector3 pos, Vector3 relativeLookat)
        {
            this.actorIndex = actor;
            this.pos = pos;
            this.relativeLookat = relativeLookat;
        }
    }
    struct SceneInfo
    {
        public List<ActorInfo> actorStart;
        public List<ActorInfo> actorEnd;
        public int frameStart;
        public int frameEnd;
        
    }

    struct HumanFace
    {
        public Vector2 center;
        public Vector2 up;
        public Vector2 down;
        public Vector2 left;
        public Vector2 right;
        public bool towardsCamera;
        public int scene_index;
        public int inner_index;

        public Vector3 pos;
        public Vector3 relativeLookAt;

        // 构造函数
        public HumanFace(Vector2 center, Vector2 up, Vector2 down, Vector2 left, Vector2 right,  bool towardsCamera, int i0, int i1, Vector3 pos, Vector3 lookat)
        {
            this.center = center;
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
            this.towardsCamera = towardsCamera;
            this.scene_index = i0;
            this.inner_index = i1;
            this.pos = pos;
            this.relativeLookAt = lookat;
        }
    }

    void Awake()
    {

        // 调用读取文件方法
        //ReadFile(filePath);

        if (ReadMode)
        {
            int start = 0;
            int end = 0;
            string fileContent = File.ReadAllText("F://DaisyDay//unity_scene.txt");
            string[] lines = fileContent.Trim().Split('\n');
            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.actorStart = new List<ActorInfo>();
            sceneInfo.actorEnd = new List<ActorInfo>();
            int check = 0;
            foreach (string line in lines)
            {
                string[] words = line.Trim().Split(' ');
                if(words.Length == 2)
                {
                    if(check > 0)
                    {
                        sceneInfo.frameStart = start; sceneInfo.frameEnd = end;
                        sceneInfos.Add(sceneInfo);
                    }
                    start = int.Parse(words[0]);
                    end = int.Parse(words[1]);
                    check = 0;
                    sceneInfo = new SceneInfo();
                    sceneInfo.actorStart = new List<ActorInfo>();
                    sceneInfo.actorEnd = new List<ActorInfo>();
                }
                else
                {
                    int actorIndex = int.Parse(words[0]);
                    Vector3 pos = new Vector3(float.Parse(words[2]), float.Parse(words[3]), float.Parse(words[4]));
                    Vector3 lookat = new Vector3(float.Parse(words[5]), float.Parse(words[6]), float.Parse(words[7]));
                    check += 1;
                    if(check % 2 == 1)
                    {
                        sceneInfo.actorStart.Add(new ActorInfo(actorIndex, pos, lookat));
                    }else
                    {
                        sceneInfo.actorEnd.Add(new ActorInfo(actorIndex, pos, lookat));
                    }


                }
            }
        }
        else
        {
            ReadSceneInfo("F://DaisyDay//scene_info.txt");
            ReadColor("F://DaisyDay//ClickPos");

            string file_str = "";
            foreach (SceneInfo s in sceneInfos)
            {
                file_str += s.frameStart.ToString() + " " + s.frameEnd.ToString() + "\n";
                for (int i = 0; i < s.actorStart.Count; i++)
                {
                    file_str += s.actorStart[i].actorIndex + " anim " + s.actorStart[i].pos.x + " " + s.actorStart[i].pos.y + " " + s.actorStart[i].pos.z + " " + s.actorStart[i].relativeLookat.x + " " + s.actorStart[0].relativeLookat.y + " " + s.actorStart[i].relativeLookat.z + "\n";
                    file_str += s.actorEnd[i].actorIndex + " anim " + s.actorEnd[i].pos.x + " " + s.actorEnd[i].pos.y + " " + s.actorEnd[i].pos.z + " " + s.actorEnd[i].relativeLookat.x + " " + s.actorEnd[i].relativeLookat.y + " " + s.actorEnd[i].relativeLookat.z + "\n";
                }
            }

            File.WriteAllText("F://DaisyDay//unity_scene.txt", file_str);
        }



    }


    // 计算 Vector2 数组的中心点
    Vector2 GetCenterPoint(Vector2[] points)
    {
        Vector2 sum = Vector2.zero;
        foreach (Vector2 point in points)
        {
            sum += point;
        }
        return sum / points.Length;
    }

    // 找到离中心点最近的 Vector2
    int GetNearestPointIndex(Vector2 centerPoint, Vector2[] points)
    {
        float minDistance = Mathf.Infinity;
        int nearestIndex = -1; // 初始值设置为 -1，以便在没有找到最近点时返回
        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector2.Distance(centerPoint, points[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }
    void ReadSceneInfo(string path)
    {
        string[] lines = File.ReadAllLines(path);
        // 遍历每一行
        foreach (string line in lines)
        {
            // 使用空格分隔行中的元素
            string[] parts = line.Split(' ');

            // 检查行中是否有足够的元素
            if (parts.Length >= 4)
            {
                startFrames.Add(int.Parse(parts[2]));
                endFrames.Add(int.Parse(parts[3]));

            }

        }
    }

    void ReadColor(string path)
    {

        // 检查文件夹是否存在
        if (Directory.Exists(path))
        {

            // 获取文件夹下的所有txt文件
            string[] files = Directory.GetFiles(path, "*.txt");

            Regex regex = new Regex(@"_(\d+)_(\d+)\.txt$");

            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.actorStart = new List<ActorInfo>();
            sceneInfo.actorEnd = new List<ActorInfo>();

            foreach (string file in files) {



                string fileName = Path.GetFileName(file);
                Match match = regex.Match(fileName);
                int firstNumber = 0;
                int secondNumber = 0;
                if (match.Success)
                {
                    // 获取匹配到的最后两个数字
                     firstNumber = int.Parse(match.Groups[1].Value);
                     secondNumber = int.Parse(match.Groups[2].Value);
                }

                string fileContent = File.ReadAllText(file);

                // 定义变量来保存坐标和颜色
                Vector2[] coordinates = new Vector2[5];
                // 0 center 1 up 2 down 3 left 4 right
                int[] cidx = new int[5];

                bool towardsCamera = true;

                int lineCount = 0;
                // 解析文件内容
                string[] lines = fileContent.Trim().Split('\n');


                for (int i = 0; i < lines.Length; i++)
                {
                    
                    string[] parts = lines[i].Trim().Split(',');
                    if (parts.Length == 3)
                    {
                        int x = Int32.Parse(parts[0]);
                        int y = Int32.Parse(parts[1]);
                        string color = parts[2];
                        coordinates[lineCount] = new Vector2(x, y);
                        if(color.Trim() == "green")
                        {
                            towardsCamera = false;
                        }
                    }
                    lineCount += 1;
                    if (lineCount == 5)
                    {
            

                        // 计算这5个 Vector2 的中心点
                        Vector2 centerPoint = GetCenterPoint(coordinates);

                        // 找到离中心点最近的那个点
                        int nearestIndex = GetNearestPointIndex(centerPoint, coordinates);

                        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

                        for (int k = 0; k < coordinates.Length; k++)
                        {
                            if (k == nearestIndex)
                            {
                                cidx[0] = k;
                                continue;
                            }
                            Vector2 dis = (coordinates[k] - centerPoint).normalized;

                            float minDot = -Mathf.Infinity;
                            int minIndex = -1;
                            for (int j = 0; j < directions.Length; j++)
                            {
                                float dot = dis.x * directions[j].x + dis.y * directions[j].y;
                                if (dot > minDot)
                                {
                                    minDot = dot;
                                    minIndex = j;
                                }
                            }

                            cidx[minIndex + 1] = k;
                        }


                        Vector2 center = coordinates[cidx[0]];
                        Vector2 up = coordinates[cidx[1]];
                        Vector2 down = coordinates[cidx[2]];
                        Vector2 left = coordinates[cidx[3]];
                        Vector2 right = coordinates[cidx[4]];


                        float z_ratio = (up.y - down.y) * 6 / IMAGE_HEIGHT;

                        float x_ration = (center.x - IMAGE_WIDTH / 2) / (IMAGE_WIDTH / 2);

                        Vector3 actorPos = new Vector3(12 * x_ration, 0, 12 / z_ratio);
                        Vector3 lookat = new Vector3(0, 0, 0);


                        float left_abs = Mathf.Abs(left.x - center.x);
                        float right_abs = Mathf.Abs(right.x - center.x);
                        if (left_abs < 1e-10 || right_abs / left_abs > 100)
                        {
                            lookat = new Vector3(-1, 0, 0);
                        }
                        else if (right_abs < 1e-10 || left_abs / right_abs > 100)
                        {
                            lookat =  new Vector3(1, 0, 0);
                        }
                        else
                        {

                            float value = left_abs / right_abs;

                            if (value > 1.2)
                            {
                                float real_value = (value - 1.2f) / 10;
                                if (real_value > 1)
                                {
                                    real_value = 1;
                                }
                                if (towardsCamera)
                                {

                                    lookat =  new Vector3(0, 0, -1) + real_value * new Vector3(1, 0, 1);
                                }
                                else
                                {

                                    lookat = new Vector3(0, 0, 1) + real_value * new Vector3(-1, 0, -1);
                                }
                            }
                            else if (value < 0.8f)
                            {
                                float real_value = value / 0.8f;
                                if (real_value > 1)
                                {
                                    real_value = 1;
                                }
                                if (towardsCamera)
                                {

                                    lookat = new Vector3(0, 0, -1) + real_value * new Vector3(-1, 0, 1);
                                }
                                else
                                {

                                    lookat = new Vector3(0, 0, 1) + real_value * new Vector3(1, 0, -1);
                                }
                            }
                            else
                            {
                                if (towardsCamera)
                                {

                                    lookat = new Vector3(0, 0, -1);
                                }
                                else
                                {

                                    lookat = new Vector3(0, 0, 1);
                                }
                            }
                        }

                        HumanFace face = new HumanFace(center, up, down, left, right, towardsCamera, firstNumber, secondNumber, actorPos, lookat);
                        faces.Add(face);

                        if (secondNumber == 1)
                        {
                            sceneInfo.actorStart.Add(new ActorInfo(sceneInfo.actorStart.Count, actorPos, lookat));
                        }
                        else
                        {
                            sceneInfo.actorEnd.Add(new ActorInfo(sceneInfo.actorEnd.Count, actorPos, lookat));
                        }


                       towardsCamera = true;
                        lineCount = 0;
                    }
                }

                if(secondNumber ==2)
                {

                    sceneInfo.frameStart = startFrames[sceneInfos.Count];
                    sceneInfo.frameEnd = endFrames[sceneInfos.Count];

                    sceneInfos.Add(sceneInfo);
                    sceneInfo = new SceneInfo();
                    sceneInfo.actorStart = new List<ActorInfo>();
                    sceneInfo.actorEnd = new List<ActorInfo>();
                }

            }
        }
    }

    public int frameCount = 379;

    private void FixedUpdate()
    {
        frameCount += 1;
        foreach (SceneInfo s in sceneInfos)
        {
            if (s.frameStart <= frameCount && s.frameEnd >= frameCount)
            {
                if (s.frameStart + 1 == frameCount) { 

                    foreach (GameObject a in actors)
                    {
                        a.transform.position = new Vector3(100, 0, 0);
                    }

                    Debug.Log("frame Count = " + frameCount + " start = " + s.frameStart + " end = " + s.frameEnd + " COUNT " + s.actorEnd.Count);
                    Debug.Log("pos start = " + s.actorStart[0].pos + "look start = " + s.actorStart[0].relativeLookat + "pos end = " + s.actorEnd[0].pos + "look end = " + s.actorEnd[0].relativeLookat);
                }

                float ratio = (float)(frameCount - s.frameStart) / (s.frameEnd - s.frameStart);
                for (int i = 0; i < s.actorStart.Count; i++)
                {
                    Vector3 pos = s.actorStart[i].pos + (s.actorEnd[i].pos - s.actorStart[i].pos) * ratio + c0Offset[s.actorStart[i].actorIndex];
                    Vector3 lookat = s.actorStart[i].relativeLookat + (s.actorEnd[i].relativeLookat - s.actorStart[i].relativeLookat) * ratio;
                    actors[s.actorStart[i].actorIndex].transform.position = pos;
                    actors[s.actorStart[i].actorIndex].transform.LookAt(pos + lookat);
                }

                break;
            }
        }
    }
}
