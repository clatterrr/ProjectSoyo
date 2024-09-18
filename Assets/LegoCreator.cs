using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Structure;

public class LegoCreator : MonoBehaviour
{
    // Start is called before the first frame update
    enum LegoPreScene
    {
        ThrowAllFigures;
        IntroduceFigure;

        //https://youtu.be/oL0juE2YM38?t=252
        BuildFromGround,
        BuildFromSky,

        BrickPile,
    }

    enum LegoSenen
    {
        CameraFrontFarToClose, // 正面，左侧面，右侧面
    }

    enum BrickType
    {
        Brick1x1,
        Brick1x2,
        Brick1x3,
        Brick1x4,
    }

    enum BrickColor
    {
        White,
    }

    enum LegoFigureType
    {

    }

    void BuildHouse()
    {
        bool horizontal = false;

        List<Brick> bricks = new List<Brick>();


    }

    enum BuildingSytle
    {
        HasWindows,
        EmptyInside;
    }
    struct StandardBuildingSettings
    {
        BuildingSytle style;
        Uint3 size;
        Uint3 start;
    }

    private List<ActorSettings> actorSettings = new List<ActorSettings>();

    struct Brick
    {
        BrickType type;
        Color color;
        bool horizontal;
        Uint3 size;
        Uint3 start;

        public Brick(BrickType type, Color color, bool hori, Uint3 size, Uint3 start)
        {
            this.type = type;
            this.color = color;
            this.horizontal = hori;
            this.size = size;
            this.start = start;
        }
    }

    private List<Brick> bricks = new List<Brick>();

    void Build(StandardBuildingSettings settings)
    {
        bricks.Add(new Brick(BrickType.Brick1x1, Color.white, true, new Uint3(6,1,1), new Uint3(0,0,0)));
    }
    void Start()
    {

    }

    string[] introFigures = {"these are 100 lego children", "let`s begin by introduing the students who will soon be going ",
        "okay now that our school has begun. we can now introduce the first staff members that will be working here", // 医院，学校，
    "there's also the one spoiled rich kid",
    "the cool popular kid"
    };

    string[] next = { "so it`s the next thing i am going to build" };
     // 地图抄cs 1.6就行了
    string[] stores = { " so let's quickly run to the store and purchase them" };// 手推车

    string[] build_entrance = { "the first thing I'm going to build is the entrance to the school",
    "so in this corner of the school I'm going to build a Lego math class inside"，
            "and i even build all these desk for these students",

    };
     // Upda te is called once per frame
    void Update()
    {
        
    }
}
