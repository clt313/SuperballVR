using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{

  // Passes the ball and bounced object data to listener
  public static RoundEndEvent roundEndEvent = new RoundEndEvent();

}

public class RoundEndEvent : UnityEvent
{

}
