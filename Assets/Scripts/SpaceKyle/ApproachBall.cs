using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class ApproachBall : Node
{
  private Transform _transform;
  private Transform _net;
  private Animator _animator;

  public ApproachBall(Transform transform)
  {
    _transform = transform;
    _animator = transform.GetComponent<Animator>();
  }

  public override NodeState Evaluate()
  {
    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    Rigidbody rb = _transform.root.GetComponent<Rigidbody>();

    float AIPlayerHitHeight = rb.position.y;
    Ball ball = GameObject.Find("Ball").GetComponent<Ball>();
    Vector3 _target = ball.getExpectedCollisionPositionAtY(AIPlayerHitHeight);
    Vector3 lookAtVec = ball.transform.position;
    lookAtVec.y = rb.position.y;

    LOCATION ballBounceLocation = gameManager.getLocationClassification(_target);

    if (Vector3.Distance(rb.position, _target) > 0.01f)
    {
      Vector3 toTarget = _target - rb.position;
      rb.velocity = toTarget.normalized * KyleBT.speed;
      _transform.LookAt(lookAtVec);

      state = NodeState.SUCCESS;
      return state;
    }
    // else
    // {
    //   rb.velocity = Vector3.zero;
    //   rb.position = _target;
    //   _transform.LookAt(lookAtVec);
    // }

    state = NodeState.FAILURE;
    return state;
  }

}

