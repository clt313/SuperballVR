using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
[JsonConverter(typeof(StringEnumConverter))]
public enum TEST_COMMANDS
{
  SERVE_T1,
  SERVE_T2,
  HIT_T1,
  HIT_T2,
  MISS,
  OOB,
  NONE
}
[System.Serializable]
public class RoundData
{
  public List<TEST_COMMANDS> roundData;
}

[System.Serializable]
public class TEST_CASE
{
  public List<RoundData> rounds;
  public int expectedScoreTeamOne;
  public int expectedScoreTeamTwo;

}

[System.Serializable]
public class TESTS
{
  public List<TEST_CASE> tests;
  public TEST_COMMANDS getCommand(int testCaseIndex, int roundIndex, int commandIndex)
  {
    if (roundIndex >= tests[testCaseIndex].rounds.Count)
    {
      return TEST_COMMANDS.NONE;
    }
    else if (commandIndex >= tests[testCaseIndex].rounds[roundIndex].roundData.Count)
    {
      return TEST_COMMANDS.NONE;
    }
    return tests[testCaseIndex].rounds[roundIndex].roundData[commandIndex];
  }
  public int size()
  {
    return tests.Count;
  }
}

public class gamerules
{

  ////////////////////////////
  // Configuration Parameters
  ////////////////////////////
  public string testCasesPath = "./Assets/Tests/tests.json";
  public string mainMenuSceneName = "MainMenu";
  public string testSceneName = "Map1";
  public string courtOneRigidbodyName = "CourtOne";
  public string courtTwoRigidbodyName = "CourtTwo";
  public string netBodyName = "GridNet";
  public string ballName = "Ball";
  public Vector3 OOB_LOCATION;
  public Vector3 bounceHeight = new Vector3(0.0f, 6.0f, 0.0f);
  public float timeFactor = 5.0f;



  ////////////////////////////
  // STATE VARIABLES
  ////////////////////////////
  public GameManager gameManager;

  GameObject courtOne;
  GameObject courtTwo;
  GameObject net;
  GameObject teamOnePlayer;
  GameObject teamTwoPlayer;
  int currentTestCase = 0;
  int currentRound = -1;
  int currentCommand = 0;
  bool waitForMissToComplete = false;

  // See "./tests.json" for example format
  TESTS testGames;

  public void getTestCases()
  {
    // Deserialize the JSON data
    JsonSerializer serializer = new JsonSerializer();
    StreamReader file = File.OpenText(testCasesPath);
    testGames = (TESTS)serializer.Deserialize(file, typeof(TESTS));
  }

  public void loadScene()
  {
    // Load the scene
    LogAssert.ignoreFailingMessages = true; // Only fail tests on failed assertions here. Not other errors in game.
    SceneManager.LoadScene(mainMenuSceneName);
    SceneManager.LoadScene(testSceneName);
  }

  public GameObject getTestPlayer(TEAM team)
  {
    var player = new GameObject();
    var cameraOffset = new GameObject();
    var mainCamera = new GameObject();
    mainCamera.transform.parent = cameraOffset.transform;
    cameraOffset.transform.parent = player.transform;
    player.AddComponent<Player>();
    if (team == TEAM.TEAM_ONE)
    {
      player.transform.position = courtOne.transform.position;
      player.transform.position += bounceHeight;
    }
    else if (team == TEAM.TEAM_TWO)
    {
      player.transform.position = courtTwo.transform.position;
      player.transform.position += bounceHeight;
    }
    return player;
  }

  public void initTests()
  {

    // Init state variables for the tests
    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    StateController.matchLength = 5; // TODO: Clean this up and have configuration as part of test case
    gameManager.resetGame();

    currentRound = -1;
    currentCommand = 0;
    waitForMissToComplete = false;

    OOB_LOCATION = GameObject.Find("OOB_TEST_MARKER").transform.position;
    courtOne = GameObject.Find(courtOneRigidbodyName);
    courtTwo = GameObject.Find(courtTwoRigidbodyName);
    net = GameObject.Find(netBodyName);

    // Create the test players
    teamOnePlayer = getTestPlayer(TEAM.TEAM_ONE);
    teamTwoPlayer = getTestPlayer(TEAM.TEAM_TWO);

    // Register listeners
    BallEvents.ballBounceEvent.AddListener(processNextCommand);
    GameEvents.roundEndEvent.AddListener(serveBall);

    // Remove players
    foreach (Player player in Object.FindObjectsOfType<Player>())
    {

      // Move all default player assets out. Only use tests ones.
      var playerID = player.gameObject.GetInstanceID();
      if (playerID != teamOnePlayer.GetInstanceID() && playerID != teamTwoPlayer.GetInstanceID())
      {
        Vector3 farFarAway = new Vector3(int.MaxValue / 2, int.MaxValue / 2, int.MaxValue / 2);
        player.transform.root.position = farFarAway;
      }
    }
    Time.timeScale = timeFactor;

    Debug.Log("Finished Setup For Tests");
  }


  [UnityTest]
  public IEnumerator gamerulesWithEnumeratorPasses()
  {
    getTestCases();
    loadScene();
    yield return null; // Load Scene needs one frame to finish loading
    initTests();

    Debug.Log($"Detected {testGames.size()} test cases.");

    for (int i = 0; i < testGames.size(); i++)
    {
      yield return null;
      LogAssert.ignoreFailingMessages = true; // Only fail tests on failed assertions here. Not other errors in game.

      // SHOULD BE WRAPPED IN simulateMatch(). But the yield return null is acting weird.
      /////////////////////////////////////////////////////////////

      gameManager.startGame();

      // Start match by manually serving
      serveBall();

      while (gameManager.isGameRunning()) // TODO: Add timeout
      {
        // Advances a frame
        yield return null;
      }

      // Check the final score
      int expectedTeamOneScore = testGames.tests[currentTestCase].expectedScoreTeamOne;
      int expectedTeamTwoScore = testGames.tests[currentTestCase].expectedScoreTeamTwo;
      Assert.AreEqual(expectedTeamOneScore, gameManager.getTeamOneScore());
      Assert.AreEqual(expectedTeamTwoScore, gameManager.getTeamTwoScore());
      currentTestCase++;
      currentRound = -1;
      currentCommand = 0;
      waitForMissToComplete = false;
      Time.timeScale = timeFactor;
      /////////////////////////////////////////////////////////////



      // simulateMatch();
    }

    Debug.Log("Finished running tests!");
  }

  void serveBall()
  {
    currentCommand = 0;
    currentCommand += waitForMissToComplete == true ? 1 : 0;
    waitForMissToComplete = false;
    TEST_COMMANDS cmd = testGames.getCommand(currentTestCase, ++currentRound, currentCommand++);
    Debug.Log($"Serving ball with command: {cmd.ToString()} at {currentTestCase} {currentRound - 1} {currentCommand}");
    if (cmd == TEST_COMMANDS.SERVE_T1)
    {
      PlayerEvents.playerServeEvent.Invoke(teamTwoPlayer.GetComponent<Player>()); // Players are switched because serve goes to other side 
    }
    else if (cmd == TEST_COMMANDS.SERVE_T2)
    {
      PlayerEvents.playerServeEvent.Invoke(teamOnePlayer.GetComponent<Player>()); // Players are switched because serve goes to other side
    }
    gameManager.expectTestServe();
  }

  // Teleport ball to wherever next command says
  void processNextCommand(GameObject listenedBall, GameObject collided)
  {

    if (!waitForMissToComplete)
    {

      TEST_COMMANDS cmd = testGames.getCommand(currentTestCase, currentRound, currentCommand);
      if (cmd == TEST_COMMANDS.SERVE_T1 || cmd == TEST_COMMANDS.SERVE_T2 || cmd == TEST_COMMANDS.NONE)
      {
        return;
      }

      Debug.Log($"CMD: {cmd.ToString()} at {currentTestCase} {currentRound} {currentCommand}");
      if (cmd == TEST_COMMANDS.OOB)
      {
        GameObject ball = GameObject.Find(ballName);
        ball.transform.position = OOB_LOCATION;
      }
      else if (cmd == TEST_COMMANDS.MISS)
      {
        // Let it bounce
        waitForMissToComplete = true;
      }
      else if (cmd == TEST_COMMANDS.HIT_T1)
      {
        GameObject ball = GameObject.Find(ballName);
        ball.transform.position = courtTwo.transform.position;
        ball.transform.position += bounceHeight;
      }
      else if (cmd == TEST_COMMANDS.HIT_T2)
      {
        GameObject ball = GameObject.Find(ballName);
        ball.transform.position = courtOne.transform.position;
        ball.transform.position += bounceHeight;
      }
      currentCommand++;
    }

  }

}