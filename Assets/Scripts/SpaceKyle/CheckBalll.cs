using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckBall : Node
{
  private Transform _transform;
  private Animator _animator;

  public CheckBall(Transform transform)
  {
    _transform = transform;
    _animator = transform.GetComponent<Animator>();
  }

  public override NodeState Evaluate()
  {
    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    if (gameManager.getIsBallInPlay())
    {
      Ball ball = GameObject.Find("Ball").GetComponent<Ball>();
      Vector3 _target = ball.getExpectedFloorCollision();
      LOCATION ballBounceLocation = gameManager.getLocationClassification(_target);

      if (isSameTeam(ballBounceLocation))
      {
        _animator.SetBool("StrafeLeft", false);
        _animator.SetBool("StrafeRight", false);
        _animator.SetBool("HittingBall", false);
        _animator.SetBool("Walking", true);

        state = NodeState.SUCCESS;
        return state;
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
