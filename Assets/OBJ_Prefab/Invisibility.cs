using UnityEngine;

public class Invisibility : MonoBehaviour
{
    [Header("Invisibility Settings")]
    public float duration = 4.0f;         // Hiệu ứng kéo dài 4 giây
    public AudioClip pickupSound;         // Âm thanh khi nhặt
    public Material invisibilityMaterial; // Material trong suốt
    
    private void Start()
    {
        // Tự xoay và nhấp nháy để tạo hiệu ứng hấp dẫn
        StartCoroutine(RotateAndBlink());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu player va chạm
        if (other.CompareTag("Player"))
        {
            // Kích hoạt hiệu ứng tàng hình
            ActivateInvisibility(other.gameObject);
            
            // Phát âm thanh
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Xóa power-up
            Destroy(gameObject);
        }
    }
    
    private void ActivateInvisibility(GameObject player)
    {
        StartCoroutine(ApplyInvisibility(player));
    }
    
    private System.Collections.IEnumerator ApplyInvisibility(GameObject player)
    {
        // Lưu trữ các renderer và material gốc
        Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
        Material[][] originalMaterials = new Material[renderers.Length][];
        
        // Lưu material gốc và áp dụng tàng hình
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
            
            // Tạo material trong suốt
            Material[] invisibleMaterials = new Material[renderers[i].materials.Length];
            for (int j = 0; j < invisibleMaterials.Length; j++)
            {
                invisibleMaterials[j] = new Material(renderers[i].materials[j]);
                
                // Làm trong suốt
                if (invisibleMaterials[j].HasProperty("_Color"))
                {
                    Color color = invisibleMaterials[j].color;
                    color.a = 0.3f; // 30% độ trong suốt
                    invisibleMaterials[j].color = color;
                }
                
                // Chuyển sang rendering mode trong suốt
                invisibleMaterials[j].SetFloat("_Mode", 2);
                invisibleMaterials[j].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                invisibleMaterials[j].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                invisibleMaterials[j].SetInt("_ZWrite", 0);
                invisibleMaterials[j].DisableKeyword("_ALPHATEST_ON");
                invisibleMaterials[j].EnableKeyword("_ALPHABLEND_ON");
                invisibleMaterials[j].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                invisibleMaterials[j].renderQueue = 3000;
            }
            
            renderers[i].materials = invisibleMaterials;
        }
        
        // Tạm thời vô hiệu hóa va chạm với obstacles (tuỳ chọn)
        Collider playerCollider = player.GetComponent<Collider>();
        bool originalIsTrigger = false;
        if (playerCollider != null)
        {
            originalIsTrigger = playerCollider.isTrigger;
            playerCollider.isTrigger = true; // Đi xuyên qua vật cản
        }
        
        // Đợi hết thời gian hiệu ứng
        yield return new WaitForSeconds(duration);
        
        // Khôi phục material gốc
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
        
        // Khôi phục va chạm
        if (playerCollider != null)
        {
            playerCollider.isTrigger = originalIsTrigger;
        }
    }
    
    private System.Collections.IEnumerator RotateAndBlink()
    {
        Renderer renderer = GetComponent<Renderer>();
        float blinkTimer = 0f;
        
        while (true)
        {
            // Xoay
            transform.Rotate(0, 120 * Time.deltaTime, 0);
            
            // Nhấp nháy
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= 0.5f)
            {
                renderer.enabled = !renderer.enabled;
                blinkTimer = 0f;
            }
            
            yield return null;
        }
    }
}
