using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        BallEvents.ballBounceEvent.AddListener(handleBallBounce);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void handleBallBounce(GameObject listenedBall, GameObject listenedCollidedObject)
    {
        Debug.Log("Game manager detected a ball bounce with: " + listenedCollidedObject.name);
    }

}
