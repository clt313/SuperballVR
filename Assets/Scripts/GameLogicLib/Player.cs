using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

  public TEAM team;

  // Start is called before the first frame update
  void Start()
  {
    team = TEAM.TEAM_ONE;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetButtonDown("XRI_Right_PrimaryButton"))
    {
      Debug.Log("Detected serve button press!");
      PlayerEvents.playerServeEvent.Invoke(this);
    }
  }
}
