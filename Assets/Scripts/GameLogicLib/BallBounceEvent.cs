using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallEvents : MonoBehaviour
{

  // Passes the ball and bounced object data to listener
  public static BounceEvent ballBounceEvent = new BounceEvent();

}

public class BounceEvent : UnityEvent<GameObject, GameObject>
{

}
