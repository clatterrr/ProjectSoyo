using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static HumanoidGenerator;
using static Structure;
using static TextureEditor;
using static UnityEngine.GraphicsBuffer;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class HumanoidGenerator : MonoBehaviour
{
    public enum ShapeName
    {
        Head,
        Body,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
        HatBottom,
        HatTop,
        LeftEye,
        RightEye,
        Mouth,
        Tail,
        LeftEar,
        RightEar,
        LeftShoulder,
        RightShoulder,
        LeftHand,
        RightHand,
        LeftToe,
        RightToe,
    }

    public enum FrameWork
    {
        Zero,
        TopCenter,
        BottomCenter,
        BottomeLeft,
        BottomeRight,
        BodyTailPos,
        Right75,
        Left75,
        HeadLeftEarPos,
        HeadRightEarPos,
        HeadLeftEyePos,
        HeadRightEyePos,
        HeadMouthPos,
        BackCenter,
        FrontCenter,

        LeftHand0,
        LeftHand1,
        LeftHand2,
        LeftHand3,
        LeftHand4,
        LeftHand5,


        RightHand0,
        RightHand1,
        RightHand2,
        RightHand3,
        RightHand4,
        RightHand5,

        Toe0,
        Toe1,
        Toe2,
        Toe3,
        Toe4,
    }

    public static string GetShapeName(ShapeName name)
    {
        string parentName = "";
        switch (name)
        {
            case ShapeName.Head:
                parentName = "Head";
                break;
            case ShapeName.Body:
                parentName = "Body";
                break;
            case ShapeName.LeftArm:
                parentName = "LeftArm";
                break;
            case ShapeName.RightArm:
                parentName = "RightArm";
                break;
            case ShapeName.LeftLeg:
                parentName = "LeftLeg";
                break;
            case ShapeName.RightLeg:
                parentName = "RightLeg";
                break;
            case ShapeName.HatBottom:
                parentName = "HatBottom";
                break;
            case ShapeName.HatTop:
                parentName = "HatTop";
                break;
            case ShapeName.LeftEar:
                parentName = "LeftEar";
                break;
            case ShapeName.RightEar:
                parentName = "RightEar";
                break;
            case ShapeName.Mouth:
                parentName = "Mouth";
                break;
            case ShapeName.LeftEye:
                parentName = "LeftEye";
                break;
            case ShapeName.RightEye:
                parentName = "RightEye";
                break;
            case ShapeName.LeftShoulder:
                parentName = "LeftShoulder";
                break;
            case ShapeName.RightShoulder:
                parentName = "RightShoulder";
                break;
            case ShapeName.LeftHand:
                parentName = "LeftHand";
                break;
            case ShapeName.RightHand:
                parentName = "RightHand";
                break;
            case ShapeName.LeftToe:
                parentName = "LeftToe";
                break;
            case ShapeName.RightToe:
                parentName = "RightToe";
                break;
            case ShapeName.Tail:
                parentName = "Tail";
                break;
            default: break;
        }
        return parentName;
    }
    public static string GenerateDesc(ShapeName name)
    {
        string[] shape_desc_str = new string[] { "default", "ear_top", "ear_side" };
        string real_name = "ear";
        List<string> possible_name = new List<string>();
        for (int i = 0; i < shape_desc_str.Length; i++)
        {
            if (shape_desc_str[i].Contains(real_name))
            {
                possible_name.Add(shape_desc_str[i]);
            }
        }
        int r = Random.Range(0, possible_name.Count);
        string final_str = possible_name[r];
        return final_str;
    }

    public static Vector3 FindFrameWork(ShapeDesc shape, FrameWork work)
    {
        Vector3 result = Vector3.zero; // Temporary variable to store the result

        switch (work)
        {
            case FrameWork.TopCenter:
                result = new Vector3(shape.center.x, shape.center.y + shape.size.y / 2, shape.center.z);
                break;
            case FrameWork.BottomCenter:
                result = new Vector3(shape.center.x, shape.center.y - shape.size.y / 2, shape.center.z);
                break;
            case FrameWork.BottomeLeft:
                result = new Vector3(shape.center.x - shape.size.x / 4, shape.center.y - shape.size.y / 2, shape.center.z);
                break;
            case FrameWork.BottomeRight:
                result = new Vector3(shape.center.x + shape.size.x / 4, shape.center.y - shape.size.y / 2, shape.center.z);
                break;

            case FrameWork.HeadLeftEarPos:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y, shape.center.z);
                break;
            case FrameWork.HeadRightEarPos:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y, shape.center.z);
                break;
            case FrameWork.HeadLeftEyePos:
                result = new Vector3(shape.center.x - shape.size.x / 4, shape.center.y + shape.size.y / 4, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.HeadRightEyePos:
                result = new Vector3(shape.center.x + shape.size.x / 4, shape.center.y + shape.size.y / 4, shape.center.z + shape.size.z / 2);
                break;

            case FrameWork.BodyTailPos:
                result = new Vector3(shape.center.x, shape.center.y - shape.size.y / 2, shape.center.z - shape.size.z / 2);
                break;
            case FrameWork.Left75:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y + shape.size.y / 4, shape.center.z);
                break;
            case FrameWork.Right75:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y + shape.size.y / 4, shape.center.z);
                break;
            case FrameWork.HeadMouthPos:
                result = new Vector3(shape.center.x, shape.center.y - shape.size.y / 4, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.BackCenter:
                result = new Vector3(shape.center.x, shape.center.y, shape.center.z - shape.size.z / 2);
                break;
            case FrameWork.FrontCenter:
                result = new Vector3(shape.center.x, shape.center.y, shape.center.z + shape.size.z / 2);
                break;

            case FrameWork.LeftHand0:
                result = new Vector3(shape.center.x , shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.LeftHand1:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z - shape.size.z / 2);
                break;
            case FrameWork.LeftHand2:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z - shape.size.z / 4);
                break;
            case FrameWork.LeftHand3:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z );
                break;
            case FrameWork.LeftHand4:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 4);
                break;
            case FrameWork.LeftHand5:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;

            case FrameWork.RightHand0:
                result = new Vector3(shape.center.x, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.RightHand1:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z - shape.size.z / 2);
                break;
            case FrameWork.RightHand2:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z - shape.size.z / 4);
                break;
            case FrameWork.RightHand3:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z);
                break;
            case FrameWork.RightHand4:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 4);
                break;
            case FrameWork.RightHand5:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;

            case FrameWork.Toe0:
                result = new Vector3(shape.center.x - shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.Toe1:
                result = new Vector3(shape.center.x - shape.size.x / 4, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.Toe2:
                result = new Vector3(shape.center.x , shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.Toe3:
                result = new Vector3(shape.center.x + shape.size.x / 4, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;
            case FrameWork.Toe4:
                result = new Vector3(shape.center.x + shape.size.x / 2, shape.center.y - shape.size.y / 2, shape.center.z + shape.size.z / 2);
                break;

            default:
                break;
        }
        result = new Vector3(result.x, result.y, -result.z);
        return result; // Return the result at the end of the method
    }

    public struct ShapeDesc
    {
        public List<GameObject> actors;
        public Vector3 localPos;
        public Vector3 center; // group total size
        public Vector3 size;
        public ShapeName name;
        public int segment;
        public string desc_str;
        public ShapeName appendName;
        public FrameWork appendWork;
        public FrameWork myWork;

        public ShapeDesc(ShapeName shapeName, Vector3 size, ShapeName appendName, FrameWork appendWork, FrameWork myWork)
        {
            this.name = shapeName;
            this.localPos = Vector3.zero;
            this.center = Vector3.zero;
            this.size = size * 0.1f;
            this.desc_str = GenerateDesc(shapeName);
            this.actors = new List<GameObject>();
            segment = 1;
            if(shapeName == ShapeName.Body)
            {
                segment = Random.Range(1, 4);
                int plus = Random.Range(0, 2);
                if (plus == 0) plus = -1;
                segment *= plus;
            }
            this.appendName = appendName;
            this.appendWork = appendWork;
            this.myWork = myWork;
        }

        public void SetActors(List<GameObject> actors)
        {
            foreach (GameObject actor in actors)
            {
                this.actors.Add(actor);
            }
        }

        public void SetLocalPos(Vector3 lp)
        {
            localPos = lp;
        }
    }

    public static Vector3 RandomSize(int x0, int x1, int y0, int y1, int z0, int z1)
    {
        return new Vector3(Random.Range(x0, x1) , Random.Range(y0, y1), Random.Range(z0, z1));
    }

    // 使用Linq打乱List
    // 使用 Fisher-Yates 洗牌算法打乱 List
    public static void ShuffleList<T>(List<T> list)
    {
        System.Random rand = new System.Random(); // 每次调用时创建新的 Random 实例

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1); // 随机选择索引
            // 交换元素
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public static List<ShapeName> ForceFixed(List<ShapeName> names, ShapeName fore, ShapeName post)
    {
        int index0 = names.IndexOf(fore);
        int index1 = names.IndexOf(post);
        if (index0 != -1 && index1 != -1 && index1 < index0)
        {
            names[index0] = fore;
            names[index1] = post;
        }
        return names;
    }
    public static GameObject CreateHumaoid(string modelName, GameObject sourceModel, Texture2D sourceTexture)
    {
        //https://news.4399.com/seer/zzph/
        string[] elements = new string[]
        {
            "fire", "water", "grass", "dragon", "fly", "elect", "stone",
            "machine", "ghost","god","ice","desert", "cow","crawl", "insect", "scary",
        };

        string[] shape_desc_str = new string[] { "default",
            "ear_none", "ear_top", "ear_side",
            "leg_none", "leg_short", "leg_tall",
            "arm_none", "arm_short", "arm_tall", "arm_strong", "arm_weak",
            "hat_none", "hat_short", "hat_tall",
            "ear_none", "ear_top", "ear_side",
            
        };

        List<ShapeDesc> parts = new List<ShapeDesc>();
        Vector3 partSize;
        Vector3 bodySize = Vector3.zero; ;
        List<ShapeName> headEnum = new List<ShapeName> {ShapeName.LeftEar, ShapeName.LeftEye, ShapeName.HatBottom, ShapeName.Mouth};
        List<ShapeName> bodyEnum = new List<ShapeName> { ShapeName.Tail, ShapeName.LeftArm, ShapeName.LeftLeg, ShapeName.LeftShoulder };
        ShuffleList(headEnum);
        ShuffleList(bodyEnum);

        int leftShoulderIndex = bodyEnum.IndexOf(ShapeName.LeftShoulder);
        int leftArmIndex = bodyEnum.IndexOf(ShapeName.LeftArm);

        if (leftShoulderIndex != -1 && leftArmIndex != -1 && leftShoulderIndex < leftArmIndex)
        {
            // 交换元素
            bodyEnum[leftShoulderIndex] = ShapeName.LeftArm;
            bodyEnum[leftArmIndex] = ShapeName.LeftShoulder;
        }
        int leftLegIndex = bodyEnum.IndexOf(ShapeName.LeftLeg);

        // 如果找到了 LeftLeg，并且它不是最后一个元素
        if (leftLegIndex != -1)
        {
            bodyEnum.Insert(leftLegIndex + 1, ShapeName.LeftToe);
        }
        else
        {
            bodyEnum.Add(ShapeName.LeftToe);
        }



        List<ShapeName> names = new List<ShapeName>() { ShapeName.Body, ShapeName.Head };
        int r = Random.Range(0, 2);
        if (r == 0)
        {
            for(int i = 0; i <  headEnum.Count; i++) names.Add(headEnum[i]);
            for(int i = 0; i < bodyEnum.Count; i++) names.Add(bodyEnum[i]);
        }
        else
        {
            for (int i = 0; i < bodyEnum.Count; i++) names.Add(bodyEnum[i]);
            for (int i = 0; i < headEnum.Count; i++) names.Add(headEnum[i]);
        }

        for(int i =0; i< names.Count; i++)
        {
            switch(names[i])
            {
                case ShapeName.Body:
                    {
                        partSize = RandomSize(6, 12, 6, 24, 6, 12);
                        bodySize = partSize;
                        parts.Add(new ShapeDesc(ShapeName.Body, partSize, ShapeName.Body, FrameWork.Zero, FrameWork.Zero));
                        break;
                    }
                case ShapeName.Head:
                    {
                        partSize = RandomSize(6, 8, 6, 8, 6, 8);
                        parts.Add(new ShapeDesc(ShapeName.Head, partSize, ShapeName.Body, FrameWork.TopCenter, FrameWork.BottomCenter));
                        break;
                    }
                case ShapeName.LeftEar:
                    {
                        partSize = RandomSize(2, 4, 4, 6, 2, 4);
                        parts.Add(new ShapeDesc(ShapeName.LeftEar, partSize, ShapeName.Head, FrameWork.HeadLeftEarPos, FrameWork.BottomCenter));
                        parts.Add(new ShapeDesc(ShapeName.RightEar, partSize, ShapeName.Head, FrameWork.HeadRightEarPos, FrameWork.BottomCenter));
                        break;
                    }
                case ShapeName.LeftArm:
                    {
                        partSize = RandomSize(4, 6, 12, 18, 4, 6);
                        parts.Add(new ShapeDesc(ShapeName.LeftArm, partSize, ShapeName.Body, FrameWork.Right75, FrameWork.Left75));
                        parts.Add(new ShapeDesc(ShapeName.RightArm, partSize, ShapeName.Body, FrameWork.Left75, FrameWork.Right75));
                        break;
                    }
                case ShapeName.LeftHand:
                    {
                        // todo ? 
                        partSize = RandomSize(1, 1, 2, 4, 1, 1);
                        int figureCount = Random.Range(2, 6);
                        ShapeDesc desc = new ShapeDesc(ShapeName.LeftHand, partSize, ShapeName.LeftArm, FrameWork.BottomCenter, FrameWork.TopCenter);
                        desc.segment = figureCount;
                        ShapeDesc desc1 = new ShapeDesc(ShapeName.RightHand, partSize, ShapeName.RightArm, FrameWork.BottomCenter, FrameWork.TopCenter);
                        parts.Add(desc);
                        desc.segment = figureCount;
                        parts.Add(desc1);
                        break;
                    }
                case ShapeName.LeftToe:
                    {
                        partSize = RandomSize(1, 1, 1, 1, 1, 3);
                        int toeCount = Random.Range(2, 5);
                        ShapeDesc desc = new ShapeDesc(ShapeName.LeftToe, partSize, ShapeName.LeftLeg, FrameWork.BottomCenter, FrameWork.BackCenter);
                        desc.segment = toeCount;
                        ShapeDesc desc1 = new ShapeDesc(ShapeName.RightToe, partSize, ShapeName.RightLeg, FrameWork.BottomCenter, FrameWork.BackCenter);
                        parts.Add(desc);
                        desc1.segment = toeCount;
                        parts.Add(desc1);
                        break;
                    }
                case ShapeName.LeftShoulder:
                    {
                        partSize = RandomSize(4, 6, 4, 6, 4, 6);
                        parts.Add(new ShapeDesc(ShapeName.LeftShoulder, partSize, ShapeName.LeftArm, FrameWork.TopCenter, FrameWork.BottomCenter));
                        parts.Add(new ShapeDesc(ShapeName.RightShoulder, partSize, ShapeName.RightArm, FrameWork.TopCenter, FrameWork.BottomCenter));
                        break;
                    }
                case ShapeName.LeftLeg:
                    {
                        partSize = RandomSize((int)(bodySize.x / 3), (int)(bodySize.x / 2), 12, 18, (int)(bodySize.z / 3), (int)(bodySize.z / 2));
                        parts.Add(new ShapeDesc(ShapeName.LeftLeg, partSize, ShapeName.Body, FrameWork.BottomeLeft, FrameWork.TopCenter));
                        parts.Add(new ShapeDesc(ShapeName.RightLeg, partSize, ShapeName.Body, FrameWork.BottomeRight, FrameWork.TopCenter));
                        break;
                    }
                case ShapeName.Mouth:
                    {
                        partSize = RandomSize(2, 4, 1, 2, 1, 1);
                        parts.Add(new ShapeDesc(ShapeName.Mouth, partSize, ShapeName.Head, FrameWork.HeadMouthPos, FrameWork.BackCenter));
                        break;
                    }
                case ShapeName.LeftEye:
                    {
                        partSize = RandomSize(2, 3, 2, 3, 1, 1);
                        parts.Add(new ShapeDesc(ShapeName.LeftEye, partSize, ShapeName.Head, FrameWork.HeadLeftEyePos, FrameWork.BackCenter));
                        parts.Add(new ShapeDesc(ShapeName.RightEye, partSize, ShapeName.Head, FrameWork.HeadRightEyePos, FrameWork.BackCenter));
                        break;
                    }
                case ShapeName.HatBottom:
                    {
                        partSize = RandomSize(4, 6, 2, 4, 4, 6);
                        parts.Add(new ShapeDesc(ShapeName.HatBottom, partSize, ShapeName.Head, FrameWork.TopCenter, FrameWork.BottomCenter));
                        partSize = RandomSize(4, 6, 4, 8, 4, 6);
                        parts.Add(new ShapeDesc(ShapeName.HatTop, partSize, ShapeName.HatBottom, FrameWork.TopCenter, FrameWork.BottomCenter));
                        break;
                    }
                case ShapeName.Tail:
                    {
                        partSize = RandomSize(1, 2, 2, 2, 4, 10);
                        parts.Add(new ShapeDesc(ShapeName.Tail, partSize, ShapeName.Body, FrameWork.BodyTailPos, FrameWork.FrontCenter));
                        break;
                    }

            }
        }



        

       


       

        








        int index = 0;

        for (int i = 0; i < parts.Count; i++)
        {
            bool isToe = (parts[i].name == ShapeName.LeftToe || parts[i].name == ShapeName.RightToe);
            Debug.Log(" parts  size = " + parts[i].size);
            int Segment = Mathf.Abs(parts[i].segment);
            List<Vector3> poss = new List<Vector3>();
            Vector3 nowPos = new Vector3(0, parts[i].size.y / 2, 0);
            List<Vector3> sizes = new List<Vector3>();
            for(int j = 0; j < Segment; j++)
            {
                int minus = j;
                if (parts[i].segment > 0)
                {
                    minus = Segment - 1 - j;
                }
                Vector3 newSize = new Vector3(parts[i].size.x - 0.1f * minus, parts[i].size.y / Segment, parts[i].size.z - 0.1f * minus);
                if(isToe) newSize = parts[i].size;
                sizes.Add(newSize);
                nowPos = nowPos - new Vector3(0, newSize.y / 2, 0);
                if (isToe) nowPos = Vector3.zero;
                poss.Add(nowPos);
                nowPos = nowPos - new Vector3(0, newSize.y / 2, 0);
            }
            List<GameObject> actors = new List<GameObject>();
            for (int j = 0; j < Segment; j++)
            {
                GameObject actor = CreateCube();
                actor.transform.localPosition = poss[j];
                actor.transform.localScale = sizes[j];
                Uint3 size = new Uint3((uint)(sizes[j].x * 10.0f), (uint)(sizes[j].y * 10.0f), (uint)(Mathf.Abs(sizes[j].z) * 10.0f));
                actor.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                actor.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, parts[i].name, size);


                /*
                Material tempM = ExpectMaterial(sourceTexture, sourceModel, parts[i].name, size);
                SaveTextureToPNG((Texture2D)tempM.mainTexture, "Assets/Temp/" + modelName + "_" + index.ToString() + ".png");
                AssetDatabase.CreateAsset(tempM, "Assets/Temp/" + modelName + "_" + index.ToString() + ".mat");
                AssetDatabase.CreateAsset(actor.GetComponent<MeshFilter>().sharedMesh, "Assets/Temp/" + modelName + "_" + index.ToString() + ".mesh");
                */
                actors.Add(actor);
                index += 1;
            }

            if (isToe)
            {
                for (int j = 0; j < parts.Count; j++)
                {
                    if (parts[j].name == parts[i].appendName)
                    {
                        Vector3 p0 = FindFrameWork(parts[j], FrameWork.Toe0) + parts[j].localPos;
                        Vector3 p1 = FindFrameWork(parts[j], FrameWork.Toe1) + parts[j].localPos;
                        Vector3 p2 = FindFrameWork(parts[j], FrameWork.Toe2) + parts[j].localPos;
                        Vector3 p3 = FindFrameWork(parts[j], FrameWork.Toe3) + parts[j].localPos;
                        Vector3 p4 = FindFrameWork(parts[j], FrameWork.Toe4) + parts[j].localPos;
                        List<Vector3> pts = new List<Vector3>();
                        if (Segment == 2) pts = new List<Vector3>() {p0, p4 };
                        else if (Segment == 3) pts = new List<Vector3>() { p0, p2, p4 };
                        else if (Segment == 4) pts = new List<Vector3>() { p0, p1, p3, p4 };
                        for(int k = 0; k < pts.Count; k++)
                        {
                            actors[k].transform.position += (pts[k] + new Vector3(0,0, -parts[i].size.z / 2));
                        }

                        break;
                    }
                }
            }
            else
            {
                Vector3 target = Vector3.zero;
                for (int j = 0; j < parts.Count; j++)
                {
                    if (parts[j].name == parts[i].appendName)
                    {
                        target = FindFrameWork(parts[j], parts[i].appendWork) + parts[j].localPos;
                        break;
                    }
                }
                Vector3 now = FindFrameWork(parts[i], parts[i].myWork);
                ShapeDesc desc = parts[i];
                desc.localPos = target - now;
                parts[i] = desc;
                for (int j = 0; j < actors.Count; j++)
                {
                    actors[j].transform.position += target - now;
                }
            }



            parts[i].SetActors(actors);

        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        // 创建一个新的空的 GameObject 作为父物体
        GameObject parentObject = new GameObject(modelName);

        // 遍历所有 part，将它们的父级设置为新创建的父物体
        int cube_index = 0;

        foreach (var part in parts)
        {
            GameObject parentObjectForPart = new GameObject(GetShapeName(part.name));
            for (int i = 0; i < part.actors.Count; i++)
            {
                part.actors[i].transform.name = "cube_" + cube_index.ToString();
                cube_index++;
                part.actors[i].transform.SetParent(parentObjectForPart.transform);
            }
            parentObjectForPart.transform.SetParent(parentObject.transform);
        }


        // 返回这个新的父物体
        return parentObject;

    }

    public static GameObject GenerateCircle(int radius, float heightOffset, float scale)
    {

        // 创建用于合并的 CombineInstance 数组
        List<CombineInstance> combineList = new List<CombineInstance>();

        List<int> heights = new List<int>
        {
            0
        };

        for (int i = -radius; i <= radius; i++)
        {
            bool finded = false;
            int start = 0;
            int end = 0;

            for (int j = -radius; j <= radius; j++)
            {

                if ((i + 0.5) * (i + 0.5) + (j + 0.5) * (j + 0.5) <= radius * radius)
                {
                    if (finded == false)
                    {
                        start = j;
                        finded = true;
                    }
                }
                else
                {
                    if (finded == true)
                    {
                        end = j;
                        finded = false;
                        break;
                    }
                }
            }
            // 创建立方体
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0, i) * scale;
            cube.transform.localScale = new Vector3(end * 2, 1, 1) * scale; // 调整立方体大小

            // 获取立方体的 MeshFilter
            MeshFilter meshFilter = cube.GetComponent<MeshFilter>();

            // 创建 CombineInstance 并加入 List
            CombineInstance combine = new CombineInstance();
            combine.mesh = meshFilter.mesh;
            combine.transform = cube.transform.localToWorldMatrix;
            combineList.Add(combine);

            // 删除临时立方体
            Destroy(cube);
        }

        // 合并 Mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineList.ToArray());

        // 创建一个新的 GameObject 来保存合并的 Mesh
        GameObject combinedObject = new GameObject("CombinedCircleMesh");
        combinedObject.AddComponent<MeshFilter>().mesh = combinedMesh;
        combinedObject.AddComponent<MeshRenderer>();

        // 给合并后的物体加上材质
        combinedObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        return combinedObject;
    }

    public static Material generateMaterial()
    {
        // 创建一个新的 2x2 大小的 Texture2D
        Texture2D greenTexture = new Texture2D(2, 2);

        // 填充整个纹理为纯绿色
        for (int x = 0; x < greenTexture.width; x++)
        {
            for (int y = 0; y < greenTexture.height; y++)
            {
                greenTexture.SetPixel(x, y, Color.green); // 设置为绿色
            }
        }

        greenTexture.SetPixel(0, 0, Color.black);
        greenTexture.SetPixel(1, 0, Color.white);
        greenTexture.SetPixel(0, 1, Color.black);
        greenTexture.SetPixel(1, 1, Color.black);

        // 应用更改，使得设置的像素生效
        greenTexture.Apply();
        greenTexture.filterMode = FilterMode.Point;
        //greenTexture = GeneratePoissonNoise();

        // 创建一个新的材质，使用 Unity 默认的 Shader
        Material greenMaterial = new Material(Shader.Find("Standard"));

        // 将刚创建的绿色纹理赋给材质的 mainTexture
        greenMaterial.mainTexture = greenTexture;

        return greenMaterial;
    }



    // 将Texture2D保存为PNG文件
    private static void SaveTextureToPNG(Texture2D texture, string path)
    {
        // 获取Texture的字节数据并保存为PNG
        byte[] textureBytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, textureBytes);

        // 将PNG文件导入为Texture资源
        AssetDatabase.ImportAsset(path);
    }

}