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

规则一：我当然知道这一部分的在讲什么，比如Niches，Niches 部分是最重要的

规则二：解释的的全部去掉

3. 只保留 n. verb. n.      n. is n.   n. is for n.

4. 要可以嵌套 Recap Big Channel Crush You，第一层是 xxx Crush You， xxx 嵌套第二层 Recap Big Channel 

5. 要可以修饰，前修饰和后修饰

6. 修饰词也要能嵌套

4. he give a book to me 分解为 he give book. book is for me.

5. 


    ItIsImportant           this is without a doubt the most important thing when it comes to going viral on YouTube
    PartGood                 you can have the greatest script and editing ever
    KeyBad                  but if you have a bad topic
    AllBad                  no one will give a about your video
    ExamplePartGood         I posted this video a couple of weeks ago great script and editing
    ExamplePartBad          but barely got any views compared to my other videos
    Explained               simply because it was a bad topic
    ViewerBad               no one in my audience cared about it
    Example                 so spend time finding good topics sometimes you'll see a trash video getting hundreds of thousands of views
    Explained               only because it was a good topic that interested a lot of people
    PleaseDo                go search up your Niche on YouTube and see what's going viral
    IActually               lately I've seen that videos about YouTube automation perform really well so I'll pick that as my topic
    Theory                  now remember you should look at what performs well for others but also what performs well for your specific Channel
	
	
	DoThings: Search Viral Niches
	DoThingsAbstract: Finding Good Topic
	Tricks
	Why: 1. abstract: ItIsImportant
			detailed: people are interested in good topic, no one cares bad topic
			keyBadAllBad
			IDoKeyBadAllBad
			DoThingsAbstractIsGood


*/

    /*
 https://youtu.be/yqQuSPOua-0?t=21
Days 1  EnemyChaseMe IRunAway IHavePower ICraftTools IamHungry IEatFarms VilagerStopMe VillagerTalkToMe ZombieAttackVillager IkillZombie VillagerSaveMe
 
 
*/
}
