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
    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Map2")
    {
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

  Vector3 getExpectedCollisionPosition(float yTarget)
  {

    Vector3 ballPosition = GetComponent<Rigidbody>().position;
    Vector3 ballVelocity = GetComponent<Rigidbody>().velocity;

    // Use d = vit + 0.5at^2 to get time in y direction till target
    // Using quadratic equation this gives t = (- vi +/- sqrt(vi**2 + 2*a*deltaD)) / a
    // Using that we just need to multiply t on vx and vz for final position on court
    float vi = ballVelocity.y;
    float deltaD = yTarget - ballPosition.y;
    float a = Physics.gravity.y;

    float t1 = (-vi + Mathf.Sqrt(vi * vi + 2 * a * deltaD)) / a;
    float t2 = (-vi - Mathf.Sqrt(vi * vi + 2 * a * deltaD)) / a;
    float t = Mathf.Max(t1, t2); // We only care about the positive t one

    float collisionX = ballPosition.x + ballVelocity.x * t;
    float collisionZ = ballPosition.z + ballVelocity.z * t;
    return new Vector3(collisionX, yTarget, collisionZ);
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
