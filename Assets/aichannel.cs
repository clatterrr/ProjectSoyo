using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Structure;
public class aichannel : MonoBehaviour
{
    // Start is called before the first frame
    // 

    struct ActorSettings2D
    {
        public GameObject actor;
        public int startFrame;
        public int endFrame;
        public Vector2 startPos;
        public Vector2 endPos;
        public float startRot;
        public float endRot;

     //   public ActorSettings2D(GameObject actor, int startFrame, int endFrame, Vector2 startPos, Vector2 endPos, float startRot, float endRot)
     //   {

      //  }
    }
    enum Effect
    {
        FromCornerToCenter,
    }

    enum ClipType
    {
        SingleVideoCover,
        Incoming,
        Speaker,
    }

    enum ASC
    {
        ScrollDownPages,
        ShowStatitics, // 频道的详情页
        Graph, // 步骤拆分
        Button, // 按钮
        Mouse, // 鼠标
        Memes,
        GuRu,
        FocusToStep, // 聚焦于图表上的一点
    }

    private float screenWidth;
    private float screenHeight;

    private Vector2 screenLeftDown;
    private Vector2 screenRightDown;
    private Vector2 screenLeftTop;
    private Vector2 screenRightTop;
    private Vector2 screenCenter;

    private List<ActorSettings2D> settings = new List<ActorSettings2D>();
    void Start()
    {

        ASC asc = ASC.ScrollDownPages;
        switch (asc)
        {
            case ASC.ScrollDownPages:
                {
                    // python control screen down
                    break;
                }
        }

        Effect ef = Effect.FromCornerToCenter;
        switch (ef)
        {
            case Effect.FromCornerToCenter:
                {
                    float lerp_value = 0.5f;
                 //   settings.Add(new ActorSettings2D(null, 0, 0, screenRightTop, Vector2.Lerp(screenLeftTop, screenCenter, lerp_value), 0, 0));

                 //   settings.Add(new ActorSettings2D(null, 0, 0, screenRightTop, Vector2.Lerp(screenLeftTop, screenCenter, lerp_value), 0, 0));

                //    settings.Add(new ActorSettings2D(null, 0, 0, screenRightTop, Vector2.Lerp(screenLeftTop, screenCenter, lerp_value), 0, 0));

                //    settings.Add(new ActorSettings2D(null, 0, 0, screenRightTop, Vector2.Lerp(screenLeftTop, screenCenter, lerp_value), 0, 0));
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    string[] notgood = { " but good idea is not everything" };
 
    // https://youtu.be/vsYxKViDZSQ?t=48

    string[] niche = { "we can see that they have an average RPM of about $5 in case you don't know RPM stands for Revenue per Mila" };

    string[] money = { "so far, youtube has paid me over _money dollars" };
    string[] systems = { " and understand the system and processes that i use" };
    string[]  un = { "unfortunately, you`re not going to get rich making a 10 hours rain sound video, those days are over" };
    string[] courses = { "i invested thousands of dollars into courses and my own projects" };
    string[] mess = { " there are tons of channels are doing mess", "you probabaly seen the guru how easy it is to do", "they usually drive around in an expensive cars and  they try to sell on the lifesytle rather than the actual process000" };
}

/*
 
https://youtu.be/kylpxijjvPg?t=170

ThinkImportant, WhatIsImport, IfNotThinkThenBad, SimpleThink, WrongThinkThenBad, GoodDoWell, BadDoNotWell (add data)
 
 */