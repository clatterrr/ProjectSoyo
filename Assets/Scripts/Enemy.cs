using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1f;

    private AnimationClip walkClip;
    private AnimationClip attackClip;
    private Animator animator;
    public int AttackTime = 60;

    private State state;

    enum State
    {
        Walk,
        Attack
    }

    private void Start()
    {
        state = State.Walk;
        walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/BeastWalk.anim");
        attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Punch.anim");
    }
    private int attack_count = 0;
    private void FixedUpdate()
    {
        FindFighter();
        attack_count--;
    }

    void ConnectClip(AnimationClip clip)
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found on this GameObject.");
            return;
        }

        // ��ȡ Animator �� Controller
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            animatorController = CreateDefaultAnimatorController();
            animator.runtimeAnimatorController = animatorController;
            Debug.Log("A new AnimatorController has been created and assigned.");
        }

        // ��鵱ǰ AnimationClip �Ƿ���Ч
        if (clip == null)
        {
            Debug.LogError("No AnimationClip assigned.");
            return;
        }
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // ���һ򴴽��뵱ǰ AnimationClip ��Ӧ�� AnimatorState
        AnimatorState newState = null;
        foreach (var layer in animatorController.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.motion == clip)
                {
                    newState = state.state;
                    break;
                }
            }
            if (newState != null) break;
        }

        if (newState == null)
        {
            // ���δ�ҵ���Ӧ��״̬���򴴽�һ���µ�״̬
            newState = animatorController.layers[0].stateMachine.AddState(clip.name);
            newState.motion = clip;
        }

        // ����ΪĬ��״̬
        animatorController.layers[0].stateMachine.defaultState = newState;
    }

    AnimatorController CreateDefaultAnimatorController()
    {
        // ��̬����һ���µ� AnimatorController
        var animatorController = new AnimatorController();

        // ����һ��Ĭ�ϵĲ��״̬��
        animatorController.AddLayer("Base Layer");
        var rootStateMachine = animatorController.layers[0].stateMachine;

        // ����һ���յ�Ĭ��״̬
        var idleState = rootStateMachine.AddState("Idle");

        // ����������Ϊ Idle ״̬���ö�����������ѡ��
        // AnimationClip idleClip = new AnimationClip();
        // idleState.motion = idleClip;

        return animatorController;
    }


    void FindFighter()
    {
        // �ҵ����д��� Fighter ����Ķ���
        Fighter[] fighters = FindObjectsOfType<Fighter>();

        if (fighters.Length > 0)
        {
            // ѡ�������Ŀ��
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            Fighter targetFighter = null;

            foreach (Fighter fighter in fighters)
            {
                // �ų��Լ�
                if (fighter.gameObject == this.gameObject)
                    continue;
                if (fighter.isDead()) {
                    continue;
                }

                float distanceToTarget = Vector3.Distance(transform.position, fighter.transform.position);
                if (distanceToTarget < closestDistance && distanceToTarget > 1)
                {
                    closestDistance = distanceToTarget;
                    targetFighter = fighter;
                }
            }

            if(targetFighter != null)
            {
                float targetDistance = Vector3.Distance(transform.position, targetFighter.transform.position);
                if (targetDistance < 2)
                {

                    if (attack_count <= 0)
                    {
                        attack_count = AttackTime;
                        targetFighter.TakeDamage(10f);
                    }
                    ConnectClip(attackClip);

                }
                else if(attack_count <= 0)
                {
                    Vector3 direction = (targetFighter.transform.position - transform.position).normalized;
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    transform.LookAt(targetFighter.transform.position);
                    ConnectClip(walkClip);
                }
            }
            else
            {

                ConnectClip(walkClip);
            }

        }
        
    }
}
