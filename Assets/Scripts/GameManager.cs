using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

  public int maxBounces = 1;
  public int maxPasses = 1;
  public int maxGameTimeSeconds = 10 * 60;
  public int maxScore = 5;
  public GameObject teamOneCourt;
  public GameObject teamTwoCourt;
  public GameObject net;

  private int scoreTeamOne = 0;
  private int scoreTeamTwo = 0;

  enum TEAM
  {
    TEAM_ONE,
    TEAM_TWO
  }

  enum ROUND_END_REASON
  {
    NONE,
    NET,
    OUT_OF_BOUNDS,
    TIME,
    TOO_MANY_TOUCHES
  }

  private bool gameRunning = false;
  private bool isBallInPlay = false;
  private int currentBounces = 0;
  private int currentPasses = 0;
  private TEAM currentServer = TEAM.TEAM_ONE;
  private TEAM currentPossession = TEAM.TEAM_ONE;
  private ROUND_END_REASON roundEndReason = ROUND_END_REASON.NONE;


  // Start is called before the first frame update
  void Start()
  {
    resetGame();
    BallEvents.ballBounceEvent.AddListener(handleBallBounce);
  }

  // Handles performing actions as a result of collisions
  // Namely starting and stopping the game
  void Update()
  {
    if (gameRunning)
    {
      gameRunning = checkGameEnded();
      if (gameRunning)
      {
        if (roundEndReason != ROUND_END_REASON.NONE)
        {
          isBallInPlay = false;

          // TODO Switch the current player
        }
      }
      else
      {
        endGame();
      }
    }
  }

  void resetGame()
  {
    gameRunning = false;
    scoreTeamOne = 0;
    scoreTeamTwo = 0;
    isBallInPlay = false;
    currentBounces = 0;
    currentPasses = 0;
    currentServer = TEAM.TEAM_ONE;
    currentPossession = TEAM.TEAM_ONE;
    roundEndReason = ROUND_END_REASON.NONE;
  }

  bool checkGameEnded()
  {
    if (scoreTeamOne >= maxScore || scoreTeamTwo >= maxScore)
    {
      return true;
    }
    // TODO time check

    return false;
  }

  void endGame()
  {
    bool isDraw = scoreTeamOne == scoreTeamTwo;
    bool teamOneWon = scoreTeamOne > scoreTeamTwo;
    string winner = isDraw ? "DRAW" : teamOneWon ? "TEAM ONE WON" : "TEAM TWO WON";
    Debug.Log($"Game Ended! {winner}");
  }

  void handleServeButtonPressed(GameObject player)
  {

    // TODO Get player team

    if (isBallInPlay && gameRunning)
    {
      return;
    }

    // TODO Spawn ball above player if team matchwes
    else if (!isBallInPlay && gameRunning)
    {

    }
  }

  // Handles anything to do with collisions
  void handleBallBounce(GameObject listenedBall, GameObject listenedCollidedObject)
  {

    if (isBallInPlay && gameRunning)
    {

      int collidedID = listenedCollidedObject.GetInstanceID();
      bool isCourtCollision = collidedID == teamOneCourt.GetInstanceID() || collidedID == teamTwoCourt.GetInstanceID();

      // TODO Player Collision
      if (false)
      {

      }

      // Court Collision
      else if (isCourtCollision)
      {
        // Switch possession
        TEAM courtWhereBallLanded = collidedID == teamOneCourt.GetInstanceID() ? TEAM.TEAM_ONE : TEAM.TEAM_TWO; ;
        if (currentPossession == courtWhereBallLanded)
        {
          currentBounces += 1;
        }
        else
        {
          currentPossession = courtWhereBallLanded;
          currentBounces = 1;
        }

      }

      // Net Collision
      else if (collidedID == net.GetInstanceID())
      {
        roundEndReason = ROUND_END_REASON.NET;
      }

      // OOB
      else
      {
        roundEndReason = ROUND_END_REASON.OUT_OF_BOUNDS;
      }
    }


    Debug.Log("Game manager detected a ball bounce with: " + listenedCollidedObject.name);
  }

}
