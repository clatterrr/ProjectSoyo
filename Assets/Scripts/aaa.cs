using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D texture;
    public Shader shader;
    void Start()
    {
        for(int i = 0; i < texture.width; i++)
            for(int j = 0; j < texture.height; j++)
            {
                Debug.Log("i == " + i + " j == " + j + " color " + texture.GetPixel(i, j));
            }
    }

    /*
 https://youtu.be/yqQuSPOua-0?t=21
Days 1  EnemyChaseMe IRunAway IHavePower ICraftTools IamHungry IEatFarms VilagerStopMe VillagerTalkToMe ZombieAttackVillager IkillZombie VillagerSaveMe
 
 
*/
}
