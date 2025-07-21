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

        // Spawn Brick
        if (bricksObj != null)
        {
            nextBrickSpawn = nextTileSpawn;
            nextBrickSpawn.x = brickLane;
            nextBrickSpawn.y = -0.57f;
            Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation);
        }

        if (highObstacleObj != null)
        {
            nextHighObstacleSpawn = nextTileSpawn;
            nextHighObstacleSpawn.x = obstacleLane; // hoặc random lane nếu muốn đa dạng
            nextHighObstacleSpawn.y = 0f; // điều chỉnh nếu cần để chạm đất
            Instantiate(highObstacleObj, nextHighObstacleSpawn, highObstacleObj.rotation);
        }

        // Spawn Freeze Circle
        if (freezeCircleObj != null)
        {
            nextFreezeCircleSpawn = nextTileSpawn;
            nextFreezeCircleSpawn.x = brickLane; // hoặc coinLane, obstacleLane tùy theo bạn muốn
            nextFreezeCircleSpawn.y = 0f;
            Instantiate(freezeCircleObj, nextFreezeCircleSpawn, freezeCircleObj.rotation);
        }

        // Spawn Obstacle
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

        // Power-ups
        if (tileCounter % powerUpFrequency == 0)
        {
            float rand = Random.value;

            nextPowerUpSpawn = nextTileSpawn;
            nextPowerUpSpawn.x = Random.value < 0.5f ? brickLane : obstacleLane;
            nextPowerUpSpawn.y = 0.2f;
            nextPowerUpSpawn.z += 2.5f;

            if (rand < 0.5f && speedUpObj != null)
                Instantiate(speedUpObj, nextPowerUpSpawn, speedUpObj.rotation);
            else if (rand < 0.8f && slowDownObj != null)
                Instantiate(slowDownObj, nextPowerUpSpawn, slowDownObj.rotation);
            else if (invisibleObj != null)
            {
                nextInvisibleSpawn = nextTileSpawn;
                nextInvisibleSpawn.x = Random.value < 0.5f ? brickLane : obstacleLane;
                nextInvisibleSpawn.y = 0.2f;
                nextInvisibleSpawn.z += 2.5f;
                Instantiate(invisibleObj, nextInvisibleSpawn, invisibleObj.rotation);
            }
        }

        // Spawn two tiles ahead
        Instantiate(tileObj, nextTileSpawn, tileObj.rotation);
        nextTileSpawn.z += 3;
        Instantiate(tileObj, nextTileSpawn, tileObj.rotation);
        nextTileSpawn.z += 3;

        StartCoroutine(spawnTile());
    }
}
