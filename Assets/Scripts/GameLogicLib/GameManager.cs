using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TEAM
{
  TEAM_ONE,
  TEAM_TWO
}

enum ROUND_END_REASON
{
  NONE,
  OUT_OF_BOUNDS,
  TOO_MANY_TOUCHES
}

public class GameManager : MonoBehaviour
{

  ////////////////////////
  // INPUT PARAMETERS
  ////////////////////////
  public int maxBounces = 1;
  public int maxPasses = 1;
  public int maxGameTimeSeconds = 10 * 60;
  public int maxScore = 5;
  public GameObject teamOneCourt;
  public GameObject teamTwoCourt;
  public GameObject net;
  public GameObject BallPrefab;


  ////////////////////////
  // GAME STATE
  ////////////////////////
  private int scoreTeamOne = 0;
  private int scoreTeamTwo = 0;

  private float startTime = -1.0f;
  private bool gameRunning = true;
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
    PlayerEvents.playerServeEvent.AddListener(handleServeButtonPressed);
  }

  // Handles performing actions as a result of collisions
  // Namely starting and stopping the game
  void Update()
  {
    if (gameRunning)
    {
      startTime = startTime == -1.0f ? Time.time : startTime;
      gameRunning = !checkGameEnded();
      if (gameRunning)
      {
        if (roundEndReason != ROUND_END_REASON.NONE)
        {
          // Round winner is whoever did not have possession of ball (last touch / court touch)
          TEAM roundWinnder = currentPossession == TEAM.TEAM_ONE ? TEAM.TEAM_TWO : TEAM.TEAM_ONE;
          addScore(roundWinnder);

          // Switch the current serving team
          currentServer = currentServer == TEAM.TEAM_ONE ? TEAM.TEAM_TWO : TEAM.TEAM_ONE;
          currentPossession = currentServer;
          currentBounces = 0;
          currentPasses = 0;

          Debug.Log($"Round Ended! Reason: {roundEndReason.ToString()}");
          Debug.Log($"Current Score: {scoreTeamOne} to {scoreTeamTwo}");
          roundEndReason = ROUND_END_REASON.NONE;

          // Dispatch event
          GameEvents.roundEndEvent.Invoke();
        }
      }
      else
      {
        endGame();
      }
    }
  }

  ////////////////////////
  // GAME LOOP FUNCTIONS
  ////////////////////////

  public void startGame()
  {
    resetGame();
    gameRunning = true;
  }

  public void resetGame()
  {
    maxScore = StateController.matchLength;
    startTime = -1.0f;
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
    else if (Time.time - startTime > maxGameTimeSeconds)
    {
      return true;
    }

    return false;
  }

  void endGame()
  {
    GameEvents.roundEndEvent.Invoke();

    bool isDraw = scoreTeamOne == scoreTeamTwo;
    bool teamOneWon = scoreTeamOne > scoreTeamTwo;
    string winner = isDraw ? "DRAW" : teamOneWon ? "TEAM ONE WON" : "TEAM TWO WON";
    Debug.Log($"Game Ended! {winner}");
    Debug.Log($"Final Score: {scoreTeamOne} to {scoreTeamTwo}");
  }

  void addScore(TEAM team)
  {
    if (team == TEAM.TEAM_ONE)
    {
      scoreTeamOne++;
    }
    else if (team == TEAM.TEAM_TWO)
    {
      scoreTeamTwo++;
    }
  }

  ////////////////////////
  // EVENT HANDLES
  ////////////////////////
  void handleServeButtonPressed(GameObject player)
  {

    // TODO Get player team and verify == currentServer

    if (isBallInPlay)
    {
      return;
    }

    // TODO Check if team matches
    else if (!isBallInPlay && gameRunning)
    {
      isBallInPlay = true;
      spawnBall(player);
    }
  }

  void spawnBall(GameObject player)
  {
    GameObject ball = (GameObject)Instantiate(BallPrefab, player.transform.position, player.transform.rotation);
    ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 3.0f, 0.0f);
    ball.name = "Ball";
    Debug.Log($"Spawning Ball At: {ball.transform.position.x}, {ball.transform.position.y}, {ball.transform.position.z}");
  }

  // Handles anything to do with collisions
  void handleBallBounce(GameObject listenedBall, GameObject listenedCollidedObject)
  {

    if (isBallInPlay && gameRunning)
    {

      int rootID = listenedCollidedObject.transform.root.GetInstanceID();
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
        TEAM courtWhereBallLanded = collidedID == teamOneCourt.GetInstanceID() ? TEAM.TEAM_ONE : TEAM.TEAM_TWO;
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

      // Net Collision, don't do anything
      else if (rootID == net.GetInstanceID())
      {

      }

      // OOB
      else
      {
        roundEndReason = ROUND_END_REASON.OUT_OF_BOUNDS;
        isBallInPlay = false;
      }

      // Check max bounces
      if (currentBounces > maxBounces)
      {
        roundEndReason = ROUND_END_REASON.TOO_MANY_TOUCHES;
        isBallInPlay = false;
      }
      Debug.Log("Game manager detected a ball bounce with: " + listenedCollidedObject.name);
    }

  }

  ////////////////////////////
  // TESTING HELPER FUNCTIONS
  ////////////////////////////
  public void expectTestServe()
  {
    currentPasses = 1; // Account for fact ball doesn't hit player hands
  }

  public int getTeamOneScore()
  {
    return scoreTeamOne;
  }

  public int getTeamTwoScore()
  {
    return scoreTeamTwo;
  }

  public bool isGameRunning()
  {
    return gameRunning;
  }
}


