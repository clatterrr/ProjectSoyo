using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  ties 自动生成？给个body的位置，自动生成不同样子的tie? python 来生成吧
///  
/// 词汇语义线索是通过测量术语之间的语义相似性来捕捉的。其基本动机是，如果我们已经知道一些产品特性种子，那么与这些种子在语义上更相似的术语更有可能是产品特性。例如，如果已知“screen”是mp3的一个产品特性，而“lcd”与“screen”在语义上高度相似，那么我们可以推断“lcd”也是一个产品特性。同样，与负标记种子在语义上相似的术语则不是产品特性。
/// 
/// 我要产生更好的什么？ 三维场景
/// 和谁比较？好三维场景和坏三维场景比较
/// 我觉得这个用来批量化修改动作，一般动作到迪士尼动作比较好
/// 
/// 
/// _back 代表摄像机需要从后面看
/// 摄像机方向怎么规定 ？
/// 
/// 每个 Part 形成一个AABB，检查摄像机能不能直接看到这儿，不能就调整
/// 
/// 每个 Part 位置不同，生成语句后，获取时间
/// 
/// MicroSoft 应该能做到
/// </summary>

public class Apply : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    public Texture2D plaintexture;
    public Texture2D colorTexture;
    public Shader shader;
    public string jsonPath = "E:\\mine\\mod1192\\src\\main\\resources\\assets\\examplemod\\geo\\enemy_zombie.geo.json";
    public int part_time = 120;
    public TextMeshProUGUI tt;

    public bool GenerateStoryStart;

    struct Cube
    {
        public string name;
        public string parent;
        public Vector3 position;
        public Vector3 scale;
        public Vector3 pivot;
        public Vector3 rotation;
        public Vector2 uv;
        public Bounds aabb;

        public Cube(string name, string parent, Vector3 position, Vector3 scale, Vector3 pivot, Vector3 rotation, Vector2 uv)
        {
            this.name = name;
            this.parent = parent;
            this.position = position;
            this.scale = scale;
            this.pivot = pivot;
            this.rotation = rotation;
            this.uv = uv;
            this.aabb = new Bounds(position + scale / 2, scale);
        }
    }

    struct Group
    {
        public List<Cube> children;
        public string name;
        public string parent;
        public Bounds aabb;
        public bool CameraBack;

        public Vector3 CameraPos;
        public Vector3 CameraTarget;

        public Group(string name, string parent)
        {
            this.name = name;
            this.parent = parent;
            this.children = new List<Cube>();
            this.aabb = new Bounds(Vector3.zero, Vector3.zero);
            this.CameraBack = false;
            this.CameraPos = Vector3.zero;
            this.CameraTarget = Vector3.zero;
        }



    }

    Vector3 ChangeCamera(Bounds aabb, bool CameraBack)
    {
        bool can = CanReachTarget(Camera.main.transform.position, aabb.min, aabb.max);
        Vector3 c = aabb.center;
        Vector3 e = aabb.extents;
        Vector3 t = Vector3.zero;

        float randomAngle = Random.Range(-120f, -90f);

        // 将角度转换为弧度
        float radians = randomAngle * Mathf.Deg2Rad;



        // 计算圆周上的点
        float x = Mathf.Cos(radians) * e.x * 6;
        float z = Mathf.Sin(radians) * e.z * 6;
        if (CameraBack)
        {
            t = new Vector3(c.x + x, c.y + e.y + 4f, c.z - z);
        }
        else
        {
            t = new Vector3(c.x + x, c.y + e.y + 4f, c.z + z);
        }
        return t;
    }

    public bool CanReachTarget(Vector3 cameraPosition, Vector3 targetMin, Vector3 targetMax)
    {
        // 计算目标长方体的中心
        Vector3 targetCenter = (targetMin + targetMax) / 2.0f;

        // 生成从摄像机位置到目标中心的射线
        Ray ray = new Ray(cameraPosition, targetCenter - cameraPosition);

        // 计算从摄像机到目标中心的距离
        float distanceToTarget = Vector3.Distance(cameraPosition, targetCenter);

        // 检查射线是否被其他AABB阻挡
        foreach (Bounds bounds in existingBounds)
        {
            if (bounds.IntersectRay(ray))
            {
                // 如果射线与某个AABB相交，再检查是否在目标的距离范围内
                if (bounds.IntersectRay(ray, out float hitDistance))
                {
                    if (hitDistance < distanceToTarget)
                    {
                        // 如果相交的距离小于到目标中心的距离，说明被阻挡
                        return false;
                    }
                }
            }
        }

        // 如果没有任何阻挡，返回true
        return true;
    }


    List<Cube> cubes;

    struct Model
    {
        public GameObject bottom;
        public GameObject top;
        public GameObject left;
        public GameObject right;
        public GameObject back;
        public GameObject front;
        public Vector3[] scales;
        public Vector3[] pos;
        public Vector2[] texture_start;
        public Vector2[] texture_range;
        public int frame;
        public float duration;
        public Shader shader;
        public Texture2D plainTexture;
        public Texture2D colorTexture;
        public Vector4 colors;

        public bool cameraBack;

        public Model(GameObject bottom, GameObject top, GameObject left, GameObject right,
            GameObject back, GameObject front, Vector2[] texture_start, Vector2[] texture_range,
            Shader shader, Texture2D plainTexture, Texture2D colorTexture)
        {
            this.bottom = bottom;
            this.top = top;
            this.left = left;
            this.right = right;
            this.back = back;
            this.front = front;
            this.frame = 0;
            this.scales = new Vector3[6];
            pos = new Vector3[6];
            this.scales[0] = this.bottom.transform.localScale;
            this.scales[1] = this.top.transform.localScale;
            this.scales[2] = this.left.transform.localScale;
            this.scales[3] = this.right.transform.localScale;
            this.scales[4] = this.back.transform.localScale;
            this.scales[5] = this.front.transform.localScale;
            pos[0] = bottom.transform.position;
            pos[1] = top.transform.position;
            pos[2] = left.transform.position;
            pos[3] = right.transform.position;
            pos[4] = back.transform.position;
            pos[5] = front.transform.position;
            this.duration = 0;
            this.texture_range = texture_range;
            this.texture_start = texture_start;
            this.shader = shader;
            this.plainTexture = plainTexture;
            this.colorTexture = colorTexture;
            this.colors = Vector4.zero;
            int pixels = 0;
            for (int k = 0; k < 6; k++)
            {
                for (int i = 0; i < texture_range[k].x; i++)
                {
                    for (int j = 0; j < texture_range[k].y; j++)
                    {
                        int px = (int)(texture_start[k].x + i);
                        int py = plainTexture.height - (int)(texture_start[k].y + j);
                        Color c = colorTexture.GetPixel(px, py);
                        this.colors += new Vector4(c.r, c.g, c.b, c.a);
                        pixels += 1;

                    }
                }

            }
            this.colors /= pixels;
            this.cameraBack = false;


        }


        void ApplayMaterial(GameObject go, Vector2 start, Vector2 range, Texture2D texture)
        {


            float texture_size = texture.height;
            Material mat = new Material(shader);
            mat.mainTexture = texture;
            go.GetComponent<MeshRenderer>().material = mat;
            go.GetComponent<MeshRenderer>().material.SetVector("_TextureScale", new Vector4(range.x / texture_size, range.y / texture_size, 0, 0));
            go.GetComponent<MeshRenderer>().material.SetVector("_UVOffset", new Vector4(start.x / texture_size, (texture_size - start.y - range.y) / texture_size, 0, 0));
        }
        public void SetZeroScale()
        {
            bottom.transform.localScale = Vector3.zero;
            top.transform.localScale = Vector3.zero;
            left.transform.localScale = Vector3.zero;
            right.transform.localScale = Vector3.zero;
            back.transform.localScale = Vector3.zero;
            front.transform.localScale = Vector3.zero;
        }

        public void SetPlainTexture()
        {
            ApplayMaterial(bottom, texture_start[0], texture_range[0], plainTexture);

            ApplayMaterial(top, texture_start[1], texture_range[1], plainTexture);

            ApplayMaterial(left, texture_start[2], texture_range[2], plainTexture);

            ApplayMaterial(right, texture_start[3], texture_range[3], plainTexture);

            ApplayMaterial(back, texture_start[4], texture_range[4], plainTexture);

            ApplayMaterial(front, texture_start[5], texture_range[5], plainTexture);
        }
        public void SetOriginTexture()
        {

            float ratio = 1;
            if (this.duration > 0)
            {
                ratio = (this.frame - this.duration) / this.duration;
            }


            Texture2D tmpTexture90 = new Texture2D(colorTexture.width, colorTexture.height);
            tmpTexture90.filterMode = FilterMode.Point;
            Texture2D tmpTexture95 = new Texture2D(colorTexture.width, colorTexture.height);
            tmpTexture95.filterMode = FilterMode.Point;
            Texture2D topTexture = new Texture2D(colorTexture.width, colorTexture.height);
            topTexture.filterMode = FilterMode.Point;



            for (int k = 2; k <= 5; k++)
            {
                for (int i = 0; i < texture_range[k].x; i++)
                {
                    int they = (int)(texture_range[k].y * ratio);
                    int px = (int)(texture_start[k].x + i);
                    for(int j = 0; j <= they; j++)
                    {
                        int py = colorTexture.height - (int)(texture_start[k].y + j);
                        Color theColor = colorTexture.GetPixel(px, py);

                        tmpTexture90.SetPixel(px, py, new Color(theColor.r * 0.9f, theColor.g * 0.9f, theColor.b * 0.9f));
                        tmpTexture95.SetPixel(px, py, new Color(theColor.r * 0.95f, theColor.g * 0.95f, theColor.b * 0.95f));
                        topTexture.SetPixel(px, py, theColor);
                    }
                    
                   // topTexture.SetPixel(px, py, );
                }
            }
            Debug.Log("xiexie");
            topTexture.Apply();
            tmpTexture90.Apply();
            tmpTexture95.Apply();


            ApplayMaterial(bottom, texture_start[0], texture_range[0], colorTexture);

            ApplayMaterial(top, texture_start[1], texture_range[1], colorTexture);

            ApplayMaterial(left, texture_start[2], texture_range[2], tmpTexture95);

            ApplayMaterial(right, texture_start[3], texture_range[3], tmpTexture90);

            ApplayMaterial(back, texture_start[4], texture_range[4], tmpTexture90);

            ApplayMaterial(front, texture_start[5], texture_range[5], tmpTexture95);
        }
        public void SetOriginScale()
        {
            float ratio = 1;
            if (this.duration > 0)
            {
                ratio = this.frame / this.duration;
            }

            bottom.transform.localScale = scales[0];
            int total_int_len = (int)(scales[2].z / 0.1f);
            int cur_int_len = (int)(ratio * total_int_len);
            float ration3 = cur_int_len / (float)total_int_len;
            float ratioy2 = (pos[2].y - pos[1].y) * ration3 + pos[1].y;

            float tbry = (pos[0].y - pos[1].y) * ration3 + pos[1].y;
            float tbrx = (pos[0].x - pos[1].x) * ration3 + pos[1].x;
            Vector3 topbottom = new Vector3(tbrx, tbry, pos[0].z);
            tbrx = (pos[1].x + topbottom.x) / 2;
            tbry = (pos[1].y + topbottom.y) / 2;
            Vector3 leftpos = new Vector3(pos[2].x, tbry , pos[2].z);
            tbrx = (pos[1].x + topbottom.x) / 2;
            tbry = (pos[1].y + topbottom.y) / 2;
            Vector3 rightpos = new Vector3(pos[3].x, tbry, pos[3].z);

           

            tbrx = (pos[1].x + topbottom.x) / 2;
            tbry = (pos[1].y + topbottom.y) / 2;
            Vector3 backpos = new Vector3(tbrx, tbry, pos[4].z);
            Vector3 frontpos = new Vector3(tbrx, tbry, pos[5].z);

            bottom.transform.position = topbottom;
            top.transform.localScale = scales[1];

            left.transform.position = leftpos;
            left.transform.localScale = new Vector3(scales[2].x, 1, scales[3].z * ration3);
            right.transform.position = rightpos;
            right.transform.localScale = new Vector3(scales[3].x, 1, scales[3].z * ration3);
            back.transform.position = backpos;
            back.transform.localScale = new Vector3(scales[4].x, 1, scales[4].z * ration3);
            front.transform.position = frontpos;
            front.transform.localScale = new Vector3(scales[5].x, 1, scales[5].z * ration3);


        }

    }

    private List<Model> parts = new List<Model>();
    void CreateCube(Vector3 pos, Vector3 scale, Vector3 pivot, Vector3 rotation, Vector2 uv)
    {
        Vector3 s0 = scale;
        scale = scale * 0.1f;
        GameObject bottom = Instantiate(prefab, pos + new Vector3(s0.x / 2, 0, s0.z / 2), Quaternion.identity);
        if (rotation != Vector3.zero)
        {
            bottom.transform.RotateAround(pivot, Vector3.left, rotation.x);
            bottom.transform.RotateAround(pivot, Vector3.up, rotation.y);
            bottom.transform.RotateAround(pivot, Vector3.forward, rotation.z);
        }
        bottom.transform.localScale = new Vector3(scale.x, 1, scale.z);

        GameObject top = Instantiate(prefab, pos + new Vector3(s0.x / 2, s0.y, s0.z / 2), Quaternion.identity);
        if (rotation != Vector3.zero)
        {
            top.transform.RotateAround(pivot, Vector3.left, rotation.x);
            top.transform.RotateAround(pivot, Vector3.up, rotation.y);
            top.transform.RotateAround(pivot, Vector3.forward, rotation.z);
        }
        top.transform.localScale = new Vector3(scale.x, 1, scale.z);

        GameObject left = Instantiate(prefab, pos + new Vector3(0, s0.y / 2, s0.z / 2), Quaternion.identity);
        left.transform.localScale = new Vector3(scale.z, 1, scale.y);
        left.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), 90f);
        left.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -90f);
        if (rotation != Vector3.zero)
        {
            left.transform.RotateAround(pivot, Vector3.left, rotation.x);
            left.transform.RotateAround(pivot, Vector3.up, rotation.y);
            left.transform.RotateAround(pivot, Vector3.forward, rotation.z);
        }


        GameObject right = Instantiate(prefab, pos + new Vector3(s0.x, s0.y / 2, s0.z / 2), Quaternion.identity);
        if (rotation != Vector3.zero)
        {
            right.transform.RotateAround(pivot, Vector3.left, rotation.x);
            right.transform.RotateAround(pivot, Vector3.up, rotation.y);
            right.transform.RotateAround(pivot, Vector3.forward, rotation.z);
        }
        right.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), 90f);
        right.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -90f);
        right.transform.localScale = new Vector3(scale.z, 1, scale.y);


        GameObject back = Instantiate(prefab, pos + new Vector3(s0.x / 2, s0.y / 2, 0), Quaternion.identity);
        if (rotation != Vector3.zero)
        {
            back.transform.RotateAround(pivot, Vector3.left, rotation.x);
            back.transform.RotateAround(pivot, Vector3.up, rotation.y);
            back.transform.RotateAround(pivot, Vector3.forward, rotation.z);
        }
        back.transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), 90f);
        back.transform.localScale = new Vector3(scale.x, 1, scale.y);


        GameObject front = Instantiate(prefab, pos + new Vector3(s0.x / 2, s0.y / 2, s0.z), Quaternion.identity);
        if (rotation != Vector3.zero)
        {
            front.transform.RotateAround(pivot, Vector3.left, rotation.x);
            front.transform.RotateAround(pivot, Vector3.up, rotation.y);
            front.transform.RotateAround(pivot, Vector3.forward, rotation.z);
        }
        front.transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), 90f);
        front.transform.localScale = new Vector3(scale.x, 1, scale.y);

        Vector2[] texture_start = new Vector2[6];
        texture_start[0] = new Vector2(uv.x + s0.x + s0.z, uv.y);
        texture_start[1] = new Vector2(uv.x + s0.z, uv.y);
        texture_start[2] = new Vector2(uv.x, uv.y + s0.z);
        texture_start[3] = new Vector2(uv.x + s0.x + s0.z, uv.y + s0.z);
        texture_start[5] = new Vector2(uv.x + s0.z + s0.x + s0.z, uv.y + s0.z);
        texture_start[4] = new Vector2(uv.x + s0.z, uv.y + s0.z);

        Vector2[] texture_range = new Vector2[6];
        texture_range[0] = new Vector2(s0.x, s0.z);
        texture_range[1] = new Vector2(s0.x, s0.z);
        texture_range[2] = new Vector2(s0.z, s0.y);
        texture_range[3] = new Vector2(s0.z, s0.y);
        texture_range[4] = new Vector2(s0.x, s0.y);
        texture_range[5] = new Vector2(s0.x, s0.y);

        parts.Add(new Model(bottom, top, left, right, back, front, texture_start, texture_range, shader, plaintexture, colorTexture));
        //go.transform.localScale = scale;
    }

    public Texture2D texture;
    private int body_index = 0;

    void AddSenetens(string[] part, string part_name)
    {
        int randomValue = Random.Range(0, part.Length + common.Length);
        if (randomValue < common.Length)
        {
            string str = common[randomValue].ToString();
            str = str.Replace("_part", part_name);
            sentences.Add(str);
        }
        else
        {
            string str = part[randomValue - common.Length].ToString();
            str = str.Replace("_part", part_name);
            sentences.Add(str);
        }
    }

    bool isUnknown(string str)
    {
        return str == "un" || str == "unknown" || str == "unknown2" || str == "bone";
    }

    public float[] ReadFloatArrayFromFile(string fileName)
    {

        // 读取文件中的所有行
        string[] lines = File.ReadAllLines(fileName);

        // 创建一个浮点数组来存储数字
        float[] numbers = new float[lines.Length];

        // 遍历每一行，并将其转换为浮点数
        for (int i = 0; i < lines.Length; i++)
        {
            if (float.TryParse(lines[i], out float number))
            {
                numbers[i] = number;
            }
            else
            {
                Debug.LogError("无法将行转换为浮点数: " + lines[i]);
            }
        }

        return numbers;
    }
    string RemoveDigitsFromString(string input)
    {
        return Regex.Replace(input, @"\d", "");
    }

    List<Bounds> existingBounds = new List<Bounds>();
    List<Group> groups = new List<Group>();
    void Start()
    {
        jsonPath = "E:\\mine\\mod1192\\src\\main\\resources\\assets\\examplemod\\geo\\" + jsonPath;
        for (int i = 0; i < plaintexture.width; i++)
            for (int j = 0; j < plaintexture.height; j++)
            {
                plaintexture.SetPixel(i, j, Color.gray);
            }
        plaintexture.Apply();

        cubes = new List<Cube>();

        Group group = new Group("", "");

        float[] str_times = ReadFloatArrayFromFile("D:\\sentences_time.txt");

        using (StreamReader reader = new StreamReader(jsonPath))
        {
            string line;
            string the_name = "";
            string the_parent = "";
            while ((line = reader.ReadLine()) != null)
            {

                // 使用正则表达式匹配引号中的内容
                MatchCollection matches = Regex.Matches(line, "\"([^\"]*)\"");
                List<string> vs = new List<string>();
                foreach (Match match in matches)
                {
                    vs.Add(match.Groups[1].Value);
                }
                if (vs.Count > 1)
                {
                    if (vs[0].Equals("name"))
                    {
                        if (group.name != "")
                        {
                            groups.Add(group);
                            group = new Group("", "");
                        }
                        the_name = vs[1];
                        group.name = the_name;
                    }
                    else if (vs[0].Equals("parent"))
                    {
                        the_parent = vs[1];
                        group.parent = the_parent;
                    }
                    else if (vs[0].Equals("pivot"))
                    {

                    }
                }

                MatchCollection matches2 = Regex.Matches(line, @"-?\d+(\.\d+)?");
                List<float> result = new List<float>();

                foreach (Match match in matches2)
                {
                    // 将匹配到的数字添加到结果列表中
                    if (float.TryParse(match.Value, out float number))
                    {
                        result.Add(number);
                    }
                }

                if (result.Count == 14)
                {
                    Cube cube = new Cube(the_name, the_parent, new Vector3(result[0], result[1], result[2]), new Vector3(result[3], result[4], result[5]), new Vector3(result[6], result[7], result[8]), new Vector3(result[9], result[10], result[11]), new Vector2(result[12], result[13]));
                    group.children.Add(cube);

                }
                else if (result.Count == 8)
                {
                    Cube cube = new Cube(the_name, the_parent, new Vector3(result[0], result[1], result[2]), new Vector3(result[3], result[4], result[5]), Vector3.zero, Vector3.zero, new Vector2(result[6], result[7]));
                    group.children.Add(cube);
                }
            }
        }
        groups.Add(group);
        float total_time = 0;
        int part_index = 0;
        List<float> times = new List<float>();

        for (int i = 0; i < groups.Count; i++)
        {

            string name = groups[i].name;
            float total_duration = 0;
            if (name.ToLower().Contains("head"))
            {
                AddSenetens(create_head, groups[i].name);
            }

            if (name.ToLower().Contains("eyebrow"))
            {
                AddSenetens(common, groups[i].name);
            }
            else if (name.ToLower().Contains("eye"))
            {
                AddSenetens(common, groups[i].name);
            }


            if (name.ToLower().Contains("mouth"))
            {
                AddSenetens(common, groups[i].name);
            }

            if (name.ToLower().Contains("stem"))
            {
                AddSenetens(common, groups[i].name);
            }

            if (name.ToLower().Contains("leave"))
            {
                AddSenetens(common, groups[i].name);
            }

            string[] splitStrings = groups[i].parent.Split('_');

            bool back_camera = false;
            if (splitStrings.Length > 1 && splitStrings[1] == "back")
            {
                back_camera = true;
            }

            if (groups[i].children.Count == 0)
            {
                total_duration = 200;
            }
            Bounds groupAABB = new Bounds(new Vector3(0, 0, 0), Vector3.zero);
            for (int j = 0; j < groups[i].children.Count; j++)
            {

                float maxx = groupAABB.max.x;
                float maxy = groupAABB.max.y;
                float maxz = groupAABB.max.z;
                float minx = groupAABB.min.x;
                float miny = groupAABB.min.y;
                float minz = groupAABB.min.z;
                if(maxx < groups[i].children[j].aabb.max.x)
                {
                    maxx = groups[i].children[j].aabb.max.x;
                }
                if (maxy < groups[i].children[j].aabb.max.y)
                {
                    maxy = groups[i].children[j].aabb.max.y;
                }
                if (maxz < groups[i].children[j].aabb.max.z)
                {
                    maxz = groups[i].children[j].aabb.max.z;
                }

                if (minx > groups[i].children[j].aabb.min.x)
                {
                    minx = groups[i].children[j].aabb.min.x;
                }
                if (miny > groups[i].children[j].aabb.min.y)
                {
                    miny = groups[i].children[j].aabb.min.y;
                }
                if (minz > groups[i].children[j].aabb.min.z)
                {
                    minz = groups[i].children[j].aabb.min.z;
                }
                groupAABB.min = new Vector3(minx, miny, minz);
                groupAABB.max = new Vector3(maxz, maxy, maxz);

                existingBounds.Add(groups[i].children[j].aabb);
                CreateCube(groups[i].children[j].position, groups[i].children[j].scale, groups[i].children[j].pivot, groups[i].children[j].rotation, groups[i].children[j].uv);

                Model tmp = parts[part_index];
                tmp.duration = 0;
                if (j < 4)
                {
                    tmp.duration = 40;//tmp.scales[2].z * part_time;
                }
               
                tmp.frame = (int)(-total_time * 2f);
                tmp.cameraBack = back_camera;
                parts[part_index] = tmp;
                total_time += tmp.duration;
                total_duration += tmp.duration;
                part_index += 1;
            }
            Group g = groups[i];
            g.CameraBack = back_camera;
            g.aabb = groupAABB;
            g.CameraTarget = groupAABB.center;
            g.CameraPos = ChangeCamera(groupAABB, back_camera);
            
            groups[i] = g;

            times.Add(total_duration);

        }

        int randomValue3 = Random.Range(0, feels.Length);
        sentences.Add(feels[randomValue3].ToString());

        string content = "";

        for (int i = 0; i < sentences.Count; i++)
        {
            if(i >= times.Count)
            {
                content +="200\n";
            }
            else
            {

                content += times[i].ToString() + "\n";
            }
            content += sentences[i] + "\n";
        }

        if (GenerateStoryStart)
        {
            using (StreamWriter writer = new StreamWriter("D://sentences.txt"))
            {
                writer.Write(content);
            }
        }
    }

    List<string> sentences = new List<string>();

    // Update is called once per frame
    void FixedUpdate()
    {
        string text = "";
        int part_index = 0;
        for(int i = 0; i < groups.Count; i++)
        {
            for (int j = 0; j < groups[i].children.Count; j++)
            {
                Model tmp = parts[part_index];
                tmp.frame = tmp.frame + 1;
                if (tmp.frame > 0)
                {
                    if (tmp.frame <= tmp.duration)
                    {
                        tmp.SetOriginScale();
                        Camera.main.transform.position = groups[i].CameraPos;
                        Camera.main.transform.LookAt(groups[i].CameraTarget);
                        text = groups[i].name + " - " + groups[i].parent;
                    }
                    else if (tmp.frame <= tmp.duration * 2)
                    {
                        tmp.SetOriginTexture();
                        Camera.main.transform.position = groups[i].CameraPos;
                        Camera.main.transform.LookAt(groups[i].CameraTarget);
                        text = groups[i].name + " - " + groups[i].parent;
                    }
                    else if (tmp.duration == 0 && tmp.frame == 1)
                    {
                        tmp.SetOriginScale();
                        tmp.SetOriginTexture();
                    }

                }
                else
                {
                    tmp.SetZeroScale();
                    tmp.SetPlainTexture();
                }
                parts[part_index] = tmp;
                part_index += 1;
            }
        }
        tt.text = text;
        Model tmp2 = parts[parts.Count - 1];
        if (tmp2.frame >= tmp2.duration * 4)
        {
            Camera.main.transform.position = new Vector3(-23f, 20f, -33f);
            Camera.main.transform.LookAt(Vector3.zero + new Vector3(0, 10, 0));
        }


    }
    // 摄像机反向 N
    // 阻挡的话，就换个方向
    struct Sentence
    {
        public string content;
    }

    string[] video_start = { "I was given the task of turning all these mobs into full-on completely destructive insane mechs to add to their playable map on the Minecraft Marketplace that's crazy let's do it" };

    string[] part_start = { "I wanted to start with making the creeper Mech that shoots out tiny creepers that explode because that just sounds awesome we'll need to use a lot " };

    string[] common = { " we need a large _part", " and the _part !!!", "we will bring a _part", "then add some realistic _part to it", "so we will make it a _part", "the shape is gonna be totally different, it gonna be super _desc", "these _part will be _desc ones like these" };

    string[] face = { " we need a large face" };

    // 当体积小于20的部件超过了5个
    string[] many = { "we'll need to use a lot of pieces to make the geometry for these" };

    string[] legs = { "let`s build the legs" };

    string[] stems = { "let`s build the stems" };

    string[] body = { " time for building the body", "and we'll add his shirt thing around his body", "and we'll start to make the shape of his body" };


    string[] start_color = { "and I think that's a pretty good body so now let's start to color her in", "I think this is good so now let's start to add some color" };

    string[] color = {"let's color in our _part", "next let's color his _part _color", "we'll color in his _part", "first I'm going to color his _part fully _color", "but used _color as the color palette",
                        "the texturing i`ll do will make all these _color areas looks like _name", "once the model is done, I get to some basic colors, you know some _color"};

    string[] create_hat = { "and let's start to add her hat on the top as well" };

    string[] create_head = { "let`s start by building the head", "next is for the head", "the next thing one want to build is head",
        "starting with this head. ", "next I'm going to change the shape of her head", "and we'll give him some eyes next let's add his nose", " next I'm going to start to give him a head" };

    string[] color_white = { "we'll give it that lifeless white skin color" };

    string[] ear = { "his classic pointy ears", " and his ears must extend much higher" };

    string[] ties = { " now finally let's start to give him a tie" };

    string[] dress = { " and Dracula's also always shockingly well dressed in a black suit with white undershirt" };

    string[] teeth = { "and also some very three very very sharp teeth", "we'll just give him some teeth on the bottom like this" };

    string[] arms = { " next I'm going to give her some arms and I'll just add a hand on the bottom here", "now let's start giving him an arm on the side" };

    string[] ways = { " we're going to make them look way more robotic " };

    string[] chests = { " and now we need to build out this chest piece as well" };

    string[] hands = { "for the hands I'll use some of these gray pieces to make these super strong fingers" };

    string[] happy = { "I'm pretty happy with the shape of this guy" };

    string[] feels = { "now that it's time to retexture I'm going to focus on giving it that rusted robot feel",
        "we'll add a bit more detail and I think we're done"
        ," I think that looks good let's see it in game"};
    // a material for each face



}
