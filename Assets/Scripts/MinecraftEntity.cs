using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using static Structure;

public class MinecraftEntity : MonoBehaviour
{
    public float speed = 2f;        // �����ƶ����ٶ�
    public float range = 20f;       // ������ߵķ�Χ
    public float minPauseTime = 1f; // ���ͣ��ʱ��
    public float maxPauseTime = 5f; // �ͣ��ʱ��

    private Vector3 targetPosition;
    private bool isWalking = true;
    private float pauseTimer = 0f;
    private float currentPauseTime = 0f;

    private Color normalMaterial;
    private Color damageMaterial;

    private float baseY = 0;

    private string audioClipPath = "Audio/creatureHurt"; // �����ļ���·����������Assets/Resources/Audio/mySound.wav

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
            // �ƶ����嵽Ŀ��λ��
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // �������ӽ�Ŀ��λ�ã�����һ���µ����Ŀ��λ��
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetRandomTargetPosition();
                SetRandomPauseTime();
                isWalking = false;
            }
        }
        else
        {
            // ������ͣ��ʱ
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= currentPauseTime)
            {
                isWalking = true;
                pauseTimer = 0f;

                // ���ѡ���Ƿ�ֹͣ�����ƶ�
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

            // ��������ĸ߶ȣ����������߹�ʽ����
            float currentHeight = Mathf.Lerp(0, 3, t) * (1 - t);

            // ���������λ��
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
        // �ڷ�Χ������һ�������Ŀ��λ��
        float randomX = Random.Range(-range, range);
        float randomZ = Random.Range(-range, range);

        // ����y�᲻��
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }

    void SetRandomPauseTime()
    {
        // �������һ����ͣʱ��
        currentPauseTime = Random.Range(minPauseTime, maxPauseTime);
    }
}
