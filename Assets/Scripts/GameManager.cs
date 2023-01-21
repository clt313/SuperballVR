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

  private bool isBallInPlay = false;
  private int currentBounces = 0;
  private int currentPasses = 0;

  // Start is called before the first frame update
  void Start()
  {
    BallEvents.ballBounceEvent.AddListener(handleBallBounce);
  }

  // Handles performing actions as a result of collisions
  // Namely starting and stopping the game
  void Update()
  {

  }

  // Handles anything to do with collisions
  void handleBallBounce(GameObject listenedBall, GameObject listenedCollidedObject)
  {
    Debug.Log("Game manager detected a ball bounce with: " + listenedCollidedObject.name);
  }

}
