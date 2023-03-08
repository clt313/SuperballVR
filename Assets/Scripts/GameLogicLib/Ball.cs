using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
  public Material glowMaterial;

  // Start is called before the first frame update
  void Start()
  {
    // Decide which material to use based on map
    // Use glow material on Map2, otherwise default
    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Map2") {
      gameObject.GetComponent<MeshRenderer>().material = glowMaterial;
      gameObject.GetComponent<Light>().enabled = true;
    }

    // Delete self on round end
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
    // Create particle effect where ball ended at
    GameObject ballEmitter = GameObject.Find("Particle Effects/BallEmitter");
    GameObject particle = GameObject.Instantiate(ballEmitter, gameObject.transform.position, Quaternion.identity);
    particle.GetComponent<ParticleSystem>().Play();
    
    Object.Destroy(this.gameObject);
  }

}
