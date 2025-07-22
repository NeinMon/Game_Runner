using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapgameflow : MonoBehaviour
{
    public Transform tileObj;
    private Vector3 nextTileSpawn;
    public Transform bricksObj;
    private Vector3 nextBrickSpawn;
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

    private Vector3 nextPowerUpSpawn;
    private Vector3 nextInvisibleSpawn;

    private int tileCounter = 0;
    public int powerUpFrequency = 3;

    public float currentArcRate = 0.5f;

    void Start()
    {
        nextTileSpawn.z = 18;
        StartCoroutine(spawnTile());
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

        // Spawn Brick (nếu có)
        if (bricksObj != null)
        {
            nextBrickSpawn = nextTileSpawn;
            nextBrickSpawn.x = brickLane;
            nextBrickSpawn.y = 0f;
            Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation);
        }

        // Spawn High Obstacle (nếu có)
        if (highObstacleObj != null)
        {
            nextHighObstacleSpawn = nextTileSpawn;
            nextHighObstacleSpawn.x = obstacleLane;
            nextHighObstacleSpawn.y = 0f;
            Instantiate(highObstacleObj, nextHighObstacleSpawn, highObstacleObj.rotation);
        }

        // Spawn Freeze Circle (nếu có)
        if (freezeCircleObj != null)
        {
            nextFreezeCircleSpawn = nextTileSpawn;
            nextFreezeCircleSpawn.x = brickLane;
            nextFreezeCircleSpawn.y = 0.1f;
            Instantiate(freezeCircleObj, nextFreezeCircleSpawn, freezeCircleObj.rotation);
        }

        // Spawn Obstacle (nếu có)
        if (obstacleObj != null)
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
            if (Random.value < currentArcRate)
            {
                // Coin vòng cung
                Vector3 coinPosition = nextTileSpawn;
                coinPosition.x = coinLane;
                float jumpForce = 2.1f;
                float gravity = -5f;
                float tMax = -2 * jumpForce / gravity;
                float dz = 0.7f;

                for (int i = 0; i < 5; i++)
                {
                    Vector3 spawnPos = coinPosition;
                    float t = i / 4.0f * tMax;
                    float y = jumpForce * t + 0.5f * gravity * t * t;
                    spawnPos.y = 0.2f + y;

                    Instantiate(coinObj, spawnPos, coinObj.rotation);

                    // Spawn vật cản dưới coin thứ 3 (nếu prefab tồn tại)
                    if (i == 2)
                    {
                        Vector3 belowCoin = spawnPos;

                        if (bricksObj != null && (Random.value < 0.5f || obstacleObj == null))
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
                // Coin thẳng hàng
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

        // Power-ups
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
}
