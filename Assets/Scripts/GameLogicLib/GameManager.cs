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
  public int maxPasses = 2;
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
          TEAM roundWinner = currentPossession == TEAM.TEAM_ONE ? TEAM.TEAM_TWO : TEAM.TEAM_ONE;
          addScore(roundWinner);

          // Switch the current serving team
          currentServer = currentServer == TEAM.TEAM_ONE ? TEAM.TEAM_TWO : TEAM.TEAM_ONE;
          currentPossession = currentServer;
          currentBounces = 0;
          currentPasses = 0;

          Debug.Log($"Round Ended! Reason: {roundEndReason.ToString()}, Winner: {roundWinner}");
          Debug.Log($"Current Score: {scoreTeamOne} to {scoreTeamTwo}");
          roundEndReason = ROUND_END_REASON.NONE;

          // Round end sound effect
          AudioManager.instance.Play((roundWinner == TEAM.TEAM_ONE) ? "RoundWin" : "RoundLose");

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
    // GameEvents.roundEndEvent.Invoke();
    GameEvents.gameEndEvent.Invoke();

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
    AudioManager.instance.Play(teamOneWon ? "GameWin" : "GameLose");
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

  // Based on the Box-Muller transform https://towardsdatascience.com/how-to-generate-random-variables-from-scratch-no-library-used-4b71eb3c8dc7
  float sampleGaussianDistribution(float mu, float sigma)
  {
    // create a new instance of the Random class
    System.Random random = new System.Random();

    // generate two random numbers that are uniformly distributed between 0 and 1
    float u1 = (float)random.NextDouble();
    float u2 = (float)random.NextDouble();

    // transform the uniformly distributed numbers to normally distributed numbers
    float z1 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
    float z2 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

    // use the first normally distributed number (z1) as the sample from the normal distribution
    float sample = mu + sigma * z1;

    return sample;
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

    else if (!isBallInPlay && gameRunning && playerTeam == currentPossession)
    {
      isBallInPlay = true;
      spawnBall(player);
    }
  }

  void spawnBall(Player player)
  {
    Vector3 playerforward = player.transform.forward;
    Vector3 ballSpawnLocation;

    playerforward.y = 0;
    playerforward = playerforward.normalized;

    ballSpawnLocation = player.getCameraPosition() + playerforward * 1.1f;
    ballSpawnLocation.y = 1.3f;

    GameObject ball = (GameObject)Instantiate(BallPrefab, ballSpawnLocation, player.gameObject.transform.rotation);
    ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 10.0f, 0.0f);
    ball.name = "Ball";
    Debug.Log($"Spawning Ball At: {ball.transform.position.x}, {ball.transform.position.y}, {ball.transform.position.z}");
    AudioManager.instance.Play("BallServe");
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
        //Debug.Log("Detected hit by " + player.name + "(" + playerTeam + ")");

        // Check if this was an AI.
        if (player.GetComponent<AIPlayer>() != null)
        {

          // Return Ball To Other Side Of Court

          // TODO: Move into function
          // CONFIGURATION
          float returnTime = 2.0f;
          float gaussianScale = 0.66f; // Scale width/length of court to one standard deviation. Smaller is tighter gaussian.

          Collider opposingCourt =
            player.team == TEAM.TEAM_ONE ?
              teamTwoCourt.GetComponent<Collider>() :
              teamOneCourt.GetComponent<Collider>();

          Vector3 opposingCourtCenter = opposingCourt.bounds.center;
          float sigmaX = gaussianScale * opposingCourt.bounds.size.x / 2.0f;
          float sigmaZ = gaussianScale * opposingCourt.bounds.size.z / 2.0f;
          float xTarget = sampleGaussianDistribution(opposingCourtCenter.x, sigmaX);
          float zTarget = sampleGaussianDistribution(opposingCourtCenter.z, sigmaZ);
          Vector3 target = new Vector3(xTarget, opposingCourtCenter.y, zTarget);

          float gravity = Physics.gravity.y;
          Vector3 currentPosition = player.GetComponent<Rigidbody>().position;
          Vector3 returnVelocity = new Vector3(
            (target.x - currentPosition.x) / returnTime,
            ((target.y - currentPosition.y) / returnTime) - (gravity * returnTime) / 2.0f,
            (target.z - currentPosition.z) / returnTime
          );
          listenedBall.GetComponent<Rigidbody>().velocity = returnVelocity;
        }

        // Switch possession or increment passes
        if (currentPossession != player.team)
        {
          currentBounces = 0;
          currentPasses = 0;
          currentPossession = player.team;
        }
        else
        {
          currentPasses++;
        }

        AudioManager.instance.Play("BallHit");
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
          currentPasses = 0;
        }
        AudioManager.instance.Play("BallBounce");
      }

      // Net Collision, don't do anything
      else if (isNetCollision)
      {
        AudioManager.instance.Play("BallNet");
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
      Debug.Log("Game manager detected a ball bounce with: " + listenedCollidedObject.name + ", Current possesor: " + currentPossession.ToString());
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
