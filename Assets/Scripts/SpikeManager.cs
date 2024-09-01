using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class SpikeManager : MonoBehaviour
{
    public GameObject SpikePrefab;
    public int ShootTime = 20;

    AnimationClip RandomClip()
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { "Assets/Animations/FightAnimations" });
        Debug.Log(" animations clips length = " + guids.Length);
        AnimationClip clip = new AnimationClip();
        if (guids.Length > 0)
        {
            // 随机选择一个 .anim 文件
            int randomIndex = Random.Range(0, guids.Length);
            string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
            // 加载 AnimationClip 并赋值
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }
        return clip;
    }

    private string folderPath = "Assets/Characters/RagdollPrefabs"; // 资源所在文件夹的路径
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 respawnPos = transform.position + new Vector3((i - 4) * 16, 0, 15);
            Instantiate(SpikePrefab, respawnPos, Quaternion.Euler(-90, 0, 0));

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });




            if (guids.Length > 0)
            {
                // 随机选择一个 Prefab 文件
                int randomIndex = Random.Range(0, guids.Length);
                string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
                GameObject walkingGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                GameObject realGameObject =  Instantiate(walkingGameObject, respawnPos, Quaternion.Euler(0, 180, 0));
                if (realGameObject.GetComponent<Fighter>() != null)
                {
                    Destroy(realGameObject.GetComponent<Fighter>());
                }
                realGameObject.transform.localScale = Vector3.one * 8f;

                // 创建一个空的 AnimatorController
                AnimatorController controller = new AnimatorController();
                controller.AddLayer("Base Layer");

                // 创建一个空的 Animation State 并设置为默认状态
                AnimatorState state = controller.layers[0].stateMachine.AddState("Default State");
                state.motion = RandomClip();

                // 将 AnimatorController 设置为 Animator 的 Controller
                realGameObject.GetComponent<Animator>().runtimeAnimatorController = controller;

            }
        }
        SelectRandomCharacter();
    }

    private GameObject selectedPrefab;
    private GameObject generatedAnimal;
    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件
    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    void SelectRandomCharacter()
    {
        // 查找指定文件夹下的所有 Prefab
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });


        if (guids.Length > 0)
        {
            // 随机选择一个 Prefab 文件
            int randomIndex = Random.Range(0, guids.Length);
            string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
            selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            selectedPrefab.GetComponent<Fighter>().uiCanvas = uiCanvas;
            selectedPrefab.GetComponent<Fighter>().healthBarPrefab = healthBarPrefab;
            selectedPrefab.GetComponent<Fighter>().healthBarOffset = new Vector3(0, 4, 0);
            float r = Random.Range(2, 4);
            selectedPrefab.transform.localScale = new Vector3(r,r, r);
            selectedPrefab.GetComponent<Fighter>().moveSpeed = Random.Range(3, 7);
            generatedAnimal = Instantiate(selectedPrefab, new Vector3(0, -18.5f, -16f), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No Prefabs found in the folder: " + folderPath);
        }
    }
    int respawnCounter = 0;
    private void FixedUpdate()
    {
        if (generatedAnimal != null && !generatedAnimal.GetComponent<Fighter>().isDead())
        {
            Camera.main.transform.LookAt(generatedAnimal.transform.position + new Vector3(10, 2, 0));
            Camera.main.transform.position = generatedAnimal.transform.position + new Vector3(20, 5, -10);
        }
        else
        {
            respawnCounter += 1;
            if(respawnCounter > 200)
            {
                respawnCounter = 0;
                SelectRandomCharacter();
            }
        }
    }

}
