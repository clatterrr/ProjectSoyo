using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class RealFighter : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSpeed = 5.0f;
    public GameObject healthBarPrefab; // ��ק���䵽 HealthBarPrefab Ԥ�Ƽ�
    public Canvas uiCanvas; // ��ק���䵽�����е� Canvas
    public Vector3 healthBarOffset = new Vector3(0, 2, 0); // Ѫ��������������λ��ƫ��

    private GameObject healthBarInstance;
    private RectTransform redBarRectTransform; // ��ɫѪ������ RectTransform
    private float maxHealth = 100f;
    private float currentHealth;

    private RealFighter closestTarget;
    private float closestDistance = Mathf.Infinity;

    private int damage = 10;
    public bool positive = true;
    private bool FirstDead = true;
    private Vector3 weaponVelocity;

    private int walkAwayCounter = 0;
    private int attackMaxTime = 0;
    private string animationName = "";

    public enum FightStrategy
    {
        WalkTo,
        Melee,
        WalkAway,
        Dead,
    }

    private FightStrategy fightStrategy;

    public void SetStrategy(FightStrategy strategy)
    {
        fightStrategy = strategy;
    }

    public void Kill()
    {
        Destroy(weapon);
        Destroy(healthBarInstance);
        Destroy(gameObject);
    }
    void Start()
    {
        currentHealth = maxHealth;
        // �� Canvas ������ HealthBar ʵ��
        healthBarInstance = Instantiate(healthBarPrefab, uiCanvas.transform);

        healthBarInstance.GetComponent<Image>().color = Color.white;
        healthBarInstance.transform.Find("RedBar").GetComponent<Image>().color = Color.red;
        redBarRectTransform = healthBarInstance.transform.Find("RedBar").GetComponent<RectTransform>();

        UpdateHealthBar();
        SetAllRigidbodiesKinematic(gameObject, true);

        target = GameObject.CreatePrimitive(PrimitiveType.Cube);
        target.transform.position = new Vector3(0, -16, -7);
        target.transform.localScale = new Vector3(1, 1, 1);
        attackMaxTime = Random.Range(200, 300);
        damage = Random.Range(20, 50);
        FirstDead = true;
        Debug.Log("attack Max Time " + attackMaxTime + " damage = " + damage);

    }

    void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;
        // ���ݽ�������������ɫѪ�����Ŀ��
        if (redBarRectTransform != null)
        {
            redBarRectTransform.localScale = new Vector3(healthPercentage, 1f, 1f);
            redBarRectTransform.localPosition = new Vector3(-50f * (1 - healthPercentage), 0, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private GameObject weapon;
    public void TakeWeapon(GameObject weaponPrefab)
    {
        Transform rightHand = FindChildRecursive(gameObject.transform, "mixamorig:RightHand");
        weapon = Instantiate(weaponPrefab, rightHand.position,  Quaternion.Euler(14,-6,154));
        weapon.transform.localScale = transform.localScale * 50;
        
    }

    // Update is called once per frame
    public bool isDead()
    {
        return currentHealth <= 0;
    }

    public bool isAlive()
    {
        return currentHealth > 0;
    }
    void UpdateHealth()
    {
        if (!isDead())
        {
            float ratio = currentHealth / maxHealth;
            if (healthBarInstance != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + healthBarOffset);
                healthBarInstance.transform.position = screenPosition;
            }
        }

        if (currentHealth <= 0f && FirstDead)
        {
            FirstDead = false;
            // ���ٽ�����ʵ��
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }
            gameObject.GetComponent<Animator>().enabled = false;
            SetAllRigidbodiesKinematic(gameObject, false);
            AddForce(new Vector3(0, 300, 0), "mixamorig:Hips", ForceMode.Impulse);
        }

    }

    public void AddForce(Vector3 forceDirection, string targetBoneName, ForceMode forceMode)
    {
        Transform targetBone = FindChildRecursive(gameObject.transform, targetBoneName);
        Rigidbody rb = targetBone.GetComponent<Rigidbody>();
        rb.AddForce(forceDirection, forceMode);
    }

    Transform FindChildRecursive(Transform parent, string targetBoneName)
    {
        // ��鵱ǰ�����Ƿ�ƥ��Ŀ������
        if (parent.name == targetBoneName)
        {
            return parent;
        }

        // ������ǰ���������������
        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, targetBoneName);
            if (result != null)
            {
                return result;
            }
        }

        // ���û���ҵ�ƥ��������壬���� null
        return null;
    }

    void SetAllRigidbodiesKinematic(GameObject obj, bool state)
    {
        // ��ȡ��ǰ�����Rigidbody���������еĻ���
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = state;
        }

        // �ݹ���Ҳ������������е�Rigidbody
        foreach (Transform child in obj.transform)
        {
            SetAllRigidbodiesKinematic(child.gameObject, state);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        closestTarget = null;
        closestDistance = Mathf.Infinity;
        FindFighter();
        AttackTarget();
        walkAwayCounter--;
        UpdateHealth();

        Transform rightHand = FindChildRecursive(gameObject.transform, "mixamorig:RightHandIndex2");
        if (weapon != null)
        {
            weapon.transform.position = rightHand.position;

            float ry = 0;
            if(weaponVelocity.x > 0)
            {
                ry = 0.0f;
            }
            else
            {
                ry = 1.0f;
            }

            weapon.transform.rotation = Quaternion.Euler(-90f, ry * 180f , 90f);
        }

    }

    private void LateUpdate()
    {
        switch (fightStrategy)
        {
            case FightStrategy.Melee:
                {
                    int downtime = (int)((float)attackMaxTime * 0.2f);
                    ExecuteSlashAction(attackMaxTime - downtime, downtime, 80);
                    break;
                }
            default:
                {

                    ExecuteSlashAction(250, 50, 0);
                    break;
                }
        }
    }

    void SelectAnimation(string path)
    {
        if(path != animationName)
        {
            animationName = path;
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/FightAnimations/" + path + ".fbx");
            AnimatorController controller = new AnimatorController();
            controller.AddLayer("Base Layer");
            AnimatorState state = controller.layers[0].stateMachine.AddState("Default State");
            state.motion = clip;
            GetComponent<Animator>().runtimeAnimatorController = controller;
        }

    }
    void FindFighter()
    {
        RealFighter[] fighters = FindObjectsOfType<RealFighter>();
        if (fighters.Length > 0)
        {
            foreach (RealFighter fighter in fighters)
            {
                // �ų��Լ�
                if (fighter.gameObject == this.gameObject)
                    continue;

                float distanceToTarget = Vector3.Distance(transform.position, fighter.transform.position);
                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = fighter;
                }
            }
        }
    }

    void AttackTarget()
    {

        // �������Ŀ���ƶ�
        if (closestTarget != null)
        {
            Debug.Log(" name = " + transform.name + " strae " + fightStrategy.ToString());
            Debug.Log(" clos name = " + closestTarget.transform.name + " strae " + closestTarget.fightStrategy.ToString());


            if (closestDistance < 3f)
            {
                SetStrategy(FightStrategy.Melee);
                closestTarget.SetStrategy(FightStrategy.Melee);
            }
            else
            {

                SetStrategy(FightStrategy.WalkTo);
                closestTarget.SetStrategy(FightStrategy.WalkTo);
            }

            Vector3 direction = (closestTarget.transform.position - transform.position).normalized;
            weaponVelocity = direction;
            if (fightStrategy == FightStrategy.WalkTo)
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {

                //transform.position -= direction * moveSpeed * Time.deltaTime;
            }
            transform.LookAt(closestTarget.transform.position);
            weapon.transform.LookAt(closestTarget.transform.position);
        }
        if(closestTarget != null)
        {
            switch (fightStrategy)
            {
                case FightStrategy.Melee:
                    {
                        SelectAnimation("IdleAiming");
                        if (SavedAttackCount > 0 && !isDead())
                        {
                            SavedAttackCount -- ; 
                            closestTarget.TakeDamage(damage); 
                            
                        }
                        break;
                    }
                case FightStrategy.WalkTo: SelectAnimation("CrouchedWalking");break;
                default: break;
            }
        }
    }

    private GameObject target = null;

    void RecursiveFindAndModify(string targetName, Transform current, Quaternion rotation, Quaternion rotation1)
    {
        // ��鵱ǰTransform�������Ƿ�������Ҫ�ҵ�
        if (current.name == targetName)
        {
            Vector3 towardObjectFromHead = target.transform.position - new Vector3(0, 35, 0) - current.position;
            if (positive)
            {
                current.rotation = rotation; // target.transform.rotation;// Quaternion.LookRotation(towardObjectFromHead, transform.up);
            }
            else
            {
               current.rotation = rotation1;
            }
            

        }

        // �ݹ��������������
        foreach (Transform child in current)
        {
            RecursiveFindAndModify(targetName, child, rotation, rotation1);
        }
    }


    public int actionRecordTime = 0;
    private int SavedAttackCount = 0;
    void ExecuteSlashAction(int riseTime, int downTime, float riseRange)
    {
        if(actionRecordTime < riseTime)
        {
            float ration = actionRecordTime * 1.0f / riseTime;
            RecursiveFindAndModify("mixamorig:RightArm", gameObject.transform,
                Quaternion.Euler(0, 0, 180 - riseRange + 2 * ration * riseRange),
                Quaternion.Euler(180, 0, - riseRange + 2 * ration * riseRange));
        }
        else if(actionRecordTime < riseTime +  downTime)
        {

            float ration = (actionRecordTime - riseTime) * 1.0f / downTime;
            RecursiveFindAndModify("mixamorig:RightArm", gameObject.transform,
                Quaternion.Euler(0, 0, 180 + riseRange - 2 * ration * riseRange),
                Quaternion.Euler(180, 0, riseRange - 2 * ration * riseRange));
        }
        actionRecordTime++;
        if(actionRecordTime == riseTime + downTime)
        {
            if(fightStrategy == FightStrategy.Melee)
            {
                SavedAttackCount++;
            }
           
            actionRecordTime = 0;
        }
    }
}
