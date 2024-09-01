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
            // ���ѡ��һ�� .anim �ļ�
            int randomIndex = Random.Range(0, guids.Length);
            string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
            // ���� AnimationClip ����ֵ
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }
        return clip;
    }

    private string folderPath = "Assets/Characters/RagdollPrefabs"; // ��Դ�����ļ��е�·��
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 respawnPos = transform.position + new Vector3((i - 4) * 16, 0, 15);
            Instantiate(SpikePrefab, respawnPos, Quaternion.Euler(-90, 0, 0));

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });




            if (guids.Length > 0)
            {
                // ���ѡ��һ�� Prefab �ļ�
                int randomIndex = Random.Range(0, guids.Length);
                string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
                GameObject walkingGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                GameObject realGameObject =  Instantiate(walkingGameObject, respawnPos, Quaternion.Euler(0, 180, 0));
                if (realGameObject.GetComponent<Fighter>() != null)
                {
                    Destroy(realGameObject.GetComponent<Fighter>());
                }
                realGameObject.transform.localScale = Vector3.one * 8f;

                // ����һ���յ� AnimatorController
                AnimatorController controller = new AnimatorController();
                controller.AddLayer("Base Layer");

                // ����һ���յ� Animation State ������ΪĬ��״̬
                AnimatorState state = controller.layers[0].stateMachine.AddState("Default State");
                state.motion = RandomClip();

                // �� AnimatorController ����Ϊ Animator �� Controller
                realGameObject.GetComponent<Animator>().runtimeAnimatorController = controller;

            }
        }
        SelectRandomCharacter();
    }

    private GameObject selectedPrefab;
    private GameObject generatedAnimal;
    public GameObject healthBarPrefab; // ��ק���䵽 HealthBarPrefab Ԥ�Ƽ�
    public Canvas uiCanvas; // ��ק���䵽�����е� Canvas
    void SelectRandomCharacter()
    {
        // ����ָ���ļ����µ����� Prefab
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });


        if (guids.Length > 0)
        {
            // ���ѡ��һ�� Prefab �ļ�
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
