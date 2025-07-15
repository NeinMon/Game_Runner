using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    public float speedMultiplier = 2.0f;  // Tăng tốc gấp đôi
    public float duration = 5.0f;         // Hiệu ứng kéo dài 5 giây
    public AudioClip pickupSound;         // Âm thanh khi nhặt
    
    private void Start()
    {
        // Tự xoay để tạo hiệu ứng hấp dẫn
        StartCoroutine(RotatePowerUp());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu player va chạm
        if (other.CompareTag("Player"))
        {
            // Kích hoạt hiệu ứng tăng tốc
            ActivateSpeedBoost(other.gameObject);
            
            // Phát âm thanh
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Xóa power-up
            Destroy(gameObject);
        }
    }
    
    private void ActivateSpeedBoost(GameObject player)
    {
        // Tìm script di chuyển của player (có thể là camera.cs hoặc player movement)
        camera playerMovement = player.GetComponent<camera>();
        if (playerMovement != null)
        {
            StartCoroutine(ApplySpeedBoost(playerMovement));
        }
    }
      private System.Collections.IEnumerator ApplySpeedBoost(camera playerMovement)
    {
        // Lưu tốc độ gốc (nếu chưa được khởi tạo thì đặt giá trị mặc định)
        float originalForwardSpeed = playerMovement.forwardSpeed != 0 ? playerMovement.forwardSpeed : 5f;
        float originalSideSpeed = playerMovement.sideSpeed != 0 ? playerMovement.sideSpeed : 3f;

        // Áp dụng tăng tốc
        playerMovement.forwardSpeed = originalForwardSpeed * speedMultiplier;
        playerMovement.sideSpeed = originalSideSpeed * speedMultiplier;
        
        // Đợi hết thời gian hiệu ứng
        yield return new WaitForSeconds(duration);
        
        // Khôi phục tốc độ gốc
        playerMovement.forwardSpeed = originalForwardSpeed;
        playerMovement.sideSpeed = originalSideSpeed;
    }
    
    private System.Collections.IEnumerator RotatePowerUp()
    {
        while (true)
        {
            transform.Rotate(0, 90 * Time.deltaTime, 0);
            yield return null;
        }
    }
}
