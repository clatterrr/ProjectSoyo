using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureEditor : MonoBehaviour
{
    public Texture2D texture; // Ҫ��ȡ������
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

        
       // modifiedTexture.SetPixels(targetColors.ToArray()); // ����ԭʼ�������������

        // �޸ĵ�һ�е���ɫΪ��ɫ
        for (int y = texture.height - 1; y >= texture.height - 8; y--)
        {
            // ����һ�е���ɫ��Ϊ��ɫ
            //modifiedTexture.SetPixel(8, y, Color.red);
        }
        modifiedTexture.filterMode = FilterMode.Point;
        // Ӧ���޸ĵ�����
        modifiedTexture.Apply();
        material.mainTexture = modifiedTexture;

        // ���޸ĺ��������Ϊ PNG �ļ�
        SaveTextureAsPNG(modifiedTexture, outputFilePath);
    }

    void Start()
    {
        // 0.25 ����4
        // Body ���� 8�� 12�� 4��Ҳ���� 0.5, 0.75, 0.25 
        Debug.Log(part.GetComponent<MeshRenderer>().bounds.size.ToString("f6"));
        if (texture == null)
        {
            Debug.LogError("�����һ������");
            return;
        }
        // ��������ĸ����������޸�ԭʼ����

        OutputTargetTexture(10, 6, 8, 64 - 16, 8, 8, 1, 1, 1, 1);
        /*
        // ���������ÿ������
        for (int y = texture.height - 9; y >= texture.height - 16; y--)
        {
            // ��ȡ���еĵ�һ��������Ϊ��׼
            Color baseColor = texture.GetPixel(8, y);
            Debug.Log($"�� {y} �еĻ�׼��ɫ: {baseColor}");

            int featureStart = 0;
            int featureEnd = 0;
            bool featured = false;
            // �������е���������
            for (int x = 9; x < 16; x++)
            {
                // ��ȡ��ǰ������ɫ
                Color currentColor = texture.GetPixel(x, y);

                // ���㵱ǰ������ɫ�ͻ�׼��ɫ�� RGBA ����
                Vector4 baseRGBA = new Vector4(baseColor.r, baseColor.g, baseColor.b, baseColor.a);
                Vector4 currentRGBA = new Vector4(currentColor.r, currentColor.g, currentColor.b, currentColor.a);

                Vector4 ratio;
                // ��������㣬ʹ���ݲ�
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

                // ��������ص����꼰���׼��ɫ�Ĳ��챶��
                Debug.Log($"����: ({x}, {y}), ��ɫ: {currentColor}, ����ڻ�׼��ɫ�ı���: {ratio}");
            }
        }
        */
    }

    void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        // ���������Ϊ PNG ����
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            // �� PNG ����д���ļ�
            File.WriteAllBytes(filePath, pngData);
            Debug.Log($"�����ѱ��浽: {filePath}");
        }
        else
        {
            Debug.LogError("�޷����������Ϊ PNG ��ʽ");
        }
    }
}
