using UnityEngine;

public class camemove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float laneDistance = 1f; // Đảm bảo giống với Player_Controller
        int randomLane = Random.Range(0, 3); // 0, 1, 2
        float x = (randomLane - 1) * laneDistance; // -1, 0, 1
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Car đứng yên, không di chuyển
    }
}
