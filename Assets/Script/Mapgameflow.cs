using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script điều khiển luồng game và spawn các đối tượng trên map
public class Mapgameflow : MonoBehaviour
{
    // === PREFABS VÀ VỊ TRÍ SPAWN ===
    public Transform tileObj;           // Prefab tile nền đường
    private Vector3 nextTileSpawn;      // Vị trí spawn tile tiếp theo
    public Transform bricksObj;         // Prefab gạch (vật cản)
    public Transform highObject; // Prefab đối tượng cao
    public float minDistanceBetweenHighObjects = 8f; // Khoảng cách tối thiểu giữa 2 highObject
    private float lastHighObjectZ = -999f; // Vị trí Z cuối cùng spawn highObject

    private Vector3 nextBrickSpawn;     // Vị trí spawn gạch tiếp theo
    public Transform obstacleObj;       // Prefab vật cản mới
    private Vector3 nextObstacleSpawn;  // Vị trí spawn vật cản tiếp theo
    public Transform coinObj;           // Prefab coin (tiền xu)
    private Vector3 nextCoinSpawn;      // Vị trí spawn coin tiếp theo

    // === POWER-UPS ===
    public Transform speedUpObj;        // Power-up tăng tốc (Thunder)
    public Transform slowDownObj;       // Power-up giảm tốc (Time)
    public Transform invisiblePowerObj; // Power-up vô hình (Invisible)
    public Transform freezeCircleObj;   // Power-up đóng băng (Freeze Circle)
    private Vector3 nextPowerUpSpawn;   // Vị trí spawn power-up tiếp theo

    // === COUNTERS VÀ FREQUENCY ===
    private int tileCounter = 0;        // Đếm số tile đã spawn
    public int powerUpFrequency = 3;    // Spawn power-up mỗi X tiles

    // === CÀI ĐẶT ĐỘ KHÓ - DIFFICULTY PROGRESSION ===
    [Header("Cài đặt độ khó")]
    public float spawnRateIncreaseInterval = 10f; // Tăng độ khó mỗi 30 giây
    public float minSpawnDelay = 0.3f;            // Thời gian spawn tối thiểu giữa các tile
    public float spawnDelayDecrease = 0.05f;      // Giảm bao nhiêu thời gian spawn mỗi lần tăng độ khó

    [Header("Mật độ vật cản")]
    public float baseObstacleChance = 0.8f;       // Tỷ lệ spawn vật cản ban đầu
    public float maxObstacleChance = 0.95f;       // Tỷ lệ spawn vật cản tối đa
    public float obstacleChanceIncrease = 0.02f;  // Tăng tỷ lệ vật cản mỗi level

    [Header("Điều chỉnh Power-up")]
    public float basePowerUpChance = 0.6f;        // Tỷ lệ spawn power-up ban đầu
    public float minPowerUpChance = 0.3f;         // Tỷ lệ spawn power-up tối thiểu
    public float powerUpChanceDecrease = 0.03f;   // Giảm tỷ lệ power-up khi game khó hơn

    [Header("Tiến trình Freeze Circle")]
    public float baseFreezeChance = 0.2f;         // Tỷ lệ freeze circle ban đầu
    public float maxFreezeChance = 0.4f;          // Tỷ lệ freeze circle tối đa
    public float freezeChanceIncrease = 0.01f;    // Tăng tỷ lệ freeze circle

    [Header("Tiến trình Coin vòng cung")]
    public float baseArcRate = 0.3f;              // Tỷ lệ coin vòng cung ban đầu
    public float maxArcRate = 0.6f;               // Tỷ lệ coin vòng cung tối đa
    public float arcRateIncrease = 0.02f;         // Tăng tỷ lệ coin vòng cung theo độ khó

    // === BIẾN RUNTIME - CÁC GIÁ TRỊ THAY ĐỔI TRONG GAME ===
    private float currentSpawnDelay = 1f;         // Thời gian spawn hiện tại
    private float currentObstacleChance;          // Tỷ lệ vật cản hiện tại
    private float currentPowerUpChance;           // Tỷ lệ power-up hiện tại
    private float currentFreezeChance;            // Tỷ lệ freeze circle hiện tại
    private float currentArcRate;                 // Tỷ lệ coin vòng cung hiện tại
    private int difficultyLevel = 0;              // Level độ khó hiện tại

    // Hàm khởi tạo khi game bắt đầu
    void Start()
    {
        nextTileSpawn.z = 18; // Đặt vị trí spawn tile đầu tiên

        // Khởi tạo các giá trị độ khó ban đầu
        currentSpawnDelay = 1f;
        currentObstacleChance = baseObstacleChance;
        currentPowerUpChance = basePowerUpChance;
        currentFreezeChance = baseFreezeChance;
        currentArcRate = baseArcRate;

        // Ghi log thông tin game bắt đầu
        Debug.Log("=== GAME BẮT ĐẦU ===");

        // Bắt đầu spawn tile và tăng độ khó
        StartCoroutine(spawnTile());
        StartCoroutine(IncreaseDifficulty());
    }

    // Hàm Update - hiện tại không sử dụng
    void Update()
    {

    }

    // Coroutine chính để spawn tile và các đối tượng game
    IEnumerator spawnTile()
    {
        yield return new WaitForSeconds(currentSpawnDelay); // Chờ theo thời gian spawn hiện tại

        // Tăng bộ đếm tile
        tileCounter++;

        // Chọn các lane khác nhau cho từng loại đối tượng (-1: trái, 0: giữa, 1: phải)
        int[] lanes = { -1, 0, 1 };
        System.Random rnd = new System.Random();

        // Thuật toán Fisher-Yates để xáo trộn ngẫu nhiên các lane
        for (int i = lanes.Length - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            int temp = lanes[i];
            lanes[i] = lanes[j];
            lanes[j] = temp;
        }

        // Gán các lane đã xáo trộn cho từng đối tượng
        int brickLane = lanes[0];    // Lane cho gạch
        int obstacleLane = lanes[1]; // Lane cho vật cản
        int coinLane = lanes[2];     // Lane cho coin

        // Thiết lập vị trí spawn vật cản (khác lane với gạch)
        nextObstacleSpawn = nextTileSpawn;
        nextObstacleSpawn.x = brickLane;
        nextObstacleSpawn.y = 0.1f; // Chiều cao nhỏ cho vật cản

        // Thiết lập vị trí spawn gạch (khác lane với vật cản)
        nextBrickSpawn = nextTileSpawn;
        nextBrickSpawn.x = obstacleLane;
        nextBrickSpawn.y = 0f;      // Mặt đất cho gạch
        nextBrickSpawn.z += 1.5f;   // Offset về phía trước trên trục Z

        // Thiết lập vị trí spawn coin (khác lane với cả gạch và vật cản)
        nextCoinSpawn = nextTileSpawn;
        nextCoinSpawn.x = coinLane;
        nextCoinSpawn.y = 0.2f;     // Chiều cao cho coin
        nextCoinSpawn.z += 1.0f;    // Vị trí bắt đầu cho coin

        // Thiết lập vị trí spawn power-up - chọn ngẫu nhiên lane không có coin
        nextPowerUpSpawn = nextTileSpawn;
        nextPowerUpSpawn.x = Random.value < 0.5f ? brickLane : obstacleLane; // Chọn giữa lane gạch và lane vật cản
        nextPowerUpSpawn.y = 0.2f;
        nextPowerUpSpawn.z += 2.5f; // Đặt sau vật cản để tránh trùng lặp trực tiếp

        // Tạo tile mà không hủy
        GameObject tile1 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        // Tạo gạch dựa trên độ khó
        if (Random.value < currentObstacleChance)
        {
            if (bricksObj != null && Random.value < currentObstacleChance)
            {
                Instantiate(bricksObj, nextBrickSpawn, bricksObj.rotation);
            }
        }

        if (highObject != null)
        {
            if (Mathf.Abs(nextTileSpawn.z - lastHighObjectZ) >= minDistanceBetweenHighObjects)
            {
                if (Random.value < 0.8f) // 30% xác suất xuất hiện highObject
                {
                    Vector3 highObjPos = nextTileSpawn;
                    highObjPos.x = 0;        // Center lane (hoặc chọn lane như coin)
                    highObjPos.z += 3f;      // Sau coin

                    Instantiate(highObject, highObjPos, highObject.rotation);
                    lastHighObjectZ = highObjPos.z; // Cập nhật vị trí spawn gần nhất
                }
            }
        }

        // Tạo vật cản mới dựa trên độ khó
        if (obstacleObj != null && Random.value < currentObstacleChance)
        {
            GameObject obstacle = Instantiate(obstacleObj, nextObstacleSpawn, obstacleObj.rotation).gameObject;
        }

        // Coin xuất hiện với tỉ lệ vòng cung tăng theo độ khó
        if (coinObj != null)
        {
            if (Random.value < currentArcRate)
            {
                // Coin vòng cung (parabol) - tạo đường cong nhảy khó khăn hơn
                Vector3 coinPosition = nextCoinSpawn;
                float jumpForce = 2.1f;    // Lực nhảy ban đầu (càng cao coin nhảy càng cao)
                float gravity = -5f;       // Trọng lực (âm để kéo coin xuống)
                float tMax = -2 * jumpForce / gravity; // Thời gian đạt đỉnh parabol
                float dz = 0.7f;          // Khoảng cách Z giữa các coin

                // Tạo 5 coin theo đường parabol
                for (int i = 0; i < 5; i++)
                {
                    Vector3 spawnPos = coinPosition;

                    // Chỉ coin thứ 2, 3, 4 (index 1,2,3) mới tạo hiệu ứng nhảy
                    if (i >= 1 && i <= 3)
                    {
                        // Tính toán thời gian t cho từng coin trong quỹ đạo parabol
                        float t = (i) / 4.0f * tMax;
                        // Công thức vật lý: y = v0*t + 0.5*g*t^2 (v0 = jumpForce, g = gravity)
                        float y = jumpForce * t + 0.5f * gravity * t * t;
                        spawnPos.y = 0.2f + y; // 0.2f là chiều cao cơ bản + độ cao parabol
                    }

                    GameObject coin = Instantiate(coinObj, spawnPos, coinObj.rotation).gameObject;
                    // Chỉ đặt vật cản phía dưới coin thứ 3 khi là coin vòng cung
                    if (i == 2 && (bricksObj != null || obstacleObj != null))
                    {
                        Vector3 belowCoin = spawnPos;
                        // 50% khả năng spawn brick, 50% khả năng spawn obstacle
                        if (Random.value < 0.5f && bricksObj != null)
                        {
                            belowCoin.y = 0f; // Chiều cao mặt đất cho brick
                            Instantiate(bricksObj, belowCoin, bricksObj.rotation);
                        }
                        else if (obstacleObj != null)
                        {
                            belowCoin.y = 0.1f; // Chiều cao nhỏ cho obstacle
                            Instantiate(obstacleObj, belowCoin, obstacleObj.rotation);
                        }
                    }
                    coinPosition.z += dz; // Di chuyển vị trí spawn coin tiếp theo
                }
            }
            else
            {
                // Coin thẳng hàng như bình thường, không có vật cản phía dưới
                Vector3 coinPosition = nextCoinSpawn;
                float dz = 0.7f; // Khoảng cách giữa các coin
                for (int i = 0; i < 5; i++)
                {
                    Vector3 spawnPos = coinPosition;
                    GameObject coin = Instantiate(coinObj, spawnPos, coinObj.rotation).gameObject;
                    coinPosition.z += dz; // Di chuyển đến vị trí coin tiếp theo
                }
            }
        }

        // Kiểm tra xem có đến lúc spawn power-up không (mỗi powerUpFrequency tile) và sử dụng tỷ lệ dựa trên độ khó
        if (tileCounter % powerUpFrequency == 0 && Random.value < currentPowerUpChance)
        {
            // Chọn ngẫu nhiên 1 trong 3 loại power-up: Thunder (tăng tốc), Time (giảm tốc), Invisible (vô hình)
            List<Transform> powerUps = new List<Transform>();
            if (speedUpObj != null) powerUps.Add(speedUpObj); // Thunder - power-up tăng tốc
            if (slowDownObj != null) powerUps.Add(slowDownObj); // Time - power-up giảm tốc
            if (invisiblePowerObj != null) powerUps.Add(invisiblePowerObj); // Invisible - power-up vô hình
            if (powerUps.Count > 0)
            {
                int idx = Random.Range(0, powerUps.Count); // Chọn ngẫu nhiên index
                Instantiate(powerUps[idx], nextPowerUpSpawn, powerUps[idx].rotation);
            }
        }

        // Spawn vật cản đặc biệt: freeze circle (vòng tròn đóng băng) với tỷ lệ thay đổi theo độ khó
        if (freezeCircleObj != null)
        {
            // Sử dụng currentFreezeChance thay vì tỷ lệ cố định 20%
            if (Random.value < currentFreezeChance)
            {
                int[] lanesForFreeze = { -1, 0, 1 };
                List<int> possibleLanes = new List<int>(lanesForFreeze);
                possibleLanes.Remove(coinLane); // Không spawn trùng lane với coin
                int freezeLane = possibleLanes[Random.Range(0, possibleLanes.Count)];
                Vector3 freezePos = nextTileSpawn;
                freezePos.x = freezeLane;
                freezePos.y = 0.1f;
                freezePos.z += 4.0f; // Đặt xa hơn coin/power-up/vật cản khác trên trục Z
                if (freezeCircleObj != null && Random.value < currentFreezeChance)
                {
                    Instantiate(freezeCircleObj, freezePos, freezeCircleObj.rotation);
                }
            }
        }

        nextTileSpawn.z += 3; // Di chuyển vị trí spawn tiếp theo

        // Tạo tile thứ hai mà không hủy
        GameObject tile2 = Instantiate(tileObj, nextTileSpawn, tileObj.rotation).gameObject;

        nextTileSpawn.z += 3; // Di chuyển vị trí spawn tiếp theo
        StartCoroutine(spawnTile()); // Tiếp tục spawn tile tiếp theo (đệ quy)
    }

    // Coroutine để tăng độ khó một cách tiến bộ
    IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRateIncreaseInterval); // Chờ theo interval tăng độ khó

            difficultyLevel++; // Tăng level độ khó

            // Giảm thời gian spawn (spawn nhanh hơn)
            currentSpawnDelay = Mathf.Max(minSpawnDelay, currentSpawnDelay - spawnDelayDecrease);

            // Tăng tỷ lệ spawn vật cản
            currentObstacleChance = Mathf.Min(maxObstacleChance, currentObstacleChance + obstacleChanceIncrease);

            // Giảm tỷ lệ spawn power-up (làm chúng hiếm hơn)
            currentPowerUpChance = Mathf.Max(minPowerUpChance, currentPowerUpChance - powerUpChanceDecrease);

            // Tăng tỷ lệ spawn freeze circle
            currentFreezeChance = Mathf.Min(maxFreezeChance, currentFreezeChance + freezeChanceIncrease);

            // Tăng tỷ lệ coin vòng cung (pattern coin khó khăn hơn)
            currentArcRate = Mathf.Min(maxArcRate, currentArcRate + arcRateIncrease);

            // Ghi log thông tin khi tăng level độ khó
            Debug.Log($"🔥 LEVEL ĐỘ KHÓ MỚI: {difficultyLevel} | Tốc độ: {currentSpawnDelay:F2}s | Vật cản: {(currentObstacleChance * 100):F0}%");


            // Thông báo mốc đặc biệt quan trọng
            if (difficultyLevel == 5)
            {
                Debug.Log("🏆 MỐC: Bạn đã sống sót qua 5 level độ khó!");
            }
            else if (difficultyLevel == 10)
            {
                Debug.Log("🔥 MỐC: Đạt level 10! Vùng nguy hiểm!");
            }
            else if (difficultyLevel == 15)
            {
                Debug.Log("⚡ MỐC: Level 15! Chỉ những người chơi giỏi nhất!");
            }
            else if (difficultyLevel % 20 == 0)
            {
                Debug.Log($"👑 MỐC HUYỀN THOẠI: Level {difficultyLevel}!");
            }
        }
    }

    // Hàm public để lấy level độ khó hiện tại
    public int GetCurrentDifficultyLevel()
    {
        return difficultyLevel;
    }

    // Hàm public để lấy thống kê độ khó hiện tại
    public string GetDifficultyStats()
    {
        return $"Level {difficultyLevel} | Tốc độ: {currentSpawnDelay:F2}s | Vật cản: {(currentObstacleChance * 100):F0}% | Power-ups: {(currentPowerUpChance * 100):F0}%";
    }
}