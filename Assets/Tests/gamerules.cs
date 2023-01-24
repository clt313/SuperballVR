using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

enum TEST_COMMANDS
{
  SERVE_T1,
  SERVE_T2,
  HIT_T1,
  HIT_T2,
  MISS,
  OOB
}

public class gamerules
{

  public bool finishedTest = false;

  public string testSceneName = "Map1";
  public string courtOneRigidbodyName = "CourtOne";
  public string courtTwoRigidbodyName = "CourtTwo";
  public string netBodyName = "GridNet";
  public string ballName = "Ball";
  public Vector3 OOB_LOCATION;
  public Vector3 bounceHeight = new Vector3(0.0f, 6.0f, 0.0f);

  public GameManager gameManager;

  GameObject courtOne;
  GameObject courtTwo;
  GameObject net;
  GameObject teamOnePlayer;
  GameObject teamTwoPlayer;
  int currentCommand = 0;
  bool waitForMissToComplete = false;

  // T1 WIN 5-0
  List<TEST_COMMANDS> commands = new List<TEST_COMMANDS>{
    // Round 1
    TEST_COMMANDS.SERVE_T1,
    TEST_COMMANDS.OOB,

    // Round 2
    TEST_COMMANDS.SERVE_T2,
    TEST_COMMANDS.HIT_T1,
    TEST_COMMANDS.OOB,

    // Round 3
    TEST_COMMANDS.SERVE_T1,
    TEST_COMMANDS.OOB,

    // Round 4
    TEST_COMMANDS.SERVE_T2,
    TEST_COMMANDS.HIT_T1,
    TEST_COMMANDS.OOB,

    // Round 5
    TEST_COMMANDS.SERVE_T1,
    TEST_COMMANDS.OOB
  };



  [OneTimeSetUp]
  public void Setup()
  {
    LogAssert.ignoreFailingMessages = true; // Only fail tests on failed assertions here. Not other errors in game.
    SceneManager.LoadScene(testSceneName);
  }

  public void initTest()
  {
    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    OOB_LOCATION = GameObject.Find("OOB_TEST_MARKER").transform.position;
    courtOne = GameObject.Find(courtOneRigidbodyName);
    courtTwo = GameObject.Find(courtTwoRigidbodyName);
    net = GameObject.Find(netBodyName);

    teamOnePlayer = new GameObject();
    teamTwoPlayer = new GameObject();
    teamOnePlayer.transform.position = courtOne.transform.position;
    teamOnePlayer.transform.position += bounceHeight;
    teamTwoPlayer.transform.position = courtTwo.transform.position;
    teamTwoPlayer.transform.position += bounceHeight;

    BallEvents.ballBounceEvent.AddListener(processNextCommand);
    GameEvents.roundEndEvent.AddListener(serveBall);

    gameManager.startGame();
    Debug.Log("Finished Setup For Tests");
  }

  // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
  // `yield return null;` to skip a frame.
  [UnityTest]
  public IEnumerator gamerulesWithEnumeratorPasses()
  {
    yield return null;
    LogAssert.ignoreFailingMessages = true; // Only fail tests on failed assertions here. Not other errors in game.
    initTest();
    float startTime = Time.time;

    // Get game started by serving
    serveBall();

    while (!finishedTest)
    {

      // Advances a frame
      yield return null;
      finishedTest = currentCommand == commands.Count;
    }

    // Wait for game to finish
    while (gameManager.isGameRunning())
    {
      yield return null;
    }

    // Check the final score
    Assert.AreEqual(5, gameManager.getTeamOneScore());
    Assert.AreEqual(0, gameManager.getTeamTwoScore());

    Debug.Log("Finished running tests!");
  }

  void serveBall()
  {
    currentCommand += waitForMissToComplete == true ? 1 : 0;
    waitForMissToComplete = false;
    TEST_COMMANDS cmd = commands[currentCommand];
    Debug.Log(cmd.ToString());
    if (cmd == TEST_COMMANDS.SERVE_T1)
    {
      PlayerEvents.playerServeEvent.Invoke(teamTwoPlayer); // Players are switched because serve goes to other side 
    }
    else if (cmd == TEST_COMMANDS.SERVE_T2)
    {
      PlayerEvents.playerServeEvent.Invoke(teamOnePlayer); // Players are switched because serve goes to other side
    }
    gameManager.expectTestServe();
    currentCommand++;
  }

  // Teleport ball to wherever next command says
  void processNextCommand(GameObject listenedBall, GameObject collided)
  {

    if (!waitForMissToComplete)
    {

      TEST_COMMANDS cmd = commands[currentCommand];
      if (cmd == TEST_COMMANDS.SERVE_T1 || cmd == TEST_COMMANDS.SERVE_T2)
      {
        return;
      }

      Debug.Log(cmd.ToString());
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
