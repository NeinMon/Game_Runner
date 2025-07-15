using UnityEngine;

public class camera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float forwardSpeed = 5f;
    public float sideSpeed = 3f;
    
    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        rb.linearVelocity = new Vector3(0, 0, forwardSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from arrow keys or WASD
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput * sideSpeed, 0, forwardSpeed);
          // Apply movement to the rigidbody
        rb.linearVelocity = movement;
    }
}
