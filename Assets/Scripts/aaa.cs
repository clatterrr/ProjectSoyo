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

    3. 只保留 n. verb. n.      n. is n.   n. is for n.    n. is adj.

    常见整句前缀 i noticed that, the reasons is that, to do / for that 

    允许生成式

    VoiceOver 和 Topics 和 Niches 都是从中选择一个

    choosing the wrong or right ____ is important

    4. 要可以嵌套 Recap Big Channel Crush You，第一层是 xxx Crush You， xxx 嵌套第二层 Recap Big Channel 

    5. 要可以修饰，前修饰和后修饰

    6. 修饰词也要能嵌套

    原理就是，我一定知道比这要更多的信息

    例如Open Website，那么后面一定接 FirstPage

    例如 IAttackEnemy, FriendHelpMe, 那么Friend 一定AttackEnemy

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

        一个名词，是什么
        更加详细的解释：一句话，20个词左右
        做得好，做得不好会怎样：很多个变种
        为什么，一句话 20 个词解释清楚

    假设我们有以下未标记的句子：

"Steve Jobs, the co-founder of Apple, was born in California."

在这个句子中，我们有两个命名实体："Steve Jobs"和"Apple"。根据文章中的方法，我们可以提取以下特征：

拼写特征：

full-string=Steve Jobs
contains(Jobs)
上下文特征：

context=co-founder
context-type=appos
使用这些特征，我们可以应用文章中提出的无监督学习方法来识别实体类型。例如，如果我们知道"co-founder"通常与"organization"（组织）相关联，我们可以推断"Apple"是一个组织。同样，由于"Steve Jobs"包含"Mr."（在其他上下文中学到的），我们可以推断"Steve Jobs"是一个人名。

    // https://www.sciencedirect.com/science/article/pii/S1319157820303712

        例如，对于包含 20 个句子的文本，第一个句子的重要性将比最后一个句子高 20 倍

     https://youtu.be/yqQuSPOua-0?t=21
    Days 1  EnemyChaseMe IRunAway IHavePower ICraftTools IamHungry IEatFarms VilagerStopMe VillagerTalkToMe ZombieAttackVillager IkillZombie VillagerSaveMe


    */
}
