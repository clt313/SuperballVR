using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckBall : Node
{
    private static int ballLayerMask = 1 << 1;
    private Transform _transform;
    private Animator _animator;

    public CheckBall(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        GameObject t = GameObject.Find("Ball");
        if(t != null)
        {
            if(Vector3.Distance(_transform.position, t.transform.position) < KyleBT.fovRange)
            {
                _animator.SetBool("StrafeLeft", false);
                _animator.SetBool("StrafeRight", false);
                _animator.SetBool("Walking", true);

                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}
