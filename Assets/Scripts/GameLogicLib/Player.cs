using UnityEngine;
using Utility;
public class Player : MonoBehaviour
{

  public TEAM team;
  private Vector3 prevRightHandPosition;
  private Vector3 prevLeftHandPosition;
  private Vector3 rightHandVelocity;
  private Vector3 leftHandVelocity;

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
    updateHandVelocities();
  }

  public virtual Vector3 getCameraPosition()
  {
    return this.transform.GetChild(0).GetChild(0).transform.position;
  }

  public virtual void hitBall(GameObject listenedBall, Collider opposingCourt)
  {
    const float toCenterCoeff = 0.7f;
    const float playerHitCoeff = 0.3f;
    float returnTime = 2.0f;

    Vector3 target = opposingCourt.bounds.center;
    Vector3 ballPosition = listenedBall.transform.position;

    // To Center
    float gravity = Physics.gravity.y;
    Vector3 currentPosition = GetComponent<Rigidbody>().position;
    Vector3 returnVelocityToCenter = new Vector3(
      (target.x - currentPosition.x) / returnTime,
      ((target.y - currentPosition.y) / returnTime) - (gravity * returnTime) / 2.0f,
      (target.z - currentPosition.z) / returnTime
    );
    returnVelocityToCenter.Normalize();

    // Where player was hitting (generally)
    Vector3 playerHitDirection = ballPosition - getClosestHandPosition(ballPosition);
    playerHitDirection.Normalize();
    // Debug.DrawLine(handCollider.transform.position, playerHitDirection * 10.0f, Color.white, 5.0f);

    Vector3 finalReturnForward = toCenterCoeff * returnVelocityToCenter + playerHitCoeff * playerHitDirection;
    finalReturnForward.Normalize();

    float returnSpeed = getHandVelocityClosestToPoint(ballPosition).magnitude * 1.5f;
    Vector3 returnVelocity = finalReturnForward * returnSpeed;
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

  // Needed since kinematic bodies don't update their velocity internally
  void updateHandVelocities()
  {
    Vector3 newRightHandPosition = Utility.FindUtil.RecursiveFindChild(transform, "HandColliderRight").GetComponent<Rigidbody>().position;
    Vector3 newLeftHandPosition = Utility.FindUtil.RecursiveFindChild(transform, "HandColliderLeft").GetComponent<Rigidbody>().position;
    rightHandVelocity = (newRightHandPosition - prevRightHandPosition) / Time.deltaTime;
    leftHandVelocity = (newLeftHandPosition - prevLeftHandPosition) / Time.deltaTime;
    prevRightHandPosition = newRightHandPosition;
    prevLeftHandPosition = newLeftHandPosition;
  }

  Vector3 getClosestHandPosition(Vector3 position)
  {
    if (Vector3.Distance(prevRightHandPosition, position) < Vector3.Distance(prevLeftHandPosition, position))
    {
      return prevRightHandPosition;
    }
    else
    {
      return prevLeftHandPosition;
    }
  }

  Vector3 getHandVelocityClosestToPoint(Vector3 position)
  {
    if (Vector3.Distance(prevRightHandPosition, position) < Vector3.Distance(prevLeftHandPosition, position))
    {
      return rightHandVelocity;
    }
    else
    {
      return leftHandVelocity;
    }
  }

}
