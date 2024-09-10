using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Structure;

public class TextureEditor
{
    public Texture2D texture; // Ҫ��ȡ������
    public Material material;
    public GameObject part;
    public static Vector3 FindChildBounds(GameObject go, string target_name)
    {
        // �����ǰ GameObject ����ƥ��Ŀ������
        if (go.name == target_name)
        {
            Bounds combinedBounds = new Bounds();
            bool hasBounds = false;

            // ��������ֱ��������
            foreach (Transform child in go.transform)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // ����ǵ�һ�������壬��ʼ�� combinedBounds
                    if (!hasBounds)
                    {
                        combinedBounds = renderer.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        // ��չ combinedBounds �԰�����ǰ������� bounds
                        combinedBounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            // ��������������� bounds.size
            if (hasBounds)
            {
                return combinedBounds.size;
            }
            return Vector3.zero;
        }

        // ������ֲ�ƥ�䣬�ݹ����������
        foreach (Transform child in go.transform)
        {
            Vector3 boundsSize = FindChildBounds(child.gameObject, target_name);
            // ����ҵ�ƥ��������壬�������� bounds.size
            if (boundsSize != Vector3.zero)
            {
                return boundsSize;
            }
        }

        // ���û���ҵ�Ŀ�����Ƶ����壬���� Vector3.zero
        return Vector3.zero;
    }

    public static Vector2 FindChildUVStart(GameObject go, string target_name)
    {
        // �����ǰ GameObject ����ƥ��Ŀ������
        if (go.name == target_name)
        {
            Mesh mesh = go.GetComponentInChildren<MeshFilter>().sharedMesh;
            float uvxmin = float.MaxValue;
            float uvymin = float.MaxValue;
            for (int i = 0; i < mesh.uv.Length; i++)
            {
                if (uvxmin > mesh.uv[i].x)
                {
                    uvxmin = mesh.uv[i].x;
                }
                if (uvymin > mesh.uv[i].y)
                {
                    uvymin = mesh.uv[i].y;
                }
            }

            return new Vector2(uvxmin, uvymin);
        }

        // ������ֲ�ƥ�䣬�ݹ����������
        foreach (Transform child in go.transform)
        {
            Vector2 uv =  FindChildUVStart(child.gameObject, target_name);
            if(uv.x < float.MaxValue && uv.y < float.MaxValue)
            {
                return uv;
            }
        }
        return new Vector2(float.MaxValue, float.MaxValue);
    }
    public static void ComputeFeatureAndResize(Texture2D texture0, Texture2D texture1, Uint2 start0, Uint2 start1, Uint2 size0, Uint2 size1)
    {
        Color[,] colors = OutputTargetTexture(texture0, (int)size1.x, (int)size1.y, (int)start0.x, (int)start0.y, (int)size0.x, (int)size0.y, 0,0,0,0);
        for(int i = 0; i < size1.x; i++)
        {
            for(int j = 0; j < size1.y; j++)
            {
                texture1.SetPixel((int)(start1.x + i), (int)(start1.y + j), colors[j, i]);
            }
        }
        texture1.Apply();
    }



    public static Material ExpectMaterial(Texture2D sourceTexture, GameObject sourceModel, 
        HumanoidGenerator.BodyPartName bodyPartName, Uint3 size)
    {
        Material material = new Material(Shader.Find("Standard"));
        Texture2D texture = new Texture2D((int)(size.x * 2 + size.z * 2), (int)(size.y + size.z));
        Uint2 textureSize = new Uint2(texture.width, texture.height);
        texture.filterMode = FilterMode.Point;

        string bodyPartNameString = "";
        switch (bodyPartName)
        {
            case HumanoidGenerator.BodyPartName.Body: bodyPartNameString = "body"; break;
            case HumanoidGenerator.BodyPartName.Head: bodyPartNameString = "head"; break;
            case HumanoidGenerator.BodyPartName.LeftArm: bodyPartNameString = "left_arm"; break;
            case HumanoidGenerator.BodyPartName.RightArm: bodyPartNameString = "right_arm"; break;
            case HumanoidGenerator.BodyPartName.LeftLeg: bodyPartNameString = "left_leg"; break;
            case HumanoidGenerator.BodyPartName.RightLeg: bodyPartNameString = "right_leg"; break;
            default: break;
        }

        Vector2 sourceStartFloat = FindChildUVStart(sourceModel, bodyPartNameString);
        Uint2 sourceStart = new Uint2((uint)(sourceStartFloat.x * sourceTexture.width), (uint)(sourceStartFloat.y * sourceTexture.height));
        Vector3 sourceSizeFloat = FindChildBounds(sourceModel, bodyPartNameString);
        Uint3 sourceSize = new Uint3((uint)(sourceSizeFloat.x * 16.01), (uint)(sourceSizeFloat.y * 16.01), (uint)(sourceSizeFloat.z * 16.01));

        Uint2 start0;
        Uint2 start1;
        Uint2 size0;
        Uint2 size1;

        /*
              _____ __x__
             |     |     |
             |     z     |
             |     |     |
        _z___ __x__ ____ __x__
       |     |     |    |     |
       |     y     y    y     |
       |     |     |    |     |
        _____ _____ __z_ _____

        */

        //front
        start0 = sourceStart + new Uint2(sourceSize.z, 0);
        start1 = new Uint2(size.z, 0);
        size0 = new Uint2(sourceSize.x, sourceSize.y);
        size1 = new Uint2(size.x, size.y);
     ComputeFeatureAndResize(sourceTexture, texture, start0, start1, size0, size1);

        

        //back
        start0 = sourceStart + new Uint2(sourceSize.z * 2 + sourceSize.x, 0);
        start1 = new Uint2(size.z * 2 + size.x, 0);
        size0 = new Uint2(sourceSize.x, sourceSize.y);
        size1 = new Uint2(size.x, size.y);
     ComputeFeatureAndResize(sourceTexture, texture, start0, start1, size0, size1);

        //left
        start0 = sourceStart;
        start1 = new Uint2(0, 0);
        size0 = new Uint2(sourceSize.z, sourceSize.y);
        size1 = new Uint2(size.z, size.y);
      ComputeFeatureAndResize(sourceTexture, texture, start0, start1, size0, size1);

        //back
        start0 = sourceStart + new Uint2(sourceSize.z + sourceSize.x, 0);
        start1 = new Uint2(size.z + size.x, 0);
        size0 = new Uint2(sourceSize.z, sourceSize.y);
        size1 = new Uint2(size.z, size.y);
   ComputeFeatureAndResize(sourceTexture, texture, start0, start1, size0, size1);

        //up
        start0 = sourceStart + new Uint2(sourceSize.z, sourceSize.y);
        start1 = new Uint2(size.z, size.y);
        size0 = new Uint2(sourceSize.x, sourceSize.z);
        size1 = new Uint2(size.x, size.z);
      ComputeFeatureAndResize(sourceTexture, texture, start0, start1, size0, size1);

        //down
        start0 = sourceStart + new Uint2(sourceSize.z + sourceSize.x, sourceSize.y);
        start1 = new Uint2(size.z + size.x, size.y);
        size0 = new Uint2(sourceSize.x, sourceSize.z);
        size1 = new Uint2(size.x, size.z);
      ComputeFeatureAndResize(sourceTexture, texture, start0, start1, size0, size1);

      //  SaveTextureAsPNG(texture, "Assets/ModifiedTexture2.png");
        material.mainTexture = texture;
        
        return material;
    }

    public static Color[,] OutputTargetTexture(Texture2D SourceTexture, int targetWidth, int targetHeight, int sourceStartX, int sourceStartY, 
            int sourceWidth, int sourceHeight, int featureXStart, int featureXEnd, int featureYStart, int featureYEnd)
    {
        Color[,] targetColors  = new Color[targetWidth, targetHeight]; 
        string outputFilePath = "Assets/ModifiedTexture.png";
        Texture2D modifiedTexture = new Texture2D(targetWidth, targetHeight);

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
                    CopyY = sourceHeight - 1;
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
                targetColors[i, j] = SourceTexture.GetPixel(sourceStartX + CopyX, sourceStartY + CopyY);
             //  Debug.Log(" i = " + i + " j = " + j);
            //   Debug.Log("CopyX = " + CopyX + " CopY" + CopyY);
            //  Debug.Log("Final " + (sourceStartX + CopyX) + " Y = " + (sourceStartY + CopyY));
           //   Debug.Log("RGBA = " + SourceTexture.GetPixel(sourceStartX + CopyX, sourceStartY + CopyY));
                modifiedTexture.SetPixel(j, i, SourceTexture.GetPixel(sourceStartX + CopyX, sourceStartY + CopyY));
            }
        }

     
        modifiedTexture.Apply();
        // ���޸ĺ��������Ϊ PNG �ļ�
       // SaveTextureAsPNG(modifiedTexture, outputFilePath);

        return targetColors;
    }

    void StartS()
    {
        // 0.25 ����4
        // Body ���� 8�� 12�� 4��Ҳ���� 0.5, 0.75, 0.25 
        //Debug.Log(part.GetComponent<MeshRenderer>().bounds.size.ToString("f6"));
        // ��������ĸ����������޸�ԭʼ����

        //OutputTargetTexture(10, 6, 8, 64 - 16, 8, 8, 1, 1, 1, 1);
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

    public static void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        // ���������Ϊ PNG ����
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            // �� PNG ����д���ļ�
            File.WriteAllBytes(filePath, pngData);
            AssetDatabase.Refresh();
            Debug.Log($"�����ѱ��浽: {filePath}");
        }
        else
        {
            Debug.LogError("�޷����������Ϊ PNG ��ʽ");
        }
    }
}
