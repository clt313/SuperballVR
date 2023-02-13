using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{

  // Event for each round end
  public static RoundEndEvent roundEndEvent = new RoundEndEvent();

  // Event for game end
  public static GameEndEvent gameEndEvent = new GameEndEvent();
}

public class RoundEndEvent : UnityEvent
{

}

public class GameEndEvent : UnityEvent
{

}
