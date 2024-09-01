using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;
using Vector3 = UnityEngine.Vector3;
using System.Text.Json;
using System.Globalization;

public class ActorMoveManager : MonoBehaviour
{
    public GameObject c0;
    public GameObject c1;
    public GameObject c2;


    public GameObject o1;
    // 加物体
    // 加音乐
    // 加互动物体


    public RuntimeAnimatorController walkingC;
    public RuntimeAnimatorController slowRunC;
    public RuntimeAnimatorController talking5C;
    public RuntimeAnimatorController lookBehindC;
    public RuntimeAnimatorController getCatchC;
    public RuntimeAnimatorController rifleIdleC;
    public RuntimeAnimatorController pickC;
    public RuntimeAnimatorController surpriseC;
    public RuntimeAnimatorController pointC;
    public RuntimeAnimatorController sittingLaunchingC; // 这以后是俩动作组合，分开手和脚的
    public RuntimeAnimatorController angryC;
    public RuntimeAnimatorController lookAroundC;
    public RuntimeAnimatorController sitTalkingC;

    public RuntimeAnimatorController standC;
    public RuntimeAnimatorController sleepC;
    public RuntimeAnimatorController threatenC;
    public RuntimeAnimatorController fearC;
    public RuntimeAnimatorController punchingC;
    public RuntimeAnimatorController defeatC;
    public RuntimeAnimatorController happyC;
    public RuntimeAnimatorController dieC;
    public Avatar standA;
    public Avatar sleepA;
    public Avatar threatenA;
    public Avatar fearA;
    public Avatar punchingA;
    public Avatar defeatA;
    public Avatar happyA;
    // Start is called before the first frame update
    public int counter = 0;
    public int startCounter = 0;

    // https://youtu.be/rWh_UWFPgos?t=2

    // 场景，c1 后面有障碍物
    // 动画, c1 往后看
    // 摄影，不动，c1 和 c2中间

    [SerializeField]
    public enum ActorMove
    {
        STANDING,
        LOOKBEHIND,
        PICK,
        SURPRISED,
        ONLYMOVE,
        POINT,
        SITTINGLAU,
        SCALEUP, // 上下缩放
        OBJECTSMALLTOBIG,
        OBJECTSTATIC,
        OBJECTROTATE,
        TURNCAMERA,
        NONE,
    }

    public enum ObjectMove
    {
        NONE,
        MOVE,
        NONEMOVE,
    }


    public enum RowCol
    {
        SINGLE,
        TWO,
        SIX,
    }

    public enum CameraSetting
    {
        STATIC,
        ROTATE,
        ZOOM,
        CHASEDOBJECT,
        MOVETOPOS1,
        GLOBAL,
        NONE,
    }

    public struct CameraStruct
    {
        public Vector3 localPos;
        public Vector3 localLookat;
        public CameraSetting setting;
        public float originDis;
        public float targetDis;

        public CameraStruct(Vector3 localPosition, Vector3 localLookatDirection, CameraSetting setting, float originDistance, float targetDistance)
        {
            localPos = localPosition;
            localLookat = localLookatDirection;
            this.setting = setting;
            originDis = originDistance;
            targetDis = targetDistance;
        }
    }

    CameraStruct CreateCamera(CameraSetting setting, int CamerPosX, int CameraPosY, int CameraPosZ, float od, float td)
    {
        Vector3 n =  new Vector3(CamerPosX, CameraPosY, CameraPosZ).normalized;
        return new CameraStruct(n, -n, setting, od, td);
    }

    CameraStruct CameraNone()
    {
        return new CameraStruct(Vector3.zero, Vector3.zero, CameraSetting.NONE, 0, 0);
    }


    // CameraPosition

    // distance

    Vector3 CameraDistance(int index, Vector3 v, float distance)
    {
        int x = index % 3 - 1;
        int z = index % 9 / 3 - 1;
        int y = index / 9 - 1;
        return v + new Vector3(x, y, z) * distance;
    }

    Vector3 CameraLook(int index, Vector3 v)
    {
        int x = index % 3 - 1;
        int z = index % 9 / 3 - 1;
        int y = index / 9 - 1;
        return v + new Vector3(x, y, z) ;
    }

    private List<int> theIndex = new List<int>();

    [SerializeField]
    public struct SceneTime
    {
        public int start;
        public int end;


        public ActorMove scene;
        public RuntimeAnimatorController theController;
        public Avatar theAvatar;
        public GameObject theActor;
        public CameraStruct theCamera;
        public Vector3 pos0;
        public Vector3 pos1;
        public Vector3 lookat;


        public RowCol rc;

        public float temp2;

        public SceneTime(ActorMove scene, RuntimeAnimatorController controller, GameObject actor, CameraStruct camera, int v1, int v2, Vector3 pos0) : this()
        {
            this.scene = scene;
            this.theController = controller;
            this.theActor = actor;
            this.theCamera = camera;
            this.start = v1;
            this.end = v2;
            this.pos0 = pos0;

        }
        public SceneTime(ActorMove scene, RuntimeAnimatorController controller, GameObject actor, CameraStruct camera, int v1, int v2,  Vector3 pos0, Vector3 pos1) : this()
        {
            this.scene  = scene;
            this.theController = controller;
            this.theActor = actor;
            this.theCamera = camera;
            this.start = v1;
            this.end = v2;
            this.pos0 = pos0;
            this.pos1 = pos1;
        }

        public SceneTime(ActorMove scene, RuntimeAnimatorController controller, Avatar avatar, GameObject actor, CameraStruct camera, int v1, int v2, Vector3 pos0, Vector3 pos1, Vector3 lookat) : this()
        {
            this.scene = scene;
            this.theController = controller;
            this.theAvatar = avatar;
            this.theActor = actor;
            this.theCamera = camera;
            this.start =v1 ;
            this.end =  v2;
            this.pos0 = pos0;
            this.pos1 = pos1;
            this.lookat = lookat;

        }

        public SceneTime(ActorMove nONE, int v1, int v2, RowCol rc) : this()
        {
            this.scene = nONE;
            this.start = v1;
            this.end = v2;
            this.rc = rc;
        }

        public ActorMove getScene()
        {
            return this.scene;
        }

        public int getStart()
        {
            return this.start;
        }

        public int getEnd()
        {
            return this.end;
        }
    }

    [SerializeField]
    public List<SceneTime> sceneTimes = new List<SceneTime>();


    [SerializeField]
    public List<ActorMove> sceneTimesss = new List<ActorMove>();

    [SerializeField]
    public List<int> sceneTimess = new List<int>();
    private int index = 0;

    // sounds 
    // enver


    void AddScene(ActorMove scene, RuntimeAnimatorController ani, GameObject actor, CameraStruct camera, int startTimes, int endTimes, Vector3 pos0, Vector3 lookat)
    {

        Avatar a = sleepA;
        if (ani == sleepC) a = sleepA;
        else if (ani == threatenC) a = threatenA;
        else if (ani == fearC) a = fearA;
        else if (ani == punchingC) a = punchingA;
        else if (ani == defeatC) a = defeatA;
        else if (ani == happyC) a = happyA;
        else if(ani == standC) a= standA;
        sceneTimes.Add(new SceneTime(scene, ani, a, actor, camera, startTimes, endTimes, pos0, Vector3.zero, lookat));
    }

    void AddScene(ActorMove scene, RuntimeAnimatorController ani, GameObject actor, CameraStruct camera, int startTimes, int endTimes, Vector3 pos0, Vector3 pos1,  Vector3 lookat)
    {

        Avatar a = sleepA;
        if (ani == sleepC) a = sleepA;
        else if (ani == threatenC) a = threatenA;
        else if (ani == fearC) a = fearA;
        else if (ani == punchingC) a = punchingA;
        else if (ani == defeatC) a = defeatA;
        else if (ani == happyC) a = happyA;
        else if (ani == standC) a = standA;
        sceneTimes.Add(new SceneTime(scene, ani, a, actor, camera, startTimes, endTimes, pos0, pos1, lookat));
    }


    // 在地上惊讶看到什么东西
    // https://youtu.be/VfUyJHgw8a4?t=6

    // 人进来
    // https://youtu.be/VfUyJHgw8a4?t=6

    void qActorMove()
    {

    }

    void qActorStatic(RuntimeAnimatorController ani, GameObject actor, int start, int end)
    {
        AddScene(ActorMove.STANDING, ani, actor, CameraNone(), start, end, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
    }

    void qItemStatic(GameObject actor, int start, int end, Vector3 pos)
    {
        AddScene(ActorMove.OBJECTSTATIC, null, actor, CameraNone(), start, end, pos, new Vector3(0, 0, 1));
    }
    void qActorStatic2(RuntimeAnimatorController ani, GameObject actor, Vector3 pos, Vector3 lookat, int start, int end)
    {
        AddScene(ActorMove.STANDING, ani, actor, CameraNone(), start, end, pos, lookat);
    }

    void qActorDynamics(RuntimeAnimatorController ani, GameObject actor, Vector3 pos0, Vector3 pos1,  int start, int end)
    {
        AddScene(ActorMove.ONLYMOVE, ani, actor, CameraNone(), start, end, pos0, pos1, pos1 - pos0);
    }

    void qCameraMoveTo(GameObject actor, Vector3 originPos, Vector3 target, Vector3 lookat, int start, int end)
    {
        CameraStruct cs = CreateCamera(CameraSetting.MOVETOPOS1, 0,0,0,0,0);
        sceneTimes.Add(new SceneTime(ActorMove.TURNCAMERA, null ,null, actor, cs, start, end, originPos, target, lookat));
    }

    void qCameraStatic(GameObject actor, Vector3 originPos, Vector3 lookat, float distance, int start, int end)
    {
        CameraStruct cs = CreateCamera(CameraSetting.STATIC, (int)originPos.x, (int)originPos.y, (int)originPos.z, distance, distance);
        sceneTimes.Add(new SceneTime(ActorMove.TURNCAMERA, null, null, actor, cs, start, end, originPos, originPos, lookat));
    }
    void qCameraGlobalStatic(Vector3 originPos, Vector3 lookat, float distance, int start, int end)
    {
        CameraStruct cs = CreateCamera(CameraSetting.GLOBAL, (int)originPos.x, (int)originPos.y, (int)originPos.z, distance, distance);
        sceneTimes.Add(new SceneTime(ActorMove.TURNCAMERA, null, null, null, cs, start, end, originPos, originPos, lookat));
    }
    void LetCameraStand()
    {


    }

    private void LetObjectMove(int level)
    {
        
    }

    void LetObjectStand()
    {

    }
    void fastShow(int timer)
    {
        int start = timer;
        int end = timer + 400;
      //  AddScene(ActorMove.ONLYMOVE, walkingC, c0, start, end, new Vector3(32, 0, 0), new Vector3(-8, 0, 0), new Vector3(-1, 0, 0));
      //  AddScene(ActorMove.TURNCAMERA, null, c0, CameraSetting.CHASEDOBJECT, start, end, CameraDistance(15, new Vector3(0,5,0), 15), CameraLook(11, new Vector3(0, 5, 0)));
    }

    void Start()
    {
        /*
        string filePathx = "F:\\DaisyDay\\first_frame.json";

        if (!File.Exists(filePathx))
        {
            Console.WriteLine("File not found.");
            return;
        }

        string targetString = "points";
        List<double> numbers = new List<double>();
        Debug.Log("finfff");
        using (StreamReader reader = new StreamReader(filePathx))
        {
            string line;
            bool foundTargetString = false;

            while ((line = reader.ReadLine()) != null)
            {
                if (foundTargetString)
                {
                    string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string part in parts)
                    {
                        string cleanPart = part.Replace(",", "");
                        if (double.TryParse(cleanPart, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                        {
                            Debug.Log(number);
                            numbers.Add(number);
                            if (numbers.Count == 20)
                            {
                                break;
                            }
                        }
                    }

                    if (numbers.Count == 20)
                    {
                        break;
                    }
                }

                if (line.Contains(targetString))
                {
                    foundTargetString = true;
                }
            }
        }

        if (numbers.Count < 20)
        {
            Console.WriteLine("Not enough numbers found after the target string.");
        }
        else
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                Console.WriteLine($"Number {i + 1}: {numbers[i]}");
            }
        }

        // 指定要遍历的目录路径
        string directoryPath = @"F:\DaisyDay";

        string[] xmlFiles = Directory.GetFiles(directoryPath, "*.xml"); // 获取指定目录下所有后缀名为.xml的文件

        float distance = 0;

        foreach (string filePath in xmlFiles)
        {
            Debug.Log("Reading file: " + filePath);
            // 使用XmlDocument解析XML文件
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // 获取所有的 <xmin> 元素
            XmlNodeList xminNodes = xmlDoc.GetElementsByTagName("xmin");
            XmlNodeList yminNodes = xmlDoc.GetElementsByTagName("ymin");
            XmlNodeList xmaxNodes = xmlDoc.GetElementsByTagName("xmax");
            XmlNodeList ymaxNodes = xmlDoc.GetElementsByTagName("ymax");

            int xmin = int.Parse(xminNodes[0].InnerText);
            int ymin = int.Parse(yminNodes[0].InnerText);
            int xmax = int.Parse(xmaxNodes[0].InnerText);
            int ymax = int.Parse(ymaxNodes[0].InnerText);

            int xysize = ((ymax - ymin) + (xmax - xmin)) / 2;

            Debug.Log("xysize = " + xysize);

            float ratio = (float)xysize / (float)1200;

            distance = ratio * 12f * 6f; // 离摄像机的距离 // 6头身

        }

        Debug.Log("distance"  +distance);
        int timeStart = 0;
        int timeEnd = timeStart + 200;
        // 读取文本文件的全部内容
        string text = File.ReadAllText("D://t.txt");

        // 按行分割文本
        string[] lines = text.Split('\n');

        GameObject theactor = c0;
        RuntimeAnimatorController trac = sleepC;

        // 遍历文本行
        for (int i = 0; i < lines.Length; i ++)
        {
            if(i % 2 == 0)
            {
                int number = int.Parse(lines[i]);
                if(number == 0)
                {
                    theactor = c0;
                }
                else
                {
                    theactor = c1;
                }
            }
            else
            {
                switch (lines[i].Trim())
                {
                    case "sleep": { trac = sleepC;break; }
                    case "threaten": { trac = threatenC; break; }
                    case "fear": { trac = fearC; break; }
                    case "punch": { trac = punchingC; break; }
                    case "defeat": { trac = defeatC; break; }
                    case "happy": { trac = happyC; break; }
                    default: break;
                }

                qActorStatic(trac, theactor, timeStart, timeEnd);
                qCameraMoveTo(theactor, new Vector3(0, 16, distance), new Vector3(0, 8, distance), new Vector3(0, 8, 0), timeStart, timeEnd);
                timeStart = timeEnd;
                timeEnd = timeStart + 200;
            }
        }


        fastShow(0);
        //fastCatch(0);
        //fastChase(0);
        //fastTwoTalkingScene(0);
        //fastFightObject(400);
        //sceneTimes.Add(new SceneTime(ActorMove.OBJECTSMALLTOBIG, null, c2, CameraSetting.ZERO, 0, 100, Vector3.zero)); // 物体暂时只支持一个
        //sceneTimes.Add(new SceneTime(ActorMove.SCALEUP, null, c0, CameraSetting.ZOOMINDYNAMIC, 0, 40, Vector3.zero)); // 物体暂时只支持一个
        //sceneTimes.Add(new SceneTime(ActorMove.STANDING, null, c0, CameraSetting.SMALLTOBIG, 0, 100, Vector3.zero));
        // s1
        //sceneTimes.Add(new SceneTime(ActorMove.ONLYMOVE, null, c0, CameraSetting.ZERO, 0, 40, new Vector3(10, 0, 0), new Vector3(0, 0, 0)));
        //sceneTimes.Add(new SceneTime(ActorMove.STANDING, pointController, c0, CameraSetting.ZERO, 41, 120, Vector3.zero));
        //sceneTimes.Add(new SceneTime(ActorMove.STANDING, sittingLaunchingController, c0, CameraSetting.ZERO, 121, 200, new Vector3(8, 0, 0)));
        //sceneTimes.Add(new SceneTime(ActorMove.STANDING, sittingLaunchingController, c1, CameraSetting.ZERO, 121, 200, Vector3.zero));

        //sceneTimes.Add(new SceneTime(ActorMove.POINT, 40, 80));
        // sceneTimes.Add(new SceneTime(ActorMove.SITTINGLAU, 80, 160, RowCol.TWO));
        // sceneTimes.Add(new SceneTime(ActorMove.SURPRISED, 0, 300));
        // sceneTimes.Add(new SceneTime(ActorMove.LOOKBEHIND, 801, 1600));
        // sceneTimes.Add(new SceneTime(ActorMove.NONE, 801, 2000));

        StartActorMove(0);
        */
        int timeStart = 0;
        int timeEnd = timeStart + 200;
        // 读取文本文件的全部内容
        string text = File.ReadAllText("D://t.txt");

        // 按行分割文本
        string[] lines = text.Split('\n');

        GameObject theactor = c0;
        RuntimeAnimatorController trac = sleepC;
        Vector3 CameraPos = Vector3.zero;
        CameraSetting css = CameraSetting.MOVETOPOS1;

        int i = 0;
        for(int xi = 0; xi < lines.Length; xi++)
        {
            if (lines[xi].Trim().Length == 0)
            {
                i = xi + 1;
            }
        }
        Debug.Log(" i = " + i + " lines = " + lines[i]);
        // 遍历文本行
        for (; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(' ');
            while (words[0].Trim() != "cc") {
                switch (words[0].Trim())
                {
                    case "c0": theactor = c0; break;
                    case "c1": theactor = c1; break;
                    default: break;
                }

                switch (words[1].Trim())
                {
                    case "stand": { trac = standC; break; }
                    case "walk": { trac = walkingC; break; }
                    case "die": { trac = dieC; break; }
                    case "punch": { trac = punchingC; break; }
                    case "holditem": { trac = rifleIdleC; break; }
                    case "item": { trac = null; break; }
                    default: break;
                }
                if(words.Length <= 2)
                {
                    qActorStatic(trac, theactor, timeStart, timeEnd);
                    
                }else if(trac == null)
                {
                    float sx = float.Parse(words[2]);
                    float sy = float.Parse(words[3]);
                    float sz = float.Parse(words[4]);
                    qItemStatic(o1, timeStart, timeEnd, new Vector3(sx, sy, sz));
                }
                else 
                {
                    float sx = float.Parse(words[2]);
                    float sy = float.Parse(words[3]);
                    float sz = float.Parse(words[4]);
                    float ex = float.Parse(words[5]);
                    float ey = float.Parse(words[6]);
                    float ez = float.Parse(words[7]);
                    if(trac == walkingC)
                    {
                        qActorDynamics(trac, theactor, new Vector3(sx, sy, sz), new Vector3(ex, ey, ez), timeStart, timeEnd);
                    }
                    else
                    {
                        qActorStatic2(trac, theactor, new Vector3(sx, sy, sz), new Vector3(ex, ey, ez), timeStart, timeEnd);
                    }
                    
                }
                i += 1;
                words = lines[i].Split(' ');
            }

            if (words[1].Trim() == "custom")
            {

                float sx = float.Parse(words[2]);
                float sy = float.Parse(words[3]);
                float sz = float.Parse(words[4]);
                float lx = float.Parse(words[5]);
                float ly = float.Parse(words[6]);
                float lz = float.Parse(words[7]);
                Vector3 pos = new Vector3(sx, sy, sz);
                Vector3 lk = new Vector3(lx, ly, lz);
                qCameraStatic(theactor, pos, lk, pos.magnitude, timeStart, timeEnd);
            }else if (words[1].Trim() == "global")
            {

                float sx = float.Parse(words[2]);
                float sy = float.Parse(words[3]);
                float sz = float.Parse(words[4]);
                float lx = float.Parse(words[5]);
                float ly = float.Parse(words[6]);
                float lz = float.Parse(words[7]);
                Vector3 pos = new Vector3(sx, sy, sz);
                Vector3 lk = new Vector3(lx, ly, lz);
                qCameraGlobalStatic(pos, lk, pos.magnitude, timeStart, timeEnd);
            }
            else
            {
                switch (words[1].Trim())
                {
                    case "cuu": { CameraPos = new Vector3(0, 1, 1); break; }
                    case "cdd": { CameraPos = new Vector3(0, -1, -1); break; }
                    case "ccd": { CameraPos = new Vector3(0, 0, -1); break; }
                    case "dcd": { CameraPos = new Vector3(-1, 0, -1); break; }
                    case "ucd": { CameraPos = new Vector3(1, 0, -1); break; }
                    case "happy": { trac = happyC; break; }
                    default: break;
                }

                switch (words[2].Trim())
                {
                    case "in": { css = CameraSetting.ZOOM; break; }
                    case "threaten": { trac = threatenC; break; }
                    case "fear": { trac = fearC; break; }
                    case "punch": { trac = punchingC; break; }
                    case "defeat": { trac = defeatC; break; }
                    case "happy": { trac = happyC; break; }
                    default: break;
                }
                float oo = float.Parse(words[3]);
                float xx = float.Parse(words[4]);
                float yy = float.Parse(words[5]);
                float zz = float.Parse(words[6]);
                qCameraStatic(theactor, CameraPos, new Vector3(xx, yy, zz), oo, timeStart, timeEnd);
            }

            timeStart = timeEnd;
            timeEnd = timeStart + 400;
        }
    }


    void SetCamera(int index)
    {
        switch (sceneTimes[index].theCamera.setting)
        {
            
            case CameraSetting.STATIC:
                {

                    Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].theCamera.localPos * sceneTimes[index].theCamera.originDis;
                    Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position + sceneTimes[index].theCamera.localLookat);
                    break;
                }
            case CameraSetting.CHASEDOBJECT:
                {

                    Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].theCamera.localPos * sceneTimes[index].theCamera.originDis;
                    Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position);
                    break;
                }
            case CameraSetting.ROTATE:
                {

                    Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].theCamera.localPos * sceneTimes[index].theCamera.originDis;
                    Camera.main.transform.Rotate(Vector3.up, sceneTimes[index].theCamera.originDis);
                    break;
                }
            case CameraSetting.ZOOM:
                {
                    Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].theCamera.localPos * sceneTimes[index].theCamera.originDis;
                    Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position);
                    break;
                }
            case CameraSetting.MOVETOPOS1:
                {
                    Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].pos0;
                    Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position);
                    break;
                }
            default: break;
        }
    }


    void StartActorMove(int index)
    {
        if (sceneTimes[index].theActor == null || sceneTimes[index].scene == ActorMove.TURNCAMERA)
        {

        }
        else
        {
            sceneTimes[index].theActor.GetComponent<Transform>().transform.position = sceneTimes[index].pos0;
            Debug.Log(" pos0 " + sceneTimes[index].pos0);
            if (sceneTimes[index].scene == ActorMove.OBJECTROTATE)
            {
                sceneTimes[index].theActor.GetComponent<Transform>().transform.Rotate(sceneTimes[index].pos1, sceneTimes[index].lookat.y, Space.World);
            }
            else
            {

                sceneTimes[index].theActor.GetComponent<Transform>().transform.LookAt(sceneTimes[index].pos0 + sceneTimes[index].lookat, Vector3.up);
            }
            if (sceneTimes[index].theActor.GetComponent<Animator>() != null)
            {
                sceneTimes[index].theActor.GetComponent<Animator>().runtimeAnimatorController = sceneTimes[index].theController;
                if(sceneTimes[index].theActor == c0)
                {
                    sceneTimes[index].theActor.GetComponent<Animator>().avatar = sceneTimes[index].theAvatar;
                }
            }
            
        }

        if (sceneTimes[index].scene == ActorMove.TURNCAMERA)
        {
            SetCamera(index);
        }
        
    }

    void EndActorMove(int index)
    {
        if (sceneTimes[index].theActor != null)
        {
            sceneTimes[index].theActor.GetComponent<Transform>().transform.position = new Vector3(100, 0, 0);
        }
        
    }

    void FixedUpdate()
    {

        theIndex.Clear();
        for(int  i = 0; i < sceneTimes.Count; i++)
        {
            if(counter == sceneTimes[i].getStart())
            {
                StartActorMove(i);
            }
            if(counter >= sceneTimes[i].getStart() && counter < sceneTimes[i].getEnd())
            {
                theIndex.Add(i);
            }
            if(counter == sceneTimes[i].getEnd())
            {
                EndActorMove(i);
            }
        }

        for(int i = 0; i < theIndex.Count; i++)
        {
            int index = theIndex[i];
            switch (sceneTimes[index].getScene())
            {

                case ActorMove.ONLYMOVE:
                    {
                        if(counter == sceneTimes[index].start)
                        {
                            sceneTimes[index].theActor.GetComponent<Transform>().transform.position = sceneTimes[index].pos0;
                        }
                        Vector3 speed = (sceneTimes[index].pos1 - sceneTimes[index].pos0) / (sceneTimes[index].end - sceneTimes[index].start);
                        sceneTimes[index].theActor.GetComponent<Transform>().transform.Translate(speed, Space.World);
                        break;
                    }
                case ActorMove.OBJECTSMALLTOBIG:
                    {
                        float scale = 10.0f * (counter - sceneTimes[index].start) / (sceneTimes[index].end - sceneTimes[index].start);
                        sceneTimes[index].theActor.GetComponent<Transform>().transform.localScale = new Vector3(scale, scale, scale);
                        break;
                    }
                case ActorMove.SCALEUP:
                    {
                        float scale = Mathf.Sin(counter * 0.1f) * 0.1f + 1.1f;
                        sceneTimes[index].theActor.GetComponent<Transform>().transform.localScale = new Vector3(1, scale, 1); 
                        break;
                    }
                case ActorMove.OBJECTROTATE:
                    {
                        Debug.Log(sceneTimes[index].theActor.GetComponent<Transform>().transform.eulerAngles.z);
                        sceneTimes[index].theActor.GetComponent<Transform>().transform.Rotate(sceneTimes[index].pos1, sceneTimes[index].lookat.x);
                        break;
                    }
                default: break;
            }

            switch (sceneTimes[index].theCamera.setting)
            {
                case CameraSetting.STATIC:
                    {
                        Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].theCamera.localPos * sceneTimes[index].theCamera.originDis + new Vector3(0, 6, 0);
                        Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position + sceneTimes[index].lookat);
                        break;
                    };
                case CameraSetting.GLOBAL:
                    {
                        Camera.main.transform.position = sceneTimes[index].pos0;
                        Camera.main.transform.LookAt(sceneTimes[index].lookat);
                        break;
                    };
                case CameraSetting.ROTATE:
                   {
                       int duration = sceneTimes[index].end - sceneTimes[index].start;
                       float lookat = (sceneTimes[index].theCamera.targetDis - sceneTimes[index].theCamera.originDis) / duration;
                        Camera.main.transform.Rotate(Vector3.up, lookat);
                       break;
                   };
                case CameraSetting.ZOOM:
                    {
                        int duration = sceneTimes[index].end - sceneTimes[index].start;
                        float zoomins = (sceneTimes[index].theCamera.targetDis - sceneTimes[index].theCamera.originDis) / duration;
                        Camera.main.transform.position = sceneTimes[index].theActor.transform.position + zoomins * sceneTimes[index].theCamera.localPos;
                        break;
                    };
                case CameraSetting.CHASEDOBJECT:
                    {

                        Camera.main.transform.position = sceneTimes[index].theActor.transform.position + sceneTimes[index].pos0;
                        Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position + sceneTimes[index].lookat);
                        break;
                    }
                case CameraSetting.MOVETOPOS1:
                    {
                        int duration = sceneTimes[index].end - sceneTimes[index].start;
                        float percents = (counter - sceneTimes[index].start) * 1.0f / duration;
                        Vector3 offset = sceneTimes[index].pos0 + percents * (sceneTimes[index].pos1 - sceneTimes[index].pos0);
                        Camera.main.transform.position = sceneTimes[index].theActor.transform.position + offset;
                        Camera.main.transform.LookAt(sceneTimes[index].theActor.transform.position + sceneTimes[index].lookat);
                        break;
                    }
                default: break;
            }
        }
 


        counter++;
    }
}
