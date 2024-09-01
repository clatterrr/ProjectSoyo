using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject c0;
    public GameObject c1;
    public RuntimeAnimatorController rac;
    void Start()
    {
        c0.GetComponent<Transform>().position = new Vector3(0,0, 0);
        c1.GetComponent<Transform>().position = new Vector3(1,0, 0);
        c0.GetComponent<Animator>().runtimeAnimatorController = rac;
        c1.GetComponent<Animator>().runtimeAnimatorController = rac;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
