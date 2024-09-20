using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Structure;
public class Days100: MonoBehaviour
{
    public struct RandomScene
    {
        public List<string> _spots;
        public List<Vector3> _pos;

        public RandomScene(int x)
        {
            this._spots = new List<string>();
            this._pos = new List<Vector3>();
        }

        public void add(string s, Vector3 p) {
            _spots.Add(s);
            _pos.Add(p);
        }

        public Vector3 find(string spot)
        {
            for(int i = 0; i < _spots.Count; i++)
            {
                if (_spots[i] == spot)
                {
                    return _pos[i];
                }
            }
            return Vector3.zero;
        }


    }

    // 动作分为 AttackFriends, AttackEnemy, AttackMe
    // 首先推入 FA, 检查有哪些人物不在FAs中的，全部设置为False,
    // 一开始就要设计好哪些人是哪些模型，对应的FA中的编号是什么
    // Scene 分为两种，首先是PreScene，用于随机产生故事。然后PreScene 来产生随机场景，主要是摄像机机位和人物行动
    public struct AnimDesc
    {
        public string animPath;
        public string animText;
    }
    public struct Segment
    {
        public List<SentenceGroup> groups;

        public Segment(int x)
        {
            groups = new List<SentenceGroup>();
        }

        public void run()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                int r = Random.Range(0, groups[i].sentence.Count - 1);
                string str = groups[i].sentence[r].content;
            }
        }
    }

    public struct SentenceGroup
    {
        public List<Sentence> sentence;

        public SentenceGroup(int x)
        {
            sentence = new List<Sentence>();
        }
    }
    public struct Sentence
    {
       
        public string content;
        public List<ActorSettings> actors;
        public List<CameraSetting> cameras;

        public Sentence(string content)
        {
            this.content = content;
            this.actors = new List<ActorSettings>();
            this.cameras = new List<CameraSetting>();
        }

        public void addActor(ActorSettings actor)
        {
            this.actors.Add(actor);
        }

        public void addCamera(CameraSetting camera)
        {
            this.cameras.Add(camera);
        }
    }

    public static void init()
    {
        RandomScene scene0 = new RandomScene(1);
        scene0.add("came_out_pos0", Vector3.zero);
        scene0.add("came_out_pos1", Vector3.zero);

        List<GameObject> actors = new List<GameObject>();
        int enemy_index = 0;
        Sentence i_came_out_0 = new Sentence("enemy came out from nowhere");
        ActorSettings actor = new ActorSettings(0, 0, actors[enemy_index], MinecraftFighter.Animation.Wait, scene0.find("came_out_pos1"), Quaternion.identity, true, null);
        i_came_out_0.addActor(actor);

        Sentence i_came_out_1 = new Sentence("enemy came out and charge to me");

        SentenceGroup i_came_out = new SentenceGroup(0);
        i_came_out.sentence.Add(i_came_out_0);
        i_came_out.sentence.Add(i_came_out_1);

        Segment segment = new Segment();
        segment.groups.Add(i_came_out);
    }

    enum PreScene
    {
        TalkRun,
        LookingActor,
        TalkHelp,
        OneBreakingCharingIn,
        Talk,
        Attack_Rush,
        Attack,

        Spawn,
        IHavePower,


        EnemyChaseMe,
        Self_GrowUp,

        Talk_EnemyThreaten,
        Talk_FriendsHappy,
        Talk_FriendThank,
        Talk_FriendsWeak,
        Talk_IMustHelpFriends,
        Talk_FriendWasTrapped,
        Talk_FriendEncourage,
        Talk_HelloToFriend,
        Talk_FriendSad,
        Talk_FuckEnmey,
        Talk_FriendMission,

        Travel_Traveling,
        Travel_Arrived,
        Travel_FindSpot,
        Travel_Confused,
        Travel_BackToHome,
        Travel_NoiseInDistance,
        Travel_SearchNoise,
        Travel_FoundNewFreind,
        Travel_DropMap,
        Travel_Sneak,
        Travel_FindTreasure,
        Travel_TakeTreasure,
        Travel_Search,

        Fight_IRunaway,
        Fight_FriendHelp,
        Fight_Attack,
        Fight_SuccessCheer,
        Fight_RunForTart,
        Fight_IWasHurt,
        Fight_Final,
        Fight_EnemyChase,
        Fight_DescribeEnemy,
        Fight_IThreatenBack,
        Fight_IBlockEnemy,
        Fight_EnemyChargeIn,

        Build_FindWool,
        Build_Bed,
    }

    struct PreSceneToScene
    {
        public PreScene psc;
        public List<SC> scs;

        public PreSceneToScene(PreScene psc, List<SC> scs)
        {
            this.psc = psc;
            this.scs = scs;
        }


    }
    // SC 名字特别长

    // 规定好粗顶点，必须包含所有人物
    enum SC
    {
        None,
        HeroEntrance,
        HeroTalkFriendInCage
    }

    enum CA
    {
        Idle, // look someone
        IdleRotationZ,
        ACtor0LookingActor1,
        Main,
        LookingMeRotate45,
        
    }

    enum AC
    {
        None,
        TalkCamera, // sometime not straight
        TalkingLoud, // head rotate large
        LookingActor,
        WalkToSpot,
        ChargeIn,
        Attack_Blast,
        Attack_DefendBackwards
    }

    enum CH
    {
        None,
        Friend,
        Enemy,
        I,
    }

    enum SP
    {
        None,
        HidenPlaace, // sometime not straight
        Cage, // head rotate large
        ChargeInStart,
        ChargeInEnd,
        EnemyAttack,
        EnemyDefend,
        IAttack,
        IDefend,
    }

    // characters
    struct FA
    {

        public CH ch0;
        public AC ac0;
        public SP sp00;
        public SP sp01;
    }

    struct PS
    {
        public string link;
        public PreScene psc;
        // 一个PreScene 可能包含多个可能的Scene
        public SC sc;
        public CA ca;
        public List<FA> fas;
        public List<string> contents;


        public PS(string link, PreScene sc, List<string> contents)
        {
            this.link = link;
            this.sc = SC.None;
            this.psc = sc;
            this.ca = CA.Idle;
            this.fas = new List<FA>();
            this.contents = contents;
        }
    }

    // 严格区分Enemy 和 I
    // 一句话中最多两个角色，

    enum ActorType
    {
        Hand,
        Player,
        Friend,
        Enemy,
    }


    private List<ActorSettings> actorSettings = new List<ActorSettings>();
    private List<CameraSetting> cameraSettings = new List<CameraSetting>();
    struct AllMyFellow
    {
        public List<ActorType> types;
        public List<GameObject> actors;
        public List<bool> actives;
        public AllMyFellow(int count)
        {
            this.types = new List<ActorType>();
            this.actors = new List<GameObject>();
            this.actives = new List<bool>();
        }

        public void Add(ActorType type, string prefab_path)
        {
            GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab_path);
            actives.Add(false);
            types.Add(type);
            actors.Add(Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        }

        public GameObject GetActor(ActorType type)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (types[i] == type)
                {
                    return actors[i];
                }
            }
            return null;
        }

        public void SetAllActiveFalse()
        {
            for (int i = 0; i < actives.Count; i++) actives[i] = false;
        }

        public void SetFalse(int startFrame, int endFrame, List<ActorSettings> actorSettings)
        {
            for (int i = 0; i < actives.Count; i++)
            {
                actorSettings.Add(addActorMove(startFrame, endFrame, actors[i], actives[i]));
            }
        }

    }


    void Start()
    {

        AllMyFellow fellow = new AllMyFellow(0);
        fellow.Add(ActorType.Friend, DataTransfer.messageToPass);

        List<PreSceneToScene> pscT = new List<PreSceneToScene>();
        pscT.Add(new PreSceneToScene(PreScene.Attack, new List<SC>() {SC.HeroEntrance }));

        // generate comments

        List<PS> scenes = new List<PS>();
        for(int scene_index = 0; scene_index < scenes.Count; scene_index++)
        {
            for(int i = 0; i < pscT.Count; i++)
                if(scenes[scene_index].psc == pscT[i].psc)
                {
                    PS p = scenes[scene_index];
                    int r = Random.Range(0, pscT[i].scs.Count);
                    p.sc = pscT[i].scs[r];
                    scenes[scene_index] = p;
                }

            fellow.SetAllActiveFalse();
            switch (scenes[scene_index].sc)
            {
                // Scene 就不应该包含Camera 信息，虽然也有要随机选择Camera
                case SC.HeroEntrance:
                    {
                        // 位置是Idle, 但具体哪儿Idle 呢？ 要根据场景设置吗？但这个场景设置快变成摄像机设置了
                        break;
                    }
            }
            fellow.SetFalse(0, 0, actorSettings);
        }
    }

    static void Prepare()
    {
        // 人物和角色动作不在这儿指定吗？
        // 至少要指定人物大致动作
        // 细致动作是WalkFast 或者WalkSlow
        // 粗任务是follow the map, 细动作可以是跑，走，或者停在那儿四处张望
        List<PS> pres = new List<PS>()
        {
            /*
            new PS("https://youtu.be/y2yRPKQ5nnE?t=115", PreScene.TalkRun, CA.Idle, CH.I, AC.TalkCamera, SP.HidenPlaace,
            "I got to find a way out of here"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=121", PreScene.LookingActor, CA.ACtor0LookingActor1, CH.I, AC.LookingActor, SP.HidenPlaace, CH.Friend, AC.LookingActor, SP.Cage,
            "I looked over to see a strange shark creature stuck in a cage"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=130", PreScene.TalkHelp, CA.Idle, CH.Friend, AC.TalkingLoud, SP.Cage,
            "I'm a shark dog duh oh come on let me out of here I can help you escape"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=131", PreScene.OneBreakingCharingIn, CA.ACtor0LookingActor1, CH.Enemy, AC.WalkToSpot, SP.ChargeInStart, SP.ChargeInEnd, CH.I, AC.LookingActor, SP.HidenPlaace,
            "the door slammed open and the _enemy found us"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=137", PreScene.Talk, CA.IdleRotationZ,CH.Enemy, AC.TalkingLoud, SP.ChargeInEnd,
            "oh no ah forget turnning this little run in for a prize"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=141", PreScene.Attack_Rush, CA.Main, CH.Enemy, AC.ChargeIn, SP.ChargeInEnd, SP.EnemyAttack, CH.I, AC.LookingActor, SP.IAttack,
            "_they rushed in"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=143", PreScene.Attack, CA.LookingMeRotate45, CH.I, AC.Attack_Blast, SP.IAttack,
            "but I blasted out a strange skull ink explosion"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=145", PreScene.Attack, CA.LookingMeRotate45, CH.Enemy, AC.Attack_DefendBackwards, SP.EnemyAttack, SP.EnemyDefend,
            "which blasted them back"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=150", PreScene.Attack, CA.Idle, CH.I, AC.TalkCamera, SP.IAttack,
            "oh, what did i just do !"),
            */
            new PS("", PreScene.Spawn, new List<string>(){
            "on day one I spawned in as a baby "  }),


            new PS("", PreScene.IHavePower, new List<string>(){
            "I ran in but out of anger I accidentally shot lava out everywhere wao I have lava Powers I can't control them" }),

            new PS("", PreScene.Talk_EnemyThreaten, new List<string>(){
            "ah the Wolves Last Hope once I Crush you little lava wolf your Forest will be in our control",
            "just give up you can't stop this little one",
            "I'll squash you look a bug too",
            "I regret everything"}),

            new PS("", PreScene.Talk_EnemyThreaten, new List<string>(){
            "I was running for my life as I ran more of the Tigers kept charging through the bushes of the trees trying to slash me down",
            "I began to run away but before I could they threw poison on me causing my vision to get blurry",
            "I began to run away through the forest with the water tiger blasting at us from behind [we have to lose him]",
            "okay just have to take him down this should be a piece of cake I walked in fully ready to face my foe",
            "time to die" ,
       "don`t let him go away" ,
       "you just don't know when to quit. do you? ",
       "I'll squash you like a bug",
       "you are not going anywhere",
       "you are not going anywhere",
       "the boss going to love this new prize we found",
       "our conquest for overworld has offically began",
             }),

            new PS("", PreScene.Fight_RunForTart, new List<string>(){
           "it wasn't long though until I came across the large lava Crystal I was looking for my leader wanted me to find this",
                "[stay away] I ran throughout the hallways and managed to close a door behind me [open this door]",
                "he tried to shoot out spews of poisonous gas at me but I did my best and dodged out of the way. [you're crazy man] after enough dodging I made it to the queen",}),

             new PS("", PreScene.Travel_BackToHome, new List<string>(){
          "I arrived back at the wasp's nest and they were so relieved to see that I took him down",
                "I was able to bring my pack leader all the way back to my Hideout"}),

             new PS("", PreScene.Self_GrowUp, new List<string>(){
          "because of this I grew stronger once again I now had sharper Fang and a larger and stronger body with 15 hearts wo I feel amazing",
             "because of my victory I grew into an adult-sized tiger I even gained five more Hearts",
             "because of this my body began to change I gained five more hearts and turned into a larger Warden snake I even have little Warden antlers ",
             }),


        new PS("", PreScene.Talk_FriendThank, new List<string>(){
          "yes we did it thank you so much it's true the lava wolf is a savior",
          "I'm just trying to do what's right why don't you guys stay with me for a while",
          "wao you did it thank you dearly go ahead the fragment is all yours",
          "thank you so much you have treated us too kindly",
          "Thanks for saving me, my name is ",
          "you did it",
        }),

                new PS("", PreScene.Talk_FriendsWeak, new List<string>(){
          "I have taken too many hits in a matter of days I'm sad to say I will be gone",
        }),
                new PS("", PreScene.Fight_IBlockEnemy, new List<string>(){
          "he tried his best to run away but we blocked him off and took him down",
        }),

        new PS("", PreScene.Talk_FuckEnmey, new List<string>(){
          "who's larger now you punk" ,"you're crazy man",
          "I was facing off against the Exterminator even though he was an old man he was tough he had deadly poisonous gas in his Arsenal and have the brute strength of nothing I'd ever faced before",
        }),

       new PS("", PreScene.Travel_Traveling, new List<string>(){
          "_i was heading back to _my base teleporting through the world",}),

              new PS("", PreScene.Travel_Arrived, new List<string>(){
          "_i found _myself in a large Village",
              "I arrived at a large Coastal Village",
              "I was traveling toward the pirate base",
              "we reached the clearing",
              "the mushroom led me over to a Strang looking Jungle Room",
              "we entered themushroom's main home",
              }),

                     new PS("", PreScene.Travel_FindSpot, new List<string>(){
          "_i spotted a pirate ship nearby and knew that it must be the doing of _thehe and _his men",
          "when _i spotted a village, this one looked as though it was starting to flood as well ",
          "and saw that it was swarming with _thehe "
          ,}),

                            new PS("", PreScene.Travel_Confused, new List<string>(){
          "_i looked around the village and things seemed to be different about this world",}),

                                   new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

          new PS("", PreScene.Build_FindWool, new List<string>(){
          "once I was finished I found a group of sheep and defeated them together Wool",}),

                                   new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

          new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),


            new PS("", PreScene.Build_FindWool, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Travel_NoiseInDistance, new List<string>(){
          "I was about to go to sleep for the night when I heard screams in the distance",
            "I heard loud howling going off in the distance",
            " I heard a strange noise from inside the cave",
            }),


            new PS("", PreScene.Travel_SearchNoise, new List<string>(){
          "oh no I need to go and see what's happening",
            "and I began a search to investigate it",
            }),

            new PS("", PreScene.Travel_FoundNewFreind, new List<string>(){
          "I found a strange Enderman creature",}),


            new PS("", PreScene.Talk_HelloToFriend, new List<string>(){
          "hey who Are You",}),

            new PS("", PreScene.Travel_DropMap, new List<string>(){
          "one of _thehe dropped a map. it looked like the coordinates to the pirate base",}),


            new PS("", PreScene.Fight_SuccessCheer, new List<string>(){
          "_theme started to cheer for _him and say _thehe was real",}),

            new PS("", PreScene.Travel_Sneak, new List<string>(){
          "_i used my teleportation abilities to sneak past _him and remain undetected",}),


            new PS("", PreScene.Talk_FriendEncourage, new List<string>(){
          "_theme assure _him that _he will be able to in time", "you have a long way to go but you are on your way"}),

            new PS("", PreScene.Fight_Attack, new List<string>(){
          " _i begin to shoot out very powerful fire blasts" ,
       " _i had control over the plant life around _me and would trap _him in place",
       " _i would use _my lava to cut _him off from reaching _him ",
       " _i came in again and slashed _him so hard",
       " _i angrily began to attack _him ",
       " _i they kept trying to fight _him ",
       " _i ran in and started to fend _him off",
       "that's when  _i noticed a new ability in _my inventory a diamond slash. _i use it on _him",
       "_i then used a special ability on _me which summoned void spikes from above",
       "_i even sent out Undead beasts to outnumber _him",
       "_he rushed at me and bashed _me with _his claws ouch"}),

            new PS("", PreScene.Fight_IWasHurt, new List<string>(){
           "I wanted to fight back but the poison was extremely lethal towards me" ,
             "I had half a heart and was dodging each of its things left and right",
             "I was getting extremely low",
             "I was knocked down to only one heart",
             "as soon as they hit I was blinded ah",
             "I thought I was surely done for",
}),

            new PS("", PreScene.Fight_EnemyChargeIn, new List<string>(){
          "charging in entered a _thehe.",
       "I was facing of against the _thehe.",
       "just then the _thehe." + " dropped down in front of me",
       "_thehe rushed in and we began to fight ",
       "I looked up and saw that _thehe." + " was charging towards me ",
       "shortly followed by a bunch of _thehe. they immediately started to run through our kingdom and kill my people", }),

            new PS("", PreScene.Fight_DescribeEnemy, new List<string>(){
          " _attacker  were way stronger than my people and could took them out with ease" ,
       "even though _thehe.  was a old man, he was tough",
       "_attacker`s  massive size and speed were far greater than me",
       "_attacker  had deadly poisonous gas in his aresnel ",
        "_attacker  have the brute strength of nothing everyone had ever faced before ",
       "he had Incredible strength and abilities",
       "I could tell with my increased strength I was putting up more of a fight",}),

            new PS("", PreScene.Talk_FuckEnmey, new List<string>(){
                 "Stay Away" ,
       "You stay away from me",
             " Stop it ",
             "No",
             "I'm sorry but I have to do this I cannot die here",
             "take this",
             "I knew I had to do something",
             "the wolves have found us we have to go",
             "if they found us, we are done for",}),

            new PS("", PreScene.Talk_FriendSad, new List<string>(){
          "without the Elder there is surely no hope in winning this War",}),

            new PS("", PreScene.Talk_FriendMission, new List<string>(){
           "my family and I were separated from the war and I don't have a home" ,
                        "this will take you to the first of five special Diamonds, the saber diamond. for each one you collect, the closer you will come to stopping the wolf Nation, do it for me, and end this war" ,
                        "there is said to be five Warden scales in total each dropped down from past Ward and snake Warriors",
                        "my son. you are very special. when it is time you shall be the one who takes the throne.",}),

            new PS("", PreScene.Travel_FindTreasure, new List<string>(){
          "and far off on the other side of it was a scale ",}),

            new PS("", PreScene.Travel_TakeTreasure, new List<string>(){
          "I did as ordered and went forward to pick it up",}),

            new PS("", PreScene.Fight_FriendHelp, new List<string>(){
          "but _friend stepped in the way and started to fight it off",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", PreScene.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),
        };
    }
}
