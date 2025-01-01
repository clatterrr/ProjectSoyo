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
        // ��̬�����ǣ��½���

        string[] keywords = new string[] { "������" };


        // Step1 ���ߣ�����

        // Step2 �ж��Ƿ�֮ǰ��̨��

        // If ��ʦ�����档�����п���ͻ�ƣ��п��ܲ�ͻ��

        // �� -> ����λ

        // �ϻ�����

        // CSV�ļ�·��
        string csvFilePath = "E:/prices.csv";

        // ��ȡCSV�ļ���������
        string[] lines = File.ReadAllLines(csvFilePath);

        List<Price> priceList = new List<Price>();

        // ������һ�У�����������б��⣩
        foreach (string line in lines.Skip(1))
        {
            // ʹ�ö��ŷָ����ָ�ÿһ��
            string[] values = line.Split(',');
            Debug.Log(line);
            // ������鳤���Ƿ��㹻
            if (values.Length >= 5)
            {
                // �����2��3��4��5�е�ֵ
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
            // ��������ema5 ���Ǽ�Ϊ����
            // ��������ema5���ǳ���10%��Ϊ����

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


        // ����һ���µ���Ϸ������Ϊ������
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // �����������λ�ã���������Ϊ(0,0,0)�����κ�����λ��
        cube.transform.position = Vector3.zero;

        // ��ȡ�������Mesh Renderer���
        MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();

        // ���ò�����ɫΪ��ɫ
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
        // ���ڣ����������ǣ��µ���ͻ��
        // ֮�����������ǣ��µ���ͻ��

        string[] nowStatic = new string[] { " eth����������˰���" };

        string[] willUp = new string[] { "����Ҫǿ��ͻ����. ��ô֮������һֱ�ǣ��ǵ������е��ĻŻ�" };
        string[] waste = new string[] { "�ϸ����ߵ���ô��", "�Լ�����������������ܻ���ô��" };

        string[] betterShape = new string[] { "����Կ���������ߵĺò���, �ߵ�˵ʵ������" };

        //string[] waste = new string[] { "����������Ҫ����ҿ�����", "����ָ���������ڵ�һ������ �ð�", "����������ʲô" };

        List<Price> price = new List<Price>();
       
    }
}
