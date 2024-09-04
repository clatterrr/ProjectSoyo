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
            timer = 0f; // ���ü�ʱ��
        }
    }

    void FireSphere()
    { 
        
        Vector3 v = GetChildPos(transform, "Loc");
        // ����һ���µ�sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * 0.1f; 
        sphere.transform.position = v; // ����sphere��λ��Ϊ���ڵ�λ��
        sphere.AddComponent<SphereMovement>().speed = new Vector3(v.x, 0,v.z); // Ϊsphere����ƶ��ű����������ٶ�


        GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere2.transform.localScale = Vector3.one * 0.1f;
        sphere2.transform.position = new Vector3(-v.x, v.y, v.z); // ����sphere��λ��Ϊ���ڵ�λ��
        sphere2.AddComponent<SphereMovement>().speed = new Vector3(-v.x, 0, v.z); // Ϊsphere����ƶ��ű����������ٶ�

        GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere3.transform.localScale = Vector3.one * 0.1f;
        sphere3.transform.position = new Vector3(-v.x, v.y, v.z); // ����sphere��λ��Ϊ���ڵ�λ��
        sphere3.AddComponent<SphereMovement>().speed = new Vector3(v.x, 0, v.z); // Ϊsphere����ƶ��ű����������ٶ�
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
        // ����Z�᷽��ǰ��
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
