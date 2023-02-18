using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ServeBall : Node
{
    private Transform _transform;
    private Transform[] _waypoints;
    private Animator _animator;

    public ServeBall(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        _waypoints = waypoints;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = _waypoints[2];
        Vector3 _target = target.position;
        _target.y = 0;

        if(KyleBT.AIserving)
        {
            _animator.SetBool("Walking", true);
            _transform.position = Vector3.MoveTowards(_transform.position, _target, KyleBT.speed * 2 * Time.deltaTime);
            _transform.LookAt(Vector3.left);

            KyleBT.AIserving = false;

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
