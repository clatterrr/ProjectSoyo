using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static AnimationSystem;
using static Days100;
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

    // ������Ϊ AttackFriends, AttackEnemy, AttackMe
    // �������� FA, �������Щ���ﲻ��FAs�еģ�ȫ������ΪFalse,
    // һ��ʼ��Ҫ��ƺ���Щ������Щģ�ͣ���Ӧ��FA�еı����ʲô
    // Scene ��Ϊ���֣�������PreScene����������������¡�Ȼ��PreScene �����������������Ҫ���������λ�������ж�
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

    // �涨���е������ƶ���������������ת
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
    // SC �����ر�

    // �涨�ôֶ��㣬���������������
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

        ITalkPlace,
        EnemyTalkPlace,
        IBackPlace,
        EnemyPlace,

        RunAwayStart,
        RunAwayEnd,
        ChaseStart,
        ChaseEnd,

    }

    // follow empty and look some one

    // camera �в�ȥ���� CameraFollower ��λ��


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
        // һ��PreScene ���ܰ���������ܵ�Scene
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

    // �ϸ�����Enemy �� I
    // һ�仰�����������ɫ��




    private List<ActorSettings> actorSettings = new List<ActorSettings>();
    private List<CameraSetting> cameraSettings = new List<CameraSetting>();
    private static TerrianCreator terrainCreator;
    struct AllMyFellow
    {
        public List<string> actorNames;
        public List<GameObject> actors;
        public List<bool> actives;
        public List<Vector3> pos;
        public List<SP> startSP;
        public List<SP> endSP;
        public List<theAnim> anim;
        public AllMyFellow(int count)
        {
            this.actorNames = new List<string>();
            this.actors = new List<GameObject>();
            this.actives = new List<bool>();
            this.anim = new List<theAnim>();
            this.pos = new List<Vector3>();
            this.startSP = new List<SP>();
            this.endSP = new List<SP>();
        }
        public void Add(string actorName, string prefab_path)
        {

            if (actorName == "CameraFollow" || actorName == "CameraLookAt")
            {
                GameObject itsmygo = new GameObject(actorName);
                itsmygo.AddComponent<AnimationSystem>();
                actors.Add(itsmygo);
            }
            else
            {
                GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab_path);
               // actors.Add(Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.identity));
            }

            actives.Add(false);
            actorNames.Add(actorName);
            anim.Add(theAnim.Wait);
            pos.Add(Vector3.zero);
            startSP.Add(SP.None);
            endSP.Add(SP.None);
        }


        public GameObject GetActor(string actorName)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actorNames[i] == actorName)
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

        public void setState(string actorName, theAnim anim, Vector3 pos)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actorNames[i] == actorName)
                {
                    this.anim[i] = anim;
                    this.pos[i] = pos;
                }
            }
        }

        public void setSP(string actorName, SP sp0, SP sp1)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actorNames[i] == actorName)
                {
                    this.startSP[i] = sp0;
                    this.endSP[i] = sp1;
                }
            }
        }

        public FilmPos GetSP(string actorName)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actorNames[i] == actorName)
                {
                    return new FilmPos(GetPos(startSP[i]), GetPos(endSP[i]));
                };
            }
            return new FilmPos(Vector3.zero, Vector3.zero);
        }

    }

    struct FilmPos
    {
        public Vector3 startPos;
        public Vector3 endPos;

        public FilmPos(Vector3 start,  Vector3 end)
        {
            startPos = start;
            endPos = end;
        }

        public Vector3 StartOffset(float forward, float rightward, float upward)
        {
            Vector3 playerForward = (endPos - startPos).normalized;
            Vector3 playerRightward = Vector3.Cross(playerForward, Vector3.up);
            return startPos + playerForward * forward + playerRightward * rightward + Vector3.up * upward;
        }

        public Vector3 EndOffset(float forward, float rightward, float upward)
        {
            Vector3 playerForward = (endPos - startPos).normalized;
            Vector3 playerRightward = Vector3.Cross(playerForward, Vector3.up);
            return endPos + playerForward * forward + playerRightward * rightward + Vector3.up * upward;
        }
    }

    static Vector3 GetPos(SP sp)
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
        //fellow.setSP(actor1, sp0, sp1);
        //actorSettings.Add(addActorMove(startFrame, endFrame, fellow.GetActor(actor1), anim, terrainCreator.GetSPPos(sp0), terrainCreator.GetSPPos(sp1), fellow.GetActor(actor2)));
    }

    void actorFastMove(int startFrame, int endFrame, string actor, theAnim anim, SP sp0, SP sp1)
    {
        actorSettings.Add(addActorMove(startFrame, endFrame, fellow.GetActor(actor), anim, GetPos(sp0), GetPos(sp1), null));
    }

    void fastCamera(int startFrame, int endFrame, ActorType actor1, Vector3 pos0, Vector3 pos1)
    {
        /*
        ActorSettings newActor = addActorMove(startFrame, endFrame, fellow.GetActor(actor1), theAnim.Wait, pos0, pos1, null);
        newActor.type = actor1;
        actorSettings.Add(newActor);
        */
    }

    // ÿ���в�ͬ������

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
        // �����pre scene �𣿺��ɻ�
        // isTalkingToFriend + isEnemyAppear == EnemyAttackFriend and other house
        // isTalkingToFriend + IDefeatEnemy == FriendThankMe
        isTalkingToFriend,
        isHurt,

    }
    enum SBool
    {
        T,
        F,
        N//����Ҫ��ά��ԭ��
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
        // �������������
        public List<Status> MustMatchstatus;
        // ����ǿ�Ƹı������
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


    // ������ط�����ʲô
    // Travel , Fight And Ok
    // Travel , Fight And RunAway
    // Find Treasure
    // Find Treasure And Fight Ok
    // Rescure Friends And Fight
    // Building
    // Meet Friends And Get Task / Item

    // Ϊʲô��������ط�
    // Follow Map
    // Follow Friends
    // Find HideOut
    // Find Base

    List<CA> middleEvent2CameraMoveList(string middleEvent)
    {
        switch (middleEvent) {
            case "enemy_chargein": return new List<CA>() { CA.FollowActorAhead };
            default: return new List<CA> { CA.FollowActorAhead };
        }
    }

    enum CA
    {
        LookBehind,
        StaticAtStart,
        //https://youtu.be/jCgRV9bRBt8?t=341
        FollowCustomLookActorCloseToFar,
        // follow �� lookat ��ͬһ������ʡ��
        FollowActorAhead,
        LookAheadMainActor,
        //https://youtu.be/KuDtmmu4sow?t=41 
        // Enemy Threaten
        LookActorAheadAround,
        // https://youtu.be/KuDtmmu4sow?t=43
        //Two Fight
        LookCenterSlightlyMove,
        // https://youtu.be/KuDtmmu4sow?t=46
        // i_runaway
        LookBehindActorFollow,
        LookAheadActorFollow,
        // https://youtu.be/KuDtmmu4sow?t=72
        LookBehindActorStatic,
        LookAheadActorStatic,
    }
    void newStart()
    {

        fellow.Add("friend", "Assets/zombie.prefab");
        fellow.Add("dark monster", "Assets/zombie.prefab");
        fellow.Add("camera follow", "");
        fellow.Add("camera lookat", "");

        string[] fightPlaces = new string[] { "i_talk", "enemy_talk", "i_attack", "enemy_attack", "i_wasback", "enemy_wasback", "enemy_chargeinstart", "enemy_chargeinend" };
 
        List<TheEvent> theEvents =  GetStory();

        for(int bigIndex = 0; bigIndex <  theEvents.Count; bigIndex++)
        {
            TheEvent te = theEvents[bigIndex];
            // big event ��Ҫ�涨�� enemy ��˭��friend ��˭
            // ����һ���ṹ���У�
            string theEnemy = "dark monster";
            string theMe = "i";

            for(int middleIndex = 0; middleIndex < te.middleEvent.Count; middleIndex++)
            {
                string middleEvent = te.middleEvent[middleIndex];
                string smallEvent = te.smallEvent[middleIndex];

                middleEvent = middleEvent.ToLower();

                // todo: step1: actor move

                switch (middleEvent) {
                    case "enemy_chargein":{actorFastMove(0, 100, theEnemy, theAnim.ChargeIn, SP.ChargeInStart, SP.ChargeInEnd);break; }
                    case "enemy_talk_threaten": { actorFastMove(0, 100, theEnemy, theAnim.Idle, SP.EnemyTalkPlace, SP.ITalkPlace); break; }
                    // attack��λ��Ҳ����
                    case "enemy_attack": { actorFastMove(0, 100, theEnemy, theAnim.Attack0, SP.EnemyAttack, SP.IAttack); break; }
                    // hurt��λ��Ҳ����
                    case "i_washurt": { actorFastMove(0, 100, theMe, theAnim.Hurt, SP.IBackPlace, SP.EnemyAttack); break; }
                    case "i_attack": { actorFastMove(0, 100, theMe, theAnim.Attack0, SP.IAttack, SP.EnemyAttack); break; }
                    case "i_runaway": { actorFastMove(0, 100, theMe, theAnim.Walk, SP.RunAwayStart, SP.RunAwayEnd); break; }
                    // idle �Ļ���һֱ�� sp0�������� enemy talk place
                    case "i_talk_runaway":{ actorFastMove(0, 100, theMe, theAnim.Idle, SP.ITalkPlace, SP.EnemyTalkPlace);break;}

                    default: break;
                }

                // step2: camera move

                List<CA> cameraMoves = middleEvent2CameraMoveList(middleEvent);
                int cameraIndex = Random.Range(0, cameraMoves.Count - 1);
                CA cameraMove = cameraMoves[cameraIndex];

                switch (cameraMove)
                {
                    case CA.StaticAtStart:
                        {
                            break;
                        }
                    case CA.FollowCustomLookActorCloseToFar:
                        {
                            // Ҫ���ģ���ȡ�۾����ֵ�λ��
                            /*
                            List<SP> sps = fellow.GetSP(theMe);
                            Vector3 playerForward = (GetPos(sps[1]) - GetPos(sps[0])).normalized;
                            Vector3 startPos = GetPos(sps[0]) + playerForward * 2 + new Vector3(0, 2, 0);
                            Vector3 endPos = GetPos(sps[1]) + playerForward * 10 + new Vector3(0, 8, 0);
                            Debug.Log(" start pos = " + startPos + " end pos = " + endPos);
                            fastCamera(0, 100, ActorType.CameraFollow, startPos, endPos);
                            fastCamera(0, 100, ActorType.CameraLookat, GetPos(sps[0]), GetPos(sps[1]));
                            */

                            break;

                        }
                    case CA.LookBehindActorFollow:
                        {
                            // get i start pos and end pos
                            FilmPos film = fellow.GetSP(theMe);
                            float backwardScale = -2;
                            float upwardScale = 5;
                            float leftrightscale = 2;
                            Vector3 cameraStartPos = film.StartOffset(backwardScale, leftrightscale, upwardScale);
                            Vector3 cameraEndPos = film.EndOffset(backwardScale, leftrightscale, upwardScale);
                            fastCamera(0, 100, ActorType.CameraLookat, film.startPos, film.endPos);
                            fastCamera(0, 100, ActorType.CameraFollow, cameraStartPos, cameraEndPos);
                            break;
                        }
                    case CA.LookActorAheadAround:
                        {
                            FilmPos film = fellow.GetSP(theMe);
                            float backwardScale = 2;
                            float upwardScale = 5;
                            float leftrightscale = 2;
                            Vector3 cameraStartPos = film.StartOffset(backwardScale, leftrightscale, upwardScale);
                            Vector3 cameraEndPos = film.EndOffset(backwardScale, leftrightscale, upwardScale);
                            fastCamera(0, 100, ActorType.CameraLookat, film.startPos, film.endPos);
                            fastCamera(0, 100, ActorType.CameraFollow, cameraStartPos, cameraEndPos);
                            break;
                        }
                    case CA.LookCenterSlightlyMove:
                        {
                            FilmPos film0 = fellow.GetSP(theMe);
                            FilmPos film1 = fellow.GetSP(theMe);
                            FilmPos film2 = new FilmPos((film0.startPos + film1.startPos) * 0.5f, (film0.endPos + film1.endPos) * 0.5f);
                            float forwardScale = 2;
                            float rightScale = 2;
                            float upwardScale = 5;
                            Vector3 cameraStartPos = film2.StartOffset(forwardScale, rightScale, upwardScale);
                            Vector3 cameraEndPos = film2.EndOffset(forwardScale, rightScale, upwardScale);
                            fastCamera(0, 100, ActorType.CameraLookat, film2.startPos, film2.endPos);
                            fastCamera(0, 100, ActorType.CameraFollow, cameraStartPos, cameraEndPos);
                            break;
                        }
                    case CA.LookBehindActorStatic:
                        {
                            FilmPos film = fellow.GetSP(theMe);
                            float forwardScale = -2;
                            float rightScale = 2;
                            float upwardScale = 5;
                            Vector3 cameraPos = film.StartOffset(forwardScale, rightScale, upwardScale);
                            fastCamera(0, 100, ActorType.CameraLookat, film.startPos, film.endPos);
                            fastCamera(0, 100, ActorType.CameraFollow, cameraPos, cameraPos);
                            break;
                        }
                    case CA.LookAheadActorStatic:
                        {
                            FilmPos film = fellow.GetSP(theMe);
                            float forwardScale = 2;
                            float rightScale = 2;
                            float upwardScale = 5;
                            Vector3 cameraPos = film.EndOffset(forwardScale, rightScale, upwardScale);
                            fastCamera(0, 100, ActorType.CameraLookat, film.startPos, film.endPos);
                            fastCamera(0, 100, ActorType.CameraFollow, cameraPos, cameraPos);
                            break;
                        }
                    default: break;
                }

                if (middleEvent.Contains("talk"))
                {
                    // ֱ��ѡ˵�������ˣ�˵���ľ�ʽ�̶����ܼ�
                    string selectString = "i must get out of here";
                }
                else
                {

                    string[] splited = middleEvent.Split("_");
                    string selectString = smallEvent;

                    /*
                    for (int soulIndex = 0; soulIndex < souls.Count; soulIndex++)
                    {
                        string middleTemp = middleEvent.ToLower();
                        if (middleTemp == "enemy_attack_me") middleTemp = "i_attack";
                        if (souls[soulIndex].middle.ToLower() == middleTemp.ToLower())
                        {
                            int rii = Random.Range(0, souls[soulIndex].sentences.Count - 1);
                            selectString = souls[soulIndex].sentences[rii];
                            break;
                        }
                    }
                    */


                    // ����Ҫ�� _main �� _sub �滻�� _enemy _friend, _i


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

                    // ���������滻

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
                    Debug.Log(" small event before = " + smallEvent);
                    Debug.Log(" small event after = " + selectString);
                }
            }
        }

        // ���ܵĵص�
        string[] placeList = new string[] { "Valley", "Cave", "WaterFall", "Temple", "Hideout" };

        // Why Go To This Place

        string[] whyGoThisPlace = new string[] { "escape", "FollowMap", "FollowFriends", "OnTheWay", "SearchBase" }; // Find Treasure, Meet Friends, 

        // What To Do This Place�� ��ҪĿ��϶�����ս�����������

        string[] WhatToDoThisPlace = new string[] { "Detect", "hideForLive", "FindTreasure", "MeetFriends", "Fight", "Building", "RescureFriends" };

        // ���ѻ���������HelpFight, HelpFindTreasure, HelpFindPeople, JoinTeam
        // ���ѵĽ�����Item, Info, Map, Treasure, TeamMember

        // Ӧ�����¼�������Ӧ���Եص㣿
        // �����Եص�ɣ�����ͬһ���ص���������ͬ





        // ����MeetFriends �� RescureFriends �⣬ ������Friends �ĸ����� 0.02
        // ����RescureFriends ���������⣬�����������ѵĸ��ʲ�ȷ��
        // ��Ҫ�и��̶����ѣ�
        // ����Fight ��ٷֱ�����ս���⣬��������ս���ĸ�����0.6

        string[] HowToMeetFixedFriends = new string[] { "OnTheWay" };

        // Fixed Friends ��������������Ȼ��һ��Building
        // ֮����ǿ��Building��ʱ��������֣��������������Σ�������




        // ս�����ѡһ��
        string[] FightEnemyLists = new string[] { "Piglins", "Wolfs", "CaveMen", "Ghosts", "Spiders" };

        // WhenTravel ����ʧ�ܣ�һ��12��������ս��ռ8��

        // ���ѵ����֣���ݣ�

        string[] theScenes = new string[] { "Born_I_Enemy0", "Fight_I_Enemy0", "FriendsHelp_I", "FightWhenTravel", "FightTakeTreasure", "Growth", "Building", "FriendsTask", "FightWhenTravelFailed", "FriendsHelp", "Building", "FightHelpVillage", "FightTakeTreasure", "FightWithBoss" };


        // ս���ɹ��Ľ�־��� ʧ�ܾ���TrappedInCage������Killed

        // ֱ��ǿ�ƿ��£�Enemey ����Boss�⣬��׷���⣬�����Բ�һ��
        // Friends ���ַ��࣬��ĸ�� FixedFriendsӪ�Ȳ�һ���췿�ӣ�һ�����,   TemporalFriends Ӫ�ȸ����񣬸�����
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

        // ������һ���PreScene, PreScene ������һ�仰�ȽϺã����Ƕ�仰�ȽϺã�


        // GameObject ֱ�Ӷ�ȡԤ����
        terrainCreator = gameObject.GetComponent<TerrianCreator>();

        // ������٣�ÿ���ʲô


        List<PreSceneToScene> pscT = new List<PreSceneToScene>();
        pscT.Add(new PreSceneToScene(PreScene.Attack, new List<SC>() { SC.HeroEntrance }));
        pscT.Add(new PreSceneToScene(PreScene.Test_Found, new List<SC>() { SC.EnemyChaseFriendRun }));


        // pre scene �� scene �Ľ׶Σ�ֻ�涨�����Ҫ��˭���������Ҫ��ʲô���������涨ϸ�ڣ�ϸ�ڿ����
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

                    // ���������ҲҪ���

                    // psc == StartSmallScene
                    {

                        // Hero.SetPos(Entrance);
                    }
                }


            //question: when to set main actor, set sub actor
            // ���������õص�

            ActorType mainActorType = ActorType.Player;
            ActorType subActorType = ActorType.Friend;

            fellow.SetAllActiveFalse();
            switch (scenes[scene_index].sc)
            {
                /*
                // Scene �Ͳ�Ӧ�ð���Camera ��Ϣ��Ҫ�н�ɫ��Ϣ
                case SC.HeroEntrance: // ��ò�ҪEntrance
                    {
                        // idle Ӧ����PreScene
                        // ֱ��Idle ����Walk ������
                        //actorSettings.Add(new ActorSettings(0, 0, Plane, theAnim.Idle, SP.Entrance,m))
                        // λ����Idle, �������Ķ�Idle �أ� Ҫ���ݳ��������𣿵�����������ÿ��������������

                        // ����ҪFalse�������Ϳ����ˡ��г�����ʱ��Clear ������
                        //actorSettings.Add(addActorMove(0,10, fellow.GetActor(ActorType.Player), theAnim.SelfTalk, GetPos(SP.Entrance), GetPos(SP.Entrance), Camera.main));
                        break;
                    }
                case SC.EnemyEntrance:
                    {
                        SetHandActive(0, 10, false);
                        // ���ʱ��Ĭ�ϲ�д hero �ĵ���hero ������Idle����active == false
                        fastMove2(0, 10, ActorType.Enemy, theAnim.ChargeIn, SP.ChargeInStart, SP.ChargeInEnd, ActorType.Camera);
                        break;
                    }
                case SC.HeroFoundVillagersInDangerous:
                    {
                        Debug.Log(" in dangerous ");
                        PS ps = scenes[scene_index];
                        ps.ca = RandomList(new List<CA>() { CA.FollowCustomLookActorCloseToFar });
                        // ��Ȼ�������һֱFollow And Lookat������ҲҪ����Follow �� Lookat���˶��켣������Ӧ����һ��Random
                        scenes[scene_index] = ps;

                        SetHandActive(0, 10, false);

                        // �����fast Move2 ҲҪ�Ǹ�
                        // ���ʱ��Ĭ�ϲ�д hero �ĵ���hero ������Idle����active == false
                        fastMove2(0, 100, ActorType.Player, theAnim.Walk, SP.WatchOverPlaceFar, SP.WatchOverPlaceClose, ActorType.Camera);
                        break;
                    }
                case SC.OneTalkAnotherListen: // �����ôѭ���أ�Ӧ�ò���ѭ��
                    {
                        PS ps = scenes[scene_index];
                        ps.ca = RandomList(new List<CA>() { CA.LookAheadMainActor });
                        // ��Ȼ�������һֱFollow And Lookat������ҲҪ����Follow �� Lookat���˶��켣������Ӧ����һ��Random
                        scenes[scene_index] = ps;

                        SetHandActive(0, 10, false);

                        // �����fast Move2 ҲҪ�Ǹ�
                        // ���ʱ��Ĭ�ϲ�д hero �ĵ���hero ������Idle����active == false
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

                    */
            }


            switch (scenes[scene_index].ca)
            {
                
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

                // ������Ƶ�
                float C0x = easeFrame0 + deltaTime / 3f;  // C0 ��ʱ��
                float C0y = easeValue0 + outTangent * deltaTime / 3f;  // C0 ��ֵ

                float C1x = easeFrame1 - deltaTime / 3f;  // C1 ��ʱ��
                float C1y = easeValue1 - inTangent * deltaTime / 3f;   // C1 ��ֵ

                // ��ʱ���һ���� [0, 1]
                float u = (currentTime - easeFrame0) / deltaTime;

                // ���α�������ֵ��ʽ
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
                    //fellow.setState(set.type, set.animation, set.posEnd);
                }
            }
        }
        //Camera.main.transform.position = fellow.GetActor(ActorType.CameraFollow).transform.position;
        // Camera.main.transform.LookAt(fellow.GetActor(ActorType.CameraLookat).transform.position);


        /*
                
            ������һ���ܴ�� LargeBluePrint ����BuildHouse, TravelToPoint, FindTreasure, 
            (Attack) �Ǹ���
            
         */

    }
    
}

