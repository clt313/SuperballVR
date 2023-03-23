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
    _net = GameObject.Find("TennisNet").transform;
    _animator = transform.GetComponent<Animator>();
  }

  public override NodeState Evaluate()
  {
    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    Rigidbody rb = _transform.root.GetComponent<Rigidbody>();

    if (gameManager.getIsBallInPlay())
    {
      float AIPlayerHitHeight = rb.position.y;
      Ball ball = GameObject.Find("Ball").GetComponent<Ball>();
      Vector3 _target = ball.getExpectedCollisionPositionAtY(AIPlayerHitHeight);
      Vector3 lookAtVec = ball.transform.position;
      lookAtVec.y = rb.position.y;

      LOCATION ballBounceLocation = gameManager.getLocationClassification(_target);

      // Make sure ball will bounce in same side of court
      if (!isSameTeam(ballBounceLocation))
      {
        state = NodeState.FAILURE;
        return state;
      }

      if (Vector3.Distance(rb.position, _target) > 0.01f)
      {

        _animator.SetBool("StrafeLeft", false);
        _animator.SetBool("StrafeRight", false);
        _animator.SetBool("Walking", true);

        Vector3 toTarget = _target - rb.position;
        rb.velocity = toTarget.normalized * KyleBT.speed * 2;
        _transform.LookAt(lookAtVec);

        state = NodeState.SUCCESS;
        return state;
      }
      else
      {
        rb.velocity = Vector3.zero;
        _transform.root.position = _target;
      }
    }


    state = NodeState.FAILURE;
    return state;
  }

  bool isSameTeam(LOCATION loc)
  {
    TEAM thisTeam = _transform.root.GetComponent<AIPlayer>().team;

    if (thisTeam == TEAM.TEAM_ONE)
    {
      return loc == LOCATION.TEAM_ONE_COURT || loc == LOCATION.TEAM_ONE_OOB;
    }
    else if (thisTeam == TEAM.TEAM_TWO)
    {
      return loc == LOCATION.TEAM_TWO_COURT || loc == LOCATION.TEAM_TWO_OOB;
    }
    return false;
  }
}

