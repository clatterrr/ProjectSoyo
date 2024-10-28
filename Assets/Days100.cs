using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static AnimationSystem;
using static Structure;
public class Days100 : MonoBehaviour
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

        public void add(string s, Vector3 p)
        {
            _spots.Add(s);
            _pos.Add(p);
        }

        public Vector3 find(string spot)
        {
            for (int i = 0; i < _spots.Count; i++)
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

    public void init2()
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
        Test_Found,

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

        Survival_IEatFarms,


        Talk_FriendStopMe,
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
        Travle_FindCaneAndMine,


        Fight_IRunaway,
        Fight_FriendHelp,
        Fight_Attack,
        Fight_SuccessCheer,
        Fight_RunForTart,
        Fight_IWasHurt,
        Fight_Final,
        Fight_EnemyChaseMe,
        Fight_DescribeEnemy,
        Fight_IThreatenBack,
        Fight_IBlockEnemy,
        Fight_EnemyChargeIn,

        Build_FindWool,
        Build_Bed,
        Build_CraftTools,
        Build_House,
        Build_LackOres,

    }

    // 规定所有的物体移动和摄像机，随机旋转
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
        HeroTalkFriendInCage,
        EnemyEntrance,
        HeroFoundVillagersInDangerous,
        OneTalkAnotherListen,
        EnemyChaseFriendRun,
        Attack,
    }

    struct CameraSettings
    {
        int startFrame;
        int endFrame;
        GameObject follow;
        Vector3 followOffset0;
        Vector3 followOffset1;
        GameObject lookat;
    }
    public enum SP
    {
        None,
        HidenPlaace, // sometime not appear
        PrePos,
        Cage, // head rotate large
        ChargeInStart,
        ChargeInEnd,
        EnemyAttack,
        EnemyDefend,
        IAttack,
        IDefend,
        Entrance,

        //https://youtu.be/jCgRV9bRBt8?t=341
        WatchOverPlaceClose,
        WatchOverPlaceFar,

        TalkPlace0,
        TalkPlace1,

        RunAwayStart,
        RunAwayEnd,
        ChaseStart,
        ChaseEnd,

    }

    // follow empty and look some one

    // camera 中才去设置 CameraFollower 的位置
    enum CA
    {
        LookBehind,
        StaticAtStart,
        //https://youtu.be/jCgRV9bRBt8?t=341
        FollowCustomLookActorCloseToFar,
        // follow 和 lookat 是同一个所以省略
        FollowActorAhead,
        LookAheadMainActor,

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




    // characters
    struct FA
    {

        public CH ch0;
        public AC ac0;
        public List<SP> sps;
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
        public bool showHand;


        public PS(string link, PreScene sc, List<string> contents)
        {
            this.link = link;
            this.sc = SC.None;
            this.psc = sc;
            this.ca = CA.StaticAtStart;
            this.fas = new List<FA>();
            this.contents = contents;
            showHand = false;
        }

        public void SetCameraStory(List<CA> cas)
        {
            this.ca = RandomList(cas);
        }
    }

    // 严格区分Enemy 和 I
    // 一句话中最多两个角色，




    private List<ActorSettings> actorSettings = new List<ActorSettings>();
    private List<CameraSetting> cameraSettings = new List<CameraSetting>();
    private TerrianCreator terrainCreator;
    struct AllMyFellow
    {
        public List<ActorType> types;
        public List<GameObject> actors;
        public List<bool> actives;
        public List<Vector3> pos;
        public List<SP> startSP;
        public List<SP> endSP;
        public List<theAnim> anim;
        public AllMyFellow(int count)
        {
            this.types = new List<ActorType>();
            this.actors = new List<GameObject>();
            this.actives = new List<bool>();
            this.anim = new List<theAnim>();
            this.pos = new List<Vector3>();
            this.startSP = new List<SP>();
            this.endSP = new List<SP>();
        }
        public void Add(ActorType type, string prefab_path)
        {

            if (type == ActorType.CameraFollow || type == ActorType.CameraLookat)
            {
                GameObject itsmygo = new GameObject(type.ToString());
                itsmygo.AddComponent<AnimationSystem>();
                actors.Add(itsmygo);
            }
            else
            {
                GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab_path);
                actors.Add(Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            }

            actives.Add(false);
            types.Add(type);
            anim.Add(theAnim.Wait);
            pos.Add(Vector3.zero);
            startSP.Add(SP.None);
            endSP.Add(SP.None);
        }


        public GameObject GetActor(ActorType type)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (types[i] == type)
                {
                    actives[i] = true;
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
            // for (int i = 0; i < actives.Count; i++) actorSettings.Add(addActorMove(startFrame, endFrame, actors[i], actives[i]));
        }


        public void addMove(int frameStart, int frameEnd, ActorType actorType, theAnim animation, Vector3 pos)
        {
            //actorSettings.Add( new ActorSettings(frameStart, frameEnd, GetActor(actorType), animation, pos, pos, Quaternion.identity, Quaternion.identity, true, null));
        }

        public void setState(ActorType type, theAnim anim, Vector3 pos)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (types[i] == type)
                {
                    this.anim[i] = anim;
                    this.pos[i] = pos;
                }
            }
        }

        public void setSP(ActorType type, SP sp0, SP sp1)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (types[i] == type)
                {
                    this.startSP[i] = sp0;
                    this.endSP[i] = sp1;
                }
            }
        }

        public List<SP> GetSP(ActorType type)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (types[i] == type) return new List<SP>() { startSP[i], endSP[i] };
            }
            return new List<SP>() { SP.None, SP.None };
        }

    }

    Vector3 GetPos(SP sp)
    {
        return terrainCreator.CorretedHeights(sp);
    }

    public void SetHandActive(int frameStart, int frameEnd, bool active)
    {

    }

    void fastMove(int startFrame, int endFrame, ActorType actor1, theAnim anim, SP sp, ActorType actor2, AllMyFellow fellow)
    {
        //actorSettings.Add(addActorMove(startFrame, endFrame, fellow.GetActor(actor1), theAnim.SelfTalk, GetPos(SP.Entrance), GetPos(SP.Entrance), fellow.GetActor(actor2)));
    }

    void fastMove2(int startFrame, int endFrame, ActorType actor1, theAnim anim, SP sp0, SP sp1, ActorType actor2)
    {
        fellow.setSP(actor1, sp0, sp1);
        //actorSettings.Add(addActorMove(startFrame, endFrame, fellow.GetActor(actor1), anim, terrainCreator.GetSPPos(sp0), terrainCreator.GetSPPos(sp1), fellow.GetActor(actor2)));
    }

    void fastMove2(int startFrame, int endFrame, ActorType actor1, theAnim anim, SP sp0, SP sp1)
    {
        //actorSettings.Add(addActorMove(startFrame, endFrame, fellow.GetActor(actor1), anim, GetPos(sp0), GetPos(sp1), null));
    }

    void fastCamera(int startFrame, int endFrame, ActorType actor1, Vector3 pos0, Vector3 pos1)
    {
        /*
        ActorSettings newActor = addActorMove(startFrame, endFrame, fellow.GetActor(actor1), theAnim.Wait, pos0, pos1, null);
        newActor.type = actor1;
        actorSettings.Add(newActor);
        */
    }

    // 每天有不同的事情

    struct DaysSceneRatio
    {
        public PreScene prev;
        public List<PreScene> addednexts;
        public List<PreScene> nexts;
        public DaysSceneRatio(PreScene prev, List<PreScene> nexts)
        {
            this.prev = prev;
            addednexts = new List<PreScene>();
            this.nexts = nexts;
        }

        public DaysSceneRatio(PreScene prev, List<PreScene> added, List<PreScene> nexts)
        {
            this.prev = prev;
            this.addednexts = added;
            this.nexts = nexts;
        }

        public List<PreScene> GetNext()
        {
            PreScene next = RandomList(this.nexts);
            if (addednexts.Count == 0) return new List<PreScene> { next };
            PreScene added = RandomList(this.addednexts);
            int r = Random.Range(0, 2);
            if (r == 0) return new List<PreScene> { next };
            return new List<PreScene> { added, next };
        }
    }

    // consider status
    enum BluePrint
    {
        FightWithBoss,
        FightWithEnemy,
        IGrowth,
        ILackToolsAndCrafts,
        ITravelToNewSpot_new,
        IHungryAndHunting, // villager stops me 
        FriendAppear,
    }
    enum StatusName
    {
        // 这个放pre scene 吗？很疑惑
        // isTalkingToFriend + isEnemyAppear == EnemyAttackFriend and other house
        // isTalkingToFriend + IDefeatEnemy == FriendThankMe
        isTalkingToFriend,
        isHurt,

    }
    enum SBool
    {
        T,
        F,
        N//不重要，维持原样
    }
    struct Status
    {
        public List<StatusName> names;
        public List<SBool> sbools;

        public Status(int x)
        {
            names = new List<StatusName>();
            sbools = new List<SBool>();
        }
        public void AddOne(StatusName name, SBool sb)
        {
            names.Add(name);
            sbools.Add(sb);
        }
        public void Clear()
        {
            names.Clear();
            sbools.Clear();
        }
        public bool Match(Status status)
        {

            for (int selfIndex = 0; selfIndex < names.Count; selfIndex++)
            {
                for (int otherIndex = 0; otherIndex < status.names.Count; otherIndex++)
                {
                    if (this.names[selfIndex] == status.names[otherIndex])
                    {
                        if (this.sbools[selfIndex] == SBool.T && status.sbools[selfIndex] == SBool.T ||
                            this.sbools[selfIndex] == SBool.F && status.sbools[selfIndex] == SBool.F)
                        {

                        }
                        else { return false; }
                    }
                }
            }
            return true;
        }
        public Status Replace(Status status)
        {
            for (int selfIndex = 0; selfIndex < names.Count; selfIndex++)
            {
                for (int otherIndex = 0; otherIndex < status.names.Count; otherIndex++)
                {
                    if (this.names[selfIndex] == status.names[otherIndex])
                    {
                        if (status.sbools[selfIndex] != SBool.N) status.sbools[selfIndex] = this.sbools[otherIndex];
                    }
                }
            }
            return status;
        }
    }

    struct BluePrintSelection
    {
        public BluePrint bluePrint;
        public List<List<PreScene>> ppss;
        // 必须满足的条件
        public List<Status> MustMatchstatus;
        // 将会强制改变的条件
        public List<Status> OverrideStatus;

        public List<PreScene> doneScens;

        public BluePrintSelection(BluePrint baseprint)
        {
            bluePrint = baseprint;
            MustMatchstatus = new List<Status>();
            OverrideStatus = new List<Status>();
            ppss = new List<List<PreScene>>();
            doneScens = new List<PreScene>();
        }

        public void AddSelection(List<PreScene> pss, Status statu0, Status statu1)
        {
            ppss.Add(pss);
            OverrideStatus.Add(statu0);
            MustMatchstatus.Add(statu1);
        }

        public Status RandomPreScene(Status currentStatus)
        {
            List<int> possibleIndex = new List<int>();
            for (int i = 0; i < ppss.Count; i++) if (MustMatchstatus[i].Match(currentStatus)) possibleIndex.Add(i);
            int sceneIndex = RandomList(possibleIndex);
            doneScens = ppss[sceneIndex];
            return OverrideStatus[sceneIndex].Replace(currentStatus);

        }
    }



    AllMyFellow fellow = new AllMyFellow(0);

    List<BluePrintSelection> bluePrintSelections = new List<BluePrintSelection>();

    private void addBP(BluePrint print)
    {
        bluePrintSelections.Add(new BluePrintSelection(print));
    }

    private void addBPDetail(List<PreScene> scene, Status matchStatus, Status overrideStatus)
    {
        BluePrintSelection bp = bluePrintSelections[bluePrintSelections.Count - 1];
        bp.ppss.Add(scene);
        bp.MustMatchstatus.Add(matchStatus);
        bp.OverrideStatus.Add(overrideStatus);
        bluePrintSelections[bluePrintSelections.Count - 1] = bp;
    }

    private Status addBPDone(Status status)
    {
        return bluePrintSelections[bluePrintSelections.Count - 1].RandomPreScene(status);
    }

    struct LARGE2SMALL
    {
        public string large;
        public List<List<string>> middles;

        public LARGE2SMALL(string large)
        {
            this.large = large;
            this.middles = new List<List<string>>();
        }

        public void Add(List<string> middles)
        {
            this.middles.Add(middles);
        }
    }

    struct ItIt
    {
        public string it;
        public string name;
        public string thei;
        public string theme;
        public string themy;
        public string themine;

        public ItIt(string it, string name, string thei, string theme, string themy, string themine)
        {
            this.it = it;
            this.name = name;
            this.thei = thei;
            this.theme = theme;
            this.themy = themy;
            this.themine = themine;
        }
    }



    struct DayRecord
    {
        public string place;
        public string WhyGoThisPlace;
        public string WhatToDoThisPlace;
        public List<string> actors;
        public List<string> actorRequests;
        public List<string> actorGifts;

        public DayRecord(string place, string why, string how, List<string> actors, List<string> requests, List<string> gifts)
        {
            this.place = place.ToString();
            this.WhyGoThisPlace = why.ToString();
            this.WhatToDoThisPlace = how.ToString();
            this.actors = actors;
            this.actorRequests = requests;
            this.actorGifts = gifts;
        }

        public DayRecord(string place, string why, string how)
        {
            this.place = place.ToString();
            this.WhyGoThisPlace = why.ToString();
            this.WhatToDoThisPlace = how.ToString();
            this.actors = new List<string>() {  };
            this.actorRequests = new List<string>() {};
            this.actorGifts = new List<string>() { };
        }

        public DayRecord(string place, string why, string how, string one)
        {
            this.place = place.ToString();
            this.WhyGoThisPlace = why.ToString();
            this.WhatToDoThisPlace = how.ToString();
            string[] ones = one.Split('_');
            this.actors = new List<string>() { ones[0] };
            this.actorRequests = new List<string>() { ones[1] };
            this.actorGifts = new List<string>() { ones[2] };
        }

        public DayRecord(string place, string why, string how, string one, string two)
        {
            this.place = place.ToString();
            this.WhyGoThisPlace = why.ToString();
            this.WhatToDoThisPlace = how.ToString();
            string[] ones = one.Split('_');
            string[] twos = two.Split('_');
            this.actors = new List<string>() { ones[0] , twos[0]};
            this.actorRequests = new List<string>() { ones[1], twos[1] };
            this.actorGifts = new List<string>() { ones[2], twos[2] };
        }
    }

    // build or imporve base
    // fight, and take treasure
    // fight, and rescure friends
    // collect materials
    // fight, when travel
    // fight, with friends` help


    // 在这个地方干了什么
    // Travel , Fight And Ok
    // Travel , Fight And RunAway
    // Find Treasure
    // Find Treasure And Fight Ok
    // Rescure Friends And Fight
    // Building
    // Meet Friends And Get Task / Item

    // 为什么会来这个地方
    // Follow Map
    // Follow Friends
    // Find HideOut
    // Find Base
    void newStart()
    {

        List<FeelMySoul> souls = PrepareSentences();
        // 可能的地点
        string[] placeList = new string[] { "Valley", "Cave", "WaterFall", "Temple", "Hideout" };

        // Why Go To This Place

        string[] whyGoThisPlace = new string[] { "escape", "FollowMap", "FollowFriends", "OnTheWay", "SearchBase" }; // Find Treasure, Meet Friends, 

        // What To Do This Place， 首要目标肯定不是战斗，而是侦察

        string[] WhatToDoThisPlace = new string[] { "Detect", "hideForLive", "FindTreasure", "MeetFriends", "Fight", "Building", "RescureFriends" };

        // 朋友会做的请求，HelpFight, HelpFindTreasure, HelpFindPeople, JoinTeam
        // 朋友的奖励，Item, Info, Map, Treasure, TeamMember

        // 应该以事件，还是应该以地点？
        // 还是以地点吧，这样同一个地点至少人相同

        List<DayRecord> fireLions = new List<DayRecord>();
        fireLions.Add(new DayRecord("IceSpikeBiome", "BornAlone", "BornAlone", null, null, null));
        fireLions.Add(new DayRecord("Village", "FindFood", "FindFood_FriendDisscusEnemy_Fight", "villager_helpfight_thank", "zombie_enemy_none"));
        fireLions.Add(new DayRecord("Cave", "FindOre", "MineOre_MakeTool_Fight", "frostman_enemy_none"));
        fireLions.Add(new DayRecord("OwnHome", "Build", "BuildFarm_PlantFood_FindFriend_BuildFriendHome", "golem_protect_none"));
        fireLions.Add(new DayRecord("OwnHome", "Build", "BuildWall_PlantFood_Fight", "firewalker_enemy_none"));
        fireLions.Add(new DayRecord("Ocean", "Find", "FriendTalkEnemy_FindEnemy", "peguin_findenemy_none"));
        fireLions.Add(new DayRecord("Village", "Find", "FriendTalkInfo_FriendTradeInfo", "villager_none_none"));
        fireLions.Add(new DayRecord("Home", "Find", "QuickFight_BuildHome", "villager_none_none"));
        fireLions.Add(new DayRecord("Home", "Find", "BuildHome_CollectWool", "villager_none_none"));
        fireLions.Add(new DayRecord("Water", "Find", "QuickFightWrong_Heal", "witcher_enemy_none"));
        // 比如fight 和 enemy 在一起的时候，就可以加一句 "enemy surround my home"
        fireLions.Add(new DayRecord("Home", "Find", "BuildHome_QuickFight_KnowNoFood", "wolf_enemy_none"));





        //todo: large 2 small

        LARGE2SMALL fight = new LARGE2SMALL("fight");
        fight.Add(new List<string>() { "Enemy_Appear", "Enemy_Talk_Threaten", "enemy_chargein", "I_Talk_RunAway", "I_RunAway" });
        fight.Add(new List<string>() { "Enemy_Appear", "I_Talk_What", "I_Found_Enemy", "Enemy_Talk_Threaten", "Enemy_Attack_Me", "i_washurt", "I_Runaway" });
        fight.Add(new List<string>() { "enemy_chargein", "i_washurt", "i_runaway", "i_havepower", "i_needtools"});
        fight.Add(new List<string>() { "i_heardnoise", "enemy_chargein", "enemy_talk_threaten", "i_talk_againstthreaten"});
        fight.Add(new List<string>() { "enemy_chargein", "enemy_talk_threaten", "enemy_attack", "i_attackquick", "i_talk_runaway" });
        fight.Add(new List<string>() { "i_foundplace", "enemy_around", "enemy_describe", "i_washurt", "i_foundchest", "i_talk_heal", "i_attackfinal" });
        fight.Add(new List<string>() { "enemy_talk_threaten", "i_talk_againstThreaten", "enemy_describe", "i_attackquick", "enemy_dropitem" });


        LARGE2SMALL lack = new LARGE2SMALL("Lack");
        lack.Add(new List<string>() { "i_needtool", "i_crafttable" });
        lack.Add(new List<string>() { "i_washungry", "i_crafttable" });


        LARGE2SMALL findTreasure = new LARGE2SMALL("FindingTreasure");
        findTreasure.Add(new List<string>() { "i_travelplace", "i_foundchest", "i_talk_scary", "i_lootchest", "i_needtreasure" });


        LARGE2SMALL findfood = new LARGE2SMALL("FindFoodInPlace");
        findfood.Add(new List<string>() { "i_findfood", "friend_come", "i_talk_sorrystolefood" });

        https://youtu.be/yqQuSPOua-0?t=38
        LARGE2SMALL discussEnemy = new LARGE2SMALL("DiscussEnemy");
        discussEnemy.Add(new List<string>() { "i_findfood", "friend_come", "i_talk_sorrystolefood" });

        LARGE2SMALL quickFight = new LARGE2SMALL("quickFight");
        quickFight.Add(new List<string>() { "i_heardnoise", "enemy_chargein", "i_attackquick", "friendthank"});


        LARGE2SMALL build = new LARGE2SMALL("building");
        quickFight.Add(new List<string>() { "i_needhome", "i_needtool", "i_crafttool", "i_buildhome", "i_buildfurniture", "i_completehome"});
        quickFight.Add(new List<string>() { "enemy_chargein", "i_attackquick", "enemy_around", "i_needsafe", "i_strengthenbuild", "i_buildwall", "i_wassafe" });

        LARGE2SMALL friendgiveitem = new LARGE2SMALL("FriendGiveItem");
        friendgiveitem.Add(new List<string>() { "i_asknewfriend", "friend_notice_me", "friend_haveitemhelpme", "friend_giveitem", "i_tryuseitemmagic", "i_oldsaying", "i_talk_iamhero" });
        friendgiveitem.Add(new List<string>() { "friend_tradable", "i_mineore", "friend_trade", "item_shouldhelp", "i_closetofinal"});

        List<LARGE2SMALL> toList = new List<LARGE2SMALL>();
        toList.Add(fight);
        toList.Add(lack);
        toList.Add(findfood);
        toList.Add(discussEnemy);
        toList.Add(quickFight);
        for (int dayIndex = 0; dayIndex < fireLions.Count; dayIndex++)
        {
            DayRecord record = fireLions[dayIndex];
            // string friends = record.actors[0];
            //  string enemy = record.actors[1];

            string[] events = record.WhatToDoThisPlace.Split('_');

            for (int eventIndex = 0; eventIndex < events.Length; eventIndex++)
            {
                string theEvent = events[eventIndex];

                Debug.Log(" Big Event = " + theEvent);

                for (int toIndex = 0; toIndex < toList.Count; toIndex++)
                {
                    if (toList[toIndex].large.ToLower() == theEvent.ToLower())
                    {
                        int ri = Random.Range(0, toList[toIndex].middles.Count - 1);
                        List<string> middleEventList = toList[toIndex].middles[ri];

                        for (int middleIndex = 0; middleIndex < middleEventList.Count; middleIndex++)
                        {
                            string middleEvent = middleEventList[middleIndex];
                            middleEvent = middleEvent.ToLower();

                            if (middleEvent.Contains("talk"))
                            {
                                // 直接选说话就行了，说话的句式固定，很简单
                                string selectString = "i must get out of here";
                            }
                            else
                            {

                                string[] splited = middleEvent.Split("_");
                                string selectString = "_main_name template string _sub_me";

                                for(int soulIndex = 0; soulIndex < souls.Count; soulIndex++)
                                {
                                    string middleTemp = middleEvent.ToLower();
                                    if (middleTemp == "enemy_attack_me") middleTemp = "i_attack";
                                    if(souls[soulIndex].middle.ToLower() == middleTemp.ToLower())
                                    {
                                        int rii = Random.Range(0, souls[soulIndex].sentences.Count - 1);
                                        selectString = souls[soulIndex].sentences[rii];
                                        break;
                                    }
                                }


                                // 首先要把 _main 和 _sub 替换成 _enemy _friend, _i


                                string mainActor = splited[0];
                                //string mainAct = splited[1];
                                string subActor = "";
                                if (splited.Length > 2) subActor = splited[2];

                                selectString = selectString.Replace("_main", "_" + mainActor);
                                selectString = selectString.Replace("_sub", "_" + subActor);

                                string enemy_name = "dark monster";
                                string friend_name = "rabbit";
                                string i_name = "fire lion";


                                selectString = selectString.Replace("_enemy_name", enemy_name);
                                selectString = selectString.Replace("_friend_name", friend_name);
                                selectString = selectString.Replace("_i_name", i_name);

                                // 其他代词替换

                                selectString = selectString.Replace("_enemy_i", "he");
                                selectString = selectString.Replace("_enemy_my", "his");
                                selectString = selectString.Replace("_enemy_me", "him");
                                selectString = selectString.Replace("_enemy_mine", "him");


                                selectString = selectString.Replace("_i_i", "i");
                                selectString = selectString.Replace("_i_my", "my");
                                selectString = selectString.Replace("_i_me", "me");
                                selectString = selectString.Replace("_i_mine", "mine");


                                selectString = selectString.Replace("_friend_i", "he");
                                selectString = selectString.Replace("_friend_my", "his");
                                selectString = selectString.Replace("_friend_me", "him");
                                selectString = selectString.Replace("_friend_mine", "him");

                                Debug.Log(" middle event = " + middleEvent);
                                Debug.Log(" sentence = " + selectString);
                            }

                        }
                    }
                }

            }
        }



        // 除了MeetFriends 和 RescureFriends 外， 其他有Friends 的概率是 0.02
        // 除了RescureFriends 是新朋友外，其他是新朋友的概率不确定
        // 需要有个固定朋友，
        // 除了Fight 会百分比遭遇战斗外，其他遭遇战斗的概率是0.6

        string[] HowToMeetFixedFriends = new string[] { "OnTheWay" };

        // Fixed Friends 连续出现三场，然后一起Building
        // 之后在强化Building的时候继续出现，又连续出现三次，就行了




        // 战斗随机选一个
        string[] FightEnemyLists = new string[] { "Piglins", "Wolfs", "CaveMen", "Ghosts", "Spiders" };

        // WhenTravel 可以失败，一共12个场景，战斗占8个

        // 朋友的名字，身份，

        string[] theScenes = new string[] { "Born_I_Enemy0", "Fight_I_Enemy0", "FriendsHelp_I", "FightWhenTravel", "FightTakeTreasure", "Growth", "Building", "FriendsTask", "FightWhenTravelFailed", "FriendsHelp", "Building", "FightHelpVillage", "FightTakeTreasure", "FightWithBoss" };


        // 战斗成功的结局就是 失败就是TrappedInCage，不会Killed

        // 直接强制卡呗，Enemey 除了Boss外，和追踪外，都可以不一样
        // Friends 三种分类，父母， FixedFriends营救并一起造房子，一起进攻,   TemporalFriends 营救给任务，给道具
        string[] largeBluePrint = new string[] { "Building", "FindingTreasure", "TravelPlace", "VisitVillage_Friends_Villagers", "ResureFriends" };
        string[] largeBluePrint2 = new string[] { "Building", "FightTakeTreasure", "FightRescureFriends", "FightWhenTravel", "FightHelpVillagers", "FriendsTask", "Growth" };
        LARGE2SMALL l0 = new LARGE2SMALL("Building");



        ItIt iti = new ItIt("i", "blood dragon", "i", "me", "my", "mine");
        ItIt itenemy = new ItIt("enemy", "dark monster", "he", "him", "his", "his");
        ItIt itdad = new ItIt("friends", "chimmy", "he", "him", "his", "his");

        List<ItIt> itits = new List<ItIt>() { iti, itenemy, itdad };







    }
    void Start()
    {

        newStart();
        return;

        Status emptyStatus = new Status(0);
        Status currentStatus = new Status(0);

        // real time blue print
        addBP(BluePrint.FightWithBoss);
        addBPDetail(new List<PreScene>() { PreScene.EnemyChaseMe, PreScene.Fight_IRunaway }, emptyStatus, emptyStatus);
        addBPDetail(new List<PreScene>() { PreScene.EnemyChaseMe, PreScene.Fight_IWasHurt, PreScene.Fight_IRunaway }, emptyStatus, emptyStatus);
        currentStatus = addBPDone(currentStatus);

        // 现在有一大堆PreScene, PreScene 究竟是一句话比较好，还是多句话比较好？


        // GameObject 直接读取预制体
        terrainCreator = gameObject.GetComponent<TerrianCreator>();
        fellow.Add(ActorType.Player, "Assets/zombie.prefab");
        fellow.Add(ActorType.Friend, "Assets/zombie.prefab");
        fellow.Add(ActorType.CameraFollow, "");
        fellow.Add(ActorType.CameraLookat, "");

        // 超级大纲，每天干什么


        List<PreSceneToScene> pscT = new List<PreSceneToScene>();
        pscT.Add(new PreSceneToScene(PreScene.Attack, new List<SC>() { SC.HeroEntrance }));
        pscT.Add(new PreSceneToScene(PreScene.Test_Found, new List<SC>() { SC.EnemyChaseFriendRun }));


        // pre scene 到 scene 的阶段，只规定摄像机要看谁，物体大致要做什么动作，不规定细节，细节靠随机
        List<PS> scenes = new List<PS>();
        scenes.Add(new PS("", PreScene.Test_Found, new List<string>()));
        for (int scene_index = 0; scene_index < scenes.Count; scene_index++)
        {
            for (int i = 0; i < pscT.Count; i++)
                if (scenes[scene_index].psc == pscT[i].psc)
                {
                    PS p = scenes[scene_index];
                    int r = Random.Range(0, pscT[i].scs.Count);
                    p.sc = pscT[i].scs[r];
                    scenes[scene_index] = p;

                    // 摄像机设置也要随机

                    // psc == StartSmallScene
                    {

                        // Hero.SetPos(Entrance);
                    }
                }


            //question: when to set main actor, set sub actor
            // 给人物设置地点

            ActorType mainActorType = ActorType.Player;
            ActorType subActorType = ActorType.Friend;

            fellow.SetAllActiveFalse();
            switch (scenes[scene_index].sc)
            {
                // Scene 就不应该包含Camera 信息。要有角色信息
                case SC.HeroEntrance: // 最好不要Entrance
                    {
                        // idle 应该用PreScene
                        // 直接Idle 或者Walk 就行了
                        //actorSettings.Add(new ActorSettings(0, 0, Plane, theAnim.Idle, SP.Entrance,m))
                        // 位置是Idle, 但具体哪儿Idle 呢？ 要根据场景设置吗？但这个场景设置快变成摄像机设置了

                        // 不需要False，不动就可以了。切场景的时候Clear 就行了
                        //actorSettings.Add(addActorMove(0,10, fellow.GetActor(ActorType.Player), theAnim.SelfTalk, GetPos(SP.Entrance), GetPos(SP.Entrance), Camera.main));
                        break;
                    }
                case SC.EnemyEntrance:
                    {
                        SetHandActive(0, 10, false);
                        // 这个时候默认不写 hero 的但是hero 仅仅是Idle，不active == false
                        fastMove2(0, 10, ActorType.Enemy, theAnim.ChargeIn, SP.ChargeInStart, SP.ChargeInEnd, ActorType.Camera);
                        break;
                    }
                case SC.HeroFoundVillagersInDangerous:
                    {
                        Debug.Log(" in dangerous ");
                        PS ps = scenes[scene_index];
                        ps.ca = RandomList(new List<CA>() { CA.FollowCustomLookActorCloseToFar });
                        // 虽然摄像机是一直Follow And Lookat，但是也要设置Follow 和 Lookat的运动轨迹，这里应该是一个Random
                        scenes[scene_index] = ps;

                        SetHandActive(0, 10, false);

                        // 这里的fast Move2 也要是个
                        // 这个时候默认不写 hero 的但是hero 仅仅是Idle，不active == false
                        fastMove2(0, 100, ActorType.Player, theAnim.Walk, SP.WatchOverPlaceFar, SP.WatchOverPlaceClose, ActorType.Camera);
                        break;
                    }
                case SC.OneTalkAnotherListen: // 这个怎么循环呢？应该不用循环
                    {
                        PS ps = scenes[scene_index];
                        ps.ca = RandomList(new List<CA>() { CA.LookAheadMainActor });
                        // 虽然摄像机是一直Follow And Lookat，但是也要设置Follow 和 Lookat的运动轨迹，这里应该是一个Random
                        scenes[scene_index] = ps;

                        SetHandActive(0, 10, false);

                        // 这里的fast Move2 也要是个
                        // 这个时候默认不写 hero 的但是hero 仅仅是Idle，不active == false
                        fastMove2(0, 100, mainActorType, theAnim.SelfTalk, SP.TalkPlace0, SP.TalkPlace0, subActorType);
                        fastMove2(0, 100, subActorType, theAnim.Wait, SP.TalkPlace1, SP.TalkPlace1, mainActorType);
                        break;
                    }
                case SC.Attack:
                    {
                        PS ps = scenes[scene_index];
                        ps.ca = RandomList(new List<CA>() { CA.LookAheadMainActor });
                        scenes[scene_index] = ps;
                        SetHandActive(0, 10, false);
                        fastMove2(0, 100, mainActorType, theAnim.Attack0, SP.TalkPlace0, SP.TalkPlace0, subActorType);
                        fastMove2(0, 100, subActorType, theAnim.Hurt, SP.TalkPlace1, SP.TalkPlace1, mainActorType);
                        break;
                    }
                case SC.EnemyChaseFriendRun:
                    {
                        PS ps = scenes[scene_index];
                        ps.ca = RandomList(new List<CA>() { CA.LookAheadMainActor });
                        scenes[scene_index] = ps;
                        SetHandActive(0, 10, false);
                        fastMove2(0, 100, mainActorType, theAnim.RunAway, SP.RunAwayStart, SP.RunAwayEnd);
                        fastMove2(0, 100, subActorType, theAnim.Chase, SP.ChaseStart, SP.ChaseEnd, mainActorType);
                        break;
                    }
                    //https://youtu.be/jCgRV9bRBt8?t=340
            }


            switch (scenes[scene_index].ca)
            {
                case CA.FollowCustomLookActorCloseToFar:
                    {
                        // 要做的，获取眼睛，手的位置
                        List<SP> sps = fellow.GetSP(ActorType.Player);
                        Vector3 playerForward = (GetPos(sps[1]) - GetPos(sps[0])).normalized;
                        Vector3 startPos = GetPos(sps[0]) + playerForward * 2 + new Vector3(0, 2, 0);
                        Vector3 endPos = GetPos(sps[1]) + playerForward * 10 + new Vector3(0, 8, 0);
                        Debug.Log(" start pos = " + startPos + " end pos = " + endPos);
                        fastCamera(0, 100, ActorType.CameraFollow, startPos, endPos);
                        fastCamera(0, 100, ActorType.CameraLookat, GetPos(sps[0]), GetPos(sps[1]));

                        break;

                    }
            }

            //fellow.SetFalse(0, 0, actorSettings);
            // added good animation

            for (int i = 0; i < 0; i++)
            {
                // Sort Anim

                // for body anim
                theAnim currentAnim;
                theAnim nextAnim;

                int startFrame = 0;
                int endFrame = 0;
                int startFrame1 = endFrame;
                int endFrame1 = 0;

                float easeRatio = 0.1f;
                int easeFrame0 = (int)((endFrame - startFrame) * (1 - easeRatio) + startFrame);
                int easeFrame1 = (int)((endFrame1 - startFrame1) * easeRatio + startFrame1);
                float easeValue0 = 1.0f;
                float easeValue1 = 0.0f;
                float weight = 0.3f;
                float outTangent = 0.03f;
                float inTangent = 0.03f;
                float currentTime = 0f;

                float deltaTime = easeFrame1 - easeFrame0;

                // 计算控制点
                float C0x = easeFrame0 + deltaTime / 3f;  // C0 的时间
                float C0y = easeValue0 + outTangent * deltaTime / 3f;  // C0 的值

                float C1x = easeFrame1 - deltaTime / 3f;  // C1 的时间
                float C1y = easeValue1 - inTangent * deltaTime / 3f;   // C1 的值

                // 将时间归一化到 [0, 1]
                float u = (currentTime - easeFrame0) / deltaTime;

                // 三次贝塞尔插值公式
                float value = Mathf.Pow(1 - u, 3) * easeValue0 +
                              3 * Mathf.Pow(1 - u, 2) * u * C0y +
                              3 * (1 - u) * Mathf.Pow(u, 2) * C1y +
                              Mathf.Pow(u, 3) * easeValue1;

                // for head anim

            }


            //  
        }
    }

    int globalFrameCount = 0;
    private void FixedUpdate()
    {
        globalFrameCount += 1;
        for (int i = 0; i < actorSettings.Count; i++)
        {
            ActorSettings set = actorSettings[i];
            if (set.RunWithHeight(globalFrameCount, terrainCreator.GetHeights(), set.type))
            {
                if (globalFrameCount == set.frameEnd)
                {
                    fellow.setState(set.type, set.animation, set.posEnd);
                }
            }
        }
        //Camera.main.transform.position = fellow.GetActor(ActorType.CameraFollow).transform.position;
        // Camera.main.transform.LookAt(fellow.GetActor(ActorType.CameraLookat).transform.position);


        /*
                
            首先有一个很大的 LargeBluePrint 比如BuildHouse, TravelToPoint, FindTreasure, 
            (Attack) 是附属
            
         */

    }

    struct FeelMySoul
    {
        public string middle;
        public List<string> sentences;

        public FeelMySoul(string middle, List<string> sentences)
        {
            this.middle = middle;
            this.sentences = sentences;
        }
    }

    List<FeelMySoul> PrepareSentences()
    {

        List<FeelMySoul> souls = new List<FeelMySoul>() {
            new FeelMySoul("i_talk_runaway", new List<string>() {"I got to find a way out of here", "Oh no I got to get out of here" }),
            new FeelMySoul("i_talk_superiseattack", new List<string>() { "oh, what did i just do", "Oh no I got to get out of here" }),


            new FeelMySoul("i_foundincage_friend", new List<string>(){"_i_i looked over to see a strange _sub_name stuck in cage" }),

            new FeelMySoul("friend_talk_intro", new List<string>(){" i am _main_name" }),
            new FeelMySoul("friend_talk_helpescape", new List<string>(){" come on let me out of here i can help you escape" }),

            new FeelMySoul("i_wakeuptrapped", new List<string>(){" I awoke and saw that I was trapped inside a _trappedPlace" }),


            new FeelMySoul("enemy_around", new List<string>(){" there were _enemy_name all around looking hungrier than ever","there are still a few _enemy_name around" }),

            new FeelMySoul("enemy_chargein", new List<string>(){" all of the _main_name started to go to town",
                "charging in entered a _enemy_name",
                "_enemy_name were attacking the _defencePlace",
                "a _enemy_name started attacking _sub_me",
                "I was facing of against the _enemy_name",
                "_enemy_name rushed in and we began to fight ",
                "shortly followed by a bunch of _enemy_name. they immediately started to run through our field and begin to kill",
                "I looked up and saw that _enemy_name was charging towards me ",
                "just then the _enemy_name dropped down in front of me",
                "_main_name rushed in",
                "and one of _main_name was charging right towards _i_me"}),

            new FeelMySoul("enemy_talk_threaten", new List<string>(){
                " we got a big lunch today fellas", "I can't wait to eat that red thing",
                "oh no you don't, pal.  get him",
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


            new FeelMySoul("i_talk_againstThreaten", new List<string>(){
                " dude I'm not your food ", "stay away",
                "Stay Away" ,
                "You stay away from me",
                " Stop it ",
                "take that",
                "No",
                "I'm sorry but I have to do this I cannot die here",
                "take this",
                "I knew I had to do something",
                "the wolves have found us we have to go",
                "if they found us, we are done for",
            }),

            new FeelMySoul("i_attack", new List<string>(){
               "_main_i begin to shoot out very powerful fire blasts" ,
               "_main_i had control over the plant life around _main_me and would trap _sub_me in place",
               "_main_i would use _main_my lava to cut _sub_me off from reaching _sub_me ",
               "_main_i came in again and slashed _sub_me so hard",
               "_main_i angrily began to attack _sub_me ",
               "_main_i they kept trying to fight _sub_me ",
               "_main_i ran in and started to fend _sub_me off",
               "that's when  _main_i noticed a new ability in _main_my inventory a diamond slash. _main_i use it on _sub_me",
               "_main_i then used a special ability on _sub_me which summoned void spikes from above",
               "_main_i even sent out Undead beasts to outnumber _sub_me",
               "_main_i rushed at me and bashed _main_me with _his claws ouch",
               "_main_i Unleashed a loud blood Roar",
               "but _main_i blasted out a strange skull ink explosion",}),

            new FeelMySoul("quick_fight", new List<string>(){
                "i fought them off quickly",
                "i helped the _friend_name and fought off the _enemy_name"}),

            new FeelMySoul("enemy_notice_me", new List<string>(){
                "but because of this all the other _main_name took notice"}),

            new FeelMySoul("i_runaway", new List<string>(){
                "but i started to run",
                "I ran for my life",
                "I quickly jumped off the _attackPlace",}),


            new FeelMySoul("enemy_chase", new List<string>(){
                "as all of them were running towards me",
                "two of the _enemy_name then appeared on either side of _i_me",
            }),


            new FeelMySoul("i_attackback", new List<string>(){
                "which blasted them back",
                "which sent the dinosaur flying back",
                "I can't wait to eat that red thing",}),

            new FeelMySoul("enemy_appear", new List<string>(){
                "the door slammed open and the _enemy found us", "I can't wait to eat that red thing",}),

            new FeelMySoul("friend_talk_thank", new List<string>(){
                  "the villager was happy and offered me more of his food and invited me to his house",
                  "yes we did it thank you so much it's true the lava wolf is a savior",
                  "I'm just trying to do what's right why don't you guys stay with me for a while",
                  "wao you did it thank you dearly go ahead the fragment is all yours",
                  "thank you so much you have treated us too kindly",
                  "Thanks for saving me, my name is ",
                  "you did it",
            }),

            new FeelMySoul("i_reachPoint", new List<string>(){
                  "_main_i found _main_myself in a large Village",
                  "_main_i arrived at a large Coastal Village",
                  "_main_i was traveling toward the pirate base",
                  "_main_i reached the clearing",
                  "the mushroom led _main_me over to a Strang looking Jungle Room",
                  "_main_i entered themushroom's main home",
            }),

            new FeelMySoul("enemy_describe", new List<string>(){  
                "_main_name were way stronger than my people and could took them out with ease" ,
                "even though _main_i was a old man, he was tough",
                "_main_i`s  massive size and speed were far greater than _sub_me",
                "_main_i had deadly poisonous gas in his aresnel ",
                "_main_i have the brute strength of nothing everyone had ever faced before ",
                "_main_i had Incredible strength and abilities",
            }),

            new FeelMySoul("i_washurt", new List<string>(){
                 "I wanted to fight back but the poison was extremely lethal towards me" ,
                 "I had half a heart and was dodging each of its things left and right",
                 "I was getting extremely low",
                 "hit me off and it almost killed me",
                 "I was knocked down to only one heart",
                 "as soon as they hit I was blinded ah",
                 "I thought I was surely done for",
            }),

           new FeelMySoul("i_havepower", new List<string>(){
               "I ran in but out of anger I accidentally shot lava out everywhere wao I have lava Powers I can't control them",
               "and then i realized what is this i can fly",
            }),

            new FeelMySoul("i_needtool", new List<string>(){
               "wow i needed tools",
            }),

            new FeelMySoul("i_craftetable", new List<string>(){
               "so i chopped down some wood and made a crafting table i was able to craft a bunch of wooden tools",
            }),

            new FeelMySoul("i_washungry", new List<string>(){
               "i was really hungry and didn't want to eat any of the other mobs",
            }),

            new FeelMySoul("i_findfood", new List<string>(){
               "so i headed to a village and i looked around and was able to find food",
            }),

            new FeelMySoul("friend_come", new List<string>(){
               "i was approached by a villager",
            }),

            new FeelMySoul("i_talk_sorrystolefood", new List<string>(){
               "i'm sorry i'm just so hungry",
            }),

            new FeelMySoul("i_heardnoise", new List<string>(){
                "as nighttime came we heard a noise",
               "I was about to go to sleep for the night when I heard screams in the distance",
               "I heard loud howling going off in the distance",
                " I heard a strange noise from inside the cave",
            }),

                      
            
            //todo: sentences
            // python 1： 读取csv 文件，转变为给kimi 读取的 txt
            // python 2: 给kimi，返回txt
            // 修改txt
            // python 4: 输出句子分类到 csv
            // python 5: 输出事件顺序到 csv
        };

        return souls;
    }

    static void Prepare()
    {


        // 人物和角色动作不在这儿指定吗？
        // 至少要指定人物大致动作
        // 细致动作是WalkFast 或者WalkSlow
        // 粗任务是follow the map, 细动作可以是跑，走，或者停在那儿四处张望
        List<PS> pres = new List<PS>()
        {
            new PS("", PreScene.Spawn, new List<string>(){
            "on day one I spawned in as a baby "  }),



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

            // 你这个不得规定，主角是谁？在干啥？地点在哪儿？
            // 要在这儿规定吗
            new PS("", PreScene.Build_CraftTools, new List<string>(){
          "i needed to upgrade my tools so i mined cobblestone",}),

            new PS("", PreScene.Build_House, new List<string>(){
          "i needed a home of my own",}),

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

