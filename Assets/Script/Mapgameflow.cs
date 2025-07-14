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
    
    private int tileCounter; // Counter to track tiles spawned
    public int powerUpFrequency = 3; // Spawn power-up every X tiles
    
    // Difficulty progression variables
    [Header("Difficulty Settings")]
    public float initialSpawnDelay = 1.0f; // Initial spawn delay
    public float minimumSpawnDelay = 0.3f; // Minimum spawn delay (max difficulty)
    public float difficultyIncreaseRate = 0.02f; // How much to decrease delay per tile
    public int difficultyIncreaseInterval = 5; // Every X tiles, increase difficulty
    
    [Header("Obstacle Frequency")]
    public float baseObstacleChance = 0.8f; // Base chance to spawn obstacles
    public float maxObstacleChance = 1.0f; // Maximum obstacle chance
    public float obstacleChanceIncrease = 0.05f; // Increase per difficulty level
    
    [Header("Power-up Frequency")]
    public int basePowerUpFrequency = 3; // Base power-up frequency
    public int maxPowerUpFrequency = 7; // Maximum power-up frequency (less frequent)
    
    [Header("Special Obstacles")]
    public float baseFreezeChance = 0.2f; // Base freeze circle chance
    public float maxFreezeChance = 0.5f; // Maximum freeze circle chance
    public float freezeChanceIncrease = 0.05f; // Increase per difficulty level
    
    // Current difficulty tracking
    private float currentSpawnDelay;
    private float currentObstacleChance;
    private int currentPowerUpFrequency;
    private float currentFreezeChance;
    private int difficultyLevel;
    
    // Level system integration
    [Header("Level Integration")]
    public Transform coinMultiplierPowerUp; // x2 coin power-up object
    public int multiplierPowerUpFrequency = 15; // Spawn every X tiles
    private bool isGamePaused;

    void Start()
    {
        nextTileSpawn.z = 18;
        InitializeDifficulty();
        StartCoroutine(SpawnTile());
    }
    
    public void PauseGame()
    {
        isGamePaused = true;
        Debug.Log("Game paused!");
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Debug.Log("Game resumed!");
    }
    
    public void ResetForNewLevel()
    {
        // Reset difficulty and counters for new level
        difficultyLevel = 0;
        tileCounter = 0;
        isGamePaused = false;
        
        // Clear existing objects (optional - depends on your game design)
        ClearExistingObjects();
        
        // Reinitialize difficulty
        InitializeDifficulty();
        
        // Reset spawn position
        nextTileSpawn.z = 18;
        
        // Restart spawning
        StartCoroutine(SpawnTile());
        
        Debug.Log("Map reset for new level!");
    }
    
    void ClearExistingObjects()
    {
        // Clear coins
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }
        
        // Clear obstacles
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        
        // Clear power-ups
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in powerUps)
        {
            Destroy(powerUp);
        }
    }
    
    void InitializeDifficulty()
    {
        currentSpawnDelay = initialSpawnDelay;
        currentObstacleChance = baseObstacleChance;
        currentPowerUpFrequency = basePowerUpFrequency;
        currentFreezeChance = baseFreezeChance;
        powerUpFrequency = currentPowerUpFrequency;
    }
    
    void UpdateDifficulty()
    {
        // Increase difficulty every difficultyIncreaseInterval tiles
        if (tileCounter % difficultyIncreaseInterval == 0)
        {
            difficultyLevel++;
            
            // Decrease spawn delay (increase speed)
            currentSpawnDelay = Mathf.Max(minimumSpawnDelay, 
                initialSpawnDelay - (difficultyLevel * difficultyIncreaseRate));
            
            // Increase obstacle chance
            currentObstacleChance = Mathf.Min(maxObstacleChance, 
                baseObstacleChance + (difficultyLevel * obstacleChanceIncrease));
            
            // Decrease power-up frequency (make them rarer)
            currentPowerUpFrequency = Mathf.Min(maxPowerUpFrequency, 
                basePowerUpFrequency + (difficultyLevel / 2));
            powerUpFrequency = currentPowerUpFrequency;
            
            // Increase freeze circle chance
            currentFreezeChance = Mathf.Min(maxFreezeChance, 
                baseFreezeChance + (difficultyLevel * freezeChanceIncrease));
            
            Debug.Log($"Difficulty Level: {difficultyLevel}, Spawn Delay: {currentSpawnDelay:F2}s, " +
                     $"Obstacle Chance: {currentObstacleChance:F2}, Freeze Chance: {currentFreezeChance:F2}");
        }
    }

    IEnumerator SpawnTile()
    {
        while (!isGamePaused)
        {
            yield return new WaitForSeconds(currentSpawnDelay);
            
            // Increment tile counter
            tileCounter++;
            
            // Update difficulty based on progress
            UpdateDifficulty();
        
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
        Instantiate(tileObj, nextTileSpawn, tileObj.rotation);

        // Instantiate bricks without destruction
        Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation);

        // Instantiate obstacles based on difficulty
        if (obstacleObj != null && Random.value < currentObstacleChance)
        {
            Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation);
            
            // At higher difficulty levels, sometimes spawn double obstacles
            if (difficultyLevel >= 5 && Random.value < 0.3f)
            {
                Vector3 doubleObstaclePos = nextObstacleSpawn;
                doubleObstaclePos.z += 1.5f; // Spawn another obstacle close behind
                Instantiate(obstacleObj, doubleObstaclePos, obstacleObj.rotation);
            }
        }

        // Coin xuất hiện với tỉ lệ arc tăng theo độ khó
        if (coinObj != null)
        {
            // Increase arc rate based on difficulty for more challenging coin patterns
            float arcRate = Mathf.Min(0.6f, 0.3f + (difficultyLevel * 0.05f));
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
                    Instantiate(coinObj, spawnPos, coinObj.rotation);
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
                    Instantiate(coinObj, spawnPos, coinObj.rotation);
                    coinPosition.z += dz;
                }
            }
        }

        // Spawn coin multiplier power-up (x2 coins)
        if (coinMultiplierPowerUp != null && tileCounter % multiplierPowerUpFrequency == 0)
        {
            Vector3 multiplierPos = nextPowerUpSpawn;
            multiplierPos.y = 0.3f; // Slightly higher than other power-ups
            multiplierPos.z += 1.0f; // Different Z position to avoid overlap
            Instantiate(coinMultiplierPowerUp, multiplierPos, coinMultiplierPowerUp.rotation);
            Debug.Log("Coin Multiplier x2 spawned!");
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

        // Spawn vật cản đặc biệt: freeze circle với tỉ lệ tăng theo độ khó
        if (freezeCircleObj != null)
        {
            // Tỉ lệ xuất hiện freeze circle tăng theo độ khó
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
                
                // At very high difficulty, sometimes spawn multiple freeze circles
                if (difficultyLevel >= 8 && Random.value < 0.2f)
                {
                    possibleLanes.Remove(freezeLane);
                    if (possibleLanes.Count > 0)
                    {
                        int secondFreezeLane = possibleLanes[Random.Range(0, possibleLanes.Count)];
                        Vector3 secondFreezePos = freezePos;
                        secondFreezePos.x = secondFreezeLane;
                        secondFreezePos.z += 1.5f;
                        Instantiate(freezeCircleObj, secondFreezePos, freezeCircleObj.rotation);
                    }
                }
            }
        }

        nextTileSpawn.z += 3;

        // Instantiate second tile without destruction
        Instantiate(tileObj, nextTileSpawn, tileObj.rotation);

        nextTileSpawn.z += 3;
        
        // Continue spawning only if not paused
        if (!isGamePaused)
        {
            StartCoroutine(SpawnTile());
        }
        }
    }
    
    // Public method to get current difficulty info (for UI display)
    public string GetDifficultyInfo()
    {
        return $"Level: {difficultyLevel} | Speed: {(1/currentSpawnDelay):F1}x | Obstacles: {(currentObstacleChance*100):F0}%";
    }
    
    // Public getter methods for other scripts to access difficulty values
    public int GetDifficultyLevel() => difficultyLevel;
    public float GetCurrentSpawnDelay() => currentSpawnDelay;
    public float GetCurrentObstacleChance() => currentObstacleChance;
    public float GetCurrentFreezeChance() => currentFreezeChance;
}