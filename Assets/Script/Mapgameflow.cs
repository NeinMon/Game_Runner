using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mapgameflow : MonoBehaviour
{
    public Transform tileObj;
    private Vector3 nextTileSpawn;
    public Transform obstacleObj;
    private Vector3 nextObstacleSpawn;
    public Transform coinObj;
    private Vector3 nextCoinSpawn;
    public Transform speedUpObj;
    public Transform slowDownObj;
    public Transform invisibleObj;
    public Transform freezeCircleObj;
    private Vector3 nextFreezeCircleSpawn;
    public Transform highObstacleObj;
    private Vector3 nextHighObstacleSpawn;
    
    // Car obstacles
    public Transform carObstacleObj;
    private Vector3 nextCarObstacleSpawn;
    
    private int currentLevel = 1;

    private Vector3 nextPowerUpSpawn;
    private Vector3 nextInvisibleSpawn;

    private int tileCounter = 0;
    public int powerUpFrequency = 3;

    public float currentArcRate = 0.5f;

    void Start()
    {
        // Lấy level hiện tại từ tên scene
        currentLevel = GetCurrentLevel();
        nextTileSpawn.z = 18;
        StartCoroutine(spawnTile());
    }
    
    private int GetCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("map"))
        {
            string mapNumberStr = sceneName.Substring(3);
            if (int.TryParse(mapNumberStr, out int mapNumber))
            {
                return mapNumber;
            }
        }
        return 1; // Default to level 1
    }

    IEnumerator spawnTile()
    {
        yield return new WaitForSeconds(1);
        tileCounter++;

        int[] lanes = { -1, 0, 1 };
        System.Random rnd = new System.Random();
        for (int i = lanes.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            int temp = lanes[i];
            lanes[i] = lanes[j];
            lanes[j] = temp;
        }

        int brickLane = lanes[0];
        int obstacleLane = lanes[1];
        int coinLane = lanes[2];

        // Spawn High Obstacle (nếu có và level cho phép)
        if (highObstacleObj != null && ShouldSpawnHighObstacles())
        {
            nextHighObstacleSpawn = nextTileSpawn;
            nextHighObstacleSpawn.x = obstacleLane;
            nextHighObstacleSpawn.y = 0f;
            Instantiate(highObstacleObj, nextHighObstacleSpawn, highObstacleObj.rotation);
        }

        // Spawn Freeze Circle (nếu có và level cho phép)
        if (freezeCircleObj != null && ShouldSpawnFreezeCircles())
        {
            nextFreezeCircleSpawn = nextTileSpawn;
            nextFreezeCircleSpawn.x = brickLane;
            nextFreezeCircleSpawn.y = 0.1f;
            Instantiate(freezeCircleObj, nextFreezeCircleSpawn, freezeCircleObj.rotation);
        }

        // Spawn Car Obstacle và Obstacle
        bool shouldSpawnCar = carObstacleObj != null && ShouldSpawnCarObstacles();
        bool shouldSpawnObstacle = obstacleObj != null && ShouldSpawnObstacles();
        
        if (shouldSpawnCar && shouldSpawnObstacle)
        {
            // Nếu cần spawn cả 2, spawn random 1 trong 2
            if (Random.value < 0.5f)
            {
                nextCarObstacleSpawn = nextTileSpawn;
                nextCarObstacleSpawn.x = obstacleLane;
                nextCarObstacleSpawn.y = 0.1f;
                nextCarObstacleSpawn.z += 2.0f;
                Instantiate(carObstacleObj, nextCarObstacleSpawn, carObstacleObj.rotation);
            }
            else
            {
                nextObstacleSpawn = nextTileSpawn;
                nextObstacleSpawn.x = obstacleLane;
                nextObstacleSpawn.y = 0f;
                nextObstacleSpawn.z += 1.5f;
                Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation);
            }
        }
        else if (shouldSpawnCar)
        {
            nextCarObstacleSpawn = nextTileSpawn;
            nextCarObstacleSpawn.x = obstacleLane;
            nextCarObstacleSpawn.y = 0.1f;
            nextCarObstacleSpawn.z += 2.0f;
            Instantiate(carObstacleObj, nextCarObstacleSpawn, carObstacleObj.rotation);
        }
        else if (shouldSpawnObstacle)
        {
            nextObstacleSpawn = nextTileSpawn;
            nextObstacleSpawn.x = obstacleLane;
            nextObstacleSpawn.y = 0.1f;
            nextObstacleSpawn.z += 1.5f;
            Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation);
        }

        // Spawn Coins
        if (coinObj != null)
        {
            // Kiểm tra xem có nên spawn coin vòng cung không (màn 3, 5, 6)
            bool shouldSpawnArcCoins = (currentLevel == 3 || currentLevel == 5 || currentLevel == 6);
            
            if (shouldSpawnArcCoins && Random.value < currentArcRate)
            {
                // Coin vòng cung (tạo vòng cung với freezeCircleObj làm trung tâm)
                Vector3 coinPosition = nextTileSpawn;
                coinPosition.x = brickLane; // Tất cả coin đều ở lane freezeCircleObj
                float jumpForce = 2.1f;
                float gravity = -5f;
                float tMax = -2 * jumpForce / gravity;

                // Tạo vòng cung với freezeCircleObj làm trung tâm
                float[] zOffsets = { -1f, -0.5f, 0f, 0.5f, 1f }; // Coin 1,2,3,4,5

                for (int i = 0; i < 5; i++)
                {
                    Vector3 spawnPos = nextTileSpawn;
                    float t = i / 4.0f * tMax;
                    float y = jumpForce * t + 0.5f * gravity * t * t;
                    
                    spawnPos.x = brickLane; // Cùng lane với freezeCircleObj
                    spawnPos.y = 0.2f + y;
                    spawnPos.z = nextTileSpawn.z + zOffsets[i]; // Coin 3 ở z=0 (trên freezeCircleObj)

                    Instantiate(coinObj, spawnPos, coinObj.rotation);
                }
            }
            else
            {
                // Coin thẳng hàng (cho tất cả màn)
                Vector3 coinPos = nextTileSpawn;
                coinPos.x = coinLane;
                coinPos.y = 0.2f;
                coinPos.z += 1.0f;
                for (int i = 0; i < 5; i++)
                {
                    Instantiate(coinObj, coinPos, coinObj.rotation);
                    coinPos.z += 0.7f;
                }
            }
        }

        // Power-ups (cho tất cả các màn)
        if (tileCounter % powerUpFrequency == 0)
        {
            float rand = Random.value;
            nextPowerUpSpawn = nextTileSpawn;
            nextPowerUpSpawn.x = (Random.value < 0.5f ? brickLane : obstacleLane);
            nextPowerUpSpawn.y = 0.2f;
            nextPowerUpSpawn.z += 2.5f;

            if (rand < 0.5f && speedUpObj != null)
            {
                Instantiate(speedUpObj, nextPowerUpSpawn, speedUpObj.rotation);
            }
            else if (rand < 0.8f && slowDownObj != null)
            {
                Instantiate(slowDownObj, nextPowerUpSpawn, slowDownObj.rotation);
            }
            else if (invisibleObj != null)
            {
                nextInvisibleSpawn = nextTileSpawn;
                nextInvisibleSpawn.x = (Random.value < 0.5f ? brickLane : obstacleLane);
                nextInvisibleSpawn.y = 0.2f;
                nextInvisibleSpawn.z += 2.5f;
                Instantiate(invisibleObj, nextInvisibleSpawn, invisibleObj.rotation);
            }
        }

        // Spawn Tiles
        Instantiate(tileObj, nextTileSpawn, tileObj.rotation);
        nextTileSpawn.z += 3;
        Instantiate(tileObj, nextTileSpawn, tileObj.rotation);
        nextTileSpawn.z += 3;

        StartCoroutine(spawnTile());
    }
    
    // Kiểm tra xem có nên spawn obstacles không
    private bool ShouldSpawnObstacles()
    {
        // Màn 2, 3, 6 có obstacles
        return currentLevel == 2 || currentLevel == 3 || currentLevel == 6;
    }
    
    // Kiểm tra xem có nên spawn freeze circles không
    private bool ShouldSpawnFreezeCircles()
    {
        // Màn 3, 5, 6 có freeze circles
        return currentLevel == 3 || currentLevel == 5 || currentLevel == 6;
    }
    
    // Kiểm tra xem có nên spawn car obstacles không
    private bool ShouldSpawnCarObstacles()
    {
        // Tất cả màn đều có car obstacles
        // Màn 1-3: xe đứng yên (không có script chạy)
        // Màn 4-6: xe chạy về phía nhân vật
        return true;
    }
    
    // Kiểm tra xem có nên spawn high obstacles không
    private bool ShouldSpawnHighObstacles()
    {
        // Chỉ màn 6 có high obstacles
        return currentLevel == 6 || currentLevel == 3;
    }
}
