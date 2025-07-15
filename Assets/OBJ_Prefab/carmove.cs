using UnityEngine;

public class carmove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Obsolete]
    void Start()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -3.5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
