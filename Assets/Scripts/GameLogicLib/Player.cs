using UnityEngine;
using System.Collections.Generic;
public class Player : MonoBehaviour
{

  public TEAM team;

  // Sum of 1/2^n from n=1 to infinity quickly converges to 1. So it can be used to estimate velocity.
  private int numberOfFramesToAverageOver = 5; // Converges to 0.96875 (close enough to 1.0f)
  private Vector3 prevRightHandPosition;
  private Vector3 prevLeftHandPosition;
  private LinkedList<Vector3> rightHandVelocity;
  private LinkedList<Vector3> leftHandVelocity;

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
    const float momentumDampeningCoeff = 0.8f;
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

    // Loosely based on conservation of momentum
    // m1v1 + m2v2 = m1v1' + m2v2'
    // Masses stay constant, and the velocity of human hand is player controlled
    // So unfortunately this by nature breaks conservation of momentum
    // So a small hack is to just min new velocity with current ball velocity * 0.5 (say dampening to stop non-stop increase)
    float currBallSpeed = listenedBall.GetComponent<Rigidbody>().velocity.magnitude;
    float returnSpeed = getHandVelocityClosestToPoint(ballPosition).magnitude * 1.5f;
    returnSpeed = Mathf.Max(currBallSpeed * momentumDampeningCoeff, returnSpeed);
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
    
    Vector3 thisFrameRightVelocity = (newRightHandPosition - prevRightHandPosition) / Time.deltaTime;
    Vector3 thisFrameLeftVelocity = (newLeftHandPosition - prevLeftHandPosition) / Time.deltaTime;
    prevRightHandPosition = newRightHandPosition;
    prevLeftHandPosition = newLeftHandPosition;

    // Add the new velocities to the linked list and remove the old one once you reach max number to average over
    rightHandVelocity.AddLast(thisFrameRightVelocity);
    leftHandVelocity.AddLast(thisFrameLeftVelocity);
    if(rightHandVelocity.Count == numberOfFramesToAverageOver) {
      rightHandVelocity.RemoveFirst();
      leftHandVelocity.RemoveFirst();
    }
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
    // Return weighted average according to series sum(1/2^n) 
    Vector3 resultingVelocity = Vector3.zero;
    int currNodeIndex = 1;
    if (Vector3.Distance(prevRightHandPosition, position) < Vector3.Distance(prevLeftHandPosition, position))
    {
      foreach(Vector3 velocity in rightHandVelocity) {
        resultingVelocity += velocity * (1/Mathf.Pow(2, currNodeIndex));
        currNodeIndex++;
      }
    }
    else
    {
      foreach(Vector3 velocity in leftHandVelocity) {
        resultingVelocity += velocity * (1/Mathf.Pow(2, currNodeIndex));
        currNodeIndex++;
      }
    }
    return resultingVelocity;
  }

}
