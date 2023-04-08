using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum COURT_POSITION
{
  COURT_ONE,
  COURT_TWO,
  OOB
}

public class Ball : MonoBehaviour
{
  public Material glowMaterial;
  public float floorPosition;

  private GameObject landingIndicator;

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

    gameObject.GetComponent<TrailRenderer>().enabled = StateController.ballTrail;

    // Delete self on round end
    GameEvents.roundEndEvent.AddListener(deleteSelf);
    if (StateController.ballTrajectory) {
      createLandingIndicator();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (landingIndicator)
      updateLandingIndicator();
  }

  void createLandingIndicator()
  {
    landingIndicator = new GameObject("Landing Indicator");
    MeshRenderer meshRenderer = landingIndicator.AddComponent<MeshRenderer>();
    meshRenderer.material = Resources.Load<Material>("LandingIndicator");
    MeshFilter meshFilter = landingIndicator.AddComponent<MeshFilter>();

    Mesh mesh = new Mesh();
    mesh.vertices = new Vector3[] { new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(0.5f, 0, 0.5f), new Vector3(-0.5f, 0, 0.5f) };
    mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
    mesh.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
    mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
    meshFilter.mesh = mesh;

    // Rotate so it faces upward
    landingIndicator.transform.position = new Vector3(0, 2, 0);
    landingIndicator.transform.rotation = Quaternion.Euler(180, 0, 0);
    landingIndicator.transform.localScale = new Vector3(1.60f, 1.0f, 1.0f);
  }

  void updateLandingIndicator()
  {
    // Add a reticle at the landing position of the ball
    Vector3 floorCollisionPoint = getExpectedFloorCollision();
    landingIndicator.transform.position = floorCollisionPoint;
  }

  public Vector3 getExpectedFloorCollision()
  {
    return getExpectedCollisionPositionAtY(floorPosition);
  }

  public Vector3 getExpectedCollisionPositionAtY(float yTarget)
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

    Object.Destroy(landingIndicator);
    Object.Destroy(this.gameObject);
  }

}
