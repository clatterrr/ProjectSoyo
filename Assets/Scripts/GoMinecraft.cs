using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using System.IO;
using static UnityEditor.SceneView;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.TextCore.Text;
using static GoMinecraft;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Text;
using static Structure;

public class GoMinecraft : MonoBehaviour
{
    public GameObject SpikeClubPrefab;
    public GameObject CannonPrefab;
    public GameObject StabPrefab;
    public GameObject RotateClubPrefab;
    public Material material;

    public GameObject ArrowPrefab;
    public GameObject EyePrefab;
    public Transform WalkPoint;

    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件

    private List<GameObject> heros;

    private string folderPath = "Assets/Characters/MinecraftCharacters"; // 资源所在文件夹的路径
    // Start is called before the first frame update

    private int[] charactersSelectArray;
    public int ActivePointer;

    public GameObject mapGuided;



    private List<CameraSetting> cameraSettings;
    private List<ActorSettings> actorSettings;
    private List<ObjectSettings> objectSettings;

    GameObject arrow;
    private List<GameObject> weapons;
    private int GlobalFrameCount = 0;

    
    void Start()
    {
        startTime = Time.time;
        ActivePointer = 0;
        charactersSelectArray = GenerateAndShuffleArray(6);
        heros = new List<GameObject>();
        cameraSettings = new List<CameraSetting>();
        actorSettings = new List<ActorSettings>();
        objectSettings = new List<ObjectSettings> { };
        weapons = new List<GameObject> { };

        GenerateWeapon(0);

        Vector3 center = new Vector3(-20, -4f, -15f);
        arrow = Instantiate(ArrowPrefab, center + new Vector3(0, 1, 0), Quaternion.identity);
        
        GenerateAllCharacters();
        NextArrow(300);
    }

    void GenerateWeapon(int frame)
    {
        
        for (int i = 0; i < weapons.Count; i++)
        {
            Destroy(weapons[i]);
        }
        weapons.Clear();
        int d = 20;
        for (int i = 0; i < 16; i++)
        {
            d = d + (20 - i);
            int dc = Random.Range(0, 4);
            if (dc == 0)
            {
                weapons.Add(Instantiate(CannonPrefab, new Vector3(d, -5, -35), Quaternion.identity));
            }
            else if (dc == 1)
            {
                weapons.Add(Instantiate(StabPrefab, new Vector3(d, 0, -15), Quaternion.identity));
            }
            else if (dc == 2)
            {
                weapons.Add(Instantiate(SpikeClubPrefab, new Vector3(d, 5, -15), Quaternion.identity));
            }
            else if (dc == 3)
            {
                weapons.Add(Instantiate(RotateClubPrefab, new Vector3(d, 0, 0), Quaternion.identity));
            }
        }
        objectSettings.Add(new ObjectSettings(frame, frame + 300, mapGuided, new Vector3(250, -4, -15), new Vector3(0, -4, -15), Quaternion.identity, Quaternion.identity));
        cameraSettings.Add(new CameraSetting(frame, frame + 300, new Vector3(30, 30, 30), mapGuided, new Vector3(0, 0, 0),false));

    }
    void GenerateAllCharacters()
    {
        Vector3 center = new Vector3(-20, -4f, -15f);
        float radius = 10f;
        int numObjects = 6;
        for(int i = 0; i < heros.Count; i++)
        {
            heros[i].GetComponent<MinecraftFighter>().SelfKill();
        }
        heros.Clear();
        for (int i = 0; i < numObjects; i++)
        {
            // 计算每个物体在圆上的角度
            float angle = i * Mathf.PI * 2f / numObjects;

            // 根据角度计算物体的位置
            float x = Mathf.Cos(angle) * radius + center.x;
            float z = Mathf.Sin(angle) * radius + center.z;
            Vector3 position = new Vector3(x, center.y, z);

            // 生成物体
            GameObject go = SelectRandomCharacter(position, true);
            go.transform.LookAt(center);
            heros.Add(go);
        }
      }

    int playDead = 150;
    void NextArrow(int frameCount)
    {

        int realIndex = charactersSelectArray[ActivePointer];
        arrow.transform.localScale = new Vector3(10, 10, 10);
        objectSettings.Add(new ObjectSettings(frameCount, frameCount + 50, arrow, arrow.transform.position, arrow.transform.position, arrow.transform.rotation, arrow.transform.rotation));
        objectSettings.Add(new ObjectSettings(frameCount + 50, frameCount + 100, arrow, arrow.transform.position, arrow.transform.position, arrow.transform.rotation, Quaternion.Euler(0, - 60 * realIndex - 180, 0)));
        cameraSettings.Add(new CameraSetting(frameCount, frameCount + 100, new Vector3(10, 10, 10), arrow, new Vector3(0, 0, 0), false));


        Record(frameCount + 100);
        actorSettings.Add(new ActorSettings(frameCount + 100, frameCount + 200, heros[realIndex], MinecraftFighter.Animation.Superise, heros[realIndex].transform.position, heros[realIndex].transform.rotation, true));
        float r = Random.Range(-7, 10);
        Vector3 p = WalkPoint.transform.position + new Vector3(0, 0, r);
        actorSettings.Add(new ActorSettings(frameCount + 200, frameCount + 20000, heros[realIndex], MinecraftFighter.Animation.Walk, p, Quaternion.Euler(0, 90, 0), true));
        cameraSettings.Add(new CameraSetting(frameCount + 200, frameCount + 20000, new Vector3(20, 10, 20), heros[realIndex], new Vector3(0, 0, 0), true));
    
    }
    private void SelectRandomTexture(Transform parent)
    {
        // 获取文件夹中所有纹理文件的路径
        string[] textureFiles = Directory.GetFiles("Assets\\Characters\\MinecraftCharacters\\Textures", "*.png"); // 可以扩展为支持更多格式

        if (textureFiles.Length == 0)
        {
            Debug.LogError("指定文件夹中没有找到任何纹理文件！");
            return;
        }

        // 随机选择一个纹理文件
        string selectedFile = textureFiles[Random.Range(0, textureFiles.Length)];
        // 加载纹理
        Texture2D texture = LoadTexture(selectedFile);
        texture.filterMode = FilterMode.Point;

        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.mainTexture = texture;
        AssignMaterialToChildren(parent, newMaterial);
    }

    void AssignMaterialToChildren(Transform parent, Material material)
    {
        // 获取当前物体的 MeshRenderer 组件
        MeshRenderer meshRenderer = parent.GetComponent<MeshRenderer>();

        // 如果当前物体有 MeshRenderer 组件，就为它赋值材质
        if (meshRenderer != null)
        {
            meshRenderer.material = material;
        }

        // 递归遍历所有子物体
        foreach (Transform child in parent)
        {
            AssignMaterialToChildren(child, material);
        }
    }
    GameObject SelectRandomCharacter(Vector3 pos, bool positive)
    {
        // 查找指定文件夹下的所有 Prefab
        string[] guids = Directory.GetFiles("Assets\\Characters\\Rainbow\\Prefab", "*.prefab");
        int r = Random.Range(0, guids.Length);
        string path = guids[r];
        //Debug.Log(path);
        //path = "Assets/Characters/Rainbow/Prefab/blue.prefab";
        GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject generatedAnimal = Instantiate(selectedPrefab, pos, Quaternion.identity);
        //SelectRandomTexture(generatedAnimal.transform);

        generatedAnimal.AddComponent<MinecraftFighter>();
        generatedAnimal.GetComponent<MinecraftFighter>().animationMode = MinecraftFighter.AnimationMode.Human;
        generatedAnimal.GetComponent<MinecraftFighter>().eyes = EyePrefab;
        generatedAnimal.GetComponent<MinecraftFighter>().uiCanvas = uiCanvas;
        generatedAnimal.GetComponent<MinecraftFighter>().healthBarOffset = new Vector3(0,8,0);
        generatedAnimal.GetComponent<MinecraftFighter>().healthBarPrefab = healthBarPrefab;
        generatedAnimal.GetComponent<MinecraftFighter>().SetAnimation(MinecraftFighter.Animation.Wait);

        generatedAnimal.AddComponent<Animator>();
        generatedAnimal.GetComponent<Animator>().applyRootMotion = true;

        return generatedAnimal;
    }

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

    public float shakeSpeed = 5.0f;  // 控制抖动速度
    public float shakeIntensity = 5f;  // 控制抖动强度

    int NoEditFrameCount = 0;
    int ChangeDeadCount = 0;

    private StringBuilder descriptionBuilder = new StringBuilder();
    private float startTime;

    void Record(int count)
    {
            float elapsedTime = Time.time - startTime;
            File.AppendAllText("D://des.txt", $"{count * 0.02f}\n");
    }
    private void FixedUpdate()
    {
        NoEditFrameCount++;
        GlobalFrameCount++;
        for (int i = 0; i < objectSettings.Count; i++)
        {
            if (GlobalFrameCount >= objectSettings[i].frameStart && GlobalFrameCount < objectSettings[i].frameEnd)
            {
                
                float ration = (GlobalFrameCount - objectSettings[i].frameStart) * 1.0f / (objectSettings[i].frameEnd - objectSettings[i].frameStart);
                objectSettings[i].thing.transform.position = Vector3.Lerp(objectSettings[i].posStart, objectSettings[i].posEnd, ration);
                objectSettings[i].thing.transform.rotation = Quaternion.Lerp(objectSettings[i].rotationStart, objectSettings[i].rotationEnd, ration);
            }
        }
        for (int i = 0; i < cameraSettings.Count; i++)
        {
            if(GlobalFrameCount >= cameraSettings[i].frameStart && GlobalFrameCount < cameraSettings[i].frameEnd)
            {
              //  Debug.Log("Check " + i + " start " + cameraSettings[i].frameStart + " end " + cameraSettings[i].frameEnd);
                if (cameraSettings[i].detectDead)
                {
                    if (cameraSettings[i].lookat != null && cameraSettings[i].lookat.GetComponent<MinecraftFighter>() != null)
                    {

                     //   Debug.Log("Check " + i + " detect dead " + cameraSettings[i].lookat.GetComponent<MinecraftFighter>().isAlive());
                        if (cameraSettings[i].lookat.GetComponent<MinecraftFighter>().isAlive())
                        {
                            Camera.main.transform.position = cameraSettings[i].posOffsetStart + cameraSettings[i].lookat.transform.position;
                            Camera.main.transform.LookAt(cameraSettings[i].lookat.transform.position);
                            break;
                        }
                    }
                }
                else
                {
                    Camera.main.transform.position = cameraSettings[i].posOffsetStart + cameraSettings[i].lookat.transform.position;
                    Camera.main.transform.LookAt(cameraSettings[i].lookat.transform.position);
                    break;
                }
            }
        }
        for (int i = 0; i < actorSettings.Count; i++)
        {
            if (GlobalFrameCount == actorSettings[i].frameStart)
            {
               // actorSettings[i].actor.GetComponent<MinecraftFighter>().SetAnimation(actorSettings[i].animation);

                actorSettings[i].actor.GetComponent<MinecraftFighter>().SetTransform(actorSettings[i].posStart, actorSettings[i].rotationStart);
            }
        }


        int realIndex = charactersSelectArray[ActivePointer];
        if (heros[realIndex].GetComponent<MinecraftFighter>().isDead())
        {
            if(ChangeDeadCount == 0)
            {
                cameraSettings.Add(new CameraSetting(GlobalFrameCount, GlobalFrameCount + playDead, new Vector3(20, 10, 20), heros[realIndex], new Vector3(0, 0, 0), false));
            }
            ChangeDeadCount++;
            if(ChangeDeadCount == playDead)
            {
                ActivePointer++;
                if(ActivePointer >= 6)
                {
                    ActivePointer = 0;
                    charactersSelectArray = GenerateAndShuffleArray(6);
                    GenerateAllCharacters();
                    GenerateWeapon(GlobalFrameCount + 50);
                    NextArrow(GlobalFrameCount + 300 + 50);
                }
                else
                {

                    NextArrow(GlobalFrameCount);
                }
            }
        }
        else
        {
            ChangeDeadCount = 0;
        }
    }
    int[] GenerateAndShuffleArray(int length)
    {
        // 生成包含 0 到 length-1 的数组
        int[] array = new int[length];
        for (int i = 0; i < length; i++)
        {
            array[i] = i;
        }

        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            // 交换元素
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }

        return array;
    }

}

