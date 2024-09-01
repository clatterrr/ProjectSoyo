using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;

public class GoGoGo : MonoBehaviour
{
    public GameObject SpikeClubPrefab;
    public GameObject CannonPrefab;
    public GameObject StabPrefab;

    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件

    private GameObject gogog;

    private string folderPath = "Assets/Characters/RagdollPrefabs"; // 资源所在文件夹的路径
    // Start is called before the first frame update
    void Start()
    {
        /*
        for(int i = 0; i < 6; i++)
        {
            int r = Random.Range(1, 4);
            int d = i * 20;
            if (r == 1)
            {

                Instantiate(SpikeClubPrefab, new Vector3(d, 5, -15), Quaternion.identity);
            }
            else if(r == 2){

                Instantiate(StabPrefab, new Vector3(d, 0, -15), Quaternion.identity);
            }
            else
            {

                Instantiate(CannonPrefab, new Vector3(d, -5, -35), Quaternion.identity);
            }
            
        }
        */
        List<GameObject> list = new List<GameObject>
        {
            StabPrefab, CannonPrefab, SpikeClubPrefab, StabPrefab, CannonPrefab, SpikeClubPrefab
        };
        for (int i = 0; i < list.Count; i++)
        {
            int d = i * 20;
            if (list[i] == CannonPrefab)
            {

                Instantiate(CannonPrefab, new Vector3(d, -5, -35), Quaternion.identity);
            }else if (list[i] == StabPrefab)
            {
                Instantiate(StabPrefab, new Vector3(d, 0, -15), Quaternion.identity);
            }
            else
            {
                Instantiate(SpikeClubPrefab, new Vector3(d, 5, -15), Quaternion.identity);
            }

        }
        gogog = SelectRandomCharacter(new Vector3(-20, -4f, -15f), true);
    }

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
            path = "Assets/Animations/FightAnimations/CrouchedWalking.anim";
            // 加载 AnimationClip 并赋值
            clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }
        return clip;
    }
    GameObject SelectRandomCharacter(Vector3 pos, bool positive)
    {
        // 查找指定文件夹下的所有 Prefab
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });


        int randomIndex = Random.Range(0, guids.Length);
        string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
        GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject generatedAnimal = Instantiate(selectedPrefab, pos, Quaternion.identity);

        generatedAnimal.transform.rotation = Quaternion.Euler(0, 90, 0);
        generatedAnimal.GetComponent<Fighter>().enabled = false;
        generatedAnimal.AddComponent<Walker>();
        generatedAnimal.GetComponent<Walker>().uiCanvas = uiCanvas;
        generatedAnimal.GetComponent<Walker>().healthBarPrefab = healthBarPrefab;
        generatedAnimal.GetComponent<Walker>().healthBarOffset = new Vector3(0, 8, 0);
        generatedAnimal.GetComponent<Walker>().positive = positive;
        float r = Random.Range(4, 5);
        generatedAnimal.transform.localScale = new Vector3(r, r, r);

        AnimatorController controller = new AnimatorController();
        controller.AddLayer("Base Layer");
        AnimatorState state = controller.layers[0].stateMachine.AddState("Default State");
        state.motion = RandomClip();
        generatedAnimal.GetComponent<Animator>().runtimeAnimatorController = controller;

        return generatedAnimal;
    }

    int DeadCount = 0;
    private void FixedUpdate()
    {
        
        Camera.main.transform.position = gogog.transform.position + new Vector3(10, 5, 20);
        Camera.main.transform.LookAt(gogog.transform.position + new Vector3(0, 2, 0));
        if(gogog.GetComponent<Walker>().isDead() ) {
            DeadCount++;
            if(DeadCount > 200)
            {
                Destroy(gogog.GetComponent<Walker>());
                Destroy(gogog);
                DeadCount = 0;
                gogog = SelectRandomCharacter(new Vector3(-20, -4f, -15f), true);
            }
        }
        
    }

}
