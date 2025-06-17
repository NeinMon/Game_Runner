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
    private Vector3 nextPowerUpSpawn;
    
    private int tileCounter = 0; // Counter to track tiles spawned
    public int powerUpFrequency = 6; // Increased from 3 to 6 - spawn every 6 tiles
    public float powerUpSpawnChance = 0.5f; // 50% chance to spawn even when counter matches

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

        // Instantiate tiles without destruction
        GameObject tile1 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        // Instantiate bricks without destruction
        GameObject brick = Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation).gameObject;

        // Instantiate new obstacle without destruction
        if (obstacleObj != null)
        {
            GameObject obstacle = Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation).gameObject;
        }

        // Calculate the end position of coin sequence
        float coinEndZ = nextCoinSpawn.z + (5 * 0.7f);
        
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

        // Setup power-up position AFTER the coin sequence
        nextPowerUpSpawn = nextCoinSpawn;
        nextPowerUpSpawn.z = coinEndZ + 0.3f; // Position after the last coin
        nextPowerUpSpawn.x = coinLane; // Use the same lane as coins

        // Check if it's time to spawn a power-up (every powerUpFrequency tiles)
        // AND apply an additional probability check
        if (tileCounter % powerUpFrequency == 0 && Random.value < powerUpSpawnChance)
        {
            // Random choice between speed-up (70%) and slow-down (30%)
            if (Random.value < 0.7f && speedUpObj != null)
            {
                GameObject speedUp = Instantiate(speedUpObj, nextPowerUpSpawn, speedUpObj.rotation).gameObject;
            }
            else if (slowDownObj != null)
            {
                GameObject slowDown = Instantiate(slowDownObj, nextPowerUpSpawn, slowDownObj.rotation).gameObject;
            }
        }

        nextTileSpawn.z += 3;

        // Instantiate second tile without destruction
        GameObject tile2 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        nextTileSpawn.z += 3;
        StartCoroutine(spawnTile());
    }
}