using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapgameflow : MonoBehaviour
{
    public Transform tileObj;
    private Vector3 nextTileSpawn;
    public Transform bricksObj;
    private Vector3 nextBrickSpawn;
    public Transform obstacleObj; // New obstacle object
    private Vector3 nextObstacleSpawn;
    public Transform coinObj; // New coin object
    private Vector3 nextCoinSpawn;
    public Transform speedUpObj; // Speed boost power-up
    public Transform slowDownObj; // Speed decrease power-up
    public Transform invisibleObj; // Invisible power-up

    private Vector3 nextPowerUpSpawn;
    private Vector3 nextInvisibleSpawn;

    private int tileCounter = 0; // Counter to track tiles spawned
    public int powerUpFrequency = 3; // Spawn power-up every X tiles

    void Start()
    {
        nextTileSpawn.z = 18;
        StartCoroutine(spawnTile());
    }

    void Update()
    {

    }

    IEnumerator spawnTile()
    {
        yield return new WaitForSeconds(1);

        // Increment tile counter
        tileCounter++;

        // Choose distinct lanes for each object type
        int[] lanes = { -1, 0, 1 };
        System.Random rnd = new System.Random();

        // Fisher-Yates shuffle to randomize lanes
        for (int i = lanes.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            int temp = lanes[i];
            lanes[i] = lanes[j];
            lanes[j] = temp;
        }

        // Assign shuffled lanes to objects
        int brickLane = lanes[0];
        int obstacleLane = lanes[1];
        int coinLane = lanes[2];

        // Setup brick position
        nextBrickSpawn = nextTileSpawn;
        nextBrickSpawn.x = brickLane;
        nextBrickSpawn.y = 0.1f;

        // Setup obstacle position (different lane than brick)
        nextObstacleSpawn = nextTileSpawn;
        nextObstacleSpawn.x = obstacleLane;
        nextObstacleSpawn.y = 0f;
        nextObstacleSpawn.z += 1.5f; // Offset in Z direction

        // Setup coin position (different lane than both brick and obstacle)
        nextCoinSpawn = nextTileSpawn;
        nextCoinSpawn.x = coinLane;
        nextCoinSpawn.y = 0.2f; // Height for coins
        nextCoinSpawn.z += 1.0f; // Start position for coins

        // Setup power-up position - randomly choose a lane that doesn't have coins
        nextPowerUpSpawn = nextTileSpawn;
        nextPowerUpSpawn.x = Random.value < 0.5f ? brickLane : obstacleLane; // Choose between brick lane and obstacle lane
        nextPowerUpSpawn.y = 0.2f;
        nextPowerUpSpawn.z += 2.5f; // Position after obstacles to avoid direct overlap

        // Setup invisible power-up position - random lane khác với coin
        nextInvisibleSpawn = nextTileSpawn;
        int[] powerUpLanes = { brickLane, obstacleLane };
        nextInvisibleSpawn.x = powerUpLanes[Random.Range(0, powerUpLanes.Length)];
        nextInvisibleSpawn.y = 0.2f;
        nextInvisibleSpawn.z += 2.5f;

        // Instantiate tiles without destruction
        GameObject tile1 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        // Instantiate new obstacle without destruction
        if (obstacleObj != null)
        {
            GameObject obstacle = Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation).gameObject;
        }

        // Instantiate 5 coins in succession
        if (coinObj != null)
        {
            Vector3 coinPosition = nextCoinSpawn;
            for (int i = 0; i < 5; i++)
            {
                GameObject coin = Instantiate(coinObj, coinPosition, coinObj.rotation).gameObject;
                coinPosition.z += 0.7f; // Space between consecutive coins
            }
        }

        // Check if it's time to spawn a power-up (every powerUpFrequency tiles)
        if (tileCounter % powerUpFrequency == 0)
        {
            float rand = Random.value;
            if (rand < 0.5f && speedUpObj != null)
            {
                GameObject speedUp = Instantiate(speedUpObj, nextPowerUpSpawn, speedUpObj.rotation).gameObject;
            }
            else if (rand < 0.8f && slowDownObj != null)
            {
                GameObject slowDown = Instantiate(slowDownObj, nextPowerUpSpawn, slowDownObj.rotation).gameObject;
            }
            else if (invisibleObj != null)
            {
                GameObject invisible = Instantiate(invisibleObj, nextInvisibleSpawn, invisibleObj.rotation).gameObject;
            }
        }

        nextTileSpawn.z += 3;

        // Instantiate second tile without destruction
        GameObject tile2 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        nextTileSpawn.z += 3;
        StartCoroutine(spawnTile());
    }
}