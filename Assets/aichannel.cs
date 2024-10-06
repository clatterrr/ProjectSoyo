using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
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
            this.word = words ;
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

        public ActorDesc(MaterialType type,  GameObject actor)
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



    struct SSTR
    {
        public Sblue sbp;
        public List<string> contents;
        public SSTR(Sblue sbp, List<string> contents)
        {
            this.sbp = sbp;
            this.contents = contents;
        }
    }

    // 基本都是图片
    enum MaterialType
    {
        Word,

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

        FastLeftToRight,

        AnyFast,
    }

    enum AreaEffect
    {
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

    public TextMeshPro tmp;

    //https://www.youtube.com/watch?v=FlizQ57zPAw 要的素材就两种，一种是搜索浏览，要整体，以及分别点开。第二种是个人主页，整体，简介信息，以及分别点开
    void addActorMove(MaterialType type, ActorEffect effect, int startFrame, int endFrame, Vector2 spos, Vector2 epos, float sangle, float eangle, float sscale, float escale)
    {
        for(int i = 0; i < actorDesc.Count; i++)
        {
            if(actorDesc[i].type == type)
            {
                if(type == MaterialType.Word)
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

    List<Vector2> ComputeAreaEffect(AreaEffect theAreaEffects, int effectIndex, Vector2 oriPos)
    {
        Vector2 startPos = oriPos;
        Vector2 endPos = oriPos;
        Vector2 rot = Vector2.zero;
        Vector2 scale = Vector2.one;
        switch (theAreaEffects)
        {
            case AreaEffect.TwoLeftRight:
                {
                    if (effectIndex % 2 == 0)
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

    void Start()
    {
        
        actorDesc.Add(new ActorDesc(MaterialType.IconYoutube, CreateCubeWithImage("Assets/AutoImages/youtube.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Avatar, CreateCubeWithImage("Assets/AutoImages/Avatar.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Clock, CreateCubeWithImage("Assets/AutoImages/clock.gif")));
        actorDesc.Add(new ActorDesc(MaterialType.Grid, CreateCubeWithImage("Assets/AutoImages/grid.png")));
        actorDesc.Add(new ActorDesc(MaterialType.Word, null));

        string specialFolderName = "Assets/AutoImages/";
        actorDesc.Add(new ActorDesc(MaterialType.Account,  CreateCubeWithImage(specialFolderName + "accountMainPage.png")));
        actorDesc.Add(new ActorDesc(MaterialType.VideoList, CreateCubeWithImage(specialFolderName + "VideoList.png")));

        // 什么样的keyword 应该有什么样的小反应。大反应是直接更换背景图
        // 到底是word 还是 actor
        keyWords.Add(new KeyWord(MaterialType.IconYoutube, SpecialWord.platform, "youtube", WordEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "you", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "your", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.Clock, SpecialWord.platform, "time", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.VideoList, SpecialWord.platform, "video", ActorEffect.Icon));
        keyWords.Add(new KeyWord(MaterialType.VideoList, SpecialWord.platform, "videos", ActorEffect.Icon));
        // keyWords.Add(new KeyWord(MaterialType.Account, SpecialWord.platform, "youtube", WordEffect.FullScreen));


        //keyWords.Add(new KeyWord(MaterialType.Avatar, SpecialWord.platform, "youtube", Effect.FullScreen));



        Random.InitState(123);
        updateStr();
        AddSelectMain(BluePrint.ThisChannleSucceed);
        AddSelect(new List<Sblue>() { Sblue.YouCanSucceed });
        Done();


        List<string> realContents = new List<string>();
        for (int i0 = 0; i0 < contents.Count; i0++)
        {
            for(int i1 = 0; i1 < contents[i0].smallBluePrint.Count; i1++)
            {
                Sblue sb = contents[i0].smallBluePrint[i1];
                for (int i2 = 0; i2 < sstrs.Count; i2++)
                {
                    if(sstrs[i2].sbp == sb)
                    {
                        int r = Random.Range(0, sstrs[i2].contents.Count);
                        Debug.Log(sstrs[i2].contents[r]);
                        realContents.Add(sstrs[i2].contents[r]);
                        break;
                    }
                }

            }
        }
        int textLength = 600;
        if (!enableVoice)
        {
            int strIndex = 0;
            int singleIndex = 0;
            for(int stri = 0; stri < contents.Count; stri++)
            {
                for(int sbp = 0; sbp < contents[stri].smallBluePrint.Count; sbp++)
                {
                    BluePrint bp = contents[stri].blueprint;
                    Sblue sb = contents[stri].smallBluePrint[sbp];
                    string str = realContents[strIndex];
                    int startFrame = strIndex * textLength;
                    int endFrame = strIndex * textLength + textLength;

                    string pattern = @"\[(.*?)\]";
                    MatchCollection matches = Regex.Matches(str, pattern);

                    List<AreaEffect> areaEffects = new List<AreaEffect>() {AreaEffect.TwoUpDown };
                    AreaEffect theAreaEffects = areaEffects[Random.Range(0, areaEffects.Count - 1)];

                    List<WordEffect> wordEffects = new List<WordEffect>() { WordEffect.LittleRotate };
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
                            List<Vector2> lv = ComputeAreaEffect(theAreaEffects, effectIndex, Vector2.zero);

                            for (int k = 0; k < actorDesc.Count; k++) if (actorDesc[k].type == MaterialType.Word)
                                {
                                    actorDesc[k].frames.Add(new ActorFrame(ActorEffect.Custom, WordEffect.Value, bracket,
                                        theStart, theStart + wordsInBracket.Count * 4 * durationForEachWord, lv[0], lv[1], 0, 0, 1, 1, 1000, 6000));
                                }
                                    
                            effectIndex++;
                        }


                        // find keywords
                        for (int k = 0; k < keyWords.Count; k++)
                        {
                            if (splited[i].ToLower() == keyWords[k].word.ToLower())
                            {
                                List<Vector2> lv = ComputeAreaEffect(theAreaEffects, effectIndex, Vector2.zero);
                                addActorMove(keyWords[k].MaterialType, ActorEffect.Icon, theStart, theStart + 4 * durationForEachWord, lv[0], lv[1], lv[2].x, lv[2].y, lv[3].x, lv[3].y);
                                effectIndex++;
                            }
                        }

                    }

                    strIndex++;
                }
            }
        }

        Debug.Log("length = " + actorDesc[4].frames.Count);
        for(int i = 0; i < actorDesc[4].frames.Count; i++)
        {
            ActorFrame af = actorDesc[4].frames[i];
            Debug.Log("wrod =" + af.word);
        }
    }


    int globalFrameCount = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        for(int i = 0; i < actorDesc.Count; i++)
        {
            bool findFrame = false;
            for(int j = 0; j < actorDesc[i].frames.Count; j++)
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
                        default: break;
                    }

                    if(actorDesc[i].type == MaterialType.Word)
                    {
                        switch (af.wordEffect)
                        {
                            case WordEffect.FromCornerToCenter: break;
                            case WordEffect.SideShrinkToCenter:
                                {
                                    float startSize = 80;
                                    float endSize = 20;
                                    tmp.fontSize = Mathf.Lerp(startSize, endSize, ratio);
                                    float startCharSpace = 10;
                                    float endCharSpaace = 0;
                                    tmp.characterSpacing = Mathf.Lerp(startCharSpace, endCharSpaace, ratio);

                                    break;
                                }
                            case WordEffect.LittleRotate:
                                {
                                    tmp.fontSize = 20;
                                    tmp.characterSpacing = 0;
                                    tmp.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(ratio * 20) * 10);
                                    break;
                                }
                            default: break;
                        }

                        tmp.text = af.word;
                        tmp.transform.position = new Vector3(targetPos.x, targetPos.y, 0);
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


            if (!findFrame)
            {
                if (actorDesc[i].type == MaterialType.Word) tmp.text = "";
                else actorDesc[i].actor.transform.position = new Vector3(1000, 1000, 1000);
            }
            
        }

        globalFrameCount++;
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
        YouShouldLearn,
        TheyAllSucceed,
        WhatIamTellingYou,
        WeMustUnderStandHe,
        ExploreHeWebsite,
        ItIsGood,
        WhatIsImportant,
        WhatShouldHappen,
        ItIsHard,
        
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

    }
    void updateStr()
    {
        // https://youtu.be/s-uP35zXVvo?t=4 快速走马灯
        sstrs.Add(new SSTR(Sblue.ThisChannelMakesMoney, new List<string>() { "it took 24 hours to make _bigmoney($260,000) with this _channelName(Instagram page)",
                    "my brand new Instagram account has grown from zero to over 250,000 followers in just 3 months",
                    "the account gets millions of views on almost every post",
                    "okay so there is a new way that people are earning $832 per week",
                    "and the best part is I've made over $10,000 from it so far",
                    "month one $88,500 month2 $113,000 and month six $ 29,90 per month",
            "I grew one of my Instagram pages to over 100,000 followers only using Ai and was able to make over $12,000 in passive income since starting this business",
        "and no I'm I'm not talking about YouTube ad Revenue I'm talking about the potential of making money with zero Instagram followers ",
            "sounds too good to be true right well using the exact strategy I grew one of my pages to over 100,000 followers and I'm already making consistent passive income",
        "so I recently came across this channel called downtown which makes videos on popular celebrity news for example Ben Affleck and Jennifer Lopez are currently trending so they created a video about it and in 3 days the video got 40,000 views which is more than my recent video",
        "so considering that the average RPM for celebrity news channels is between $125 and $4 this channel could be making anywhere from $750 to $2,400 per month and that's just the minimum I'm confident they're making much more",
        "these types of nature reels are going viral nowadays all over the Internet especially on Instagram this faceless Instagram account has gained over 400,000 subscribers in just a few months and is making thousands of dollars every month"}));
        sstrs.Add(new SSTR(Sblue.TheyWasInvitedToTheShow, new List<string>() { "the owner of this account literally got invited on _showName(Shark Tank) yeah the show" }));
        sstrs.Add(new SSTR(Sblue.YouCanSucceed, new List<string>() { "[what if] I told you that you could monetize time your YouTube shorts in just [one day]" }));

        sstrs.Add(new SSTR(Sblue.WhyISucceed, new List<string>() { "and the way I did that was by focusing on these motivational AI videos",
        "by combining free AI tools with canva"}));


        sstrs.Add(new SSTR(Sblue.HeHasALotRevenue, new List<string>() { "for how well she profited from this Instagram page again $260,000 24 hours from a page that has just 270 posts and 188,000 followers she made over $3 million in Revenue in a single year since launching it" }));
        sstrs.Add(new SSTR(Sblue.DataIsMore, new List<string>() { "and by the way all these stats are from November of 2023 so she very well might have already doubled that" }));
        sstrs.Add(new SSTR(Sblue.YouShouldLearn, new List<string>() { "if you are still sleeping on _platformName(faceless Instagram Pages) then I don't know what you're doing", 
            "but as a beginner the hard part is learning to do those things better than everyone else",
        "but please forget this for now because if you skip this video you won't be able to generate similar images with these prompts your results will not be the same you need to know some tricks also you won't be able to learn how to make money with a similar account and you will miss some valuable information to make at least $5,000 per month with these videos so please don't skip the last part"}));
        sstrs.Add(new SSTR(Sblue.TheyAllSucceed, new List<string>() { "there are several accounts using this exact same strategy that she's using to make millions today" }));
        sstrs.Add(new SSTR(Sblue.WhatIamTellingYou, new List<string>() { "I'm going to reverse engineer this entire business model for you and show you how you can replicate this success for free",
               "in fact I'm going to be showing you how you can create an entire clothing business line just like this one using a print on demand site called gelato later in this video",
        "and that is exactly what I'm going to show you for the rest of this video",
        "so in this video I'm going to reveal the account, I'm going to show you how I made money from it, and I'm going to give you the entire framework",
        "but good news if you follow the rest of the steps that I lay out in this video I promise that will never happen again",
        "if you want to create content that'll be pushed by the algorithm start using my viral checklist this is literally what I used to grow from 0 to 250,000 followers in 3 months",
        "'m going to walk you through my entire process and show you how you cas you can see the algorithm realized that this post wasn actually optimize your content and Trigger each of these checkboxes later in the video",
        "and this side of the business is what we are going to explore in today's video showing you the process step by step from the idea to the first dollar",
        "nd I'm going to start by showing you how to create these motivational stoic videos",// 联动
        "don't tell anyone I'm going to teach you that step by step",
        "I will show everything with proof of how they are making thousands of dollars with this account the best part of this Niche is that you can make a similar video in just a few minutes without spending much time on a script and voiceover ",
        "and my mission is to show you how to easily start a similar Channel with the help of AI",
        "I will show you how people make thousands of dollars using Ai and Google Trend I will also share with you a special technique for scaling this idea and making more money",

                                                                                             }));
        sstrs.Add(new SSTR(Sblue.WeMustUnderStandHe, new List<string>() { "but first we need to understand what she's selling and how she's doing it",
        "before we get into my workflow creation process you should understand how your favorite creators are tricking you into watching their videos and you can do all of these things"}));

        // website https://youtu.be/69uaGjCxvNQ?t=99
        sstrs.Add(new SSTR(Sblue.ExploreHeWebsite, new List<string>() { "if we come to her website we can see that she's selling merch these are t-shirts and hoodies " }));
        sstrs.Add(new SSTR(Sblue.ItIsGood, new List<string>() { "if we look at _websiteName website they're really nice" }));
        sstrs.Add(new SSTR(Sblue.WhatIsImportant, new List<string>() { "if we look at _websiteName website they're really nice",
        "where I I've learned one very important secret that none of the gurus are telling you",
        "the truth is you need a very strategic plan to get long-term results with YouTube automation", 
            "it's about having the right strategy from day one","so this is the most important part",
            "believe it or not choosing a good Niche is one of the most critical steps in this framework",
        "this is what I like to call a negative hook  nand it's one of the most powerful methods to hook people's attention",
        "this is really important if your account has less than 10,000 followers "
        }));
        sstrs.Add(new SSTR(Sblue.WhatShouldHappen, new List<string>() { "you need to get people to click on your video then watch your video, and finally enjoy their time watching your video",
        "well a good Niche needs to be something that is trending or that the world needs right now something that you can make money from something that you have skills and experience with"}));
        sstrs.Add(new SSTR(Sblue.ThinkAboutIt, new List<string>() { "you need to get people to click on your video then watch your video, and finally enjoy their time watching your video",
        "whenever you make a piece of content start by asking yourself these seven questions will someone stop scrolling and actually look at this post will someone spend a long time looking at this will someone want to save this for later will someone want to share this with a friend will someone comment something interesting on this will they feel satisfied that they consumed this and will they want to follow my account for more like this in the future if you can honestly answer yes to all seven of these questions then you have a viral post",
            "just think if you're passionate about something like high jump, that doesn't necessarily mean that high jump videos are a good Niche for Instagram,there are three other factors that are equally important",
        "before we get into my workflow creation process you should understand how your favorite creators are tricking you into watching their videos and you can do all of these things"}));
        sstrs.Add(new SSTR(Sblue.OtherGuruNotDoWell, new List<string>() { "coming up with good video ideas can be really tough",
            "as you can see the algorithm realized that this post wasn't getting the same level of attention and that it would be pointless for them to continue showing it to more and more people and this second situation is what I see happen to most people on Instagram they spend so much time creating content that isn't optimized for the algorithm which leads to them getting no views and then wondering what went wrong",
        "and I'm sure you've seen countless videos about the monetizable shorts but the sad truth is that not a single person has been able to monetize these videos to their maximum potential"}));

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


        sstrs.Add(new SSTR(Sblue.Special_Caption, new List<string>() {
            "first use keywords in your captions strategically because captions tie into a concept called SEO or search engine optimization",
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


        /*
         The next step is to capture attention, but not just any attention—don’t aim for going viral. Instead, focus on crafting content that resonates with your ideal customer. Begin by sharing helpful insights, whether it’s through tweets, reels, YouTube videos, shorts, or LinkedIn posts. The key is to offer value that's relevant to your audience, as this will help position you as an expert in your field. When people recognize that you genuinely know your stuff, they’ll be more inclined to trust you, which can lead to future sales. You'll need to consistently do this over time.
         
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

    string[] ideas = { "coming up with good video ideas can be really tough " , "but a good idea isn't everything you also need to peque their interest" };

    string[] how_to_solve_problem = { "so one of the best ways that I use to do it is to research " };

    string[] problems_post = { "and this is another place that most people mess up" };

    string[] theyDoWrong = { "a lot of small YouTubers start scripting their video and then they film it and then they edit and then thumbnails and then title but that is a huge mistake" };
 
    // https://youtu.be/vsYxKViDZSQ?t=48

    string[] niche = { "we can see that they have an average RPM of about $5 in case you don't know RPM stands for Revenue per Mila" };

    string[] money = { "so far, youtube has paid me over _money dollars" };
    string[] systems = { " and understand the system and processes that i use" };
    string[]  un = { "unfortunately, you`re not going to get rich making a 10 hours rain sound video, those days are over" };
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