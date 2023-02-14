using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ApproachBall : Node
{
    private Transform _transform;

    public ApproachBall(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("Ball");

        if(Vector3.Distance(_transform.position, target.position) > 0.01f)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, target.position, KyleBT.speed * Time.deltaTime);
            _transform.LookAt(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
