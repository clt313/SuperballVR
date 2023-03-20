using UnityEngine;
using Utility;
using System.Collections.Generic;
public class AIPlayer : Player
{

  public static Dictionary<DIFFICULTY, float> gaussianScaleDifficulties = new Dictionary<DIFFICULTY, float>{
    {DIFFICULTY.EASY, 0.8f},
    {DIFFICULTY.MEDIUM, 0.66f},
    {DIFFICULTY.HARD, 0.33f}
  };

  public static Dictionary<DIFFICULTY, float> returnTimeDifficulties = new Dictionary<DIFFICULTY, float>{
    {DIFFICULTY.EASY, 2.0f},
    {DIFFICULTY.MEDIUM, 1.5f},
    {DIFFICULTY.HARD, 1.0f}
  };

  // Start is called before the first frame update
  public override void Start()
  {
    team = TEAM.TEAM_TWO;
    GameEvents.roundEndEvent.AddListener(handleRoundEnd);
  }

  // Update is called once per frame
  public override void Update()
  {
  }

  public void handleRoundEnd()
  {
    KyleBT.AIserving = true;
    Invoke(nameof(attemptToServe), 2.0f);
  }

  public void attemptToServe()
  {
    PlayerEvents.playerServeEvent.Invoke(this);
  }

  public override Vector3 getCameraPosition()
  {
    return this.transform.position;
  }

  public override void hitBall(GameObject listenedBall, Collider opposingCourt)
  {
    // Return Ball To Other Side Of Court

    // CONFIGURATION
    float returnTime = returnTimeDifficulties[GameManager.gameDifficulty];
    float gaussianScale = gaussianScaleDifficulties[GameManager.gameDifficulty]; // Scale width/length of court to one standard deviation. Smaller is tighter gaussian.

    Vector3 opposingCourtCenter = opposingCourt.bounds.center;
    float sigmaX = gaussianScale * opposingCourt.bounds.size.x / 2.0f;
    float sigmaZ = gaussianScale * opposingCourt.bounds.size.z / 2.0f;
    float xTarget = StatUtil.sampleGaussianDistribution(opposingCourtCenter.x, sigmaX);
    float zTarget = StatUtil.sampleGaussianDistribution(opposingCourtCenter.z, sigmaZ);
    Vector3 target = new Vector3(xTarget, opposingCourtCenter.y, zTarget);

    float gravity = Physics.gravity.y;
    Vector3 currentPosition = GetComponent<Rigidbody>().position;
    Vector3 returnVelocity = new Vector3(
      (target.x - currentPosition.x) / returnTime,
      ((target.y - currentPosition.y) / returnTime) - (gravity * returnTime) / 2.0f,
      (target.z - currentPosition.z) / returnTime
    );
    listenedBall.GetComponent<Rigidbody>().velocity = returnVelocity;
  }

}
