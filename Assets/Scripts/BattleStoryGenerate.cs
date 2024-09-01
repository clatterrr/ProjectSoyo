using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BattleStoryGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cameraStart;
    public GameObject cameraEnd;

    public GameObject c0Start;
    public GameObject c1Start;
    public GameObject c0End;
    public GameObject c1End;

    public GameObject c0;
    public GameObject c1;

    Vector3 GetPos(GameObject go)
    {
        return go.transform.position;
    }
    Vector3 GetLookAt(GameObject go)
    {
        return Matrix4x4.Rotate(go.transform.rotation).GetColumn(2).normalized;
    }

    string Back(Vector3 pos)
    {
        return pos.x.ToString() + " " + pos.y.ToString() + " " + pos.z.ToString() + " ";
    }
    void Start()
    {
        c0Start.GetComponent<MeshRenderer>().enabled = false;
        c1Start.GetComponent<MeshRenderer>().enabled = false;
        c0End.GetComponent<MeshRenderer>().enabled = false;
        c1End.GetComponent<MeshRenderer>().enabled = false;



        string the_str = "";
        the_str += "c0 anim " + Back(GetPos(c0Start)) + Back(GetPos(c0End)) + Back(GetLookAt(c0Start)) + Back(GetLookAt(c0End)) + "\n";
        the_str += "c1 anim " + Back(GetPos(c1Start)) + Back(GetPos(c1End)) + Back(GetLookAt(c1Start)) + Back(GetLookAt(c1End)) + "\n";
        the_str += "camera anim " + Back(GetPos(cameraStart)) + Back(GetPos(cameraEnd)) + Back(GetLookAt(cameraStart)) + Back(GetLookAt(cameraEnd)) + "0 100\n";
        File.WriteAllText("D://write.txt", the_str);

    }

    int frame = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        frame++;
        if (frame < 100)
        {
            float ratio = (float)((float)frame / 100.0);
            Vector3 offset = cameraEnd.transform.position - cameraStart.transform.position;
            Camera.main.transform.position = offset * ratio + cameraStart.transform.position;
            Camera.main.transform.rotation = Quaternion.Slerp(cameraStart.transform.rotation, cameraEnd.transform.rotation, ratio);

            c0.transform.position = c0Start.transform.position + ratio * (c0End.transform.position - c0Start.transform.position);
            c1.transform.position = c1Start.transform.position + ratio * (c1End.transform.position - c1Start.transform.position);
            c0.transform.rotation = Quaternion.Slerp(c0Start.transform.rotation, c0End.transform.rotation, ratio);
            c1.transform.rotation = Quaternion.Slerp(c1Start.transform.rotation, c1End.transform.rotation, ratio);


        }
    }
}
