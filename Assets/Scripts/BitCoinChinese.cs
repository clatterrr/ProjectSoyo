using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BitCoinChinese : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 形态，上涨，下降，

        string[] keywords = new string[] { "阻力，" };


        // Step1 周线，日线

        // Step2 判断是否之前的台数

        // If 大师，两面。盘整有可能突破，有可能不突破

        // 涨 -> 阻力位

        // 废话分析

        // CSV文件路径
        string csvFilePath = "E:/prices.csv";

        // 读取CSV文件的所有行
        string[] lines = File.ReadAllLines(csvFilePath);

        List<Price> priceList = new List<Price>();

        // 跳过第一行（如果它包含列标题）
        foreach (string line in lines.Skip(1))
        {
            // 使用逗号分隔符分割每一行
            string[] values = line.Split(',');
            Debug.Log(line);
            // 检查数组长度是否足够
            if (values.Length >= 5)
            {
                // 输出第2、3、4、5列的值
                Debug.Log($"Column 2: {values[1]}, Column 3: {values[2]}, Column 4: {values[3]}, Column 5: {values[4]}");

                float open = float.Parse(values[3]);
                float close = float.Parse(values[4]);
                float high = float.Parse(values[1]);
                float low = float.Parse(values[2]);

                priceList.Add(new Price(open, close, high, low, 0, 0));
            }
            else
            {
                Console.WriteLine("Not enough columns in the row.");
            }
        }

        /*
        for (int dayIndex = 0; dayIndex < priceList.Count; dayIndex++)
        {
            // 连续三天ema5 上涨即为上涨
            // 连续三天ema5上涨超过10%即为大涨

            List<float> ema5List = new List<float>() { priceList[dayIndex - 2].ema5, priceList[dayIndex - 1].ema5, priceList[dayIndex].ema5 };

            bool rise = true;
            bool riseBig = true;
            bool fall = true;
            bool fallBig = true;
            bool staticPrice = true;

            for (int ema5Index = 1; ema5Index < ema5List.Count; ema5Index++)
            {
                float ratio = (ema5List[ema5Index] - ema5List[ema5Index - 1]) / ema5List[ema5Index - 1];
                if (ratio < 0.1) riseBig = false;
                if (ratio < 0.03) rise = false;
                if (ratio > -0.1) fallBig = false;
                if (ratio > -0.03) fall = false;
                if (Mathf.Abs(ratio) >= 0.03) staticPrice = false;
            }
        }
        */


        // 创建一个新的游戏对象作为立方体
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // 设置立方体的位置，可以设置为(0,0,0)或者任何其他位置
        cube.transform.position = Vector3.zero;

        // 获取立方体的Mesh Renderer组件
        MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();

        // 设置材质颜色为绿色
        meshRenderer.material.color = Color.green;
    }

    enum KShape
    {
        Static,
        Up,
        Down,
    }

    struct Price
    {
       public float highest;
        public float lowest;
        public float open;
        public float close;
        public float ema5;
        public float ema20;

        public Price(float highest, float lowest, float open, float close, float ema5, float ema20)
        {
            this.close = close;
            this.highest = highest;
            this.lowest = lowest;
            this.open = open;
            this.ema5 = ema5;
            this.ema20 = ema20;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // 现在，盘整，上涨，下跌，突破
        // 之后，盘整，上涨，下跌，突破

        string[] nowStatic = new string[] { " eth在这边盘整了半年" };

        string[] willUp = new string[] { "现在要强势突破了. 那么之后它就一直涨，涨得让人有点心慌慌" };
        string[] waste = new string[] { "上个月走的怎么样", "以及接下来这个月它可能会怎么走" };

        string[] betterShape = new string[] { "你可以看这个月线走的好不好, 走的说实话不好" };

        //string[] waste = new string[] { "接下来我们要给大家看的是", "现在指导我们现在的一个东西 好吧", "我们现在是什么" };

        List<Price> price = new List<Price>();
       
    }
}
