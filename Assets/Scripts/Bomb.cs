using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{


    // Update is called once per frame

    private Vector3 moveDirection = new Vector3 (0f, 0f, 1f);
    private float moveSpeed = 15.0f;

    private void Start()
    {
        moveSpeed = Random.Range(10, 30);
        frameCount = 0;
    }
    private bool couldTakeDamage = true;

    int frameCount = 0;
    void Update()
    {
        Vector3 movement = moveDirection.normalized * moveSpeed * Time.deltaTime;

        // ÒÆ¶¯ÎïÌå
        transform.Translate(movement, Space.World);

        frameCount++;
        if(frameCount > 1000)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (couldTakeDamage)
        {
            couldTakeDamage = false;
        }
        else
        {
            return;
        }

        Walker fighter = other.GetComponent<Walker>();
        if (fighter != null)
        {
            fighter.TakeDamage(50f);
            
        }


        MinecraftFighter walker = other.GetComponent<MinecraftFighter>();
        if (walker != null)
        {
            walker.TakeDamage(70);
        }

        MinecraftEntity entity = other.GetComponent<MinecraftEntity>();
        if (walker != null)
        {
            entity.TakeDamge(10);
        }
    }

}
