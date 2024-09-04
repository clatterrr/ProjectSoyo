using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Structure;

public class SplitPeaController : MonoBehaviour
{
    private float timer;
    private float fireInterval = 2f;
    private float sphereSpeed = 1f;
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            FireSphere();
            timer = 0f; // 重置计时器
        }
    }

    void FireSphere()
    { 
        
        Vector3 v = GetChildPos(transform, "Loc");
        // 创建一个新的sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * 0.1f; 
        sphere.transform.position = v; // 设置sphere的位置为火炮的位置
        sphere.AddComponent<SphereMovement>().speed = new Vector3(v.x, 0,v.z); // 为sphere添加移动脚本，并设置速度


        GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere2.transform.localScale = Vector3.one * 0.1f;
        sphere2.transform.position = new Vector3(-v.x, v.y, v.z); // 设置sphere的位置为火炮的位置
        sphere2.AddComponent<SphereMovement>().speed = new Vector3(-v.x, 0, v.z); // 为sphere添加移动脚本，并设置速度

        GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere3.transform.localScale = Vector3.one * 0.1f;
        sphere3.transform.position = new Vector3(-v.x, v.y, v.z); // 设置sphere的位置为火炮的位置
        sphere3.AddComponent<SphereMovement>().speed = new Vector3(v.x, 0, v.z); // 为sphere添加移动脚本，并设置速度
    }
}

public class SphereMovement : MonoBehaviour
{
    public Vector3 speed;

    private void Start()
    {
        AddBoxCollider(gameObject);
    }
    void Update()
    {
        // 沿着Z轴方向前进
        transform.Translate(speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ZombieEntity>() != null)
        {
            MinecraftEntity entity = other.GetComponent<MinecraftEntity>();
            entity.TakeDamge();
        }
    }
}
