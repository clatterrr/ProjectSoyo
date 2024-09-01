using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Bomb prefabToSpawn; // 要生成的 Prefab
    private Transform centerTransform;

    private int frameCounter = 0;
    private int targetFrameCount;
    // Start is called before the first frame update
    void Start()
    {
        centerTransform = transform.Find("center");
        targetFrameCount = Random.Range(30, 60);
        transform.rotation = Quaternion.Euler(-90, 0, 90);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frameCounter ++;
        if(frameCounter >= targetFrameCount )
        {
            frameCounter = 0;
            targetFrameCount = Random.Range(30, 60);
            Bomb bomb = Instantiate(prefabToSpawn, centerTransform.position, Quaternion.identity);
        }
    }
}
