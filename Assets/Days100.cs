using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Structure;
public static class Days100
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

    enum SC
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

    struct PS
    {
        string link;
        SC sc;
        CA ca;
        CH ch0;
        AC ac0;
        SP sp00;
        SP sp01;
        CH ch1;
        AC ac1;
        SP sp10;
        SP sp11;
        List<string> contents;


        public PS(string link, SC sc, List<string> contents)
        {
            this.link = link;
            this.sc = sc;
            this.ca = CA.Idle;
            this.ch0 = CH.None;
            this.ch1 = CH.None;
            this.ac0 = AC.None;
            this.ac1 = AC.None;
            this.sp00 = this.sp01 = this.sp10 = this.sp11 = SP.None;
            this.contents = contents;
        }

        public PS(string link, SC sc, CA ca, CH ch, AC ac, SP sp,
            string content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.ch0 = ch;
            this.ac0 = ac;
            this.sp00 = sp;
            this.sp01 = sp;
            this.ch1 = CH.None;
            this.ac1 = AC.None;
            this.sp10 = SP.None;
            this.sp11 = SP.None;
            this.contents = new List<string>() { content };
        }

        public PS(string link, SC sc, CA ca, CH ch, AC ac, SP sp, SP sp2,
    string content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.ch0 = ch;
            this.ac0 = ac;
            this.sp00 = sp;
            this.sp01 = sp2;
            this.ch1 = CH.None;
            this.ac1 = AC.None;
            this.sp10 = SP.None;
            this.sp11 = SP.None;
            this.contents = new List<string>() { content };
        }

        public PS(string link, SC sc, CA ca, CH ch, AC ac, SP sp, CH ch1, AC ac1, SP sp1,
            string content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.ch0 = ch;
            this.ac0 = ac;
            this.sp00 = sp;
            this.sp01 = sp;
            this.ch1 = ch1;
            this.ac1 = ac1;
            this.sp10 = sp1;
            this.sp11 = sp1;
            this.contents = new List<string>() { content };
        }

        public PS(string link, SC sc, CA ca, CH ch, AC ac, SP sp, SP sp01, CH ch1, AC ac1, SP sp1,
    string content)
        {
            this.link = link;
            this.sc = sc;
            this.ca = ca;
            this.ch0 = ch;
            this.ac0 = ac;
            this.sp00 = sp;
            this.sp01 = sp01;
            this.ch1 = ch1;
            this.ac1 = ac1;
            this.sp10 = sp1;
            this.sp11 = sp1;
            this.contents = new List<string>() { content };
        }
    }

    // 严格区分Enemy 和 I
    // 一句话中最多两个角色，

    static void Prepare()
    {
        List<PS> pres = new List<PS>()
        {
            new PS("https://youtu.be/y2yRPKQ5nnE?t=115", SC.TalkRun, CA.Idle, CH.I, AC.TalkCamera, SP.HidenPlaace,
            "I got to find a way out of here"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=121", SC.LookingActor, CA.ACtor0LookingActor1, CH.I, AC.LookingActor, SP.HidenPlaace, CH.Friend, AC.LookingActor, SP.Cage,
            "I looked over to see a strange shark creature stuck in a cage"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=130", SC.TalkHelp, CA.Idle, CH.Friend, AC.TalkingLoud, SP.Cage,
            "I'm a shark dog duh oh come on let me out of here I can help you escape"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=131", SC.OneBreakingCharingIn, CA.ACtor0LookingActor1, CH.Enemy, AC.WalkToSpot, SP.ChargeInStart, SP.ChargeInEnd, CH.I, AC.LookingActor, SP.HidenPlaace,
            "the door slammed open and the _enemy found us"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=137", SC.Talk, CA.IdleRotationZ,CH.Enemy, AC.TalkingLoud, SP.ChargeInEnd,
            "oh no ah forget turnning this little run in for a prize"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=141", SC.Attack_Rush, CA.Main, CH.Enemy, AC.ChargeIn, SP.ChargeInEnd, SP.EnemyAttack, CH.I, AC.LookingActor, SP.IAttack,
            "_they rushed in"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=143", SC.Attack, CA.LookingMeRotate45, CH.I, AC.Attack_Blast, SP.IAttack,
            "but I blasted out a strange skull ink explosion"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=145", SC.Attack, CA.LookingMeRotate45, CH.Enemy, AC.Attack_DefendBackwards, SP.EnemyAttack, SP.EnemyDefend,
            "which blasted them back"),
            new PS("https://youtu.be/y2yRPKQ5nnE?t=150", SC.Attack, CA.Idle, CH.I, AC.TalkCamera, SP.IAttack,
            "oh, what did i just do !"),

            new PS("", SC.Spawn, new List<string>(){
            "on day one I spawned in as a baby "  }),


            new PS("", SC.IHavePower, new List<string>(){
            "I ran in but out of anger I accidentally shot lava out everywhere wao I have lava Powers I can't control them" }),

            new PS("", SC.Talk_EnemyThreaten, new List<string>(){
            "ah the Wolves Last Hope once I Crush you little lava wolf your Forest will be in our control",
            "just give up you can't stop this little one",
            "I'll squash you look a bug too",
            "I regret everything"}),

            new PS("", SC.Talk_EnemyThreaten, new List<string>(){
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

            new PS("", SC.Fight_RunForTart, new List<string>(){
           "it wasn't long though until I came across the large lava Crystal I was looking for my leader wanted me to find this",
                "[stay away] I ran throughout the hallways and managed to close a door behind me [open this door]",
                "he tried to shoot out spews of poisonous gas at me but I did my best and dodged out of the way. [you're crazy man] after enough dodging I made it to the queen",}),

             new PS("", SC.Travel_BackToHome, new List<string>(){
          "I arrived back at the wasp's nest and they were so relieved to see that I took him down",
                "I was able to bring my pack leader all the way back to my Hideout"}),

             new PS("", SC.Self_GrowUp, new List<string>(){
          "because of this I grew stronger once again I now had sharper Fang and a larger and stronger body with 15 hearts wo I feel amazing",
             "because of my victory I grew into an adult-sized tiger I even gained five more Hearts",
             "because of this my body began to change I gained five more hearts and turned into a larger Warden snake I even have little Warden antlers ",
             }),


        new PS("", SC.Talk_FriendThank, new List<string>(){
          "yes we did it thank you so much it's true the lava wolf is a savior",
          "I'm just trying to do what's right why don't you guys stay with me for a while",
          "wao you did it thank you dearly go ahead the fragment is all yours",
          "thank you so much you have treated us too kindly",
          "Thanks for saving me, my name is ",
          "you did it",
        }),

                new PS("", SC.Talk_FriendsWeak, new List<string>(){
          "I have taken too many hits in a matter of days I'm sad to say I will be gone",
        }),
                new PS("", SC.Fight_IBlockEnemy, new List<string>(){
          "he tried his best to run away but we blocked him off and took him down",
        }),

        new PS("", SC.Talk_FuckEnmey, new List<string>(){
          "who's larger now you punk" ,"you're crazy man",
          "I was facing off against the Exterminator even though he was an old man he was tough he had deadly poisonous gas in his Arsenal and have the brute strength of nothing I'd ever faced before",
        }),

       new PS("", SC.Travel_Traveling, new List<string>(){
          "_i was heading back to _my base teleporting through the world",}),

              new PS("", SC.Travel_Arrived, new List<string>(){
          "_i found _myself in a large Village",
              "I arrived at a large Coastal Village",
              "I was traveling toward the pirate base",
              "we reached the clearing",
              "the mushroom led me over to a Strang looking Jungle Room",
              "we entered themushroom's main home",
              }),

                     new PS("", SC.Travel_FindSpot, new List<string>(){
          "_i spotted a pirate ship nearby and knew that it must be the doing of _thehe and _his men",
          "when _i spotted a village, this one looked as though it was starting to flood as well ",
          "and saw that it was swarming with _thehe "
          ,}),

                            new PS("", SC.Travel_Confused, new List<string>(){
          "_i looked around the village and things seemed to be different about this world",}),

                                   new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

          new PS("", SC.Build_FindWool, new List<string>(){
          "once I was finished I found a group of sheep and defeated them together Wool",}),

                                   new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

          new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),


            new PS("", SC.Build_FindWool, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Travel_NoiseInDistance, new List<string>(){
          "I was about to go to sleep for the night when I heard screams in the distance",
            "I heard loud howling going off in the distance",
            " I heard a strange noise from inside the cave",
            }),


            new PS("", SC.Travel_SearchNoise, new List<string>(){
          "oh no I need to go and see what's happening",
            "and I began a search to investigate it",
            }),

            new PS("", SC.Travel_FoundNewFreind, new List<string>(){
          "I found a strange Enderman creature",}),


            new PS("", SC.Talk_HelloToFriend, new List<string>(){
          "hey who Are You",}),

            new PS("", SC.Travel_DropMap, new List<string>(){
          "one of _thehe dropped a map. it looked like the coordinates to the pirate base",}),


            new PS("", SC.Fight_SuccessCheer, new List<string>(){
          "_theme started to cheer for _him and say _thehe was real",}),

            new PS("", SC.Travel_Sneak, new List<string>(){
          "_i used my teleportation abilities to sneak past _him and remain undetected",}),


            new PS("", SC.Talk_FriendEncourage, new List<string>(){
          "_theme assure _him that _he will be able to in time", "you have a long way to go but you are on your way"}),

            new PS("", SC.Fight_Attack, new List<string>(){
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

            new PS("", SC.Fight_IWasHurt, new List<string>(){
           "I wanted to fight back but the poison was extremely lethal towards me" ,
             "I had half a heart and was dodging each of its things left and right",
             "I was getting extremely low",
             "I was knocked down to only one heart",
             "as soon as they hit I was blinded ah",
             "I thought I was surely done for",
}),

            new PS("", SC.Fight_EnemyChargeIn, new List<string>(){
          "charging in entered a _thehe.",
       "I was facing of against the _thehe.",
       "just then the _thehe." + " dropped down in front of me",
       "_thehe rushed in and we began to fight ",
       "I looked up and saw that _thehe." + " was charging towards me ",
       "shortly followed by a bunch of _thehe. they immediately started to run through our kingdom and kill my people", }),

            new PS("", SC.Fight_DescribeEnemy, new List<string>(){
          " _attacker  were way stronger than my people and could took them out with ease" ,
       "even though _thehe.  was a old man, he was tough",
       "_attacker`s  massive size and speed were far greater than me",
       "_attacker  had deadly poisonous gas in his aresnel ",
        "_attacker  have the brute strength of nothing everyone had ever faced before ",
       "he had Incredible strength and abilities",
       "I could tell with my increased strength I was putting up more of a fight",}),

            new PS("", SC.Talk_FuckEnmey, new List<string>(){
                 "Stay Away" ,
       "You stay away from me",
             " Stop it ",
             "No",
             "I'm sorry but I have to do this I cannot die here",
             "take this",
             "I knew I had to do something",
             "the wolves have found us we have to go",
             "if they found us, we are done for",}),

            new PS("", SC.Talk_FriendSad, new List<string>(){
          "without the Elder there is surely no hope in winning this War",}),

            new PS("", SC.Talk_FriendMission, new List<string>(){
           "my family and I were separated from the war and I don't have a home" ,
                        "this will take you to the first of five special Diamonds, the saber diamond. for each one you collect, the closer you will come to stopping the wolf Nation, do it for me, and end this war" ,
                        "there is said to be five Warden scales in total each dropped down from past Ward and snake Warriors",
                        "my son. you are very special. when it is time you shall be the one who takes the throne.",}),

            new PS("", SC.Travel_FindTreasure, new List<string>(){
          "and far off on the other side of it was a scale ",}),

            new PS("", SC.Travel_TakeTreasure, new List<string>(){
          "I did as ordered and went forward to pick it up",}),

            new PS("", SC.Fight_FriendHelp, new List<string>(){
          "but _friend stepped in the way and started to fight it off",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),

            new PS("", SC.Talk_FriendsHappy, new List<string>(){
          "_theme is real. we will it be saved",}),
        };
    }
}
