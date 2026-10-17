using UnityEngine;

public class FloatingObject : MonoBehaviour
{

    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    private Vector3 startingPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.position;      //  remembers where the barrel began
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startingPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;      // creates a smooth wave that the barrel moves between poistive and negative values

        transform.position = new Vector3(startingPosition.x, newY, startingPosition.z);         // X and Z stay the same, barrel moves between Y values in newY
    }
}
