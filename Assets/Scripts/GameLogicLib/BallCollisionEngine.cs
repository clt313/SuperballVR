using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionEngine : MonoBehaviour
{


  // Start is called before the first frame update
  void Start()
  {
    float refreshRate = 1.0f / 90.0f;
    // InvokeRepeating("customPhysicsEngine", 1.0f, refreshRate);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter(Collider hit)
  {
    if (hit.transform.root.GetComponent<Ball>() != null)
    {
      Debug.Log("BALL IN PHYSICS ENGINE!");
      var ballRB = hit.transform.root.GetComponent<Rigidbody>();
      // ballRB.velocity *= -1;
      Vector3 contact = GetComponent<Collider>().ClosestPointOnBounds(hit.transform.position);
      Vector3 normal = (contact - hit.transform.position).normalized;
      var handSpeed = GetComponent<Rigidbody>().velocity.magnitude;
      hit.gameObject.GetComponent<Rigidbody>().velocity = normal * 10;
    }
  }

  void customPhysicsEngine()
  {

    // Create a box collider of the given size and shape
    Vector3 center = GetComponent<BoxCollider>().center;
    Vector3 size = GetComponent<BoxCollider>().size;
    Collider[] hits = Physics.OverlapBox(center, size);

    foreach (Collider hit in hits)
    {
      if (hit.transform.root.GetComponent<Ball>() != null)
      {
        Debug.Log("BALL IN PHYSICS ENGINE!");
        var ballRB = hit.transform.root.GetComponent<Rigidbody>();
        ballRB.velocity *= -1;
      }
    }


    // Ball ball = GameObject.Find("Ball");

    //     // Create a ray from the VR hand in the direction of the ball
    // Vector3 origin = transform.position;
    // Vector3 direction = (Ball.position - VRHand.position).normalized;
    // Ray ray = new Ray(origin, direction);

    // // Cast the ray and detect any collisions
    // RaycastHit hit;
    // if (Physics.Raycast(ray, out hit))
    // {
    //     if (hit.collider.tag == "Ball")
    //     {
    //         Debug.Log("Collision detected!");
    //     }
    // }

  }
}
