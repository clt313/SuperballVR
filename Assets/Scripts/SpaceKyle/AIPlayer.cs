using UnityEngine;
using Utility;
using System.Collections.Generic;
public class AIPlayer : Player
{

  public static Dictionary<StateController.AiDifficulty, float> gaussianScaleDifficulties = new Dictionary<StateController.AiDifficulty, float>{
    {StateController.AiDifficulty.Easy, 0.8f},
    {StateController.AiDifficulty.Normal, 0.66f},
    {StateController.AiDifficulty.Hard, 0.33f}
  };

  public static Dictionary<StateController.AiDifficulty, float> speedDifficulties = new Dictionary<StateController.AiDifficulty, float>{
    {StateController.AiDifficulty.Easy, 2.0f},
    {StateController.AiDifficulty.Normal, 2.3f},
    {StateController.AiDifficulty.Hard, 3.0f}
  };

  public static Dictionary<StateController.AiDifficulty, float> returnTimeDifficulties = new Dictionary<StateController.AiDifficulty, float>{
    {StateController.AiDifficulty.Easy, 2.0f},
    {StateController.AiDifficulty.Normal, 1.5f},
    {StateController.AiDifficulty.Hard, 1.0f}
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
    float returnTime = returnTimeDifficulties[StateController.aiDifficulty];
    float gaussianScale = gaussianScaleDifficulties[StateController.aiDifficulty]; // Scale width/length of court to one standard deviation. Smaller is tighter gaussian.

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
