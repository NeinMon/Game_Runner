using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject coinObject; // Kéo thả prefab đồng xu vào đây trong Inspector

    public Transform tile10bj;
    private Vector3 nextTileSpawn;
    public Transform bricksobj;
    private Vector3 nextBricksSpawn;
    private int randX;
    public Transform smcrateobj;
    private Vector3 nextSmcrateSpawn;
    public Transform dbcrateobj;
    private Vector3 nextDbcrateSpawn;
    public Transform carobj;
    private Vector3 nextCarSpawn;
    private int randchoice;
    
    // Power-up objects
    public GameObject speedBoostObject;    // Tăng tốc (màu xanh lá)
    public GameObject slowDownObject;      // Giảm tốc (màu đỏ)
    public GameObject invisibilityObject;  // Tàng hình (màu tím)
    
    private Vector3 nextPowerUpSpawn;

    void Start()
    {
        nextTileSpawn.z = 15; // Set the initial spawn position for the tile
        StartCoroutine(SpawnTile()); // Continue spawning tiles
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnTile()
    {
        // Wait for 1 second before spawning the tile
        yield return new WaitForSeconds(1);
        randX = Random.Range(-1, 2); // Generate a random X position within the range
        nextBricksSpawn = nextTileSpawn;
        nextBricksSpawn.x = randX; // Apply the random X offset
        // Instantiate the tile at the next spawn position
        Instantiate(tile10bj, nextTileSpawn, tile10bj.rotation);

        SpawnCoinsOnTile(nextTileSpawn);

        Instantiate(bricksobj, nextBricksSpawn, bricksobj.rotation);
        nextTileSpawn.z += 7; // Adjust as needed for your game logic


        randX = Random.Range(-1, 2); // Generate a new random X position for the next item
        nextSmcrateSpawn.z = nextTileSpawn.z;
        // nextSmcrateSpawn.y = 0.5f;
        nextSmcrateSpawn.x = randX;        Instantiate(tile10bj, nextTileSpawn, tile10bj.rotation);
        SpawnCoinsOnTile(nextTileSpawn);
        
        // Spawn power-up với 25% cơ hội
        if (Random.value < 0.25f)
        {
            SpawnPowerUpOnTile(nextTileSpawn);
        }
        
        Instantiate(smcrateobj, nextSmcrateSpawn, smcrateobj.rotation);
        if (randX == 0)
        {
            randX = 1;
            // Spawn an additional item
        }
        else if (randX == 1)
        {
            randX = -1;
            // Spawn an additional item
        }
        else
        {
            randX = 0;
            // Spawn an additional item
        }
        randchoice = Random.Range(0, 2);
        if (randchoice == 0)
        {
            nextDbcrateSpawn.z = nextTileSpawn.z;
            // nextDbcrateSpawn.y = .151f;
            nextDbcrateSpawn.x = randX;
            Instantiate(dbcrateobj, nextDbcrateSpawn, dbcrateobj.rotation);
        }
        else
        {
            nextCarSpawn.z = nextTileSpawn.z;
            nextCarSpawn.y = 0.2f;
            nextCarSpawn.x = randX;
            Instantiate(carobj, nextCarSpawn, carobj.rotation);

        }







        // Update the next spawn position
        nextTileSpawn.z += 7; // Adjust as needed for your game logic
        StartCoroutine(SpawnTile()); // Continue spawning tiles

    }    // Hàm tạo 5 đồng xu theo hàng dọc
    void SpawnCoinsOnTile(Vector3 tilePosition)
    {
        // Số lượng đồng xu muốn tạo
        int coinCount = 5;
        
        // Khoảng cách giữa các đồng xu theo trục Z (hàng dọc)
        float coinSpacing = 0.5f;
        
        // Chọn lane cho đồng xu (có thể cùng lane với vật thể)
        float coinX = GetLaneForCoins(tilePosition);
        
        // Tính toán vị trí Z bắt đầu (có thể dịch chuyển nếu cùng lane với vật thể)
        float baseStartZ = GetCoinStartZ(tilePosition, coinX);
        float startZ = baseStartZ - (coinCount - 1) * coinSpacing / 2;
        
        // Độ cao của đồng xu
        float coinHeight = tilePosition.y + 0.2f;
        
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 coinPosition = new Vector3(
                coinX,                       // Lane đã chọn (có thể cùng với vật thể)
                coinHeight,                  // Độ cao vừa phải
                startZ + i * coinSpacing     // Vị trí Z đã được dịch chuyển nếu cần
            );
            
            // Tạo đồng xu tại vị trí đã tính toán
            Instantiate(coinObject, coinPosition, coinObject.transform.rotation);
        }
    }      // Hàm tìm lane cho đồng xu (có thể cùng lane với vật thể)
    float GetLaneForCoins(Vector3 tilePosition)
    {
        // 30% cơ hội chọn lane trống, 70% cơ hội chọn lane có vật thể (để tạo thách thức hấp dẫn)
        bool preferEmptyLane = Random.value > 0.7f;
        
        // Danh sách các lane có thể (-1, 0, 1)
        float[] possibleLanes = { -1f, 0f, 1f };
        
        if (preferEmptyLane)
        {
            // Tìm lane trống trước
            foreach (float lane in possibleLanes)
            {
                if (IsLaneEmpty(lane))
                {
                    return lane;
                }
            }
        }
        
        // Ưu tiên chọn lane có vật thể để tạo thách thức thú vị
        return possibleLanes[Random.Range(0, possibleLanes.Length)];
    }
    
    // Kiểm tra xem lane có trống không
    bool IsLaneEmpty(float lane)
    {
        // Kiểm tra va chạm với brick
        if (Mathf.Approximately(nextBricksSpawn.x, lane))
            return false;
            
        // Kiểm tra va chạm với smcrate
        if (Mathf.Approximately(nextSmcrateSpawn.x, lane))
            return false;
            
        // Kiểm tra va chạm với dbcrate (nếu được spawn)
        if (randchoice == 0 && Mathf.Approximately(nextDbcrateSpawn.x, lane))
            return false;
            
        // Kiểm tra va chạm với car (nếu được spawn)
        if (randchoice == 1 && Mathf.Approximately(nextCarSpawn.x, lane))
            return false;
            
        return true;
    }
      // Tính toán vị trí Z cho đồng xu khi cùng lane với vật thể
    float GetCoinStartZ(Vector3 tilePosition, float coinX)
    {
        bool hasObstacleInLane = !IsLaneEmpty(coinX);
        
        if (hasObstacleInLane)
        {
            // Có vật thể trong lane - tạo thách thức thú vị
            int challengeType = Random.Range(0, 4);
            
            switch (challengeType)
            {
                case 0:
                    // Đặt đồng xu phía trước vật thể (cần timing để nhặt)
                    return tilePosition.z + 2.5f;
                    
                case 1:
                    // Đặt đồng xu phía sau vật thể (cần né vật thể rồi quay lại)
                    return tilePosition.z - 2.5f;
                    
                case 2:
                    // Đặt đồng xu sát bên cạnh vật thể (cần kỹ năng di chuyển chính xác)
                    return tilePosition.z + 0.8f;
                    
                case 3:
                    // Đặt đồng xu xen kẽ với vật thể (tạo đường đi zigzag)
                    return tilePosition.z - 0.8f;
                    
                default:
                    return tilePosition.z + 2.0f;
            }
        }
        else
        {
            // Lane trống - đặt đồng xu ở giữa tile
            return tilePosition.z;
        }
    }
    
    // Hàm tạo power-up trên tile
    void SpawnPowerUpOnTile(Vector3 tilePosition)
    {
        // Chọn ngẫu nhiên loại power-up (0: tăng tốc, 1: giảm tốc, 2: tàng hình)
        int powerUpType = Random.Range(0, 3);
        
        // Tìm lane trống cho power-up
        float powerUpX = GetEmptyLaneForPowerUp(tilePosition);
        
        // Vị trí spawn power-up
        Vector3 powerUpPosition = new Vector3(
            powerUpX,
            tilePosition.y + 0.1f,  // Cao hơn đồng xu để dễ nhận biết
            tilePosition.z + Random.Range(-1.5f, 1.5f)  // Ngẫu nhiên vị trí Z
        );
        
        // Spawn power-up tương ứng
        switch (powerUpType)
        {
            case 0:
                if (speedBoostObject != null)
                    Instantiate(speedBoostObject, powerUpPosition, speedBoostObject.transform.rotation);
                break;
            case 1:
                if (slowDownObject != null)
                    Instantiate(slowDownObject, powerUpPosition, slowDownObject.transform.rotation);
                break;
            case 2:
                if (invisibilityObject != null)
                    Instantiate(invisibilityObject, powerUpPosition, invisibilityObject.transform.rotation);
                break;
        }
    }
    
    // Tìm lane trống cho power-up
    float GetEmptyLaneForPowerUp(Vector3 tilePosition)
    {
        float[] possibleLanes = { -1f, 0f, 1f };
        
        // Ưu tiên tìm lane hoàn toàn trống
        foreach (float lane in possibleLanes)
        {
            if (IsLaneEmpty(lane))
            {
                return lane;
            }
        }
        
        // Nếu không có lane trống, chọn ngẫu nhiên
        return possibleLanes[Random.Range(0, possibleLanes.Length)];
    }
}
