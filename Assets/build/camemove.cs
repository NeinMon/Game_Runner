using UnityEngine;

public class camemove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
