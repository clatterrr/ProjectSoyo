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

        // 获取 Animator 的 Controller
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            animatorController = CreateDefaultAnimatorController();
            animator.runtimeAnimatorController = animatorController;
            Debug.Log("A new AnimatorController has been created and assigned.");
        }

        // 检查当前 AnimationClip 是否有效
        if (clip == null)
        {
            Debug.LogError("No AnimationClip assigned.");
            return;
        }
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        // 查找或创建与当前 AnimationClip 对应的 AnimatorState
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
            // 如果未找到对应的状态，则创建一个新的状态
            newState = animatorController.layers[0].stateMachine.AddState(clip.name);
            newState.motion = clip;
        }

        // 设置为默认状态
        animatorController.layers[0].stateMachine.defaultState = newState;
    }

    AnimatorController CreateDefaultAnimatorController()
    {
        // 动态创建一个新的 AnimatorController
        var animatorController = new AnimatorController();

        // 创建一个默认的层和状态机
        animatorController.AddLayer("Base Layer");
        var rootStateMachine = animatorController.layers[0].stateMachine;

        // 创建一个空的默认状态
        var idleState = rootStateMachine.AddState("Idle");

        // 可以在这里为 Idle 状态设置动画剪辑（可选）
        // AnimationClip idleClip = new AnimationClip();
        // idleState.motion = idleClip;

        return animatorController;
    }


    void FindFighter()
    {
        // 找到所有带有 Fighter 组件的对象
        Fighter[] fighters = FindObjectsOfType<Fighter>();

        if (fighters.Length > 0)
        {
            // 选择最近的目标
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            Fighter targetFighter = null;

            foreach (Fighter fighter in fighters)
            {
                // 排除自己
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
