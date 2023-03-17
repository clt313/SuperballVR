using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ApproachBall : Node
{
    private Transform _transform;
    private Transform _net;

    public ApproachBall(Transform transform)
    {
        _transform = transform;
        _net = GameObject.Find("TennisNet").transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = GameObject.Find("Ball").transform;
        Vector3 _target = target.position;
        _target.y = _transform.position.y;

        if(Vector3.Distance(_transform.position, _target) > 0.01f)
        {
            // if (_transform.position.x > _net.position.x)
            // {
            //     _transform.position.x = 0.5f;
            // }
            // Debug.Log("Approaching");
            _transform.position = Vector3.MoveTowards(_transform.position, _target, KyleBT.speed * 2 * Time.deltaTime);
            _transform.LookAt(_target);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
