using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public Material material;

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    public static Color newColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public struct ColorHelper
    {
        public Color upColor0;
        public Color upColor1;
        public Color midColor0;
        public Color midColor1;
        public Color downColor0;
        public Color downColor1;

        public ColorHelper(Color c0, Color c1)
        {
            this.upColor0 = c0;
            this.upColor1 = c1;
            this.midColor0 = c0;
            this.midColor1 = c1;
            this.downColor0 = c0;
            this.downColor1 = c1;
        }

        public ColorHelper(Color c0, Color c1, Color c2, Color c3)
        {
            this.upColor0 = c0;
            this.upColor1 = c1;
            this.midColor0 = c2;
            this.midColor1 = c3;
            this.downColor0 = c2;
            this.downColor1 = c3;
        }

        public ColorHelper(Color c0, Color c1, Color c2, Color c3, Color c4, Color c5)
        {
            this.upColor0 = c0;
            this.upColor1 = c1;
            this.midColor0 = c2;
            this.midColor1 = c3;
            this.downColor0 = c4;
            this.downColor1 = c5;
        }
    }
    public static Texture CreateWaterTexture(int width, int height)
    {
        ColorHelper colorHelper = new ColorHelper(newColor(88, 115, 174), newColor(10, 50, 125));
        return GeneratePoissonNoise(width, height, colorHelper);
    }
    public static Texture CreateWoodTexture(int width, int height)
    {
        ColorHelper colorHelper = new ColorHelper(newColor(44, 37, 27), newColor(222, 169, 101));
        return GeneratePoissonNoise(width, height, colorHelper);
    }
    public static Texture2D GeneratePoissonNoise(int mapWidth, int mapHeight, ColorHelper helper)
    {
        float[,] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, 0.9f);


        Texture2D texture = new Texture2D(mapWidth, mapHeight);
        texture.filterMode = FilterMode.Point;

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(y < mapHeight / 4)
                {
                    colourMap[y * mapWidth + x] = Color.Lerp(helper.downColor0, helper.downColor1, noiseMap[x, y]);
                }
                else if(y < mapHeight / 4 * 3)
                {
                    colourMap[y * mapWidth + x] = Color.Lerp(helper.midColor0, helper.midColor1, noiseMap[x, y]);
                }
                else
                {
                    colourMap[y * mapWidth + x] = Color.Lerp(helper.upColor0, helper.upColor1, noiseMap[x, y]);
                }
                
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colourMap);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        return texture;
    }
    void SaveTextureAsAsset(Texture2D texture, string filePath)
    {
        // 确保路径存在
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 将 Texture2D 转换为 PNG 数据
        byte[] textureBytes = texture.EncodeToPNG();

        // 将 PNG 文件保存到指定路径
        File.WriteAllBytes(filePath, textureBytes);

        // 刷新资产数据库，以便新文件被Unity检测到
        AssetDatabase.Refresh();
    }
    void Start()
    {
        //material.mainTexture = GeneratePoissonNoise(44, 37, 27, 222, 169, 101);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
