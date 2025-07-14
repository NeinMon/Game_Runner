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
    public Transform invisiblePowerObj; // Invisible power-up
    public Transform freezeCircleObj; // Freeze circle power-up
    private Vector3 nextPowerUpSpawn;
    
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

        // Setup obstacle position (different lane than brick)
        nextObstacleSpawn = nextTileSpawn;
        nextObstacleSpawn.x = brickLane;
        nextObstacleSpawn.y = 0.1f;

        // Setup brick position (different lane than obstacle)
        nextBrickSpawn = nextTileSpawn;
        nextBrickSpawn.x = obstacleLane;
        nextBrickSpawn.y = 0f;
        nextBrickSpawn.z += 1.5f; // Offset in Z direction

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

        // Instantiate tiles without destruction
        GameObject tile1 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        // Instantiate bricks without destruction
        GameObject brick = Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation).gameObject;

        // Instantiate new obstacle without destruction
        if (obstacleObj != null)
        {
            GameObject obstacle = Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation).gameObject;
        }

        // Coin xuất hiện: 70% thẳng hàng, 30% vòng cung (parabol, có object dưới coin giữa)
        if (coinObj != null)
        {
            float arcRate = 0.3f;
            if (Random.value < arcRate)
            {
                // Coin vòng cung (parabol)
                Vector3 coinPosition = nextCoinSpawn;
                float jumpForce = 2.1f;
                float gravity = -5f;
                float tMax = -2 * jumpForce / gravity;
                float dz = 0.7f;
                for (int i = 0; i < 5; i++)
                {
                    Vector3 spawnPos = coinPosition;
                    if (i >= 1 && i <= 3)
                    {
                        float t = (i) / 4.0f * tMax;
                        float y = jumpForce * t + 0.5f * gravity * t * t;
                        spawnPos.y = 0.2f + y;
                    }
                    GameObject coin = Instantiate(coinObj, spawnPos, coinObj.rotation).gameObject;
                    // Chỉ coin vòng cung mới có object phía dưới coin thứ 3
                    if (i == 2 && (bricksObj != null || obstacleObj != null))
                    {
                        Vector3 belowCoin = spawnPos;
                        if (Random.value < 0.5f && bricksObj != null)
                        {
                            belowCoin.y = 0f;
                            Instantiate(bricksObj, belowCoin, bricksObj.rotation);
                        }
                        else if (obstacleObj != null)
                        {
                            belowCoin.y = 0.1f;
                            Instantiate(obstacleObj, belowCoin, obstacleObj.rotation);
                        }
                    }
                    coinPosition.z += dz;
                }
            }
            else
            {
                // Coin thẳng hàng như bình thường, không có object phía dưới
                Vector3 coinPosition = nextCoinSpawn;
                float dz = 0.7f;
                for (int i = 0; i < 5; i++)
                {
                    Vector3 spawnPos = coinPosition;
                    GameObject coin = Instantiate(coinObj, spawnPos, coinObj.rotation).gameObject;
                    coinPosition.z += dz;
                }
            }
        }

        // Check if it's time to spawn a power-up (every powerUpFrequency tiles)
        if (tileCounter % powerUpFrequency == 0)
        {
            // Random chọn 1 trong 3 power-up: Thunder, Time, Invisible
            List<Transform> powerUps = new List<Transform>();
            if (speedUpObj != null) powerUps.Add(speedUpObj); // Thunder
            if (slowDownObj != null) powerUps.Add(slowDownObj); // Time
            if (invisiblePowerObj != null) powerUps.Add(invisiblePowerObj); // Invisible
            if (powerUps.Count > 0)
            {
                int idx = Random.Range(0, powerUps.Count);
                Instantiate(powerUps[idx], nextPowerUpSpawn, powerUps[idx].rotation);
            }
        }

        // Spawn vật cản đặc biệt: freeze circle (nếu có)
        if (freezeCircleObj != null)
        {
            // 20% tỉ lệ xuất hiện freeze circle ở lane ngẫu nhiên, không trùng với coinLane
            if (Random.value < 0.2f)
            {
                int[] lanesForFreeze = { -1, 0, 1 };
                List<int> possibleLanes = new List<int>(lanesForFreeze);
                possibleLanes.Remove(coinLane); // Không trùng lane với coin
                int freezeLane = possibleLanes[Random.Range(0, possibleLanes.Count)];
                Vector3 freezePos = nextTileSpawn;
                freezePos.x = freezeLane;
                freezePos.y = 0.1f;
                freezePos.z += 4.0f; // Đặt xa hơn coin/power/vật cản khác trên trục Z
                Instantiate(freezeCircleObj, freezePos, freezeCircleObj.rotation);
            }
        }

        nextTileSpawn.z += 3;

        // Instantiate second tile without destruction
        GameObject tile2 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        nextTileSpawn.z += 3;
        StartCoroutine(spawnTile());
    }
}