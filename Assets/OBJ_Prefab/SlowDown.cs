using UnityEngine;

public class SlowDown : MonoBehaviour
{
    [Header("Slow Down Settings")]
    public float speedMultiplier = 0.5f;  // Giảm tốc xuống 50%
    public float duration = 3.0f;         // Hiệu ứng kéo dài 3 giây
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
            // Kích hoạt hiệu ứng giảm tốc
            ActivateSlowDown(other.gameObject);
            
            // Phát âm thanh
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Xóa power-up
            Destroy(gameObject);
        }
    }
      private void ActivateSlowDown(GameObject player)
    {
        // Tìm script di chuyển của player
        camera playerMovement = player.GetComponent<camera>();
        if (playerMovement != null)
        {
            StartCoroutine(ApplySlowDown(playerMovement));
        }
    }
    
    private System.Collections.IEnumerator ApplySlowDown(camera playerMovement)
    {
        // Lưu tốc độ gốc (nếu chưa được khởi tạo thì đặt giá trị mặc định)
        float originalForwardSpeed = playerMovement.forwardSpeed != 0 ? playerMovement.forwardSpeed : 5f;
        float originalSideSpeed = playerMovement.sideSpeed != 0 ? playerMovement.sideSpeed : 3f;
        
        // Áp dụng giảm tốc
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
            transform.Rotate(0, -90 * Time.deltaTime, 0);
            yield return null;        }
    }
}