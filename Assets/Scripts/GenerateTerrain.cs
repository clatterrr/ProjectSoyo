using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    public Material grass;
    public Material stone;
    public Material path;

    public GameObject blocks;

    void Start()
    {
        for (int j = 0; j < 5; j++)
        {
            float grassRatio = 0.9f;
            for (int i = 0; i < 30; i++)
            {

                Vector3 sd = new Vector3(-70 + 10 * i, -10, -35  + j * 10);
                GameObject block = Instantiate(blocks, sd, Quaternion.identity);
                Material m = grass;
                if(j == 0 || j == 4)
                {
                    m = stone;
                }
                else
                {
                    float r = Random.Range(0.0f,1.0f);
                    if(r < grassRatio)
                    {
                        m = grass;
                        grassRatio -= 0.1f;
                    }
                    else
                    {
                        m = path;
                        grassRatio = 0.9f;
                    }
                }
                block.GetComponent<MeshRenderer>().material = m;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
