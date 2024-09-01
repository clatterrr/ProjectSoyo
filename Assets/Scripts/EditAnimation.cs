using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditAnimation : MonoBehaviour
{
    public GameObject boneSpine;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.GetComponent<Animator>().enabled = false;
        boneSpine.transform.rotation = Quaternion.Euler(0,90,0);
    }
}
