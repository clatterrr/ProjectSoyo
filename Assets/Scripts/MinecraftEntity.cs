using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using static Structure;

public class MinecraftEntity : MonoBehaviour
{
    public float speed = 2f;        // 物体移动的速度
    public float range = 20f;       // 随机游走的范围
    public float minPauseTime = 1f; // 最短停留时间
    public float maxPauseTime = 5f; // 最长停留时间

    private Vector3 targetPosition;
    private bool isWalking = true;
    private float pauseTimer = 0f;
    private float currentPauseTime = 0f;

    private Color normalMaterial;
    private Color damageMaterial;

    private float baseY = 0;

    private string audioClipPath = "Audio/creatureHurt"; // 声音文件的路径，假设在Assets/Resources/Audio/mySound.wav

    private AudioSource audioSource;

    private float health = 0;

    private AudioClip hitClip;

    void Start()
    {
        AddBoxCollider(gameObject);
        SetRandomTargetPosition();
        SetRandomPauseTime();

        normalMaterial = GetAnyMateiral(transform).color;
        damageMaterial = GetAnyMateiral(transform).color;
        damageMaterial.r = Mathf.Clamp01(damageMaterial.r + 0.5f);
        damageMaterial.g = Mathf.Clamp01(damageMaterial.g - 0.5f);
        damageMaterial.b = Mathf.Clamp01(damageMaterial.b - 0.5f);
        TakeDamge();
        baseY = transform.position.y;

        audioSource = gameObject.AddComponent<AudioSource>();
        hitClip = Resources.Load<AudioClip>(audioClipPath);

        
    }

    enum Status
    {
        Idle,
        Walking,
        Hurting,
        Dead,
    }

    private Status status;
    private int DamageMaxCount = 60;
    private int DamageCount = 0;

    public void TakeDamge()
    {
        DamageCount = 0;
        if (hitClip != null)
        {
            audioSource.clip = hitClip;
            audioSource.Play();
        }
    }

    Material GetAnyMateiral(Transform parent)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            return parent.GetComponent<MeshRenderer>().material;
        }
        foreach (Transform child in parent)
        {
            GetAnyMateiral(child);
        }
        return new Material(Shader.Find("Standard"));
    }

    void SetChildMaterial(Transform parent, Color color)
    {
        if (parent.GetComponent<MeshRenderer>() != null)
        {
            Material material = parent.GetComponent<MeshRenderer>().material;
            material.color = color;
            parent.GetComponent<MeshRenderer>().material = material;
        }
        foreach (Transform child in parent)
        {
            SetChildMaterial(child, color);
        }
    }

    void RandomWalk()
    {
        if (isWalking)
        {
            // 移动物体到目标位置
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 如果物体接近目标位置，设置一个新的随机目标位置
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetRandomTargetPosition();
                SetRandomPauseTime();
                isWalking = false;
            }
        }
        else
        {
            // 处理暂停计时
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= currentPauseTime)
            {
                isWalking = true;
                pauseTimer = 0f;

                // 随机选择是否停止继续移动
                if (Random.value > 0.5f)
                {
                    SetRandomPauseTime();
                    isWalking = false;
                }
            }
        }
    }

    void FixedUpdate()
    {
        //RandomWalk();
        DamageCount++;
        if(DamageCount < DamageMaxCount)
        {
            status = Status.Hurting;
            float t = DamageCount * 1.0f/ DamageMaxCount;

            // 计算物体的高度，根据抛物线公式计算
            float currentHeight = Mathf.Lerp(0, 3, t) * (1 - t);

            // 更新物体的位置
           // transform.position = new Vector3(transform.position.x, baseY +  currentHeight, transform.position.z);
          //  SetChildMaterial(transform, damageMaterial);
        }
        else
        {
            status = Status.Idle;
            SetChildMaterial(transform, normalMaterial);
        }

        
    }

    void SetRandomTargetPosition()
    {
        // 在范围内生成一个随机的目标位置
        float randomX = Random.Range(-range, range);
        float randomZ = Random.Range(-range, range);

        // 保持y轴不变
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }

    void SetRandomPauseTime()
    {
        // 随机设置一个暂停时间
        currentPauseTime = Random.Range(minPauseTime, maxPauseTime);
    }
}
