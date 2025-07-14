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
    
    // Difficulty progression variables
    [Header("Difficulty Settings")]
    public float spawnRateIncreaseInterval = 30f; // Increase difficulty every 30 seconds
    public float minSpawnDelay = 0.3f; // Minimum delay between tile spawns
    public float spawnDelayDecrease = 0.05f; // How much to decrease spawn delay
    
    [Header("Obstacle Density")]
    public float baseObstacleChance = 0.8f; // Base chance to spawn obstacles
    public float maxObstacleChance = 0.95f; // Maximum obstacle spawn chance
    public float obstacleChanceIncrease = 0.02f; // How much to increase per difficulty level
    
    [Header("Power-up Adjustments")]
    public float basePowerUpChance = 0.6f; // Base chance for power-ups to spawn
    public float minPowerUpChance = 0.3f; // Minimum power-up spawn chance
    public float powerUpChanceDecrease = 0.03f; // Decrease power-up chance as game gets harder
    
    [Header("Freeze Circle Progression")]
    public float baseFreezeChance = 0.2f; // Base freeze circle chance
    public float maxFreezeChance = 0.4f; // Maximum freeze circle chance
    public float freezeChanceIncrease = 0.01f; // Increase freeze circle chance
    
    [Header("Coin Arc Progression")]
    public float baseArcRate = 0.3f; // Base chance for arc coins
    public float maxArcRate = 0.6f; // Maximum arc rate
    public float arcRateIncrease = 0.02f; // Increase arc rate with difficulty
    
    // Runtime variables
    private float currentSpawnDelay = 1f;
    private float currentObstacleChance;
    private float currentPowerUpChance;
    private float currentFreezeChance;
    private float currentArcRate;
    private int difficultyLevel = 0;

    void Start()
    {
        nextTileSpawn.z = 18;
        
        // Initialize difficulty values
        currentSpawnDelay = 1f;
        currentObstacleChance = baseObstacleChance;
        currentPowerUpChance = basePowerUpChance;
        currentFreezeChance = baseFreezeChance;
        currentArcRate = baseArcRate;
        
        // Log game start with initial difficulty settings
        Debug.Log("=== GAME STARTED ===");
        Debug.Log($"Initial Difficulty Level: {difficultyLevel}");
        Debug.Log($"Spawn Delay: {currentSpawnDelay:F2}s | Obstacle Chance: {currentObstacleChance:F2} | Power-up Chance: {currentPowerUpChance:F2} | Freeze Chance: {currentFreezeChance:F2} | Arc Rate: {currentArcRate:F2}");
        Debug.Log($"Difficulty will increase every {spawnRateIncreaseInterval} seconds");
        
        StartCoroutine(spawnTile());
        StartCoroutine(IncreaseDifficulty());
    }

    void Update()
    {

    }

    IEnumerator spawnTile()
    {
        yield return new WaitForSeconds(currentSpawnDelay);
        
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

        // Instantiate bricks based on difficulty
        if (Random.value < currentObstacleChance)
        {
            GameObject brick = Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation).gameObject;
            if (difficultyLevel >= 5) // Only log after level 5 to avoid spam
            {
                Debug.Log($"🧱 Brick spawned at lane {obstacleLane} (Chance: {(currentObstacleChance * 100):F0}%)");
            }
        }

        // Instantiate new obstacle based on difficulty
        if (obstacleObj != null && Random.value < currentObstacleChance)
        {
            GameObject obstacle = Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation).gameObject;
            if (difficultyLevel >= 5) // Only log after level 5 to avoid spam
            {
                Debug.Log($"🚧 Obstacle spawned at lane {brickLane} (Chance: {(currentObstacleChance * 100):F0}%)");
            }
        }

        // Coin xuất hiện với tỉ lệ vòng cung tăng theo độ khó
        if (coinObj != null)
        {
            if (Random.value < currentArcRate)
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

        // Check if it's time to spawn a power-up (every powerUpFrequency tiles) and use difficulty-based chance
        if (tileCounter % powerUpFrequency == 0 && Random.value < currentPowerUpChance)
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

        // Spawn vật cản đặc biệt: freeze circle với difficulty-based chance
        if (freezeCircleObj != null)
        {
            // Sử dụng currentFreezeChance thay vì 20% cố định
            if (Random.value < currentFreezeChance)
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

    // Coroutine to progressively increase difficulty
    IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRateIncreaseInterval);
            
            difficultyLevel++;
            
            // Decrease spawn delay (faster spawning)
            currentSpawnDelay = Mathf.Max(minSpawnDelay, currentSpawnDelay - spawnDelayDecrease);
            
            // Increase obstacle spawn chance
            currentObstacleChance = Mathf.Min(maxObstacleChance, currentObstacleChance + obstacleChanceIncrease);
            
            // Decrease power-up spawn chance (make them rarer)
            currentPowerUpChance = Mathf.Max(minPowerUpChance, currentPowerUpChance - powerUpChanceDecrease);
            
            // Increase freeze circle spawn chance
            currentFreezeChance = Mathf.Min(maxFreezeChance, currentFreezeChance + freezeChanceIncrease);
            
            // Increase arc coin rate (more challenging coin patterns)
            currentArcRate = Mathf.Min(maxArcRate, currentArcRate + arcRateIncrease);
            
            // Log difficulty level up with detailed information
            Debug.Log("=== DIFFICULTY LEVEL UP! ===");
            Debug.Log($"🔥 NEW DIFFICULTY LEVEL: {difficultyLevel}");
            Debug.Log($"⚡ Spawn Speed: {currentSpawnDelay:F2}s (Min: {minSpawnDelay:F2}s)");
            Debug.Log($"🚧 Obstacle Chance: {(currentObstacleChance * 100):F1}% (Max: {(maxObstacleChance * 100):F1}%)");
            Debug.Log($"💎 Power-up Chance: {(currentPowerUpChance * 100):F1}% (Min: {(minPowerUpChance * 100):F1}%)");
            Debug.Log($"❄️ Freeze Circle Chance: {(currentFreezeChance * 100):F1}% (Max: {(maxFreezeChance * 100):F1}%)");
            Debug.Log($"🌙 Arc Coin Rate: {(currentArcRate * 100):F1}% (Max: {(maxArcRate * 100):F1}%)");
            Debug.Log("==========================");
            
            // Check for max level achievements
            if (currentSpawnDelay <= minSpawnDelay && difficultyLevel > 1)
            {
                Debug.Log("⚡ MAX SPEED REACHED! Spawning at maximum rate!");
            }
            
            if (currentObstacleChance >= maxObstacleChance && difficultyLevel > 1)
            {
                Debug.Log("🚧 MAX OBSTACLE DENSITY! Every tile now has maximum obstacles!");
            }
            
            if (currentPowerUpChance <= minPowerUpChance && difficultyLevel > 1)
            {
                Debug.Log("💎 MINIMUM POWER-UPS! Power-ups are now extremely rare!");
            }
            
            if (currentFreezeChance >= maxFreezeChance && difficultyLevel > 1)
            {
                Debug.Log("❄️ MAX FREEZE RATE! Freeze circles are everywhere!");
            }
            
            if (currentArcRate >= maxArcRate && difficultyLevel > 1)
            {
                Debug.Log("🌙 MAX ARC COINS! All coin patterns are now challenging arcs!");
            }
            
            // Special milestone notifications
            if (difficultyLevel == 5)
            {
                Debug.Log("🏆 MILESTONE: You've survived 5 difficulty levels! The game is getting intense!");
            }
            else if (difficultyLevel == 10)
            {
                Debug.Log("🔥 MILESTONE: Level 10 reached! You're in the danger zone now!");
            }
            else if (difficultyLevel == 15)
            {
                Debug.Log("⚡ MILESTONE: Level 15! Only the best players reach this far!");
            }
            else if (difficultyLevel % 20 == 0)
            {
                Debug.Log($"👑 LEGENDARY MILESTONE: Level {difficultyLevel}! You are a master player!");
            }
        }
    }

    // Public getter for current difficulty level
    public int GetCurrentDifficultyLevel()
    {
        return difficultyLevel;
    }
    
    // Public getter for current difficulty stats
    public string GetDifficultyStats()
    {
        return $"Level {difficultyLevel} | Speed: {currentSpawnDelay:F2}s | Obstacles: {(currentObstacleChance * 100):F0}% | Power-ups: {(currentPowerUpChance * 100):F0}%";
    }
}