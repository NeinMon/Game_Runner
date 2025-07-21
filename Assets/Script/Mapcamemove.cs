using UnityEngine;
using System.Collections;

public class Mapcamemove : MonoBehaviour
{
    public float initialSpeed = 3f;
    public float speedIncreaseAmount = 0.5f;
    public float speedIncreaseInterval = 15f;
    private Rigidbody rb;
    private float currentSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = initialSpeed;

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, 0, currentSpeed);

            // Start the coroutine to increase speed every 15 seconds
            StartCoroutine(IncreaseSpeedOverTime());
        }
        else
        {
            Debug.LogWarning("Rigidbody component is missing on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Coroutine to increase speed periodically
    IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(speedIncreaseInterval);

            // Increase the speed
            currentSpeed += speedIncreaseAmount;

            // Apply the new speed if we have a Rigidbody
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0, 0, currentSpeed);
                Debug.Log("Speed increased to: " + currentSpeed);
            }
        }
    }
}
