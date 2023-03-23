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
    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    if (gameManager.getIsBallInPlay())
    {
      float AIPlayerHitHeight = _transform.position.y;
      Ball ball = GameObject.Find("Ball").GetComponent<Ball>();
      Vector3 _target = ball.getExpectedCollisionPositionAtY(AIPlayerHitHeight);
      Vector3 lookAtVec = ball.transform.position;
      lookAtVec.y = _transform.position.y;

      LOCATION ballBounceLocation = gameManager.getLocationClassification(_target);

      // Make sure ball will bounce in same side of court
      if (!isSameTeam(ballBounceLocation))
      {
        state = NodeState.FAILURE;
        return state;
      }

      if (Vector3.Distance(_transform.position, _target) > 0.01f)
      {
        // if (_transform.position.x > _net.position.x)
        // {
        //     _transform.position.x = 0.5f;
        // }
        // Debug.Log("Approaching");
        _transform.position = Vector3.MoveTowards(_transform.position, _target, KyleBT.speed * 2 * Time.deltaTime);
        _transform.LookAt(lookAtVec);

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

