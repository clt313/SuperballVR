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
  int currentFrame = 0;

  // T1 WIN
  List<TEST_COMMANDS> commands = new List<TEST_COMMANDS>{
    TEST_COMMANDS.SERVE_T1,
    TEST_COMMANDS.MISS,
    TEST_COMMANDS.MISS,
    TEST_COMMANDS.SERVE_T1,
    TEST_COMMANDS.MISS,
    TEST_COMMANDS.MISS,
    TEST_COMMANDS.SERVE_T1,
    TEST_COMMANDS.MISS
  };



  [OneTimeSetUp]
  public void Setup()
  {
    SceneManager.LoadScene(testSceneName);
    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    courtOne = GameObject.Find(courtOneRigidbodyName);
    courtTwo = GameObject.Find(courtTwoRigidbodyName);
    net = GameObject.Find(netBodyName);
    teamOnePlayer = new GameObject();
    teamTwoPlayer = new GameObject();
    OOB_LOCATION = GameObject.Find("OOB_TEST_MARKER").transform.position;
    gameManager.startGame();
  }

  // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
  // `yield return null;` to skip a frame.
  [UnityTest]
  public IEnumerator gamerulesWithEnumeratorPasses()
  {
    LogAssert.ignoreFailingMessages = true; // Only fail tests on failed assertions here. Not other errors in game.
    float startTime = Time.time;
    BallEvents.ballBounceEvent.AddListener(processNextCommand);

    while (!finishedTest)
    {

      // Advances a frame
      yield return null;
      finishedTest = currentFrame == commands.Count;
    }
    Debug.Log("Finished running test!");
  }

  // Teleport ball to wherever next command says
  void processNextCommand(GameObject listenedBall, GameObject listenedCollidedObject)
  {
    TEST_COMMANDS cmd = commands[currentFrame];
    if (cmd == TEST_COMMANDS.OOB)
    {
      GameObject ball = GameObject.Find(ballName);
      ball.transform.position = OOB_LOCATION;
    }
    else if (cmd == TEST_COMMANDS.MISS)
    {
      // Let it bounce
    }
    else if (cmd == TEST_COMMANDS.SERVE_T1)
    {
      PlayerEvents.playerServeEvent.Invoke(teamOnePlayer);
    }
    else if (cmd == TEST_COMMANDS.SERVE_T2)
    {
      PlayerEvents.playerServeEvent.Invoke(teamTwoPlayer);
    }
    else if (cmd == TEST_COMMANDS.HIT_T1)
    {
      GameObject ball = GameObject.Find(ballName);
      ball.transform.position = courtOne.GetComponent<Rigidbody>().centerOfMass;
      ball.transform.position += bounceHeight;
    }
    else if (cmd == TEST_COMMANDS.HIT_T2)
    {
      GameObject ball = GameObject.Find(ballName);
      ball.transform.position = courtTwo.GetComponent<Rigidbody>().centerOfMass;
      ball.transform.position += bounceHeight;
    }
    currentFrame++;
  }
}
