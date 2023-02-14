using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckBallInRange : Node
{
    private Transform _transform;
    private Animator _animator;

    public CheckBallInRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        GameObject t = GameObject.Find("Ball");
        if(t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Transform target = t.transform;
        if(Vector3.Distance(_transform.position, target.position) <= KyleBT.hitRange)
        {
            _animator.SetBool("BasicMotions@Jump01", true);
            _animator.SetBool("BasicMotions@Walk01-Forwards", false);
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
