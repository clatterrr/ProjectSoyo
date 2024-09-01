using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TreeEditor;
using UnityEditor;
using UnityEngine;

public class BenchCreateTerrain : MonoBehaviour
{
    // Start is called before the first frame update
    private Material grass_material;
    private Material stone_material;

    public GameObject blockPrefab;

    public GameObject cubePrefab;  // 1x1x1的方块模型预制件
    public int terrainSize = 40;   // 地形的总大小（40x40）
    public int plainSize = 20;     // 平原的大小（20x20）
    public float hillHeight = 5f;  // 丘陵的最大高度
    public float noiseScale = 0.1f;  // 噪声缩放系数，值越小区域越大
    Texture2D LoadTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }

    Material AddMaterial(string name)
    {
        string path  = "Assets/Blocks/Textures/" + name + ".png";
        Texture2D grass_texture = LoadTexture(path);
        grass_texture.filterMode = FilterMode.Point;
        Material mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = grass_texture;
        return mat;
    }
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    void Start()
    {
        grass_material = AddMaterial("grass_carried");
        stone_material = AddMaterial("stone");


        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        gameObject.AddComponent<MeshRenderer>();

        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        int halfTerrain = terrainSize / 2;
        int halfPlain = plainSize / 2;

        CombineInstance[] combine = new CombineInstance[terrainSize * terrainSize * 5];
        int index = 0;

        for (int x = -halfTerrain; x < halfTerrain; x++)
        {
            for (int z = -halfTerrain; z < halfTerrain; z++)
            {
                bool isInPlain = Mathf.Abs(x) <= halfPlain && Mathf.Abs(z) <= halfPlain;
                if (isInPlain) continue;

                float xCoord = (float)x / terrainSize * noiseScale;
                float zCoord = (float)z / terrainSize * noiseScale;
                float noiseValue = Mathf.PerlinNoise(xCoord, zCoord);

                int height = isInPlain ? 1 : Mathf.FloorToInt(Mathf.Lerp(1, 5, noiseValue));

                for (int y = 1; y < height; y++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);

                    combine[index].mesh = cube.GetComponent<MeshFilter>().sharedMesh;
                    combine[index].transform = cube.transform.localToWorldMatrix;
                    index++;

                    //Destroy(cube);
                }
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);
        meshFilter.mesh = combinedMesh;
        meshCollider.sharedMesh = combinedMesh;
    }
}

