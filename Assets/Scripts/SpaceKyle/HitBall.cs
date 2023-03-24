using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class HitBall : Node
{
    private Animator _animator;

    public HitBall(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        _animator.SetBool("Walking", false);
        _animator.SetBool("HittingBall", true);

        state = NodeState.RUNNING;
        return state;
    }

}
