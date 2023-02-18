using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class PatrolCourt : Node
{
    private Transform _transform;
    private Transform[] _waypoints;
    private Animator _animator;

    private int _currentWaypointIndex = 0;

    // private float _waitTime = 1f;
    // private float _waitCounter = 0f;
    // private bool _waiting = false;

    public PatrolCourt(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        _waypoints = waypoints;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        // if(_waiting)
        // {
        //     _waitCounter += Time.deltaTime;

        //     if(_waitCounter >= _waitTime)
        //     {
        //         _waiting = false;
        //         _animator.SetBool("Walking", true);
        //     }
        // }
        // else
        // {
            _animator.SetBool("Walking", false);
            _animator.SetBool("HittingBall", false);
            Transform wp = _waypoints[_currentWaypointIndex];
            Vector3 _wp = wp.position;
            // _wp.y = 0f;

            if(_currentWaypointIndex == 0)
            {
                _animator.SetBool("StrafeRight", true);
                _animator.SetBool("StrafeLeft", false);
            }
            else{
                _animator.SetBool("StrafeRight", false);
                _animator.SetBool("StrafeLeft", true);
            }

            if(Vector3.Distance(_transform.position, _wp) < 0.01f)
            {
                _transform.position = _wp;
                // _waitCounter = 0f;
                // _waiting = true;
                if(_currentWaypointIndex == 0)
                {
                    _currentWaypointIndex = 1;
                }
                else{
                    _currentWaypointIndex = 0;
                }
                //_animator.SetBool("Walking", false);
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, _wp, KyleBT.speed * Time.deltaTime);
                _transform.LookAt(Vector3.left);
            }
        //}

        state = NodeState.RUNNING;
        return state;
    }
}
