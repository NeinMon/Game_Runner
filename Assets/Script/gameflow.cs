using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameflow : MonoBehaviour
{
    public Transform tileObj;
    public float tileLength = 20f; // Chiều dài 1 tile, chỉnh đúng với prefab của bạn
    private Vector3 nextTileSpawn = Vector3.zero;

    // Thêm offset X để map căn giữa với lane giữa của nhân vật
    [SerializeField] private float tileOffsetX = 0f; // Điều chỉnh trong Inspector nếu cần

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Đặt vị trí spawn tile đầu tiên trùng với lane giữa của nhân vật
        nextTileSpawn = new Vector3(tileOffsetX, 0, 0);
        // Spawn một số tile ban đầu
        for (int i = 0; i < 5; i++)
        {
            SpawnTile();
        }
        StartCoroutine(SpawnTileRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnTile()
    {
        // Đảm bảo tile sinh ra đúng offset X
        Vector3 spawnPos = nextTileSpawn;
        spawnPos.x = tileOffsetX;
        Instantiate(tileObj, spawnPos, tileObj.rotation);
        nextTileSpawn.z += tileLength;
    }

    IEnumerator SpawnTileRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Thời gian sinh tile mới
            SpawnTile();
        }
    }
}
