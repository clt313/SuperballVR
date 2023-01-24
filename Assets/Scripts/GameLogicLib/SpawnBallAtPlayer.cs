using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallAtPlayer : MonoBehaviour
{

    public GameObject Player;
    public float spawnDistance;
    public GameObject BallPrefab;
    long prevSpawnTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        long timeAsLong = (long)(Time.time);
        if (timeAsLong % 10 == 0 && timeAsLong != prevSpawnTime)
        {
            Vector3 forward = Player.transform.forward;
            forward.y = 0;
            Instantiate(BallPrefab, Player.transform.position + forward, Player.transform.rotation);
            prevSpawnTime = timeAsLong;
        }
    }
}
