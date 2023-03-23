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
    Rigidbody rb = _transform.root.GetComponent<Rigidbody>();
    // _wp.y = 0f;

    if (_currentWaypointIndex == 0)
    {
      _animator.SetBool("StrafeRight", true);
      _animator.SetBool("StrafeLeft", false);
    }
    else
    {
      _animator.SetBool("StrafeRight", false);
      _animator.SetBool("StrafeLeft", true);
    }

    if (Vector3.Distance(rb.position, _wp) < 0.01f)
    {
      rb.position = _wp;
      rb.velocity = Vector3.zero;
      // _waitCounter = 0f;
      // _waiting = true;
      if (_currentWaypointIndex == 0)
      {
        _currentWaypointIndex = 1;
      }
      else
      {
        _currentWaypointIndex = 0;
      }
      //_animator.SetBool("Walking", false);
    }
    else
    {
      Vector3 toWayPoint = _wp - rb.position;
      rb.velocity = toWayPoint.normalized * KyleBT.speed;
      _transform.LookAt(Vector3.left);
    }
    //}

    state = NodeState.RUNNING;
    return state;
  }
}
