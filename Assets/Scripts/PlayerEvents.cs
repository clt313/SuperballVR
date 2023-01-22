using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents : MonoBehaviour
{

  // Passes the ball and bounced object data to listener
  public static PlayerServeEvent playerServeEvent = new PlayerServeEvent();

}

public class PlayerServeEvent : UnityEvent<GameObject>
{

}
