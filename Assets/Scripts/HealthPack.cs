using UnityEngine;

public class HealthPack : MonoBehaviour
{

    public int healthAmount = 25;



    private void OnTriggerEnter(Collider other)
    {
        ShipHealth shipHealth = other.GetComponent<ShipHealth>();

        if (shipHealth != null)
        {
            shipHealth.RestoreHealth(healthAmount);

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
