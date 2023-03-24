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
    Vector3 _target = target.position;
    _target.y = _transform.position.y;

    if(Vector3.Distance(_transform.position, _target) <= KyleBT.hitRange)
    {
        Debug.Log("In Range");
        _animator.SetBool("StrafeRight", false);
        _animator.SetBool("StrafeLeft", false);
        _animator.SetBool("HittingBall", false);
        _animator.SetBool("Walking", true);

        //_transform.position = Vector3.MoveTowards(_transform.position, _target, KyleBT.speed * 2 * Time.deltaTime);
        _transform.LookAt(Vector3.left);

        state = NodeState.SUCCESS;
        return state;
    }

    state = NodeState.FAILURE;
    return state;
  }
}
