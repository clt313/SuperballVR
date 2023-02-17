using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{

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
}
