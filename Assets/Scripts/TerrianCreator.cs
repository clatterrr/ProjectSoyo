using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using static Days100;

public class TerrianCreator : MonoBehaviour
{
    // Start is called before the first frame update
    private Material grass_material;
    private Material stone_material;


    private GameObject cubePrefab;  // 1x1x1的方块模型预制件
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
        cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Blocks/grass_block.prefab");
        grass_material = AddMaterial("grass_carried");
        stone_material = AddMaterial("stone");


        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        gameObject.AddComponent<MeshRenderer>();

        GenerateTerrain();
    }
    Dictionary<Vector2Int, int> terrainHeights = new Dictionary<Vector2Int, int>();
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
                terrainHeights[new Vector2Int(x, z)] = height;
                for (int y = 0; y < height; y++)
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
    public Dictionary<Vector2Int, int> GetHeights()
    {
        return terrainHeights;
    }

    public Vector3 CorretedHeights(SP sp)
    {
        Vector3 pos = GetSPPos(sp);
        Vector2Int posr = new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y + 0.5f));
        posr.x = Mathf.Max(-terrainSize/2, posr.x);
        posr.y = Mathf.Max(-terrainSize/2, posr.y);
        posr.x = Mathf.Min(terrainSize / 2 - 1, posr.x);
        posr.y = Mathf.Min(terrainSize / 2 - 1, posr.y);
        int y = terrainHeights[posr];
        return new Vector3(pos.x, y, pos.z);
    }

    public Vector3 CorretedHeights(Vector3 pos)
    {
        int y =  terrainHeights[new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y + 0.5f))];
        return new Vector3(pos.x, y, pos.z);
    }

    public Vector3 GetSPPos(Days100.SP sp)
    {
        Vector3 p = GetSPPos2(sp);
        p.y = terrainHeights[new Vector2Int((int)p.x, (int)p.z)];
        return p;
    }
    public Vector3 GetSPPos2(Days100.SP sp)
    {
        switch (sp)
        {
            case SP.WatchOverPlaceClose: return new Vector3(3, 0, 2);
            case SP.WatchOverPlaceFar: return new Vector3(9, 0, 2);
                // Talk 一般都是Talk 后打怪的
        //   case SP.TalkPlace0: return new Vector3(2, 0, 2);
         //   case SP.TalkPlace1: return new Vector3(3, 0, 2);


            case SP.ChaseStart: return new Vector3(3, 0, 2);
            case SP.ChaseEnd: return new Vector3(-3, 0, 2);
            case SP.RunAwayStart: return new Vector3(1, 0, 2);
            case SP.RunAwayEnd: return new Vector3(-5, 0, 2);
            default: return Vector3.zero;
        }
    }

    // 敌人永远从 z + 10 charge in, 并且


}

