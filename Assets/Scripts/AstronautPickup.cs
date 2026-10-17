using UnityEngine;

public class AstronautPickup : MonoBehaviour
{
    public int pointsAwarded = 100;




    private void OnTriggerEnter(Collider other)
    {
        ShipHealth shipHealth = other.GetComponent<ShipHealth>();

        if (shipHealth != null)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(pointsAwarded);
            }

            Destroy(gameObject);
        }
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
