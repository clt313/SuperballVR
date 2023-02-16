using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{

  // Start is called before the first frame update
  public override void Start()
  {
    team = TEAM.TEAM_TWO;
  }

  // Update is called once per frame
  public override void Update()
  {

  }

  public override Vector3 getCameraPosition()
  {
    return this.transform.position;
  }
}
