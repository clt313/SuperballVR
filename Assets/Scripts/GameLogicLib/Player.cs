using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    if (Input.GetButtonDown("XRI_Right_PrimaryButton") || Input.GetButtonDown("XRI_Left_PrimaryButton")
        || Input.GetButtonDown("XRI_Right_GripButton") || Input.GetButtonDown("XRI_Left_GripButton"))
    {
      Debug.Log("Detected serve button press!");
      PlayerEvents.playerServeEvent.Invoke(this);
    }
  }

  public virtual Vector3 getCameraPosition()
  {
    return this.transform.GetChild(0).GetChild(0).transform.position;
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

    Debug.Log("Hitting Wall!");

    // If dot product is positive that means we are moving into wall...stop that
    if (Vector3.Dot(vectorToCenter, currentVelocity) > 0.0f)
    {
      GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }
  }
}