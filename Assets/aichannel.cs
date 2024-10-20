using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Structure;
using Random = UnityEngine.Random;

public class aichannel : MonoBehaviour
{

    struct ActorFrame
    {
        public ActorEffect effect;
        public int startFrame;
        public int endFrame;
        public Vector2 startPos;
        public Vector2 endPos;
        public float startRot;
        public float endRot;
        public float startScale;
        public float endScale;
        public float startValue;
        public float endValue;

        public WordEffect wordEffect;
        public string word;

        public ActorFrame(ActorEffect effect, int startFrame, int endFrame, Vector2 startPos, Vector2 endPos,
            float startRot, float endRot, float startScale, float endScale)
        {
            this.effect = effect;
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.startPos = startPos;
            this.endPos = endPos;
            this.startRot = startRot;
            this.endRot = endRot;
            this.startScale = startScale;
            this.endScale = endScale;
            startValue = 0;
            endValue = 0;
            this.word = "";
            this.wordEffect = WordEffect.None;
        }

        public ActorFrame(ActorEffect effect, WordEffect wordEffect, string words, int startFrame, int endFrame, Vector2 startPos, Vector2 endPos,
    float startRot, float endRot, float startScale, float endScale, float startValue, float endValue)
        {
            this.effect = effect;
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.startPos = startPos;
            this.endPos = endPos;
            this.startRot = startRot;
            this.endRot = endRot;
            this.startScale = startScale;
            this.endScale = endScale;
            this.word = words;
            this.wordEffect = wordEffect;
            this.startValue = startValue;
            this.endValue = endValue;
        }
    }
    struct ActorDesc
    {
        public GameObject actor;
        public Vector3 baseScale;
        public MaterialType type;
        public List<ActorFrame> frames;

        public ActorDesc(MaterialType type, GameObject actor)
        {
            this.actor = actor;
            this.type = type;
            frames = new List<ActorFrame>();
            if (type != MaterialType.Word)
                baseScale = actor.transform.localScale;
            else baseScale = Vector3.one;
        }


    }



    private float screenWidth;
    private float screenHeight;

    private Vector2 screenLeftDown;
    private Vector2 screenRightDown;
    private Vector2 screenLeftTop;
    private Vector2 screenRightTop;
    private Vector2 screenCenter;

    private List<ActorDesc> actorDesc = new List<ActorDesc>();

    struct ProblemSet
    {
        public string problem;
        public string solver;
        List<int> children;
        int index;
    }




    struct ContentSelector
    {
        public BluePrint blueprint;
        public List<List<Sblue>> smallBluePrint;

        public ContentSelector(BluePrint print)
        {
            blueprint = print;
            smallBluePrint = new List<List<Sblue>>();
        }
    }

    List<ContentSelector> selectors = new List<ContentSelector>();
    List<aiContent> contents = new List<aiContent>();
    List<SSTR> sstrs = new List<SSTR>();

    List<SSSTR> theSentences = new List<SSSTR>();
    void AddSelectMain(BluePrint prints)
    {
        selectors.Add(new ContentSelector(prints));
    }
    void AddSelect(List<Sblue> prints)
    {
        ContentSelector cs = selectors[selectors.Count - 1];
        cs.smallBluePrint.Add(prints);
        selectors[selectors.Count - 1] = cs;
    }

    struct FrameRange
    {
        public int frameStart;
        public int frameEnd;
        public FrameRange(int frameStart, int frameEnd)
        {
            this.frameStart = frameStart;
            this.frameEnd = frameEnd;
        }
    }

    struct Channel
    {

        List<List<FrameRange>> actorChannel;
        public Channel(int x)
        {
            actorChannel = new List<List<FrameRange>>();
            for(int i = 0; i < 4; i++)
            {
                actorChannel.Add(new List<FrameRange>());
            }
        }

        public int GetChannel(int start, int end)
        {
            for(int k = 0; k < 4; k++)
            {
                bool could = true;
                for (int i = 0; i < actorChannel[k].Count; i++)
                {
                    if (start < actorChannel[k][i].frameEnd && start >= actorChannel[k][i].frameStart) { could = false; continue; }
                    if (end < actorChannel[k][i].frameEnd && end >= actorChannel[k][i].frameStart) { could = false; continue; }
                    if (end >= actorChannel[k][i].frameEnd && start <= actorChannel[k][i].frameStart) { could = false; continue; }
                }

                if (could) { actorChannel[k].Add(new FrameRange(start, end)); return k; }
            }
            return -1;
        }
    }

    void Done()
    {
        ContentSelector cs = selectors[selectors.Count - 1];
        int index = Random.Range(0, cs.smallBluePrint.Count - 1);
        contents.Add(new aiContent(cs.blueprint, cs.smallBluePrint[index]));
    }

    enum SpecialWord
    {
        platform,
    }
    struct aiContent
    {
        public BluePrint blueprint;
        public List<Sblue> smallBluePrint;
        public List<SpecialWord> words;
        public List<string> wordStr;
        public aiContent(BluePrint print, List<Sblue> sbp)
        {
            blueprint = print;
            smallBluePrint = sbp;
            words = new List<SpecialWord>();
            wordStr = new List<string>();
        }
    }


    enum BluePrint
    {
        ThisChannleSucceed,
        OtherGuruIsWrong,
        WhatIsMostImportant,
        BreakIntoParts,
        WhatIsProblem,
        HowToSolve,
        FinalEffort,
    }



    // six sigma https://youtu.be/4EDYfSl-fmc?t=50
    // https://youtu.be/vhGG2XDwAuE?t=90
    // 这种，时间，效率之类的很容易工业自动化
    // https://www.youtube.com/watch?v=SaZttbQUjLI 抽象方法
    // https://www.youtube.com/@escaping.ordinary/videos
    // 一般具体方法：就是标题
    // 特别具体方法：就是要点什么按钮

    //like this: https://youtu.be/s-uP35zXVvo?t=764
    enum Sblue
    {
        ThisChannelMakesMoney,
        YouCanSucceed,
        WhyISucceed,
        TheyWasInvitedToTheShow,
        HeHasALotRevenue,
        DataIsMore,
        IfNotThinkThenBad,
        GoodDoWell,
        YouSeeYouSucceed,
        TheyAllSucceed,
        WhatIamTellingYou,
        WeMustUnderStandHe,
        ExploreHeWebsite,
        ItIsGood,
        WhatIsImportant,
        WhatShouldHappen,
        ItIsHard,
        ListenToTheEnd,

        ThinkAboutIt, // 属于在WhatIsTheProblem, HowToSolveIt,
        OtherGuruNotDoWell,
        IthinkGoodButBad,
        SearchTrends,
        PreSummary,

        ButPowerful, // 说过了很多遍，但依然有用
        Analysis,
        AnalysisAttention,
        ItEasy,

        Frame_All,
        Frame_Next,

        Help_SaveYourTime,

        Special_Caption,
        Special_SEO,
        Special_Topic,
        Special_GPT,
        Special_Consistent,
        Special_Branding,
        Special_Hooking,
        Special_Script,
        Special_Adsense,
        Special_Affi,
        Special_Special,

        Special_RPM,

        Tool_Canva,//https://youtu.be/at_3hqBaDgI?t=292

    }

    struct SSSTR
    {
        public SSblue ssbp;
        public List<string> contents;
        public SSSTR(SSblue sbp, List<string> contents)
        {
            this.ssbp = sbp;
            this.contents = contents;
        }
    }
    struct SSTR
    {
        public Sblue sbp;
        public List<SSblue> contents;
        public SSTR(Sblue sbp, List<SSblue> contents)
        {
            this.sbp = sbp;
            this.contents = contents;
        }
    }

    // 基本都是图片
    enum MaterialType
    {
        Word,
        Gear,
        Light,

        IconYoutube,
        IconShopify,
        IconChatgpt,
        IconTiktok,
        IconYoutubeShorts,
        IconCapcut,

        Clock,
        Grid,

        Icon,
        Income,
        Avatar,

        Account, // 主页，也就是频道名词
        ScrollDown1, // 从详情页到浏览页
        ScroolDown2, // 更深入的浏览页
        VideoList,

        MainPage, // 网站主页
        MainContent, // 所有账户的战士
        Pricing, // 定价,

        // https://www.youtube.com/watch?v=OIKYzuxKVyk
        SucceedProduct,
        FailedProduct,
        CommentOnProduct,

    }


    enum ActorEffect
    {
        FromCornerToCenter,
        FullScreen,
        Icon,

        Custom,

        ZeroJump,

        FastLeftToRight,

        AnyFast,
    }

    enum AreaEffect
    {
        TwoLeftRightCamera,
        TwoLeftRight,
        TwoUpDown,
        OneStack,
    }
    enum WordEffect
    {
        FromCornerToCenter,
        FullScreen,
        Icon,
        //https://youtu.be/_uY-mpc1cQI?t=1
        SideShrinkToCenter,

        LittleRotate,
        Value,
        EachWordTarget,
        None,

    }
    struct KeyWord
    {
        public MaterialType MaterialType;
        public SpecialWord sp;
        public string word;
        public WordEffect wordEffect;
        public ActorEffect actorEffect;
        public KeyWord(MaterialType type, SpecialWord word, string theword, WordEffect effect)
        {
            this.MaterialType = type;
            this.sp = word;
            this.word = theword;
            this.wordEffect = effect;
            this.actorEffect = ActorEffect.Icon;
        }

        public KeyWord(MaterialType type, SpecialWord word, string theword, ActorEffect effect)
        {
            this.MaterialType = type;
            this.sp = word;
            this.word = theword;
            this.wordEffect = WordEffect.Icon;
            this.actorEffect = effect;
        }
    }

    List<KeyWord> keyWords = new List<KeyWord>();

    private List<GameObject> tmp;

    //https://www.youtube.com/watch?v=FlizQ57zPAw 要的素材就两种，一种是搜索浏览，要整体，以及分别点开。第二种是个人主页，整体，简介信息，以及分别点开
    void addActorMove(MaterialType type, ActorEffect effect, int startFrame, int endFrame, Vector2 spos, Vector2 epos, float sangle, float eangle, float sscale, float escale)
    {
        for (int i = 0; i < actorDesc.Count; i++)
        {
            if (actorDesc[i].type == type)
            {
                if (type == MaterialType.Word)
                {
                    actorDesc[i].frames.Add(new ActorFrame(effect, startFrame, endFrame, spos, epos, sangle, eangle, sscale, escale));
                }
                else
                {
                    actorDesc[i].frames.Add(new ActorFrame(effect, startFrame, endFrame, spos, epos, sangle, eangle, sscale, escale));
                }

            }
        }
    }

    List<Vector2> ComputeAreaEffect(AreaEffect theAreaEffects, int effectIndex, Vector2 oriPos, int startFrame, int endFrame, string str)
    {

        int channelIndex = channel.GetChannel(startFrame, endFrame);
        Debug.Log("start = " + startFrame + " str = " + str);
        Vector2 startPos = oriPos;
        Vector2 endPos = oriPos;
        Vector2 rot = Vector2.zero;
        Vector2 scale = Vector2.one;
        switch (theAreaEffects)
        {
            case AreaEffect.TwoLeftRight:
                {
                    if (channelIndex == 0)
                    {
                        startPos = new Vector2(-16, 0) + oriPos;
                        endPos = new Vector2(-4, 0) + oriPos;
                    }
                    else
                    {
                        startPos = new Vector2(16, 0) + oriPos;
                        endPos = new Vector2(4, 0) + oriPos;
                    }

                    break;
                }
            case AreaEffect.TwoLeftRightCamera:
                {

                    if (effectIndex % 2 == 0)
                    {
                        startPos = new Vector2(-16, 0) + oriPos;
                        endPos = new Vector2(-4, 0) + oriPos;
                        cameraSettings.Add(new CameraSetting(startFrame, endFrame, endPos));
                    }
                    else
                    {
                        startPos = new Vector2(16, 0) + oriPos;
                        endPos = new Vector2(4, 0) + oriPos;
                        cameraSettings.Add(new CameraSetting(startFrame, endFrame, endPos));
                    }

                    break;
                }
            case AreaEffect.TwoUpDown:
                {
                    if (effectIndex % 2 == 0)
                    {
                        startPos = new Vector2(0, -16) + oriPos;
                        endPos = new Vector2(0, -4) + oriPos;
                    }
                    else
                    {
                        startPos = new Vector2(0, 16) + oriPos;
                        endPos = new Vector2(0, 4) + oriPos;
                    }

                    break;
                }
            case AreaEffect.OneStack:
                {
                    scale = new Vector2(4, 1);
                    break;
                }
            default: break;
        }
        return new List<Vector2>() { startPos, endPos, rot, scale };
    }


    public bool enableVoice;
    List<CameraSetting> cameraSettings = new List<CameraSetting>();
    Channel channel = new Channel(0);
    //todo: start
    void Start()
    {

        tmp = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            tmp.Add(new GameObject("Word_" + i));
            tmp[i].AddComponent<TextMeshPro>();
        }

        GameObject clock = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AutoImages/alarm_0.prefab");
        actorDesc.Add(new ActorDesc(MaterialType.IconYoutube, CreateCubeWithImage("Assets/AutoImages/youtube.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Avatar, CreateCubeWithImage("Assets/AutoImages/Avatar.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Clock, Instantiate(clock, Vector3.zero, Quaternion.identity)));
        actorDesc.Add(new ActorDesc(MaterialType.Grid, CreateCubeWithImage("Assets/AutoImages/grid.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Gear, CreateCubeWithImage("Assets/AutoImages/gear.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Light, CreateCubeWithImage("Assets/AutoImages/light.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Word, null));

        string specialFolderName = "Assets/AutoImages/";
        actorDesc.Add(new ActorDesc(MaterialType.Account, CreateCubeWithImage(specialFolderName + "accountMainPage.png")));
        actorDesc.Add(new ActorDesc(MaterialType.VideoList, CreateCubeWithImage(specialFolderName + "VideoList.png")));

        // 什么样的keyword 应该有什么样的小反应。大反应是直接更换背景图
        // 到底是word 还是 actor
       // keyWords.Add(new KeyWord(MaterialType.IconYoutube, SpecialWord.platform, "youtube", WordEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Account, SpecialWord.platform, "channel", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "you", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "your", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "guys", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Clock, SpecialWord.platform, "time", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.VideoList, SpecialWord.platform, "video", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.VideoList, SpecialWord.platform, "videos", ActorEffect.Icon));
        // keyWords.Add(new KeyWord(MaterialType.Account, SpecialWord.platform, "youtube", WordEffect.FullScreen));


        //keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "youtube", Effect.FullScreen));
        List<SSTR> smallRules = new List<SSTR>();
        smallRules.Add(new SSTR(Sblue.ThisChannelMakesMoney, new List<SSblue>() { SSblue.ThisChannelMakesMoney_Intro,
            SSblue.ThisChannelMakesMoney_ThisChannel, SSblue.ThisChannelMakesMoney_HasFollowers, SSblue.ThisChannelMakesMoney_MakeMoney,
            SSblue.ThisChannelMakesMoney_WithTech, SSblue.ThisChannelMakesMoney_InShortTimes}));


        Random.InitState(123);
        updateStr();
        AddSelectMain(BluePrint.ThisChannleSucceed);
        AddSelect(new List<Sblue>() { Sblue.ThisChannelMakesMoney });
        Done();


        List<string> realContents = new List<string>();
        for (int i0 = 0; i0 < contents.Count; i0++)
        {
            for (int i1 = 0; i1 < contents[i0].smallBluePrint.Count; i1++)
            {
                Sblue sb = contents[i0].smallBluePrint[i1];
                for (int i3 = 0; i3 < smallRules.Count; i3++)
                {
                    if (smallRules[i3].sbp == sb)
                    {
                        // small blue to ssmall blue rules 的规则
                        for(int i2= 0; i2 < smallRules[i3].contents.Count; i2++)
                        {
                            SSblue detailedName = smallRules[i3].contents[i2];
                            for (int i4 = 0; i4 < theSentences.Count; i4++)
                            {
                                if (theSentences[i4].ssbp == detailedName)
                                {
                                    int r = Random.Range(0, theSentences[i4].contents.Count);
                                    Debug.Log(theSentences[i4].contents[r]);
                                    realContents.Add(theSentences[i4].contents[r]);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }

            }
        }
        int textLength = 200;
        if (!enableVoice)
        {
            for (int strIndex = 0; strIndex < realContents.Count; strIndex++)
            {
                    string str = realContents[strIndex];
                    int startFrame = strIndex * textLength;
                    int endFrame = strIndex * textLength + textLength;

                    string pattern = @"\[(.*?)\]";
                    MatchCollection matches = Regex.Matches(str, pattern);

                    List<AreaEffect> areaEffects = new List<AreaEffect>() { AreaEffect.TwoLeftRight };
                    AreaEffect theAreaEffects = areaEffects[Random.Range(0, areaEffects.Count - 1)];

                    List<WordEffect> wordEffects = new List<WordEffect>() { WordEffect.EachWordTarget };
                    WordEffect theWordEffects = wordEffects[Random.Range(0, wordEffects.Count - 1)];

                    // 计算字符串的总单词数（用来计算每个单词的时间片段）
                    string[] splited = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int durationForEachWord = textLength / splited.Length;
                    List<string> wordsInBracket = new List<string>();
                    string bracket = "";
                    int effectIndex = 0;
                    for (int i = 0; i < splited.Length; i++)
                    {

                        // the keywords
                        bracket = "";
                        wordsInBracket.Clear();
                        int theStart = startFrame + i * durationForEachWord;
                        if (splited[i].Contains("["))
                        {
                            while (!splited[i].Contains("]"))
                            {
                                wordsInBracket.Add(splited[i].Replace("[", ""));
                                bracket += splited[i].Replace("[", "") + " ";
                                i++;
                            }
                            wordsInBracket.Add(splited[i].Replace("]", ""));
                            bracket += splited[i].Replace("]", "") + " ";
                            List<Vector2> lv = ComputeAreaEffect(theAreaEffects, effectIndex, Vector2.zero, theStart, theStart + wordsInBracket.Count * 4 * durationForEachWord, bracket);

                            for (int k = 0; k < actorDesc.Count; k++) if (actorDesc[k].type == MaterialType.Word)
                                {
                                    actorDesc[k].frames.Add(new ActorFrame(ActorEffect.Custom, theWordEffects, bracket,
                                        theStart, theStart + wordsInBracket.Count * 4 * durationForEachWord, lv[0], lv[1], 0, 0, 1, 1, 1000, 6000));
                                }

                            effectIndex++;
                        }

                        // find keywords
                        for (int k = 0; k < keyWords.Count; k++)
                        {
                            if (splited[i].ToLower() == keyWords[k].word.ToLower())
                            {
                                List<Vector2> lv = ComputeAreaEffect(theAreaEffects, effectIndex, Vector2.zero, theStart, theStart + durationForEachWord, splited[i].ToLower());
                                addActorMove(keyWords[k].MaterialType, ActorEffect.ZeroJump, theStart, theStart + 4 * durationForEachWord, lv[0], lv[1], lv[2].x, lv[2].y, lv[3].x, lv[3].y);
                                effectIndex++;
                            }
                        }

                    }

            }
        }


        for (int i = 0; i < cameraSettings.Count - 1; i++)
        {

            if (cameraSettings[i].frameEnd < cameraSettings[i + 1].frameStart - 1)
            {
                // cameraSettings.Insert(i, new CameraSetting(cameraSettings[i].frameEnd + 1, cameraSettings[i + 1].frameStart, Vector3.zero, null, Vector3.zero, false));
            }
            else
            {

            }
            CameraSetting cs = cameraSettings[i];
            cs.frameEnd = cameraSettings[i + 1].frameStart;
            cameraSettings[i] = cs;
        }

        for (int i = 0; i < cameraSettings.Count - 1; i++)
        {
            Debug.Log(" i = " + i + " start + " + cameraSettings[i].frameStart + " end = " + cameraSettings[i].frameEnd + " pos = " + cameraSettings[i].posOffsetEnd);
        }
    }

    int globalFrameCount = 0;
    // Update is called once per frame
    // todo: update
    void FixedUpdate()
    {
        int gapFrames = 10;
        for (int i = 0; i < cameraSettings.Count; i++)
        {
            CameraSetting cs = cameraSettings[i];
            if (globalFrameCount >= cs.frameStart && globalFrameCount < cs.frameEnd)
            {
                Vector3 targePos = cs.posOffsetEnd;
                if (globalFrameCount - cs.frameStart < gapFrames)
                {
                    if (i > 0)
                    {
                        float ratio = (globalFrameCount - cs.frameStart + gapFrames) * 1.0f / (2.0f * gapFrames);
                        targePos = Vector3.Lerp(cameraSettings[i - 1].posOffsetEnd, cs.posOffsetEnd, ratio);
                    }
                }
                else if (cs.frameEnd - globalFrameCount < gapFrames)
                {
                    if (i < cameraSettings.Count - 1)
                    {
                        float ratio = 0.5f - (cs.frameEnd - globalFrameCount) * 1.0f / (2.0f * gapFrames);
                        targePos = Vector3.Lerp(cs.posOffsetEnd, cameraSettings[i + 1].posOffsetEnd, ratio);
                    }
                }
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
                Camera.main.transform.position = new Vector3(targePos.x, targePos.y, -10);


            }
        }

        for (int i = 0; i < actorDesc.Count; i++)
        {
            bool findFrame = false;
            for (int j = 0; j < actorDesc[i].frames.Count; j++)
            {
                ActorFrame af = actorDesc[i].frames[j];
                if (globalFrameCount >= af.startFrame && globalFrameCount < af.endFrame)
                {
                    float c = af.endFrame - af.startFrame;
                    float a = globalFrameCount - af.startFrame;
                    float ratio = 0;
                    if (a < c * 0.25f) ratio = 0;
                    else if (a > c * 0.75f) ratio = 1;
                    else
                    {
                        ratio = (a - c * 0.25f) / (c * 0.5f);
                    }

                    Vector2 targetPos = Vector2.Lerp(af.startPos, af.endPos, ratio);
                    float targetAngle = Mathf.Lerp(af.startRot, af.endRot, ratio);
                    float targetScale = Mathf.Lerp(af.startScale, af.endScale, ratio);

                    switch (af.effect)
                    {
                        case ActorEffect.Custom:
                            {
                                // Calculate offset vector (normal to the line connecting startPos and endPos)
                                Vector2 dir = (af.endPos - af.startPos).normalized;  // Direction vector from start to end
                                Vector2 normal = new Vector2(-dir.y, dir.x);         // Perpendicular (tangent is 0)

                                // Offset is 1/3 the distance between start and end positions
                                float offsetDistance = Vector2.Distance(af.startPos, af.endPos) / 3.0f;
                                Vector2 controlPos = af.startPos + (af.endPos - af.startPos) * 0.5f + normal * offsetDistance;

                                // Quadratic Bézier interpolation for position
                                targetPos = Mathf.Pow(1 - ratio, 2) * af.startPos
                                                    + 2 * (1 - ratio) * ratio * controlPos
                                                    + Mathf.Pow(ratio, 2) * af.endPos;

                                // Quadratic Bézier interpolation for rotation (no offset for rotation)
                                targetAngle = Mathf.Pow(1 - ratio, 2) * af.startRot
                                                    + 2 * (1 - ratio) * ratio * ((af.startRot + af.endRot) / 2)  // Midpoint control for smooth transition
                                                    + Mathf.Pow(ratio, 2) * af.endRot;
                                break;
                            }
                        case ActorEffect.ZeroJump:
                            {
                                List<float> scaleList = new List<float>() { 0, 1.2f, 1 };
                                List<float> timeList = new List<float>() { 0.1f, 0.2f, 0.3f };
                                float realScale = scaleList[0];
                                if (ratio < timeList[0]) realScale = scaleList[0];
                                if(ratio > timeList[timeList.Count - 1]) realScale = scaleList[scaleList.Count - 1]; 
                                for (int li = 0; li < timeList.Count - 1; li++){
                                    if(ratio >= timeList[li] && ratio < timeList[li + 1]){
                                        float r2 = (ratio - timeList[li]) / (timeList[li + 1] - timeList[li]);
                                        realScale = r2 * (scaleList[li + 1] - scaleList[li]) + scaleList[li];
                                       
                                        break;
                                    }
                                }
                                targetScale = realScale;
                                break;
                            }
                        default: break;
                    }

                    if (actorDesc[i].type == MaterialType.Word)
                    {
                        string theword = af.word;
                        string[] splited = theword.Split(" ");

                        switch (af.wordEffect)
                        {
                            case WordEffect.FromCornerToCenter: break;
                            case WordEffect.SideShrinkToCenter:
                                {
                                    float startSize = 80;
                                    float endSize = 20;

                                    float startCharSpace = 10;
                                    float endCharSpaace = 0;
                                    tmp[0].GetComponent<TextMeshPro>().characterSpacing = Mathf.Lerp(startCharSpace, endCharSpaace, ratio);
                                    tmp[0].GetComponent<TextMeshPro>().fontSize = Mathf.Lerp(startSize, endSize, ratio);


                                    break;
                                }
                            case WordEffect.LittleRotate:
                                {
                                    tmp[0].GetComponent<TextMeshPro>().fontSize = 20;
                                    tmp[0].GetComponent<TextMeshPro>().characterSpacing = 0;
                                    tmp[0].GetComponent<TextMeshPro>().transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(ratio * 20) * 10);
                                    break;
                                }
                            case WordEffect.EachWordTarget:
                                {
                                    tmp[0].GetComponent<TextMeshPro>().fontSize = 20;
                                    tmp[0].GetComponent<TextMeshPro>().characterSpacing = 0;
                                    tmp[0].GetComponent<TextMeshPro>().transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(ratio * 20) * 10);

                                    float bFrame = af.startFrame + (af.endFrame - af.startFrame) / 4f;
                                    float cFrame = (af.endFrame - af.startFrame) / 2f;
                                    float sFrame = cFrame / splited.Length;
                                    theword = "";
                                    for (int k = 0; k < splited.Length; k++)
                                    {
                                        tmp[k].GetComponent<TextMeshPro>().fontSize = 20;
                                        tmp[k].GetComponent<TextMeshPro>().characterSpacing = 0;
                                        tmp[k].GetComponent<TextMeshPro>().transform.rotation = Quaternion.Euler(0, 0, 0);

                                        float sstartFrame = bFrame + k * sFrame;
                                        float localRatio;
                                        if (globalFrameCount < sstartFrame) localRatio = 0;
                                        else if (globalFrameCount >= sstartFrame + sFrame) localRatio = 1;
                                        else localRatio = (globalFrameCount - sstartFrame) / sFrame;

                                        float voffset = Mathf.Lerp(-50f, 0f, localRatio);
                                        theword = theword + "<voffset=" + voffset.ToString() + ">" + splited[k] + " ";
                                    }
                                    break;
                                }
                            case WordEffect.Value:
                                {
                                    float targetValue = Mathf.Lerp(af.startValue, af.endValue, ratio);
                                    theword = af.word.Replace("_money", targetValue.ToString());
                                    break;
                                }
                            default: break;
                        }
                        tmp[0].GetComponent<TextMeshPro>().text = theword;
                        //tmp[0].gameObject.transform.position = new Vector3(targetPos.x, targetPos.y, 0);
                    }
                    else
                    {
                        actorDesc[i].actor.transform.position = new Vector3(targetPos.x, targetPos.y, 0);
                        actorDesc[i].actor.transform.rotation = Quaternion.Euler(0, targetAngle, 180);
                        actorDesc[i].actor.transform.localScale = actorDesc[i].baseScale * targetScale;
                    }

                    findFrame = true;
                    break;
                }
            }


            if (!findFrame && actorDesc[i].type != MaterialType.Word) actorDesc[i].actor.transform.position = new Vector3(1000, 1000, 1000);

        }

        globalFrameCount++;
    }

    struct Tech
    {
        public List<string> names;
        public List<string> ables;
        public Tech(string[] names, string[] ables)
        {
            this.names = new List<string>();
            for(int i = 0; i < names.Length; i++)
            {
                this.names.Add(names[i]);   
            }
            this.ables = new List<string>();
            for(int i= 0;  i < ables.Length; i++)
            {
                this.ables.Add(ables[i]);
            }
            
        }
    }

    List<Tech> techList = new List<Tech>();
    enum SSblue
    {
        ThisChannelMakesMoney_Intro,
        ThisChannelMakesMoney_ThisChannel,
        ThisChannelMakesMoney_HasFollowers,
        ThisChannelMakesMoney_WithTech,
        ThisChannelMakesMoney_MakeMoney,
        ThisChannelMakesMoney_InShortTimes,

        ResearchChannel_ThisChannel,
        ResearchChannel_IsAbout,
        ResearchChannel_IsGood,

        YouCanDoIt_Intro,
        YouCanDoIt_YouCanDo,
        YouCandoIt_WithEasy,

        IReveal_Intro,
        IReveal_IReveal,
        IReveal_Abstract,
        IReveal_Detailed,
        IReveal_InTheVideo,

        GuruDoBad_Intro,
        GuruDoBad_ManyGuru,
        GuruDoBad_TakeMoney,
        GuruDoBad_DoBad,
        GuruDoBad_IDoGood,

        ThisIsPowerful_ThisStrategy,
        ThisIsPowerful_CanSucceed,
        ThisIsPowerful_InAnyCase,
        ThisIsPowerful_FreeEasy,
        ThisIsPowerful_PeopleDontKnow,

        // Detect Youtube Channel With Detail

        Special_NowItistime,
        Special_TheGoal,
        Special_KeyIsImportant,
        Special_OtherIsGood,
        Special_ThisIsBad,
        Special_ThanAllBad,
        Special_MostPeopleNotRealize,
        Special_MostPeopleDoBad,
        Special_WantProvenMethod,
        Special_UseProvenMethod,
        Special_FindKey,
        Special_ButHow,
        Special_PleaseStoppingDoBad,
        Special_PleaseDoGood,

        AnysisVideo_WatchVideo,
        AnysisVideo_InShortTime,
        AnysisVideo_HasFollowersView,
        AnysisVideo_MakeMoney,
        AnysisVideo_ThinkAboutIt,
        AnysisVideo_SomeGood,
        AnysisVideo_ButAllBad,
        AnysisVideo_ButHow,
        AnysisVideo_ViewrLeave,

        Tool_Any_Use,
        Tool_Any_UseDetailed,
        Tool_Any_UseDifferent,    
        Tool_Any_Result,
        Tool_Any_AllPlatform,
        Tool_Any_IUseThisPlatform,

        Topic_NicheShouldBeGood,
        Topic_AVG,

        Tool_Full,

        Research_SearchTrends,
        Research_YouShouldEnjoy,
        Research_ISearchSoMuch,
        Research_ISummary,

        Misc_NoOneTellingYou,

    }
    void updateStr()
    {
        techList.Add(new Tech(new string[] { "monetization strategy" }, new string[]{"monetize your YouTube channel", "monetize your first video on a brand new YouTube channel" }));

        theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_Intro, new List<string>() { "all right guys listen up", "crazy right" }));
        theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_ThisChannel, new List<string>() { "my brand new youtube channel", "this spaceless YouTube channel" }));
        theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_HasFollowers, new List<string>() { "has grown from zero to over 250,000 followers", "in just 3 months" }));
        theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_MakeMoney, new List<string>() { "made over $10,000 from it" }));
        theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_WithTech, new List<string>() { "only using Ai" }));
        theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_InShortTimes, new List<string>() { "it took 24 hours", "in the last few months" }));

        theSentences.Add(new SSSTR(SSblue.YouCanDoIt_Intro, new List<string>() { "what if I told you that" }));
        theSentences.Add(new SSSTR(SSblue.YouCanDoIt_YouCanDo, new List<string>() { "you could start making money on YouTube" }));
        theSentences.Add(new SSSTR(SSblue.YouCandoIt_WithEasy, new List<string>() { "from your very first video", "Even if you have zero subscribers" }));

        theSentences.Add(new SSSTR(SSblue.IReveal_Intro, new List<string>() {
            " I know it sounds unbelievable but stick with me",
            "in fact", "and that is exactly what" }));
        theSentences.Add(new SSSTR(SSblue.IReveal_IReveal, new List<string>() { 
            "I'm going to be showing you", "I'm going to show you", "i will show you",
            " I'll show you exactly",
            "I'm about to break down exactly",
            "I'm going to reveal a little known strategy that",
            "and my mission is to show you",
            "how I turned the small channel into a money-making machine",
            "I'm going to break down that strategy for you and show you exactly",
            "and I'm going to start by showing you", "I will show everything with proof of" }));
        theSentences.Add(new SSSTR(SSblue.IReveal_Abstract, new List<string>() { 
            "how you can create an entire clothing business line just like this one", 
            "how you can replicate this success for free", 
            "how I made money from it",
            "how it's done step by step",
            "how to scale this idea and making more money",
            "how you could walk through my entire process",
            "and give you the entire framework" }));
        theSentences.Add(new SSSTR(SSblue.IReveal_Detailed, new List<string>() { 
            "just like this one using a print on demand site called gelato", 
            "Even if you have zero subscribers", "with just 15,000 subscribers",
            "with you a special technique"}));
        theSentences.Add(new SSSTR(SSblue.IReveal_InTheVideo, new List<string>() { "for the rest of this video", "in this video", "by the end of this video " }));


        theSentences.Add(new SSSTR(SSblue.GuruDoBad_Intro, new List<string>() { "I know what you're probably thinking", "and I'm sure you've seen" }));
        theSentences.Add(new SSSTR(SSblue.GuruDoBad_ManyGuru, new List<string>() { "there are so many so-called gurus",
            "countless videos about the monetizable shorts",
            "there's already a few animal facts tutorials out on YouTube", }));
        theSentences.Add(new SSSTR(SSblue.GuruDoBad_TakeMoney, new List<string>() { "charge thousands of dollars to teach" }));
        theSentences.Add(new SSSTR(SSblue.GuruDoBad_DoBad, new List<string>() {
            "nobody else is talking about because it's that powerful and they don't want you to know using the same strategy",
            "but the sad truth is that not a single person has been able to monetize these videos to their maximum potential" }));
        theSentences.Add(new SSSTR(SSblue.GuruDoBad_IDoGood, new List<string>() { " but you're about to learn it for free" }));

        theSentences.Add(new SSSTR(SSblue.ThisIsPowerful_ThisStrategy, new List<string>() {
            "_StrategyName is a powerful strategy",
            "this unique monetization strategy Works" }));
        theSentences.Add(new SSSTR(SSblue.ThisIsPowerful_CanSucceed, new List<string>() { "that can help you _DoThing", "but it can be incredibly effective when done right" }));
        theSentences.Add(new SSSTR(SSblue.ThisIsPowerful_InAnyCase, new List<string>() { "regardless of how many subscribers you have zero 10 100", "even if you're just starting out" }));
        theSentences.Add(new SSSTR(SSblue.ThisIsPowerful_FreeEasy, new List<string>() { "the best part is you don't need to invest a single penny to get started" }));
        theSentences.Add(new SSSTR(SSblue.ThisIsPowerful_PeopleDontKnow, new List<string>() { "it's a technique that not many people know about" }));


        // topic title cover scripts
        // NowItIsTime -> IsImportant -> ThisChannelDoGood -> WhyDetail -> ThinkAboutIt
        // https://youtu.be/ZmZ2Yqc7RLY?t=176
        // NowItisTime -> TheGoal -> ThinkAbout -> _ isImportant  -> topic explain -> the goal 
        theSentences.Add(new SSSTR(SSblue.Special_NowItistime, new List<string>() { 
            "we need to start working on our videos and the first step is of course finding a _Name",
            "let's talk about content creat ation",
            }));
        theSentences.Add(new SSSTR(SSblue.Special_TheGoal, new List<string>() {
            "to maximize your growth on YouTube shorts",
            "your primary goal as a YouTube shorts Creator is to craft content that captivates your audience from start to finish",
            }));
        theSentences.Add(new SSSTR(SSblue.Special_KeyIsImportant, new List<string>() { 
            "your _name is literally 90% of the reason whether your video goes viral or not",
            "it's about having the right strategy from day one",
            "this is definitely the most important part of the video",
            " forget likes and comments Focus solely on one key metric",
            "aside from topic your script is what will make or break your video",
            "the topic of a video is the difference between 100 and 1 million views regardless of your script writing voice over and editing"
            "this is a super important step you need to master if you want your shorts to go viral",
            "so this is the most important part to have a good _name",
            "the most important thing is the structure of your video how you present the story",
            "a good _name is really important if your account has less than 10,000 followers",
            "a good topic is the core of all viral videos" }));
        theSentences.Add(new SSSTR(SSblue.Special_OtherIsGood, new List<string>() { 
            "you can have the most visually appealing video on all of YouTube",
            "lots of creators will start the video with some crazy hook to trick the viewer into watching it",
            "you can have the greatest editing in the world",
            "if you can consistently create shorts people actually want to watch your channel will blow up" }));
        theSentences.Add(new SSSTR(SSblue.Special_ThisIsBad, new List<string>() { 
            " but if it's on a topic no one cares about ",
            "if your hook is not interesting enough",
            "but if you have a shitty script",
            "and without it your video ",
            "neglect this and all work you put into your videos is a complete waste of time",
            "and then the video has nothing to do with it",
            "most YouTube automation channels that are similar to mine tell you to use chat GPT",
            "many channels create some random script with chat GPT and expect their video to go viral",
            "they create videos on topics they think other people are interested in" }));
        theSentences.Add(new SSSTR(SSblue.Special_ThanAllBad, new List<string>() { 
            "it won't get any views",
            "it doesn't matter",
            "trust me I've tried it you'll end up getting no views",
            "which is just not how it works",
            "the viewer will just scroll past it",
            "won't get any views",
            "when they're really not" }));
        theSentences.Add(new SSSTR(SSblue.Special_MostPeopleNotRealize, new List<string>() { "and this is where where I see a lot of channels go wrong" }));
        theSentences.Add(new SSSTR(SSblue.Special_MostPeopleDoBad, new List<string>() { "what if I told you that" }));

        // we want proven method -> we of course use proven method

        theSentences.Add(new SSSTR(SSblue.Special_WantProvenMethod, new List<string>() { "again here we also want to go with a proven concept" }));
        theSentences.Add(new SSSTR(SSblue.Special_UseProvenMethod, new List<string>() { "and with the AI tools that are available today we can actually create scripts based on a proven Concept in literally less than 5 minutes" }));
        theSentences.Add(new SSSTR(SSblue.Special_FindKey, new List<string>() { "what if I told you that" }));
        theSentences.Add(new SSSTR(SSblue.Special_ButHow, new List<string>() { "but how do you know what editing style you should go for" }));
        theSentences.Add(new SSSTR(SSblue.Special_PleaseStoppingDoBad, new List<string>() { "what if I told you that" }));
        theSentences.Add(new SSSTR(SSblue.Special_PleaseDoGood, new List<string>() { "think of your core audience you're trying to Target who they are and what do they actually want to watch" }));


        theSentences.Add(new SSSTR(SSblue.Misc_NoOneTellingYou, new List<string>() { "where I I've learned one very important secret that none of the gurus are telling you" }));

        // AnysisVideo_WatchVideo Content HasFolloerws MakeMoney InShortTime ButHow ThinkAboutIt

        // SomeGood SomeBad AllBad ViewerLeave https://youtu.be/vK92mLFJVJs?t=413

        // Strategy 
        // 

        // Tool
        // 
        // Create Script 
        // 

        /*
        =========================== Strategy ========================
        
        Copy And Paste
        https://youtu.be/Dbt94EhvRFo?t=179
        https://youtu.be/FBniLhnuJR0?t=160


        ======================== Topic =============================

        Niche

        https://youtu.be/VIu2dzOCRrw?t=471
        https://youtu.be/8XIeElXiYx4?t=23
        https://youtu.be/_0nm9_m3yyE?t=59

        Algorithm Recommandation

        https://youtu.be/s027zsl5d8Q?t=346

        Long form Video And Short Form Video
        https://youtu.be/s027zsl5d8Q?t=379

        Hook
        
        https://youtu.be/mFZrGTmPSQ0?t=283

        Retention
        https://youtu.be/s027zsl5d8Q?t=339
        https://youtu.be/lc4lygDmHbI?t=201

        SEO
        https://youtu.be/DRB8NJLAw6E?t=639

        Marketing
        https://youtu.be/ra0glXW0Aoo?t=557

        Topic 

        https://youtu.be/VIu2dzOCRrw?t=113
        https://youtu.be/lc4lygDmHbI?t=36

        
        Video Idea

        https://youtu.be/ra0glXW0Aoo?t=341

         =========================== Tool ===========================

        Search 

        https://youtu.be/_0nm9_m3yyE?t=59
        https://youtu.be/QNlL1wmEY9I?t=61 // 感觉和Topic差不多

        Trends

        https://youtu.be/_0nm9_m3yyE?t=154

        Anysis Channel
        https://youtu.be/8XIeElXiYx4?t=59

        Create Accout 
        
        https://youtu.be/LEJGFnjIWmQ?t=112
        https://youtu.be/ra0glXW0Aoo?t=269
        https://youtu.be/DRB8NJLAw6E?t=72

        View Money Statics
        https://youtu.be/VIu2dzOCRrw?t=87

        Tiktok CRP 
        
        https://youtu.be/km_xKwWBEr0?t=61

        Extract Thumbnail

        https://youtu.be/FBniLhnuJR0?t=250


        
        Shopify

        https://youtu.be/Gwtj5IoA18Q?t=501
        https://youtu.be/mFZrGTmPSQ0?t=136

        Image 

        Leonard https://youtu.be/LEJGFnjIWmQ?t=440
        https://youtu.be/LEJGFnjIWmQ?t=539
        https://youtu.be/aH-_PhP7_tI?t=178
        https://youtu.be/pVykPqQDIe8?t=405
        https://youtu.be/8XIeElXiYx4?t=111
        https://youtu.be/8XIeElXiYx4?t=131
        https://youtu.be/3Vzfoq73iYQ?t=565
        https://youtu.be/Gwtj5IoA18Q?t=294
        https://youtu.be/MgMoTXaWGRw?t=284
        https://youtu.be/pT3ZUZ2DL3g?t=88


        Voice Over 
        
        https://youtu.be/km_xKwWBEr0?t=351
        https://youtu.be/-nEQf9kywoQ?t=184
        https://youtu.be/ra0glXW0Aoo?t=384
        https://youtu.be/DRB8NJLAw6E?t=371
        https://youtu.be/pVykPqQDIe8?t=517
        https://youtu.be/8rZHPdttLM8?t=204
        https://youtu.be/VOfC13e3oHk?t=1542
        https://youtu.be/pT3ZUZ2DL3g?t=158
        https://youtu.be/QNlL1wmEY9I?t=213

        Generate Video

        https://youtu.be/-nEQf9kywoQ?t=125
        https://youtu.be/ZmL48JRVre8?t=314
        https://youtu.be/kQn4B4xwquE?t=188
        https://youtu.be/VIu2dzOCRrw?t=248
        https://youtu.be/_0nm9_m3yyE?t=171
        https://youtu.be/3Vzfoq73iYQ?t=416
        https://youtu.be/ORImeD8k3H4?t=580
        https://youtu.be/MgMoTXaWGRw?t=717
        https://youtu.be/VOfC13e3oHk?t=814
         
        Create Scripts
         
        https://youtu.be/km_xKwWBEr0?t=351
        https://youtu.be/LEJGFnjIWmQ?t=183
        https://youtu.be/DRB8NJLAw6E?t=308
        https://youtu.be/pVykPqQDIe8?t=317
        https://youtu.be/8XIeElXiYx4?t=100
        https://youtu.be/_0nm9_m3yyE?t=59
        https://youtu.be/_0nm9_m3yyE?t=578
        https://youtu.be/3Vzfoq73iYQ?t=188
        https://youtu.be/ORImeD8k3H4?t=251
        https://youtu.be/MgMoTXaWGRw?t=152
        https://youtu.be/VOfC13e3oHk?t=397
        https://youtu.be/VOfC13e3oHk?t=1780
        https://youtu.be/pT3ZUZ2DL3g?t=77
        https://youtu.be/QNlL1wmEY9I?t=131

        Edit

        https://youtu.be/aH-_PhP7_tI?t=431
        https://youtu.be/DRB8NJLAw6E?t=487
        https://youtu.be/uxyqzNdNE6Q?t=352
        https://youtu.be/pVykPqQDIe8?t=613
        https://youtu.be/VIu2dzOCRrw?t=274
        https://youtu.be/8XIeElXiYx4?t=162
        https://youtu.be/mFZrGTmPSQ0?t=495
        https://youtu.be/VOfC13e3oHk?t=291
        https://youtu.be/VOfC13e3oHk?t=700
        https://youtu.be/pT3ZUZ2DL3g?t=275
        https://youtu.be/QNlL1wmEY9I?t=375
        https://youtu.be/nsGvN6ZU-rE?t=404

        ThumbNail

        https://youtu.be/VIu2dzOCRrw?t=401

        Title Tags
        https://youtu.be/VIu2dzOCRrw?t=429

        Music
        https://youtu.be/8XIeElXiYx4?t=187

        

        Publishing

        https://youtu.be/ra0glXW0Aoo?t=534

        
        
         
         ====================================== Over ===========================
         */

        // https://youtu.be/znb3_gFSaac?t=89
        // TheyDoBad

        //https://youtu.be/LEJGFnjIWmQ?t=378

        theSentences.Add(new SSSTR(SSblue.Tool_Full, new List<string>() {
            " for _WhatItCanDo come to _PlatformName it has _WhatIsProduce." +
            " sign up for for a free account and come to this page simply _HowToUse. _WhatIsProduce is perfect for our video" }));

        // ThisChannelMakeMoney -> IReveal -> 3FrameWork -> Niche -> Topic -> Scripts 

        /*
         *
         * ChatGpt Writting 
         * 
         * Pix Verse 
        
        Tool Structure

        _PlatformName = 11 labs
        _WhatIsProduce = perfect tones / voice
        _WhatItCanDo = creating a voice
        _HowToUse = copy your script one paragraph and paste
        
         step three creating the voice over for creating a voice come to 11 Labs it has perfect tone and sound just sign up for for a free account and come to this page simply copy your script one paragraph and paste it here this voice is perfect for our video 
         
         */

        theSentences.Add(new SSSTR(SSblue.AnysisVideo_WatchVideo, new List<string>() { 
            "take a look at this short and see if you can guess what mistake this Creator made",
            "in my case I saw a huge surge in viral content related to health specifically from Andrew huberman",
            "let me give you an example"}));
        theSentences.Add(new SSSTR(SSblue.AnysisVideo_InShortTime, new List<string>() { "this short was posted a couple of days ago",
        "I posted this video a couple of weeks ago"}));

        theSentences.Add(new SSSTR(SSblue.AnysisVideo_HasFollowersView, new List<string>() {
            "he's also a m of following of millions of people across different social media platforms",
            "and today it has close to a million views", " and it was a great video I thought at least" }));
        theSentences.Add(new SSSTR(SSblue.AnysisVideo_MakeMoney, new List<string>() { "now don't get me wrong this is a good topic" }));
        theSentences.Add(new SSSTR(SSblue.AnysisVideo_ButHow, new List<string>() {
            "so why did that video perform so much better even though the quality was worse",
            "but what is it that made this hook perform so well let's break it down" }));

        theSentences.Add(new SSSTR(SSblue.AnysisVideo_ThinkAboutIt, new List<string>() {
            "hink of your core audience what experience are they looking for is it a slow samul Style video or a highp short like Mr Beast",
            "understanding the current algorithm is crucial",
            "let's stop right here before I reveal it just think to yourself if you can spot it drop a comment if you think you know and don't cheat" }));
        theSentences.Add(new SSSTR(SSblue.AnysisVideo_SomeGood, new List<string>() { 
            "now don't get me wrong this is a good topic",
            
        }));
        theSentences.Add(new SSSTR(SSblue.AnysisVideo_ButAllBad, new List<string>() { "but it still would have never gone viral on any platform" }));
        theSentences.Add(new SSSTR(SSblue.AnysisVideo_ViewrLeave, new List<string>() { "most viewers will just scroll past once they get the answer they came for which will kill your attention\n",
        "don't do this it'll just leave the viewer frustrated and they'll probably click off before getting to the end of the video"}));

        // First Step -> Tool Any Use -> use Detailed -> Any Use Different -> Result https://youtu.be/7B4xwHaz6ag?t=77
        // _Tool: ChatGpt _Use: Image Generation _Goal: prompt _Material: bull and hybird
        theSentences.Add(new SSSTR(SSblue.Tool_Any_Use, new List<string>() { "you can easily generate these by using chat GPT" }));
        theSentences.Add(new SSSTR(SSblue.Tool_Any_UseDetailed, new List<string>() { "open chat GPT and ask for an image generation prompt for a bull and Scorpion hybrid" }));
        theSentences.Add(new SSSTR(SSblue.Tool_Any_UseDifferent, new List<string>() { "if you want to create different hybrid animals just replace Bull and Scorpion with the animals of your choice" }));
        theSentences.Add(new SSSTR(SSblue.Tool_Any_Result, new List<string>() { "now that you have the prompt " }));
        
        theSentences.Add(new SSSTR(SSblue.Tool_Any_AllPlatform, new List<string>() { "you can use platforms like Leonardo AI idiogram or mid Journey" }));
        theSentences.Add(new SSSTR(SSblue.Tool_Any_IUseThisPlatform, new List<string>() { "for this demonstration I'll use idiogram " }));

        theSentences.Add(new SSSTR(SSblue.Topic_NicheShouldBeGood, new List<string>() { " we need to conduct market research to ensure that we're selecting a niche with genuine viewer interest " }));
        theSentences.Add(new SSSTR(SSblue.Topic_AVG, new List<string>() { " we need to conduct market research to ensure that we're selecting a niche with genuine viewer interest " }));


        theSentences.Add(new SSSTR(SSblue.Research_SearchTrends, new List<string>() { " average view duration avd measures the time that the average viewer spends watching your video " }));
        theSentences.Add(new SSSTR(SSblue.Research_YouShouldEnjoy, new List<string>() { " as always try to find a topic that you genuinely enjoy otherwise you might risk getting bored or or losing motivation " }));
        theSentences.Add(new SSSTR(SSblue.Research_ISearchSoMuch, new List<string>() { "look after analyzing thousands of YouTube shorts" }));
        theSentences.Add(new SSSTR(SSblue.Research_ISummary, new List<string>() { "I identified two critical factors for success __" }));

        // theSentences.Add(new SSSTR(SSblue.ThisChannelMakesMoney_Intro, new List<string>() { }));
        /*
         * sstrs.Add(new SSTR(Sblue.WhatIsImportant, new List<string>() { "if we look at _websiteName website they're really nice",
        "where I I've learned one very important secret that none of the gurus are telling you",
        "the truth is you need a very strategic plan to get long-term results with YouTube automation",
            "it's about having the right strategy from day one","so this is the most important part",
            "believe it or not choosing a good Niche is one of the most critical steps in this framework",
        "this is what I like to call a negative hook  nand it's one of the most powerful methods to hook people's attention",
        "this is really important if your account has less than 10,000 followers "
        }));
         * 
         sstrs.Add(new SSTR(Sblue.OtherGuruNotDoWell, new List<string>() { "coming up with good video ideas can be really tough",
            "as you can see the algorithm realized that this post wasn't getting the same level of attention and that it would be pointless for them to continue showing it to more and more people and this second situation is what I see happen to most people on Instagram they spend so much time creating content that isn't optimized for the algorithm which leads to them getting no views and then wondering what went wrong",
        "and I'm sure you've seen countless videos about the monetizable shorts but the sad truth is that not a single person has been able to monetize these videos to their maximum potential",
        "I know what you're probably thinking there's already a few animal facts tutorials out on YouTube but here's the thing these so-called gurus are not transparent with you nobody shows you real step-by-step methods or how to create your first faceless YouTube business instead these clowns are just focused on their own views",
        "t's top secret stuff on YouTube nobody else is talking about because it's that powerful and they don't want you to know using the same strategy "}));

         * 
         *       
        "so in this video I'm going to reveal the account, I'm going to show you how I made money from it, and I'm going to give you the entire framework",
        "but good news if you follow the rest of the steps that I lay out in this video I promise that will never happen again",
        "if you want to create content that'll be pushed by the algorithm start using my viral checklist this is literally what I used to grow from 0 to 250,000 followers in 3 months",
        "i'm going to walk you through my entire process and show you how you cas you can see the algorithm realized that this post wasn actually optimize your content and Trigger each of these checkboxes later in the video",
        "and this side of the business is what we are going to explore in today's video showing you the process step by step from the idea to the first dollar",
        "and I'm going to start by showing you how to create these motivational stoic videos",// 联动
        "so in this video I'm going to break down that strategy for you and show you exactly how you can do the same with almost no effort so you can also monetize your YouTube shorts channel in just one day",
        "don't tell anyone I'm going to teach you that step by step",
        "and by the end of this video I will show you exactly how you can do the same for your channel as well when I first started out I was earning",
        "first let's make a fun plan for a video today here's what I'm going to share with all of you", // abstract
        "I'm about to break down exactly how I turned the small channel into a money-making machine even with just 15,000 subscribers all right",
        "so in this video I'm going to show you exactly how you can also start your own similar animal facts faceless YouTube channel using Ai and don't worry you'll get super high quality videos with my step-by-step neverbe seen strategy even with AI",
        "I will show everything with proof of how they are making thousands of dollars with this account the best part of this Niche is that you can make a similar video in just a few minutes without spending much time on a script and voiceover ",
        "and my mission is to show you how to easily start a similar Channel with the help of AI",
        "I will show you how people make thousands of dollars using Ai and Google Trend I will also share with you a special technique for scaling this idea and making more money",

                                                                                             }));
        // https://youtu.be/s-uP35zXVvo?t=4 快速走马灯
        sstrs.Add(new SSTR(Sblue.ThisChannelMakesMoney, new List<string>() { "it took 24 hours to make _bigmoney($260,000) with this _channelName(Instagram page)",
                    "{my brand new Instagram account} {has grown from zero to over 250,000 followers} {in just 3 months}",
                    "the account gets millions of views on almost every post",
                    "all right guys listen up in the last few months this spaceless YouTube channel has raked in over $60,000 just from uploading these simple animal facts videos and get this all these videos are sponsored",
                    "okay so there is a new way that people are earning $832 per week",
                    "and the best part is I've made over $10,000 from it so far",
                    "month one $88,500 month2 $113,000 and month six $ 29,90 per month",
                    "crazy right now if we take a closer look at this channel you'll see that almost all the videos have close to 1 million views and the most popular ones have over 30 million views these videos are still making money today that's insane passive income like seriously freaking insane",
            "I grew one of my Instagram pages to over 100,000 followers only using Ai and was able to make over $12,000 in passive income since starting this business",
        "and no I'm I'm not talking about YouTube ad Revenue I'm talking about the potential of making money with zero Instagram followers ",
            "sounds too good to be true right well using the exact strategy I grew one of my pages to over 100,000 followers and I'm already making consistent passive income",
        "so I recently came across this channel called downtown which makes videos on popular celebrity news for example Ben Affleck and Jennifer Lopez are currently trending so they created a video about it and in 3 days the video got 40,000 views which is more than my recent video",
        "so considering that the average RPM for celebrity news channels is between $125 and $4 this channel could be making anywhere from $750 to $2,400 per month and that's just the minimum I'm confident they're making much more",
        "these types of nature reels are going viral nowadays all over the Internet especially on Instagram this faceless Instagram account has gained over 400,000 subscribers in just a few months and is making thousands of dollars every month"}));
        sstrs.Add(new SSTR(Sblue.TheyWasInvitedToTheShow, new List<string>() { "the owner of this account literally got invited on _showName(Shark Tank) yeah the show",
        "when I first started out I was earning absolutely nothing exactly not, not a single penny but I didn't let that discourage me I knew I had to keep pushing keep improving my content and eventually something would click and that's exactly what happened I kept grinding putting out video after video Until finally one of them caught fire it was this video right here"}));
        sstrs.Add(new SSTR(Sblue.YouCanSucceed, new List<string>() { "[what if] I told you that you could monetize time your YouTube shorts in just [one day]" }));

        sstrs.Add(new SSTR(Sblue.WhyISucceed, new List<string>() { "and the way I did that was by focusing on these motivational AI videos",
        "by combining free AI tools with canva"}));


        sstrs.Add(new SSTR(Sblue.HeHasALotRevenue, new List<string>() { "for how well she profited from this Instagram page again $260,000 24 hours from a page that has just 270 posts and 188,000 followers she made over $3 million in Revenue in a single year since launching it" }));
        sstrs.Add(new SSTR(Sblue.DataIsMore, new List<string>() { "and by the way all these stats are from November of 2023 so she very well might have already doubled that" }));
        sstrs.Add(new SSTR(Sblue.YouSeeYouSucceed, new List<string>() { "if you are still sleeping on _platformName(faceless Instagram Pages) then I don't know what you're doing",
            "but as a beginner the hard part is learning to do those things better than everyone else",
            "people usually pay thousands of dollars for this information by buying courses and they still don't reveal what I'm going to reveal to you today but don't worry I'm going to give it to you for free",
            "I'm also going to reveal one last trick at the end of the video that I've learned from running multiple faceless YouTube channels this tip will ow you to actually make money from your first video onwards yes it's true the last trick I'm going to share with you will blow your mind and it's going to allow you to monetize your animal facts faceless YouTube channel from the first video onwards",
            "but guess what if you have the patience to watch this video Until the End you'll have the knowledge needed to start your own animal faceless YouTube automation business just like the channel",
        "but please forget this for now because if you skip this video you won't be able to generate similar images with these prompts your results will not be the same you need to know some tricks also you won't be able to learn how to make money with a similar account and you will miss some valuable information to make at least $5,000 per month with these videos so please don't skip the last part"}));
        sstrs.Add(new SSTR(Sblue.TheyAllSucceed, new List<string>() { "there are several accounts using this exact same strategy that she's using to make millions today" }));

        sstrs.Add(new SSTR(Sblue.WeMustUnderStandHe, new List<string>() { "but first we need to understand what she's selling and how she's doing it",
        "before we get into my workflow creation process you should understand how your favorite creators are tricking you into watching their videos and you can do all of these things"}));

        // website https://youtu.be/69uaGjCxvNQ?t=99
        sstrs.Add(new SSTR(Sblue.ExploreHeWebsite, new List<string>() { "if we come to her website we can see that she's selling merch these are t-shirts and hoodies " }));
        sstrs.Add(new SSTR(Sblue.ItIsGood, new List<string>() { "if we look at _websiteName website they're really nice" }));
        sstrs.Add(new SSTR(Sblue.ListenToTheEnd, new List<string>() { "make sure you stick around till the end",
        "so stick around because you don't want to miss these amazing secrets on how I made my channel a big hit with just 15,000 subs"}));

        sstrs.Add(new SSTR(Sblue.WhatShouldHappen, new List<string>() { "you need to get people to click on your video then watch your video, and finally enjoy their time watching your video",
        "well a good Niche needs to be something that is trending or that the world needs right now something that you can make money from something that you have skills and experience with"}));
        sstrs.Add(new SSTR(Sblue.ThinkAboutIt, new List<string>() { "you need to get people to click on your video then watch your video, and finally enjoy their time watching your video",
        "whenever you make a piece of content start by asking yourself these seven questions will someone stop scrolling and actually look at this post will someone spend a long time looking at this will someone want to save this for later will someone want to share this with a friend will someone comment something interesting on this will they feel satisfied that they consumed this and will they want to follow my account for more like this in the future if you can honestly answer yes to all seven of these questions then you have a viral post",
            "just think if you're passionate about something like high jump, that doesn't necessarily mean that high jump videos are a good Niche for Instagram,there are three other factors that are equally important",
        "before we get into my workflow creation process you should understand how your favorite creators are tricking you into watching their videos and you can do all of these things"}));

        sstrs.Add(new SSTR(Sblue.ItIsHard, new List<string>() { "coming up with good video ideas can be really tough" }));

        sstrs.Add(new SSTR(Sblue.IthinkGoodButBad, new List<string>() { "so even though I think it's one of the best videos in terms of quality and information it's still flopped",
            "but then when I try to talk about Tik Tok it doesn't really do well" }));
        sstrs.Add(new SSTR(Sblue.SearchTrends, new List<string>() { "so one of the best ways that I use to do it is to research the trends in my Niche",
            "then I cross check them with keyword softwares to validate that those are good ideas that people are interested in" }));
        sstrs.Add(new SSTR(Sblue.PreSummary, new List<string>() {
            "well I figured out a seven-part framework that can be applied to any account to generate followers like clockwork, and even make money, and it actually works" }));
        sstrs.Add(new SSTR(Sblue.ButPowerful, new List<string>() {
            "and I know some of you are probably rolling your eyes because you've heard this a thousand times",
            "but don't worry this is not going to be one of those videos that tells you to just go pick something that you're passionate",
        "often get overlooked because quite frankly they're a little boring but even though it might be Dole the information stored here has the power to take you from 10,000 views to over a million views on the exact same piece of content to generate as much organic reach as possible use the guidelines I'm about to share"}));
        sstrs.Add(new SSTR(Sblue.Analysis, new List<string>() {
            "notice how they start the video with a negative statement that indicates that you're doing something wrong. this makes you hyper aware of your problem so you keep watching hoping for a solution" }));
        sstrs.Add(new SSTR(Sblue.AnalysisAttention, new List<string>() {
            "the truth is you only have about 3 seconds to hook someone on Instagram if you waste This Time by introducing yourself or talking about something irrelevant you're never going to get any views" }));
        sstrs.Add(new SSTR(Sblue.ItEasy, new List<string>() {
            "as you can see this literally took me less than 60 seconds and you can do the same thing and make a post that gets over 100,000 likes just look at some of these viral posts from the top creators on Instagram all of them can be made in canva in less than 5 minutes",
        "and when it comes to my account that grew from 0 to 250,000 followers you can actually make videos like this inside of canva as well it does take a lot longer than this",
        "nd seriously I think that the strategy is so simple that it's actually genius yep so here is what they are doing",
        "and if you're wondering how I monetize these reals was by selling digital products related to my Niche this is a much easier and faster way to earn money as it bypasses requirements like the YouTube Partner program or Tik Tok creativity program"}));

        sstrs.Add(new SSTR(Sblue.Special_Adsense, new List<string>() {
            "irst I'll tell you exactly how much money I made from something called Google AdSense on my geekbot AI Channel" }));
        sstrs.Add(new SSTR(Sblue.Special_Affi, new List<string>() {
            "up to now it's pretty exciting next I'll let you in on how much money I earn just by telling people about some really cool products that's called affiliate marketing" }));
        sstrs.Add(new SSTR(Sblue.Special_RPM, new List<string>() {
            "you can expect an RPM Revenue premal anywhere from $5 to $30 for those who don't know RPM is basically how much money you make per 1,000 views on your videos now RPM can vary a ton depending on a few key factors " }));

        // xxx 不容易，但是我会教你最容易的方法(抽象)
        // 意识流，高效指南
        //
        // 最容易的方法是(具体)


        sstrs.Add(new SSTR(Sblue.Special_Caption, new List<string>() {
            "first use keywords in your captions strategically because captions tie into a concept called SEO or search engine optimization",
            "here's where it gets interesting naming your channel can be a pain in the ass " +
            "but don't worry we're using chat GPT to make this super easy open chat GPT " +
            "and type something like give me 10 cool names for an animal facts YouTube channel boom " +
            "you got a list of potential names and seconds " +
            "for example we're taking inspiration from our competitor zrank we might get names like wild Whispers creature Chronicles animal Antics 101 and nature nuggets " +
            "pick a name that resonates with you and sounds catchy as hell remember the name should be memorable and reflect the kind of content you're going to produce",
        "so the proven formula is simple if we find trending topics with high search volume and turn them into engaging videos we will start building an audience and eventually make money"}));
        sstrs.Add(new SSTR(Sblue.Special_SEO, new List<string>() {
            "if you aren't familiar with how SEO works on Instagram just think about searching something on Google when you search how to hard boil eggs on Google tons tons of articles are going to pop up explaining how to do it those articles likely have the keywords hard boil eggs or how to in them" }));
        sstrs.Add(new SSTR(Sblue.Special_Topic, new List<string>() {
            "so the first thing you need to do is decide on which topic you want to create videos about and to save you some time I recommend choosing a topic that falls under one of three categories health wealth or relationships" }));
        sstrs.Add(new SSTR(Sblue.Help_SaveYourTime, new List<string>() {
            "so to save you even more time I've created a list with over 300 ideas for each category feel free to check it out" }));
        sstrs.Add(new SSTR(Sblue.Special_GPT, new List<string>() {
            "using this formula chck GPT will generate five options that you can use and this bio optimization will boost the chances of people discovering your page so at this point your page should be ready" }));
        sstrs.Add(new SSTR(Sblue.Frame_Next, new List<string>() {
            "and we can move to the most fun part which is content creation" }));
        sstrs.Add(new SSTR(Sblue.Frame_All, new List<string>() {
            "you just need three things a collection of prompts text to image Ai and image to video AI I have created a list of the best prompts to generate amazing images" }));


        sstrs.Add(new SSTR(Sblue.Special_Consistent, new List<string>() {
            "but of course for this formula to work you must stay consistent in uploading downtown uploads two videos a day which is why they get so many views so even if not every video goes viral the momentum from consistent uploading will automatically grow the channel" }));
        sstrs.Add(new SSTR(Sblue.Special_Branding, new List<string>() {
            "When you give a TED Talk, you’re marketing. When you ask your boss for a raise, you’re marketing. When you raise money for the local playground, you’re marketing. And yes, when you’re trying to grow your division at work, that’s marketing too. For a long time, during the days when marketing and advertising were the same thing, marketing was reserved for vice presidents with a budget." ,
        "Marketers don’t use consumers to solve their company’s problem; they use marketing to solve other people’s problems. They have the empathy to know that those they seek to serve don’t want what the marketer wants, don’t believe what they believe, and don’t care about what they care about. They probably never will."}));
        sstrs.Add(new SSTR(Sblue.Special_Script, new List<string>() {
            "all right everyone listen up a good script can make make or break your video it's what keeps the viewers hooked engaged and coming back for more if your script sucks no one's sticking around no matter how flashy the visuals are so let's dive into how to write the perfect script for your animal facts video"}));

        sstrs.Add(new SSTR(Sblue.Special_Hooking, new List<string>() {
            "a script is the backbone of your video it ensures your content structured engaging and flows well hypertension means viewers watch your video longer which signals to YouTube that your content's worth promoting more promotion means more views and more views mean more money simple as"}));
        */
        /*
         The next step is to capture attention, but not just any attention—don’t aim for going viral. Instead, focus on crafting content that resonates with your ideal customer. Begin by sharing helpful insights, whether it’s through tweets, reels, YouTube videos, shorts, or LinkedIn posts. The key is to offer value that's relevant to your audience, as this will help position you as an expert in your field. When people recognize that you genuinely know your stuff, they’ll be more inclined to trust you, which can lead to future sales. You'll need to consistently do this over time.


        We tell stories. Stories that resonate and hold up over time. Stories that are true, because we made them true with our actions and our products and our services.

We make connections. Humans are lonely, and they want to be seen and known. People want to be part of something. It’s safer that way, and often more fun.

We create experiences. Using a product, engaging with a service. Making a donation, going to a rally, calling customer service. Each of these actions is part of the story; each builds a little bit of our connection. As marketers, we can offer these experiences with intent, doing them on purpose.
         */
    }

    //https://youtu.be/vsYxKViDZSQ?t=8
    string[] share = { "and today I'm going to expose the entire thing for free", };

    string[] quickmoney = { "this YouTube channel has made over $50,000 since it started just 2 months ago" };

    string[] goal = { "the goal was simple, i wanted to get monetized as soon as possible," , "what i learned was a important secret",
        "the truth is you need a very strategic plan to get long-term results",
        "it`s about having the right strategy from day one", "using a proven method that actually works",
        "and that`s what exactly what i`m going to show you for the rest of this video",
        "that take us into the first step of my process which is coming up a good idea",
        };

    string[] TheyDoGood = { " and look these channels who talk about Tik Tok they get crazy amounts of views" };

    string[] IDoBad = { "but then when I try to talk about Tik Tok it doesn't really do well" };

    string[] detailBanned = { "because about 15% of my audience live in" };

    string[] notgood = { " but good idea is not everything" };

    // 经典的句式，what is the problem, how to solve it。只有这个后面这个节点会分吧

    string[] ideas = { "coming up with good video ideas can be really tough ", "but a good idea isn't everything you also need to peque their interest" };

    string[] how_to_solve_problem = { "so one of the best ways that I use to do it is to research " };

    string[] problems_post = { "and this is another place that most people mess up" };

    string[] theyDoWrong = { "a lot of small YouTubers start scripting their video and then they film it and then they edit and then thumbnails and then title but that is a huge mistake" };

    // https://youtu.be/vsYxKViDZSQ?t=48

    string[] niche = { "we can see that they have an average RPM of about $5 in case you don't know RPM stands for Revenue per Mila" };

    string[] money = { "so far, youtube has paid me over _money dollars" };
    string[] systems = { " and understand the system and processes that i use" };
    string[] un = { "unfortunately, you`re not going to get rich making a 10 hours rain sound video, those days are over" };
    string[] courses = { "i invested thousands of dollars into courses and my own projects" };
    string[] mess = { " there are tons of channels are doing mess", "you probabaly seen the guru how easy it is to do", "they usually drive around in an expensive cars and  they try to sell on the lifesytle rather than the actual process000" };

    string[] NoPainNoGain = { "but I will say that YouTube automation is not the right fit for you if you're just looking for a get-rich scheme and not really putting much effort in" };



}

/*
 
todo: pipeline

https://youtu.be/kylpxijjvPg?t=170

ThinkImportant, WhatIsImport, IfNotThinkThenBad, SimpleThink, WrongThinkThenBad, GoodDoWell, BadDoNotWell (add data)
 

WhatIsImportant 

TheySucceed 
OtherGuruIsWrong

# -*- coding: utf-8 -*-# -*- coding: utf-8 -*-
"""
Created on Thu Oct  3 15:57:42 2024

@author: acer
"""

import os
import threading
import time
from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options
# 创建 ChromeOptions 实例
chrome_options = Options()

# 禁用“正在受自动软件控制”的提示
chrome_options.add_experimental_option('excludeSwitches', ['enable-automation'])

# 禁用其他提示和警告
chrome_options.add_experimental_option('excludeSwitches', ['enable-automation', 'enable-logging'])

# 禁用浏览器通知和警报
chrome_options.add_argument("--disable-infobars")
chrome_options.add_argument("--disable-notifications")
chrome_options.add_argument("--disable-extensions")
chrome_options.add_argument("--disable-popup-blocking")

# 关闭自动化相关的日志信息
chrome_options.add_argument("--log-level=3")

# 定义录制屏幕的函数
def record_screen():
    ffmpeg_path = "D:\\software\\xunjie\\XunJieVoice\\resources\\app\\ffmpeg.exe"
    os.system(ffmpeg_path + " -y -video_size 1920x1080 -f gdigrab -framerate 30 -i desktop -t 20 D:/output2.mp4")

# 定义浏览器滑动的函数
def scroll_browser():
    driver = webdriver.Chrome(executable_path='D:/wpw/Lalicat/chrome/119.0.6045.106/chromedriver.exe', options=chrome_options)  # 确保 chromedriver 在系统路径中
    driver.get('https://www.tiktok.com/@friqtao')  # 你需要访问的网页

    # 模拟缓慢滑动
    for _ in range(10):  # 滑动10次
        driver.find_element_by_tag_name('body').send_keys(Keys.PAGE_DOWN)
        time.sleep(1)  # 每次滑动停留1秒

# 创建两个线程：一个用于录制屏幕，另一个用于浏览器滑动
record_thread = threading.Thread(target=record_screen)
scroll_thread = threading.Thread(target=scroll_browser)

# 启动两个线程
record_thread.start()
scroll_thread.start()

# 等待两个线程都完成
record_thread.join()
scroll_thread.join()

"""
Created on Thu Oct  3 15:57:42 2024

@author: acer
"""

import azure.cognitiveservices.speech as speechsdk
import os
import json
subscription_key, region = "412aa3b510054e959968d5ea4459e829", "japaneast"

audio_filename = "D://voice.wav"
# 检查音频文件是否存在
if not os.path.exists(audio_filename):
    print(f"Error: The file '{audio_filename}' does not exist.")
else:
    # 创建语音配置
    speech_config = speechsdk.SpeechConfig(subscription=subscription_key, region=region)
    speech_config.request_word_level_timestamps()
    # 创建音频输入配置，从本地文件加载音频
    audio_input = speechsdk.AudioConfig(filename=audio_filename)
    
    # 创建语音识别器
    speech_recognizer = speechsdk.SpeechRecognizer(speech_config=speech_config, audio_config=audio_input)

    # 开始识别
    print("Recognizing from audio file...")

    # 进行一次性识别
    result = speech_recognizer.recognize_once()

    # 处理识别结果
    if result.reason == speechsdk.ResultReason.RecognizedSpeech:
        # print("Recognized: {}".format(result.json))
        
        # 解析 JSON 字符串
        data = json.loads(result.json)
        
        # 找到 confidence 最高的结果
        best_result = max(data['NBest'], key=lambda x: x['Confidence'])
        
        # 输出结果
        highest_confidence_lexical = best_result['Lexical']
        words_info = [
            {'Word': word['Word'], 'Offset': word['Offset'], 'Duration': word['Duration']}
            for word in best_result['Words']
        ]
        
        print("Highest Confidence Lexical:", highest_confidence_lexical)
        print("Words Info:", words_info)
        
    elif result.reason == speechsdk.ResultReason.NoMatch:
        print("No speech could be recognized: {}".format(result.no_match_details))
    elif result.reason == speechsdk.ResultReason.Canceled:
        cancellation_details = result.cancellation_details
        print("Speech Recognition canceled: {}".format(cancellation_details.reason))
        if cancellation_details.reason == speechsdk.CancellationReason.Error:
            print("Error details: {}".format(cancellation_details.error_details))
            print("Did you set the speech resource key and region values?")

 */

// 碰到Youtube Channel 