using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static HumanoidGenerator;
using static Structure;
using static TextureEditor;
using static UnityEditor.PlayerSettings;
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

        // plants
        P_Stem,
        P_LeftRootLeaf,
        P_RightRootLeaf,
        P_Head,
        P_LeftEye,
        P_RightEye,
        P_Mouth,
        P_BackLeaf,

        PH_Stem,
        PH_LeftRootLeaf,
        PH_RightRootLeaf,
        PH_Head,
        PH_LeftEye,
        PH_RightEye,
        PH_Mouth,
        PH_Lip,
        PH_BackLeaf,
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
    public static string GenerateDesc(ShapeName name)
    {
        string[] shape_desc_str = new string[] { "default", "ear_top", "ear_side" };
        string real_name = "ear";
        List<string> posSliceible_name = new List<string>();
        for (int i = 0; i < shape_desc_str.Length; i++)
        {
            if (shape_desc_str[i].Contains(real_name))
            {
                posSliceible_name.Add(shape_desc_str[i]);
            }
        }
        int r = Random.Range(0, posSliceible_name.Count);
        string final_str = posSliceible_name[r];
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

    public enum SpecialBuilds
    {
        None,
        TopDown,
        Sphere,
        BigMouthZ,
        FlatCircleShrink,

        Hollow,
        HollowCurve,
        SolidSphere,
        Curve, 
    }

    public enum ShapeDir
    {
        Full,
        XP,
        XM,
        YP,
        YM,
        ZP,
        ZM,
    }
    public struct ShapeDesc
    {
        public List<GameObject> actors;
        public Vector3 localPos;
        public Vector3 center; // group total size
        public Vector3 size;
        public ShapeName name;
        public int segment;
        public SpecialBuilds segmet;
        public int direction;
        public string desc_str;
        public ShapeName appendName;
        public FrameWork appendWork;
        public FrameWork myWork;
        public ShapeDir dir;
        public List<float> values;

        public ShapeDesc(ShapeName shapeName, Vector3 size, ShapeName appendName, FrameWork appendWork, FrameWork myWork)
        {
            this.name = shapeName;
            this.localPos = Vector3.zero;
            this.center = Vector3.zero;
            this.size = size * 0.1f;
            this.desc_str = GenerateDesc(shapeName);
            this.actors = new List<GameObject>();
            segment = 1;
            direction = 0;
            this.segmet = SpecialBuilds.None;
            if (shapeName == ShapeName.Body)
            {
                segment = Random.Range(1, 4);
                int plus = Random.Range(0, 2);
                if (plus == 0) plus = -1;
                segment *= plus;
                this.segmet = SpecialBuilds.TopDown;
            }
            this.appendName = appendName;
            this.appendWork = appendWork;
            this.myWork = myWork;
            dir = ShapeDir.Full;
            values = new List<float>();
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

    public static Vector3 RandomSize(int x0, int x1)
    {
        int r = Random.Range(x0, x1);
        return new Vector3(r,r,r);
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

    struct Pros
    {
        public string name;
        public List<string> posSlice;
        public List<float> values;
        public Pros(string name, string p0, float v0){
            this.name = name;
            this.posSlice = new List<string>() { p0 };
            this.values = new List<float>() { v0 };
        }

        public string RandomString()
        {
            float tvalue = 0;
            for(int i = 0; i < values.Count; i++)
            {
                tvalue += values[i];
                values[i] = tvalue;
            }
            tvalue = Random.Range(0, tvalue);
            int select = 0;
            for (int i = 0; i < values.Count; i++) if (tvalue > values[i] - values[0]) { select = i; break; }
            return posSlice[select];
        }
    }
    public static GameObject CreateHumaoid(string modelName, GameObject sourceModel, Texture2D sourceTexture, bool checkScene)
    {
        //https://news.4399.com/seer/zzph/

        Dictionary<Tuple<string, string>, float> myDictionary = new Dictionary<Tuple<string, string>, float>();


        string[] elements = new string[]
        {
            "fire", "water", "grass", "dragon", "fly", "elect", "stone",
            "machine", "ghost","god","ice","desert", "cow","crawl", "insect", "scary", "mush",
        };

        string[] shape_desc_str = new string[] { "default",
            "head_bodyfront", "head_boytop",
           "ear_top", "ear_side",
            "leg_short", "leg_tall",
            "arm_height_short", "arm_height_tall", "arm_width_strong", "arm_width_weak", 
           "hat_short", "hat_tall",
           "ear_top", "ear_side",
            
        };

        List<string> properties = new List<string>();
        properties.Add("fire");
        myDictionary.Add(Tuple.Create("fire", "hat_none"), 100);
        myDictionary.Add(Tuple.Create("fire", "ear_top"), 100);

        Dictionary<string, float> myExists = new Dictionary<string, float>();
        List<Pros> pros = new List<Pros>();
        foreach (string str in shape_desc_str)
        {
            string[] trimmedStrs = str.Split('_');
            string tStrs = "";
            for(int i = 0; i < trimmedStrs.Length - 1; i++)
            {
                if (i > 0) tStrs += "_";
                tStrs += trimmedStrs[i];
            }
            string partStr = trimmedStrs[0];
            bool part_exist = true;
            for (int i = 0; i < properties.Count; i++)
            {
                if(myDictionary.ContainsKey(Tuple.Create(properties[i], partStr + "_none")))
                {
                    part_exist = false;
                }
            }
           // Debug.Log(" str = " + str + " part = " + partStr + " tsr = " + tStrs + " ex = " + part_exist);
            if (part_exist)
            {
                float value = 1.0f;
                for (int i = 0; i < properties.Count; i++)
                {
                    if (myDictionary.TryGetValue(Tuple.Create(properties[i], tStrs), out float addedValue))
                    {
                        value += addedValue;
                    }
                }
                bool update = true;
                for(int i = 0; i < pros.Count; i++)
                    if(pros[i].name == tStrs)
                    {
                        pros[i].posSlice.Add(str);
                        pros[i].values.Add(value);
                       // Debug.Log(" tStrs + " + tStrs + " str = " + str + " value = " + value);
                        update = false;
                        break;
                    }
                if (update == true) pros.Add(new Pros(tStrs, str, value));

            }
        }

        // 这尼玛不就是Descriptor 吗？传过去吧
        List<string> theProperty = new List<string>();
        for (int i = 0; i < pros.Count; i++) theProperty.Add(pros[i].RandomString());
        for (int i = 0; i < pros.Count; i++) Debug.Log(theProperty[i]);


        List<ShapeDesc> parts = new List<ShapeDesc>();
        Vector3 partSize;
        Vector3 bodySize = Vector3.zero; ;
        // List<ShapeName> headEnum = new List<ShapeName> {ShapeName.LeftEar, ShapeName.LeftEye, ShapeName.HatBottom, ShapeName.Mouth};
        // List<ShapeName> bodyEnum = new List<ShapeName> { ShapeName.Tail, ShapeName.LeftArm, ShapeName.LeftLeg, ShapeName.LeftShoulder };
        List<ShapeName> headEnum = new List<ShapeName> {ShapeName.LeftEar, ShapeName.LeftEye};
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

        // todo : build from legs
        names = new List<ShapeName>() { ShapeName.PH_Stem, ShapeName.PH_LeftRootLeaf, ShapeName.PH_Head, ShapeName.PH_Mouth, ShapeName.PH_Lip, ShapeName.PH_LeftEye, ShapeName.PH_BackLeaf};
        //names = new List<ShapeName>() {  ShapeName.PH_Head };
        for (int i =0; i< names.Count; i++)
        {
            switch(names[i])
            {
                case ShapeName.Body:
                    {
                        partSize = RandomSize(6, 10, 6, 16, 6, 10);
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
                        Uint2 arm_height = new Uint2(12, 18);
                        if (theProperty.Contains("arm_height_short")) { arm_height = new Uint2(6, 10); }
                        Uint2 arm_width = new Uint2(5, 8);
                        if (theProperty.Contains("arm_width_strong")) { arm_width = new Uint2(3, 5); }

                        partSize = RandomSize((int)arm_width.x, (int)arm_width.y, (int)arm_height.x, (int)arm_height.y, (int)arm_width.x, (int)arm_width.y);
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
                        partSize = RandomSize((int)(bodySize.x / 3), (int)(bodySize.x / 2), 6, 10, (int)(bodySize.z / 3), (int)(bodySize.z / 2));
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
                        partSize = RandomSize(2, 2, 2, 2, 1, 1);
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
                case ShapeName.P_Head:
                    {
                        partSize = RandomSize(8, 12, 8, 12, 8, 12);
                        ShapeDesc sd = new ShapeDesc(ShapeName.P_Head, partSize, ShapeName.P_Head, FrameWork.Zero, FrameWork.Zero);
                        sd.segment = 2;
                        sd.segmet = SpecialBuilds.Sphere;
                        parts.Add(sd);
                        break;
                    }
                case ShapeName.P_Mouth:
                    {
                        partSize = RandomSize(2, 4, 2, 4, 4, 8);
                        ShapeDesc sd = new ShapeDesc(ShapeName.P_Mouth, partSize, ShapeName.P_Head, FrameWork.FrontCenter, FrameWork.BackCenter);
                        sd.segment = 1;
                        sd.segmet = SpecialBuilds.BigMouthZ;
                        parts.Add(sd);
                        break;
                    }
                case ShapeName.P_LeftEye:
                    {
                        partSize = RandomSize(2, 2, 2, 2, 1, 1);
                        parts.Add(new ShapeDesc(ShapeName.P_LeftEye, partSize, ShapeName.P_Head, FrameWork.HeadLeftEyePos, FrameWork.BackCenter));
                        parts.Add(new ShapeDesc(ShapeName.P_RightEye, partSize, ShapeName.P_Head, FrameWork.HeadRightEyePos, FrameWork.BackCenter));
                        break;
                    }
                case ShapeName.P_Stem:
                    {
                        partSize = RandomSize(1, 3, 4, 12, 1, 3);
                        parts.Add(new ShapeDesc(ShapeName.P_Stem, partSize, ShapeName.P_Head, FrameWork.BottomCenter, FrameWork.TopCenter));
                        break;
                    }
                case ShapeName.P_LeftRootLeaf:
                    {
                        partSize = RandomSize(4, 8, 1, 3, 4, 8);
                        parts.Add(new ShapeDesc(ShapeName.P_LeftRootLeaf, partSize, ShapeName.P_Stem, FrameWork.BottomeLeft, FrameWork.Right75));
                        parts.Add(new ShapeDesc(ShapeName.P_RightRootLeaf, partSize, ShapeName.P_Stem, FrameWork.BottomeRight, FrameWork.Left75));
                        break;
                    }
                case ShapeName.P_BackLeaf:
                    {
                        partSize = RandomSize(1, 3, 1, 3, 1, 3);
                        parts.Add(new ShapeDesc(ShapeName.P_BackLeaf, partSize, ShapeName.P_Head, FrameWork.BackCenter, FrameWork.FrontCenter));
                        break;
                    }
                case ShapeName.PH_Stem:
                    {
                        partSize = RandomSize(2, 5, 10, 16, 2, 5);
                        ShapeDesc sd = new ShapeDesc(ShapeName.PH_Stem, partSize, ShapeName.PH_Stem, FrameWork.Zero, FrameWork.Zero);
                        sd.segmet = SpecialBuilds.Hollow;
                        sd.segment = 1;
                        sd.dir = ShapeDir.YP;
                        parts.Add(sd);
                        break;
                    }
                case ShapeName.PH_Head:
                    {
                        // 2 x radius
                        partSize = RandomSize(6,8) * 2;
                        bodySize = partSize;
                        ShapeDesc sd = new ShapeDesc(ShapeName.PH_Head, partSize, ShapeName.PH_Stem, FrameWork.TopCenter, FrameWork.BottomCenter);
                        sd.segmet = SpecialBuilds.SolidSphere;
                        sd.segment = 1;
                        parts.Add(sd);
                        break;
                    }
                case ShapeName.PH_Mouth:
                    {
                        partSize = RandomSize(2, 4, 2, 4, 4, 6);
                        ShapeDesc sd = new ShapeDesc(ShapeName.PH_Mouth, partSize, ShapeName.PH_Head, FrameWork.FrontCenter, FrameWork.BackCenter);
                        sd.segmet = SpecialBuilds.Hollow;
                        sd.direction = 1;
                        sd.segment = 1;
                        sd.dir = ShapeDir.ZP;
                        parts.Add(sd);
                        break;
                    }
                case ShapeName.PH_Lip:
                    {
                        int r0 = (int)(bodySize.x / 2);
                        partSize = RandomSize(r0, r0 + 2, r0, r0 + 2, 4, 6);
                        ShapeDesc sd = new ShapeDesc(ShapeName.PH_Lip, partSize, ShapeName.PH_Mouth, FrameWork.FrontCenter, FrameWork.BackCenter);
                        sd.segmet = SpecialBuilds.SolidSphere;
                        sd.dir = ShapeDir.ZP;
                        sd.segment = 1;
                        parts.Add(sd);
                        break;
                    }
                case ShapeName.PH_LeftRootLeaf:
                    {
                        partSize = RandomSize(6, 10, 2, 4, 6, 10);
                        ShapeDesc sd0 = new ShapeDesc(ShapeName.PH_LeftRootLeaf, partSize, ShapeName.PH_Stem, FrameWork.BottomeLeft, FrameWork.Right75);
                        ShapeDesc sd1 = new ShapeDesc(ShapeName.PH_RightRootLeaf, partSize, ShapeName.PH_Stem, FrameWork.BottomeRight, FrameWork.Left75);
                        sd0.segmet = SpecialBuilds.HollowCurve;
                        sd1.segmet = SpecialBuilds.HollowCurve;
                        for(int k = 0; k <= partSize.x; k++) {
                            float he = (1 - k / partSize.x) * partSize.y;
                            sd0.values.Add(he);  sd1.values.Add(he); 
                        }
                        parts.Add(sd0);
                        parts.Add(sd1);
                        break;
                    }
                case ShapeName.PH_LeftEye:
                    {
                        partSize = RandomSize(2, 2, 2, 2, 1, 1);
                        parts.Add(new ShapeDesc(ShapeName.PH_LeftEye, partSize, ShapeName.PH_Head, FrameWork.HeadLeftEyePos, FrameWork.BackCenter));
                        parts.Add(new ShapeDesc(ShapeName.PH_RightEye, partSize, ShapeName.PH_Head, FrameWork.HeadRightEyePos, FrameWork.BackCenter));
                        break;
                    }
                case ShapeName.PH_BackLeaf:
                    {
                        partSize = RandomSize(6, 8, 1, 3, 6, 8);
                        ShapeDesc sd = new ShapeDesc(ShapeName.PH_BackLeaf, partSize, ShapeName.PH_Head, FrameWork.BackCenter, FrameWork.FrontCenter); 
                        sd.segment = 1;
                        sd.segmet = SpecialBuilds.Hollow;
                        sd.dir = ShapeDir.YP;
                        parts.Add(sd);
                        break;
                    }
                    {
                        // curve
                    }
            }
        }



        int index = 0;
        DataTransfer.indexToIndex = new List<int>();

        for (int i = 0; i < parts.Count; i++)
        {
            bool isToe = (parts[i].name == ShapeName.LeftToe || parts[i].name == ShapeName.RightToe);
            int Segment = Mathf.Abs(parts[i].segment);
            List<Vector3> posSlice = new List<Vector3>();
            List<Vector3> sizeSlice = new List<Vector3>();
            List<Quaternion> rotateSlice = new List<Quaternion>();

            bool saveOnce = false;

            switch (parts[i].segmet)
            {
                case SpecialBuilds.TopDown:
                    {
                        Vector3 nowPos = new Vector3(0, parts[i].size.y / 2, 0);
                        for (int j = 0; j < Segment; j++)
                        {
                            int minus = j;
                            if (parts[i].segment > 0)
                            {
                                minus = Segment - 1 - j;
                            }
                            Vector3 newSize = new Vector3(parts[i].size.x - 0.1f * minus, parts[i].size.y / Segment, parts[i].size.z - 0.1f * minus);
                            if (isToe) newSize = parts[i].size;
                            sizeSlice.Add(newSize);
                            nowPos = nowPos - new Vector3(0, newSize.y / 2, 0);
                            if (isToe) nowPos = Vector3.zero;
                            posSlice.Add(nowPos);
                            nowPos = nowPos - new Vector3(0, newSize.y / 2, 0);
                        }
                        break;
                    }
                case SpecialBuilds.Sphere:
                    {
                        posSlice.Add(new Vector3(0, 0, 0));
                        Vector3 baseOffset = new Vector3(parts[i].size.x - 0.1f * (Segment - 1) * 2, parts[i].size.y - 0.1f * (Segment - 1) * 2, parts[i].size.z - 0.1f * (Segment - 1) * 2);
                        sizeSlice.Add(baseOffset);
                        for (int j = 0; j < Segment - 1; j++)
                        {
                            float offsetx = baseOffset.x * 0.5f + (0.5f + j) * 0.1f;
                            float offsety = baseOffset.y * 0.5f + (0.5f + j) * 0.1f;
                            float offsetz = baseOffset.z * 0.5f + (0.5f + j) * 0.1f;
                            float sizex = baseOffset.x - 0.1f * j;
                            float sizey = baseOffset.y - 0.1f * j;
                            float sizez = baseOffset.z - 0.1f * j;
                            posSlice.Add(new Vector3(offsetx, 0, 0));
                            posSlice.Add(new Vector3(-offsetx, 0, 0));
                            posSlice.Add(new Vector3(0, offsety, 0));
                            posSlice.Add(new Vector3(0, -offsety, 0));
                            posSlice.Add(new Vector3(0, 0, offsetz));
                            posSlice.Add(new Vector3(0, 0, -offsetz));
                            sizeSlice.Add(new Vector3(0.1f, sizey, sizez));
                            sizeSlice.Add(new Vector3(0.1f, sizey, sizez));
                            sizeSlice.Add(new Vector3(sizex, 0.1f, sizez));
                            sizeSlice.Add(new Vector3(sizex, 0.1f, sizez));
                            sizeSlice.Add(new Vector3(sizex, sizey, 0.1f));
                            sizeSlice.Add(new Vector3(sizex, sizey, 0.1f));
                        }
                        break;
                    }
                case SpecialBuilds.BigMouthZ: // retangle z
                    {

                        int sx = (int)(parts[i].size.x * 10 * 0.5);
                        int sz = (int)(parts[i].size.x * 10 * 0.5);
                        int radius = 1;
                        posSlice.Add(new Vector3(0, 0, 0)); 
                        sizeSlice.Add(new Vector3((sx - radius) * 0.2f, parts[i].size.y, sz));
                        int baseWidth = sz - radius;
                        List<int> widths = GetCircle(0, radius);
                        for(int j = 0; j < widths.Count; j += 3)
                        {
                            int startx = widths[j];
                            int continuex = widths[j + 1];
                            int widthz = widths[j + 2];

                            posSlice.Add(new Vector3(0, 0, 0));
                            sizeSlice.Add(new Vector3((startx + continuex) * 0.2f, parts[i].size.y, widthz));
                            posSlice.Add(new Vector3(0, 0, 0));
                            sizeSlice.Add(new Vector3(-(startx + continuex) * 0.2f, parts[i].size.y, widthz));
                        }
                        saveOnce = true;

                        break;
                    }

                case SpecialBuilds.HollowCurve:
                    {

                        saveOnce = true;
                        int sx = (int)(parts[i].size.x * 10 * 0.5);
                        int sz = (int)(parts[i].size.x * 10 * 0.5);
                        for (int px = 0;  px < sx; px++)
                        {
                            float they = parts[i].values[px];
                            for(int pz = 0; pz < sz; pz++)
                            {
                                float dis0 = (float)((px + 0.5) * (px + 0.5) / sx / sx + (pz + 0.5) * (pz + 0.5) / sz / sz);
                                float dis1 = (float)((px + 0.5) * (px + 0.5) / sx / sx + (pz + 1.5) * (pz + 1.5) / sz / sz);
                                if(dis0 <= 1 && dis1 > 0)
                                {
                                    posSlice.Add(new Vector3((px + 1) * 0.05f, they, 0));
                                    posSlice.Add(new Vector3(-(px + 1) * 0.05f, they, 0));
                                    sizeSlice.Add(new Vector3(0.1f, 0.1f, pz * 0.2f));
                                    sizeSlice.Add(new Vector3(0.1f, 0.1f, pz * 0.2f));
                                }
                            }
                        }


                        break;
                    }
                case SpecialBuilds.Curve:
                    {
                        // Spider Legs
                        // 只能一个叠一个，不能4个叠4个
                        saveOnce = true;
                        int sx = (int)(parts[i].size.x * 10 * 0.5);
                        int sz = (int)(parts[i].size.x * 10 * 0.5);
                        for (int px = 0; px < sx; px++)
                        {
                            float they = parts[i].values[px * 2 + 0];
                            float thez = parts[i].values[px * 2 + 1];
                            posSlice.Add(new Vector3(px, they, thez));
                            sizeSlice.Add(new Vector3(0.1f, 0.1f, 0.1f));
                        }


                        break;
                    }
                case SpecialBuilds.Muscle:
                    {
                        int sx = (int)(parts[i].size.x * 10 * 0.5);
                        int sz = (int)(parts[i].size.x * 10 * 0.5);
                        // Spider Legs
                        // 只能一个叠一个，不能4个叠4个
                        saveOnce = true;
                        posSlice.Add(new Vector3(0, 0, 0));
                        sizeSlice.Add(new Vector3(0.1f, 0.1f, 0.3f));
                        posSlice.Add(new Vector3(0, parts[i].size.y / 9, 0));
                        sizeSlice.Add(new Vector3(0.1f, 0.1f, 0.1f));
                        posSlice.Add(new Vector3(0, -parts[i].size.y / 9, 0));
                        sizeSlice.Add(new Vector3(0.1f, 0.1f, 0.1f));
                        rotateSlice.AddRange(new Quaternion[] { Quaternion.identity, Quaternion.identity, Quaternion.identity });

                        float rotatedx = (sx - 0.5f) / 2;
                        float rotateSize = (sx - 0.5f);
                        posSlice.AddRange(new Vector3[] { new Vector3(rotatedx, 0, -1.5f), new Vector3(rotatedx, 0, 1.5f),
                                new Vector3(-rotatedx, 0, -1.5f),new Vector3(-rotatedx, 0, 1.5f),
                                new Vector3(rotatedx, -1.5f, 0), new Vector3(rotatedx, 1.5f, 0),
                                new Vector3(-rotatedx, -1.5f, 0),new Vector3(-rotatedx, 1.5f, 0)
                        });
                        for(int j = 0; j < 8;j++)sizeSlice.Add(new Vector3(0.1f, 0.1f, rotateSize));
                        rotateSlice.AddRange(new Quaternion[] { 
                            Quaternion.Euler(0,15,0), Quaternion.Euler(0,-15,0), Quaternion.Euler(0,15,0), Quaternion.Euler(0,-15,0),
                            Quaternion.Euler(0,0,15), Quaternion.Euler(0,0,-15), Quaternion.Euler(0,0,15), Quaternion.Euler(0,0,-15),
                        });

                        break;
                    }
                case SpecialBuilds.HR_Body:
                    {
                        //https://youtu.be/M2fBL0Z9aVw?t=720 怀疑用旋转的，还不如我用立方体的
                        float width = (parts[i].size.x * 10 / parts[i].segment);
                        int sx = (int)(parts[i].size.x * 10 * 0.5f);
                        int sz = (int)(parts[i].size.z * 10 * 0.5f);
                        for(int px = 0; px < parts[i].size.x; px++)
                        {
                            for(int pz = 0; pz < parts[i].size.x; pz++)
                            {
                                float dis0 = (float)((px + 0.5) * (px + 0.5) / sx / sx + (pz + 0.5) * (pz + 0.5) / sz / sz);
                                float dis1 = (float)((px + 0.5) * (px + 0.5) / sx / sx + (pz + 1.5) * (pz + 1.5) / sz / sz);
                                if (dis0 <= 1 && dis1 > 0)
                                {
                                    posSlice.Add(new Vector3((px + 1) * 0.05f, they, 0));
                                    posSlice.Add(new Vector3(-(px + 1) * 0.05f, they, 0));
                                    sizeSlice.Add(new Vector3(0.1f, 0.1f, pz * 0.2f));
                                    sizeSlice.Add(new Vector3(0.1f, 0.1f, pz * 0.2f));
                                }
                            }
                        }
                    }
                case SpecialBuilds.RoundCircle:
                    {
                        /*
                           
                         / ---  \
                         |      |
                         \ --- /
                         */
                        //https://youtu.be/M2fBL0Z9aVw?t=720 怀疑用旋转的，还不如我用立方体的
                        float radius = 0.1f;
                        posSlice.Add(new Vector3(0,0,0));
                        sizeSlice.Add(new Vector3(parts[i].size.x, parts[i].size.y, parts[i].size.z - radius - radius));
                        posSlice.Add(new Vector3(0, 0, (parts[i].size.z + radius ) / 2));
                        sizeSlice.Add(new Vector3(parts[i].size.x - radius - radius, parts[i].size.y, radius));
                        posSlice.Add(new Vector3(0, 0, -(parts[i].size.z + radius) / 2));
                        sizeSlice.Add(new Vector3(parts[i].size.x - radius - radius, parts[i].size.y, radius));
                        rotateSlice.AddRange(new Quaternion[] { Quaternion.identity, Quaternion.identity, Quaternion.identity, Quaternion.identity });

                        Vector3 bp = new Vector3(parts[i].size.x / 2 - radius, 0, parts[i].size.z / 2 - radius);
                        Vector3 bs = new Vector3(radius * 1.5f, parts[i].size.y, radius * 1.5f);
                        posSlice.AddRange(new Vector3[] { new Vector3(bp.x, bp.y, bp.z), new Vector3(-bp.x, bp.y, bp.z), new Vector3(bp.x, bp.y, -bp.z), new Vector3(-bp.x, bp.y, -bp.z) });
                        sizeSlice.AddRange(new Vector3[] {bs,bs,bs,bs});
                        rotateSlice.AddRange(new Quaternion[] { Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 45, 0) });

                    }
                case SpecialBuilds.RoundCircleForCircle:
                    {
                        // X 轴适合做身体，Y轴做肌肉和脑袋
                        /*
                           
                         / ---  \
                         |      |
                         \ --- /
                         */
                        //https://youtu.be/M2fBL0Z9aVw?t=720 怀疑用旋转的，还不如我用立方体的

                        for(int px = 0; px < 0; px++)
                        {
                            float outterR = 0.1f;
                            float innerR = 0.1f;
                            posSlice.Add(new Vector3(0, 0, 0));
                            sizeSlice.Add(new Vector3(0.1f, (outterR - innerR) * 2, outterR * 2));
                            posSlice.Add(new Vector3(0, 0, outterR + innerR / 2));
                            sizeSlice.Add(new Vector3(0.1f, innerR, outterR - innerR - innerR));
                            posSlice.Add(new Vector3(0, 0, -outterR - innerR / 2));
                            sizeSlice.Add(new Vector3(0.1f, innerR, outterR - innerR - innerR));
                            rotateSlice.AddRange(new Quaternion[] { Quaternion.identity, Quaternion.identity, Quaternion.identity, });

                            Vector3 bp = new Vector3(0, outterR - innerR, outterR - innerR);
                            Vector3 bs = new Vector3(0.1f, innerR * 1.5f, innerR * 1.5f);
                            posSlice.AddRange(new Vector3[] { new Vector3(bp.x, bp.y, bp.z), new Vector3(-bp.x, bp.y, bp.z), new Vector3(bp.x, bp.y, -bp.z), new Vector3(-bp.x, bp.y, -bp.z) });
                            sizeSlice.AddRange(new Vector3[] { bs, bs, bs, bs });
                            rotateSlice.AddRange(new Quaternion[] { Quaternion.Euler(45, 0, 0), Quaternion.Euler(45, 0, 0), Quaternion.Euler(45, 0, 0), Quaternion.Euler(45, 0, 0) });
                        }

                        // up wards

                        float outterR = 0.1f;
                        float innerR = 0.1f;
                        posSlice.Add(new Vector3(0.05f, 0, outterR));
                        sizeSlice.Add(new Vector3(innerR * 1.5f, outterR, innerR * 1.5f));
                        posSlice.Add(new Vector3(0.05f, 0, -outterR));
                        sizeSlice.Add(new Vector3(innerR * 1.5f, outterR, innerR * 1.5f));


                        posSlice.Add(new Vector3(0.05f, outterR, 0));
                        sizeSlice.Add(new Vector3(innerR * 1.5f, innerR * 1.5f, outterR));
                        posSlice.Add(new Vector3(0.05f, -outterR, 0));
                        sizeSlice.Add(new Vector3(innerR * 1.5f, innerR * 1.5f, outterR)); 
                        rotateSlice.AddRange(new Quaternion[] { Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 0, 45), Quaternion.Euler(0, 45, 0) });


                    }
                case SpecialBuilds.Angle:
                    {
                        saveOnce = true;
                        float currentRadius = 4;
                        float accRadius = 0;
                        while (currentRadius >= 1)
                        {
                            accRadius += currentRadius * 0.5f;

                            posSlice.Add(new Vector3(accRadius, 0, 0));
                            sizeSlice.Add(new Vector3(accRadius, accRadius , accRadius));
                            rotateSlice.Add(Quaternion.Euler(posSlice.Count * 15, 0, 0));

                            accRadius += currentRadius * 0.5f;
                            currentRadius -= 0.5f;
                        }

                    }
                case SpecialBuilds.Hollow:
                    {

                        saveOnce = true;

                        int radius = (int)(parts[i].size.x * 10 * 0.5);
                        int basex = (int)(radius / Mathf.Sqrt(2) + 0.5f); // 5 -> 3.5 -> 4
                        posSlice.Add(new Vector3(0, 0, 0));

                        switch (parts[i].dir)
                        {
                            case ShapeDir.YP:
                            case ShapeDir.YM:
                                {
                                    sizeSlice.Add(new Vector3(basex * 0.2f, parts[i].size.y, basex * 0.2f));
                                    break;
                                }
                            case ShapeDir.ZP:
                            case ShapeDir.ZM:
                                {
                                    sizeSlice.Add(new Vector3(basex * 0.2f, basex * 0.2f, parts[i].size.z));
                                    break;
                                }
                            case ShapeDir.XP:
                            case ShapeDir.XM:
                                {
                                    sizeSlice.Add(new Vector3(parts[i].size.x, basex * 0.2f, basex * 0.2f));
                                    break;
                                }
                            default: break;
                        }

                        Debug.Log(" size = " + sizeSlice.Count + " poss = " + posSlice.Count);

                        for (int px = basex; px < radius; px++)
                        {
                            int possibleDis = -1;
                            for (int pz = 0; pz < radius; pz++)
                            {
                                float dis = (float)((px + 0.5) * (px + 0.5) + (pz + 0.5) * (pz + 0.5));
                                if (dis <= radius * radius) possibleDis = pz;
                            }
                            int startPx = px;
                            possibleDis += 1;
                            while (true)
                            {
                                float dis = (float)((px + 1.5) * (px + 1.5) + (possibleDis + 0.5) * (possibleDis + 0.5));
                                if (dis <= radius * radius) px += 1;
                                else
                                {
                                    switch (parts[i].dir)
                                    {
                                        case ShapeDir.YP:
                                        case ShapeDir.YM:
                                            {
                                                Vector3 bpos = new Vector3((px + 1 + startPx) * 0.05f, 0, 0);
                                                Vector3 bsize = new Vector3((px + 1 - startPx) * 0.1f, parts[i].size.y, possibleDis * 0.2f);

                                                posSlice.Add(new Vector3(bpos.x, bpos.y, bpos.z));
                                                sizeSlice.Add(new Vector3(bsize.x, bsize.y, bsize.z));
                                                posSlice.Add(new Vector3(-bpos.x, bpos.y, bpos.z));
                                                sizeSlice.Add(new Vector3(bsize.x, bsize.y, bsize.z));

                                                posSlice.Add(new Vector3(bpos.z, bpos.y, bpos.x));
                                                sizeSlice.Add(new Vector3(bsize.z, bsize.y, bsize.x));
                                                posSlice.Add(new Vector3(bpos.z, bpos.y, -bpos.x));
                                                sizeSlice.Add(new Vector3(bsize.z, bsize.y, bsize.x));
                                                break;
                                            }
                                        case ShapeDir.ZP:
                                        case ShapeDir.ZM:
                                            {
                                                Vector3 bpos = new Vector3(0 ,(px + 1 + startPx) * 0.05f, 0);
                                                Vector3 bsize = new Vector3(possibleDis * 0.2f, (px + 1 - startPx) * 0.1f, parts[i].size.z);

                                                posSlice.Add(new Vector3(bpos.x, bpos.y, bpos.z));
                                                sizeSlice.Add(new Vector3(bsize.x, bsize.y, bsize.z));
                                                posSlice.Add(new Vector3(bpos.x, -bpos.y, bpos.z));
                                                sizeSlice.Add(new Vector3(bsize.x, bsize.y, bsize.z));

                                                posSlice.Add(new Vector3(bpos.y, bpos.x, bpos.z));
                                                sizeSlice.Add(new Vector3(bsize.y, bsize.x, bsize.z));
                                                posSlice.Add(new Vector3(-bpos.y, bpos.x, bpos.z));
                                                sizeSlice.Add(new Vector3(bsize.y, bsize.x, bsize.z));
                                                break;
                                            }
                                        default: break;
                                    }

                                    
                                    break;
                                }
                            }
                        }
                        Debug.Log(" size = " + sizeSlice.Count + " poss = " + posSlice.Count);
                        break;
                    }
                case SpecialBuilds.SolidSphere:
                    {
                        saveOnce = true;

                        int radius =  (int)(parts[i].size.x * 10 * 0.5);
                        int basex = (int)(radius / Mathf.Sqrt(2) - 0.5f); // 5 -> 3.5 -> 4
                        Debug.Log(" x = " + parts[i].size.x + " radius = " + radius + " basex = " + basex);

                        switch (parts[i].dir)
                        {
                            case ShapeDir.Full:
                                {
                                    posSlice.Add(new Vector3(0, 0, 0));
                                    sizeSlice.Add(new Vector3(basex * 0.2f, basex * 0.2f, basex * 0.2f));
                                    break;
                                }
                            case ShapeDir.ZM:
                                {
                                    posSlice.Add(new Vector3(0, 0, -basex * 0.05f));
                                    sizeSlice.Add(new Vector3(basex * 0.2f, basex * 0.2f, basex * 0.1f));
                                    break;
                                }
                            case ShapeDir.ZP:
                                {
                                    posSlice.Add(new Vector3(0, 0, basex * 0.05f));
                                    sizeSlice.Add(new Vector3(basex * 0.2f, basex * 0.2f, basex * 0.1f));
                                    break;
                                }
                            default: break;
                        }


                        for(int px = basex; px <= radius; px++)
                        {
                            int possibleDis = -1;
                            for (int pz = 0; pz <= radius; pz++)
                            {
                                float dis = (float)((px + 0.5) * (px + 0.5) + (pz + 0.5) * (pz + 0.5));
                                if (dis <= radius * radius) possibleDis = pz;
                            }
                            int startPx = px;
                            possibleDis += 1;
                            while (true)
                            {
                                float dis = (float)((px + 1.5) * (px + 1.5) + (possibleDis + 0.5) * (possibleDis + 0.5));
                                if (dis <= radius * radius) px += 1;
                                else
                                {
                                    List<Vector3> bpos = new List<Vector3>();
                                    List<Vector3> bsize = new List<Vector3>();
                                    int bbasex = (int)(possibleDis / Mathf.Sqrt(2) + 0.5f);
                                    Debug.Log(" for 2= " + bbasex + " ppdis = " + possibleDis);
                                    bpos.Add(new Vector3((px + startPx) * 0.05f + 0.05f, 0, 0));
                                    float bbasey = bbasex * 0.2f;
                                    if (px == basex) bbasey -= 0.2f;
                                    bsize.Add(new Vector3((px + 1 - startPx) * 0.1f, bbasey, bbasex * 0.2f));
                                    for (int ppx = bbasex; ppx < possibleDis; ppx++)
                                    {
                                        int ppDis = -1;
                                        for(int ppz = 0; ppz < possibleDis; ppz++)
                                        {
                                            float ddis = (float)((ppx + 0.5) * (ppx + 0.5) + (ppz + 0.5) * (ppz + 0.5));
                                            if (ddis <= possibleDis * possibleDis) ppDis = ppz;
                                        }
                                        int startppx = ppx;
                                        while (true)
                                        {
                                            float ddis = (float)((ppx + 1.5) * (ppx + 1.5) + (ppDis + 0.5) * (ppDis + 0.5));
                                            if (ddis <= possibleDis * possibleDis) ppx += 1;
                                            else
                                            {
                                                Vector3 bbpos = new Vector3((px + startPx) * 0.05f + 0.05f, 0, (ppx + startppx) * 0.05f + 0.05f);
                                                Vector3 bbsize = new Vector3((px + 1 - startPx) * 0.1f, (ppDis + 1) * 0.2f, (ppx + 1 - startppx) * 0.1f);
                                                bpos.Add(new Vector3(bbpos.x, bbpos.y, bbpos.z));
                                                bsize.Add(new Vector3(bbsize.x, bbsize.y, bbsize.z));
                                                bpos.Add(new Vector3(bbpos.x, bbpos.y, -bbpos.z));
                                                bsize.Add(new Vector3(bbsize.x, bbsize.y, bbsize.z));
                                                bpos.Add(new Vector3(bbpos.x, bbpos.z, bbpos.y));
                                                bsize.Add(new Vector3(bbsize.x, bbsize.z, bbsize.y));
                                                bpos.Add(new Vector3(bbpos.x, -bbpos.z, bbpos.y));
                                                bsize.Add(new Vector3(bbsize.x, bbsize.z, bbsize.y));
                                                break;
                                            }
                                        }
                                    }
                                    for(int bposindex = 0; bposindex < bpos.Count; bposindex++)
                                    {
                                        Vector3 pos0 = bpos[bposindex];
                                        Vector3 size0 = bsize[bposindex];

                                        switch (parts[i].dir)
                                        {
                                            case ShapeDir.Full:
                                                {
                                                    posSlice.Add(new Vector3(pos0.x, pos0.y, pos0.z));
                                                    sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z));
                                                    posSlice.Add(new Vector3(-pos0.x, pos0.y, pos0.z));
                                                    sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z));

                                                   // if (bposindex > 0)
                                                    {
                                                       posSlice.Add(new Vector3(pos0.z, pos0.x, pos0.y));
                                                       sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y));
                                                       posSlice.Add(new Vector3(pos0.z, -pos0.x, pos0.y));
                                                       sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y));
                                                    }

                                                   
                                      
                                                        posSlice.Add(new Vector3(pos0.y, pos0.z, pos0.x));
                                                        sizeSlice.Add(new Vector3(size0.y, size0.z, size0.x));
                                                        posSlice.Add(new Vector3(pos0.y, pos0.z, -pos0.x));
                                                        sizeSlice.Add(new Vector3(size0.y, size0.z, size0.x));
                                                    

                                                    
                                                    break;
                                                }
                                            case ShapeDir.ZM:
                                                {
                                                    if (pos0.z == 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.x, pos0.y, pos0.z - size0.z / 4));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z / 2));
                                                        posSlice.Add(new Vector3(-pos0.x, pos0.y, pos0.z - size0.z / 4));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z / 2));
                                                    }else if(pos0.z < 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.x, pos0.y, pos0.z));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z));
                                                        posSlice.Add(new Vector3(-pos0.x, pos0.y, pos0.z));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z));
                                                    }

                                                    if(pos0.y == 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.z, pos0.x, pos0.y - size0.y / 4));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y / 2));
                                                        posSlice.Add(new Vector3(pos0.z, -pos0.x, pos0.y - size0.y / 4));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y / 2));
                                                    }
                                                    else if(pos0.y < 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.z, pos0.x, pos0.y));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y));
                                                        posSlice.Add(new Vector3(pos0.z, -pos0.x, pos0.y));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y));
                                                    }

                                                    posSlice.Add(new Vector3(pos0.y, pos0.z, -pos0.x));
                                                    sizeSlice.Add(new Vector3(size0.y, size0.z, size0.x));
                                                    break;
                                                }
                                            case ShapeDir.ZP:
                                                {
                                                    if (pos0.z == 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.x, pos0.y, pos0.z + size0.z / 4));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z / 2));
                                                        posSlice.Add(new Vector3(-pos0.x, pos0.y, pos0.z + size0.z / 4));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z / 2));
                                                    }
                                                    else if (pos0.z > 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.x, pos0.y, pos0.z));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z));
                                                        posSlice.Add(new Vector3(-pos0.x, pos0.y, pos0.z));
                                                        sizeSlice.Add(new Vector3(size0.x, size0.y, size0.z));
                                                    }

                                                    if (pos0.y == 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.z, pos0.x, pos0.y + size0.y / 4));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y / 2));
                                                        posSlice.Add(new Vector3(pos0.z, -pos0.x, pos0.y + size0.y / 4));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y / 2));
                                                    }
                                                    else if (pos0.y > 0)
                                                    {
                                                        posSlice.Add(new Vector3(pos0.z, pos0.x, pos0.y));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y));
                                                        posSlice.Add(new Vector3(pos0.z, -pos0.x, pos0.y));
                                                        sizeSlice.Add(new Vector3(size0.z, size0.x, size0.y));
                                                    }

                                                    posSlice.Add(new Vector3(pos0.y, pos0.z, pos0.x));
                                                    sizeSlice.Add(new Vector3(size0.y, size0.z, size0.x));
                                                    break;
                                                }
                                            default: break;
                                        }
                                       


                                    }


                                    break;
                                }
                            }
                        }
                        break;
                        
                    }
                default:
                    {
                        posSlice.Add(new Vector3(0, 0, 0));
                        sizeSlice.Add(parts[i].size);
                        break;
                    }
            }


            List<GameObject> actors = new List<GameObject>();
            int startIndex = index;
            for (int j = 0; j < posSlice.Count; j++)
            {
                GameObject actor = CreateCube();
                actor.transform.localPosition = posSlice[j];
                actor.transform.localScale = sizeSlice[j];
                Uint3 size = new Uint3((uint)(sizeSlice[j].x * 10.0f), (uint)(sizeSlice[j].y * 10.0f), (uint)(Mathf.Abs(sizeSlice[j].z) * 10.0f));
                actor.GetComponent<MeshFilter>().sharedMesh.SetUVs(0, ComputeUVs(size));
                actor.GetComponent<MeshRenderer>().material = ExpectMaterial(sourceTexture, sourceModel, parts[i].name, size);

                if (!saveOnce || (saveOnce && j == 0))
                {

                    Material tempM = ExpectMaterial(sourceTexture, sourceModel, parts[i].name, size);
                    SaveTextureToPNG((Texture2D)tempM.mainTexture, "Assets/Temp/" + modelName + "_" + index.ToString() + ".png");
                    //AssetDatabase.CreateAsset(tempM, "Assets/Temp/" + modelName + "_" + index.ToString() + ".mat");
                    AssetDatabase.CreateAsset(actor.GetComponent<MeshFilter>().sharedMesh, "Assets/Temp/" + modelName + "_" + index.ToString() + ".mesh");
                }

                if (saveOnce) DataTransfer.indexToIndex.Add(startIndex);
                else DataTransfer.indexToIndex.Add(index);
                
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
                Debug.Log(" offset target = " + target + " now = " + now);
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
        parentObject.transform.position = Vector3.zero;
        GameObject allObject = new GameObject("All");
        allObject.transform.position = new Vector3(0, 0,0);
        allObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        allObject.transform.SetParent(parentObject.transform);
        // 遍历所有 part，将它们的父级设置为新创建的父物体
        int cube_index = 0;

        foreach (var part in parts)
        {

            string part_name = part.name.ToString();
            part_name = part_name.Replace("PH_", "");
            part_name = part_name.Replace("P_", "");
            part_name = part_name.Replace("A_", "");
            part_name = part_name.Replace("H_", "");
            GameObject parentObjectForPart = new GameObject(part.name.ToString());
            for (int i = 0; i < part.actors.Count; i++)
            {
                part.actors[i].transform.name = "cube_" + cube_index.ToString();
                cube_index++;
                part.actors[i].transform.SetParent(parentObjectForPart.transform);
            }
            parentObjectForPart.transform.SetParent(allObject.transform);
            parentObject.transform.localRotation = Quaternion.identity;
        }

        Bounds totalBounds = new Bounds(parentObject.transform.position, Vector3.zero);
        CalculateBoundsRecursive(parentObject.transform, ref totalBounds);
        allObject.transform.position = new Vector3(0, -(totalBounds.center.y - totalBounds.size.y / 2),0);
        Debug.Log(" center = " + totalBounds.center.y + " sziey = " + totalBounds.size.y);
        // 返回这个新的父物体
        return parentObject;

    }

    private static void CalculateBoundsRecursive(Transform current, ref Bounds totalBounds)
    {
        // 检查是否有 Renderer 组件，如果有，就用它的 bounds 进行包围框计算
        Renderer renderer = current.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 如果 totalBounds 还是空的，直接赋值
            if (totalBounds.size == Vector3.zero)
            {
                totalBounds = renderer.bounds;
            }
            else
            {
                // 合并当前子物体的包围框
                totalBounds.Encapsulate(renderer.bounds);
            }
        }

        // 遍历所有子物体，继续递归计算包围框
        foreach (Transform child in current)
        {
            CalculateBoundsRecursive(child, ref totalBounds);
        }
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