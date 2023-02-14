using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckBall : Node
{
    private static int ballLayerMask = 1 << 6;
    private Transform _transform;

    public CheckBall(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("Ball");
        if(t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position, KyleBT.fovRange, ballLayerMask);

            if(colliders.Length > 0)
            {
                parent.parent.SetData("Ball", colliders[0].transform);
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
