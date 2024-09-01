using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class TwoBattleGround : MonoBehaviour
{

    public Canvas uiCanvas; // 拖拽分配到场景中的 Canvas
    public GameObject healthBarPrefab; // 拖拽分配到 HealthBarPrefab 预制件

    private GameObject fighter1;
    private GameObject fighter2;

    private string folderPath = "Assets/Characters/RagdollPrefabs"; // 资源所在文件夹的路径
    // Start is called before the first frame update
    void Start()
    {
        fighter1 = SelectRandomCharacter(new Vector3(0, -18.5f, -16f), true);
        fighter2 = SelectRandomCharacter(new Vector3(30, -18.5f, -16f), false);

        SelectRandomGun();
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
            path = "Assets/Animations/FightAnimations/IdleAiming.anim";
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


        generatedAnimal.GetComponent<Fighter>().enabled = false;
        generatedAnimal.AddComponent<RealFighter>();
        generatedAnimal.GetComponent<RealFighter>().uiCanvas = uiCanvas;

        generatedAnimal.GetComponent<RealFighter>().healthBarPrefab = healthBarPrefab;
        generatedAnimal.GetComponent<RealFighter>().healthBarOffset = new Vector3(0,6,0);
        generatedAnimal.GetComponent<RealFighter>().TakeWeapon(SelectRandomGun());
        generatedAnimal.GetComponent<RealFighter>().positive = positive;
        float r = Random.Range(2, 4);
        selectedPrefab.transform.localScale = new Vector3(r, r, r);

        AnimatorController controller = new AnimatorController();
        controller.AddLayer("Base Layer");
        AnimatorState state = controller.layers[0].stateMachine.AddState("Default State");
        state.motion = RandomClip();
        generatedAnimal.GetComponent<Animator>().runtimeAnimatorController = controller;

        return generatedAnimal;
    }

    GameObject SelectRandomGun()
    {
        // 查找指定文件夹下的所有 Prefab
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Animations/Weapon" });

        if (guids.Length > 0)
        {
            // 随机选择一个 Prefab 文件
            int randomIndex = Random.Range(0, guids.Length);
            string path = AssetDatabase.GUIDToAssetPath(guids[randomIndex]);
            GameObject weapon = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return weapon;
        }
        return null;
    }

    int respawnCounter = 0;
    private void FixedUpdate()
    {
        Vector3 center = (fighter1.transform.position + fighter2.transform.position) * 0.5f ;
        Camera.main.transform.position = center + new Vector3(10, 10, -10);
        Camera.main.transform.LookAt(center);

        if(fighter1.GetComponent<RealFighter>().isDead() || fighter2.GetComponent<RealFighter>().isDead()) {
        
            respawnCounter++;
            if(respawnCounter > 200)
            {
                fighter1.GetComponent<RealFighter>().Kill();
                fighter2.GetComponent<RealFighter>().Kill() ;
                fighter1 = SelectRandomCharacter(new Vector3(0, -18.5f, -16f), true);
                fighter2 = SelectRandomCharacter(new Vector3(30, -18.5f, -16f), false);
                respawnCounter = 0;
            }
        }
    }
}
