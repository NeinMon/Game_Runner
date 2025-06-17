using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameflow : MonoBehaviour
{
    public Transform tileObj;
    private Vector3 nextTileSpawn;
    public Transform bricksObj;
    private Vector3 nextBrickSpawn;
    public Transform obstacleObj; // New obstacle object
    private Vector3 nextObstacleSpawn;
    private int randX;
    private int randObsX;

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
        randX = Random.Range(-1, 2);
        randObsX = Random.Range(-1, 2);
        
        // Setup brick position
        nextBrickSpawn = nextTileSpawn;
        nextBrickSpawn.x = randX;
        nextBrickSpawn.y = 0.1f;
        
        // Setup obstacle position (different lane than brick)
        nextObstacleSpawn = nextTileSpawn;
        nextObstacleSpawn.x = randObsX == randX ? (randX + 1) % 3 - 1 : randObsX; // Ensure different X from brick
        nextObstacleSpawn.y = 0.15f; // Slightly higher than bricks
        nextObstacleSpawn.z += 1.5f; // Offset in Z direction
        
        // Instantiate and destroy tiles after 50 seconds
        GameObject tile1 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;
        Destroy(tile1, 50f);
        
        // Instantiate and destroy bricks after 50 seconds
        GameObject brick = Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation).gameObject;
        Destroy(brick, 50f);
        
        // Instantiate and destroy new obstacle after 50 seconds
        if (obstacleObj != null) {
            GameObject obstacle = Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation).gameObject;
            Destroy(obstacle, 50f);
        }
        
        nextTileSpawn.z += 3;
        
        // Instantiate and destroy second tile after 50 seconds
        GameObject tile2 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;
        Destroy(tile2, 50f);
        
        nextTileSpawn.z += 3;
        StartCoroutine(spawnTile());
    }
}
