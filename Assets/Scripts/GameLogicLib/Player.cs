using UnityEngine;
using Utility;
public class Player : MonoBehaviour
{

  public TEAM team;

  // Start is called before the first frame update
  public virtual void Start()
  {
    team = TEAM.TEAM_ONE;
  }

  // Update is called once per frame
  public virtual void Update()
  {
    if (StateController.inputEnabled
        && (Input.GetButtonDown("XRI_Right_PrimaryButton") || Input.GetButtonDown("XRI_Left_PrimaryButton")
        || Input.GetButtonDown("XRI_Right_GripButton") || Input.GetButtonDown("XRI_Left_GripButton")))
    {
      Debug.Log("Detected serve button press!");
      PlayerEvents.playerServeEvent.Invoke(this);
    }
  }

  public virtual Vector3 getCameraPosition()
  {
    return this.transform.GetChild(0).GetChild(0).transform.position;
  }

  public virtual void hitBall(GameObject listenedBall, Collider opposingCourt)
  {
    float returnTime = 2.0f;
    float gaussianScale = 0; // Scale width/length of court to one standard deviation. Smaller is tighter gaussian.

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

  // Prevent going through walls/boundaries
  void OnCollisionEnter(Collision collision)
  {
    preventMovementIntoWall(collision);
  }
  void OnCollisionStay(Collision collision)
  {
    preventMovementIntoWall(collision);
  }

  void preventMovementIntoWall(Collision collision)
  {
    GameObject other = collision.gameObject;
    Vector3 otherCenter = other.GetComponent<Collider>().bounds.center;
    Vector3 vectorToCenter = otherCenter - GetComponent<Rigidbody>().position;
    Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;

    // Debug.Log("Hitting Wall!");

    // If dot product is positive that means we are moving into wall...stop that
    if (Vector3.Dot(vectorToCenter, currentVelocity) > 0.0f)
    {
      GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }
  }

}