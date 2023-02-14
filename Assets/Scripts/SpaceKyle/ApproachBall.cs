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
        Transform target = GameObject.Find("Ball").transform;
        Vector3 _target = target.position;
        _target.y = 0;

        if(Vector3.Distance(_transform.position, _target) > 0.01f)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, _target, KyleBT.speed * Time.deltaTime);
            _transform.LookAt(_target);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
