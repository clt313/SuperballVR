using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    GameEvents.roundEndEvent.AddListener(deleteSelf);
  }

  // Update is called once per frame
  void Update()
  {

  }

  // Dispatch event
  private void OnCollisionEnter(Collision collision)
  {
    BallEvents.ballBounceEvent.Invoke(this.gameObject, collision.gameObject);
  }

  void deleteSelf()
  {
    GameObject ballEmitter = GameObject.Find("Particle Effects/BallEmitter");
    GameObject particle = GameObject.Instantiate(ballEmitter, gameObject.transform.position, Quaternion.identity);
    particle.GetComponent<ParticleSystem>().Play();
    Object.Destroy(this.gameObject);
  }

}
