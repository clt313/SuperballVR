using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public enum TEAM
{
  TEAM_ONE,
  TEAM_TWO
}

enum ROUND_END_REASON
{
  NONE,
  OUT_OF_BOUNDS,
  TOO_MANY_BOUNCES,
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
  public TMP_Text[] teamOneScoreTexts;
  public TMP_Text[] teamTwoScoreTexts;
  public TMP_Text[] maxScoreTexts;
  public GameObject endgameUI;
  public GameObject player;
  public GameObject leftController;
  public GameObject rightController;
  public TMP_Text endgameText;
  public AudioSource bounceAudio;


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
  private Player previousPossessor = null;


  // Start is called before the first frame update
  void Start()
  {
    startGame();
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
    updateScoreboard();
    foreach (TMP_Text maxScoreText in maxScoreTexts)
      maxScoreText.SetText(maxScore.ToString());
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

    endgameText.SetText((isDraw ? "It's a draw!" : teamOneWon ? "You won!" : "You lost!") +
        "\nFinal Score: " + scoreTeamOne + " - " + scoreTeamTwo);

    // Yes this is copy-pasted from PauseMenu, but the assembly definition won't just let me use it :)
    // Enable interaction rays
    leftController.GetComponent<XRInteractorLineVisual>().enabled = true;
    rightController.GetComponent<XRInteractorLineVisual>().enabled = true;

    // Summon pause menu in front of player camera
    endgameUI.SetActive(true);
    Vector3 pos = player.transform.position;
    pos.x += player.transform.forward.x * 2;
    pos.z += player.transform.forward.z * 2;
    endgameUI.transform.position = pos;
    Vector3 angle = new Vector3(player.transform.forward.x, 0, player.transform.forward.z);
    endgameUI.transform.forward = angle;
    Time.timeScale = 0f;

    // Win/Lose Sound Effects
    FindObjectOfType<AudioManager>().Play(teamOneWon ? "GameWin" : "GameLose");
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
    updateScoreboard();
  }

  void updateScoreboard()
  {
    foreach (TMP_Text teamOneScoreText in teamOneScoreTexts)
      teamOneScoreText.SetText(scoreTeamOne.ToString());
    foreach (TMP_Text teamTwoScoreText in teamTwoScoreTexts)
      teamTwoScoreText.SetText(scoreTeamTwo.ToString());
  }

  ////////////////////////
  // EVENT HANDLES
  ////////////////////////
  void handleServeButtonPressed(Player player)
  {

    TEAM playerTeam = player.team;

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

  void spawnBall(Player player)
  {
    GameObject ball = (GameObject)Instantiate(BallPrefab, player.getGameraPosition(), player.gameObject.transform.rotation);
    ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 10.0f, 0.0f);
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
      bool isNetCollision = collidedID == net.GetInstanceID();
      Player player = listenedCollidedObject.transform.root.GetComponentInChildren<Player>();

      if (player)
      {
        TEAM playerTeam = player.team;
        bool isHandCollision = listenedCollidedObject.name == "HandColliderLeft" || listenedCollidedObject.name == "HandColliderRight";
        // GameObject playerLeftHand = player.transform.root.Find("HandColliderLeft").gameObject;
        // GameObject playerRightHand = player.transform.root.Find("HandColliderRight").gameObject;
        // Check for hands
        // if (listenedCollidedObject.GetInstanceID() == playerLeftHand.GetInstanceID() || listenedCollidedObject.GetInstanceID() == playerRightHand.GetInstanceID())
        if (isHandCollision)
        {
          if (previousPossessor && player.GetInstanceID() == previousPossessor.GetInstanceID())
          {
            currentPasses = maxPasses; // Indicate too many touches
          }
          else if (currentPossession != player.team)
          {
            currentBounces = 0;
            currentPasses = 0;
            currentPossession = player.team;
          }
          else
          {
            currentPasses++;
          }
          previousPossessor = player;
        }
        else
        {
          Debug.Log("Ignoring XRRig collision!");
        }

        // Don't do anything if XRRig
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
      else if (isNetCollision)
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
        roundEndReason = ROUND_END_REASON.TOO_MANY_BOUNCES;
        isBallInPlay = false;
      }
      // Check max passes
      if (currentPasses > maxPasses)
      {
        roundEndReason = ROUND_END_REASON.TOO_MANY_TOUCHES;
        isBallInPlay = false;
      }
      Debug.Log("Game manager detected a ball bounce with: " + listenedCollidedObject.name);
      FindObjectOfType<AudioManager>().Play("BallBounce");
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


