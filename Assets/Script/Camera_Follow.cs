using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    public Transform target;            // Nhân vật cần theo dõi
    public float smoothSpeed = 0.125f;  // Tốc độ mượt di chuyển của camera
    
    // Thêm biến để giữ lại vị trí ban đầu của camera
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    // Offset sẽ được tính toán tự động dựa vào vị trí ban đầu
    private Vector3 offset;
      private void LateUpdate()
    {
        if (target == null)
        {
            // Không làm gì nếu không có target - giữ nguyên vị trí camera ban đầu
            return;
        }
        
        // Camera chỉ follow trục Z của nhân vật, giữ nguyên X và Y ban đầu
        Vector3 desiredPosition = new Vector3(
            initialPosition.x, // Giữ nguyên X
            initialPosition.y, // Giữ nguyên Y
            target.position.z + offset.z // Chỉ cập nhật Z
        );
        
        // Smoothly di chuyển camera đến vị trí mong muốn
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Giữ nguyên góc xoay ban đầu được thiết lập trong Editor
        transform.rotation = initialRotation;
    }    private void Start()
    {
        // Lưu vị trí và góc xoay ban đầu của camera (đã được set trong Editor)
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Tự động tìm Player nếu target chưa được thiết lập
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Camera_Follow: Đã tự động tìm thấy Player.");
            }
            else
            {
                Debug.LogWarning("Camera_Follow: Không tìm thấy GameObject với tag 'Player'. Vui lòng gán target trong Inspector.");
                return; // Thoát nếu không tìm thấy target
            }
        }
        
        // Tính toán offset dựa trên vị trí ban đầu của camera và vị trí hiện tại của target
        offset = initialPosition - target.position;
        
        Debug.Log("Camera offset được tính: " + offset);
    }
}
