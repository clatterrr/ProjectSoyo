using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureEditor : MonoBehaviour
{
    public Texture2D texture; // 要读取的纹理
    public Material material;
    public GameObject part;

    void OutputTargetTexture(int targetWidth, int targetHeight, int sourceStartX, int sourceStartY, 
            int sourceWidth, int sourceHeight, int featureXStart, int featureXEnd, int featureYStart, int featureYEnd)
    {
        Color[,] targetColors  = new Color[targetHeight, targetWidth]; 
        string outputFilePath = "Assets/ModifiedTexture.png";
        Texture2D modifiedTexture = new Texture2D(targetHeight, targetWidth);

        int CopyY = 0;
        for(int i = 0; i < targetHeight; i++)
        {
            if (targetHeight >= sourceHeight)
            {
                int yOffset = (targetHeight - sourceHeight) / 2;
                if (i < yOffset)
                {
                    CopyY = 0;
                }
                else if (i < sourceHeight + yOffset)
                {
                    CopyY = i - yOffset;
                }
                else
                {
                    CopyY = sourceHeight - 1;
                }
            }
            else
            {
                int featureHeight = featureYEnd - featureYStart;
                int yOffset = (sourceHeight - featureHeight) / 2;
                if (i == 0)
                {
                    CopyY = 0;
                }
                else if (i == targetHeight - 1)
                {
                    CopyY = sourceWidth - 1;
                }
                else if (i < yOffset)
                {
                    CopyY = i;
                }
                else if (i >= yOffset + featureHeight)
                {
                    CopyY = i;
                }
                else
                {
                    CopyY = i - (featureHeight - featureYStart);
                }
            }

            for (int j = 0; j < targetWidth; j++)
            {

                int CopyX = j;
                if (targetWidth >= sourceWidth)
                {
                    int xOffset = (targetWidth - sourceWidth) / 2;
                    if (j < xOffset)
                    {
                        CopyX = 0;
                    }
                    else if (j < sourceWidth + xOffset)
                    {
                        CopyX = j - xOffset;
                    }
                    else
                    {
                        CopyX = sourceWidth - 1;
                    }
                }
                else
                {
                    int featureWidth = featureXEnd - featureXStart;
                    int xOffset = (sourceWidth - featureWidth) / 2;
                    if(j == 0)
                    {
                        CopyX = 0;
                    }else if(j == targetWidth - 1)
                    {
                        CopyX = sourceWidth - 1;
                    }else if(j < xOffset)
                    {
                        CopyX = j;
                    }else if(j >= xOffset + featureWidth)
                    {
                        CopyX = j;
                    }
                    else
                    {
                        CopyX = j - (featureWidth - featureXStart);
                    }
                }
                //targetColors[i, j] = texture.GetPixel(CopyX, CopyY);
                Debug.Log(" i = " + i + " j = " + j);
                Debug.Log("CopyX = " + CopyX + " CopY" + CopyY);
                Debug.Log("Final " + (sourceStartX + CopyX) + " Y = " + (sourceStartY + CopyY));
                Debug.Log("RGBA = " + texture.GetPixel(sourceStartX + CopyX, sourceStartY + CopyY));
                modifiedTexture.SetPixel(i, j, texture.GetPixel(sourceStartX + CopyX, sourceStartY + CopyY));
            }
        }

        
       // modifiedTexture.SetPixels(targetColors.ToArray()); // 复制原始纹理的所有像素

        // 修改第一列的颜色为红色
        for (int y = texture.height - 1; y >= texture.height - 8; y--)
        {
            // 将第一列的颜色设为红色
            //modifiedTexture.SetPixel(8, y, Color.red);
        }
        modifiedTexture.filterMode = FilterMode.Point;
        // 应用修改到纹理
        modifiedTexture.Apply();
        material.mainTexture = modifiedTexture;

        // 将修改后的纹理保存为 PNG 文件
        SaveTextureAsPNG(modifiedTexture, outputFilePath);
    }

    void Start()
    {
        // 0.25 就是4
        // Body 就是 8， 12， 4，也就是 0.5, 0.75, 0.25 
        Debug.Log(part.GetComponent<MeshRenderer>().bounds.size.ToString("f6"));
        if (texture == null)
        {
            Debug.LogError("请分配一个纹理！");
            return;
        }
        // 创建纹理的副本，避免修改原始纹理

        OutputTargetTexture(10, 6, 8, 64 - 16, 8, 8, 1, 1, 1, 1);
        /*
        // 遍历纹理的每个像素
        for (int y = texture.height - 9; y >= texture.height - 16; y--)
        {
            // 获取该行的第一个像素作为基准
            Color baseColor = texture.GetPixel(8, y);
            Debug.Log($"第 {y} 行的基准颜色: {baseColor}");

            int featureStart = 0;
            int featureEnd = 0;
            bool featured = false;
            // 遍历该行的其他像素
            for (int x = 9; x < 16; x++)
            {
                // 获取当前像素颜色
                Color currentColor = texture.GetPixel(x, y);

                // 计算当前像素颜色和基准颜色的 RGBA 比例
                Vector4 baseRGBA = new Vector4(baseColor.r, baseColor.g, baseColor.b, baseColor.a);
                Vector4 currentRGBA = new Vector4(currentColor.r, currentColor.g, currentColor.b, currentColor.a);

                Vector4 ratio;
                // 避免除以零，使用容差
                float epsilon = 0.0001f;
                ratio.x = (Mathf.Abs(baseRGBA.x) > epsilon) ? currentRGBA.x / baseRGBA.x : 0;
                ratio.y = (Mathf.Abs(baseRGBA.y) > epsilon) ? currentRGBA.y / baseRGBA.y : 0;
                ratio.z = (Mathf.Abs(baseRGBA.z) > epsilon) ? currentRGBA.z / baseRGBA.z : 0;
                ratio.w = (Mathf.Abs(baseRGBA.w) > epsilon) ? currentRGBA.w / baseRGBA.w : 0;

                float score = Mathf.Abs(ratio.x) + Mathf.Abs(ratio.y) + Mathf.Abs(ratio.z);

                if(featured == false && score > 1)
                {
                    featured = true;
                    featureStart = x;

                }else if(featured == true && score < 1)
                {
                    featureEnd = x;
                    break;
                }

                // 输出该像素的坐标及与基准颜色的差异倍数
                Debug.Log($"坐标: ({x}, {y}), 颜色: {currentColor}, 相对于基准颜色的倍数: {ratio}");
            }
        }
        */
    }

    void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        // 将纹理编码为 PNG 数据
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            // 将 PNG 数据写入文件
            File.WriteAllBytes(filePath, pngData);
            Debug.Log($"纹理已保存到: {filePath}");
        }
        else
        {
            Debug.LogError("无法将纹理编码为 PNG 格式");
        }
    }
}
