using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class HitBall : Node
{
    private Animator _animator;

    private float _hitTime = 1f;
    private float _hitCounter = 0f;

    public HitBall(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = GameObject.Find("Ball").transform;
        _hitCounter += Time.deltaTime;

        if(_hitCounter >= _hitTime)
        {
            Debug.Log("Done");
            _animator.SetBool("HittingBall", false);
            _animator.SetBool("Walking", true);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
