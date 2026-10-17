using UnityEngine;



public class ProjectileDamage : MonoBehaviour
{
    public int damageAmount = 1;



    private void OnCollisionEnter(Collision collision)
    {
        UFOHealth ufoHealth = collision.gameObject.GetComponent<UFOHealth>();

        if (ufoHealth != null)
        {
            ufoHealth.TakeDamage(25);
            Destroy(gameObject);
            return;
        }

        TargetHealth target = collision.gameObject.GetComponent<TargetHealth>();            // Checks to see if object has TargetHealth, applies bullet damage if so

        if (target != null)
        {
            target.TakeDamage(damageAmount);
            Destroy(gameObject);
        }

    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider bulletCollider = GetComponent<Collider>();

        GameObject ship = GameObject.Find("StarShipPawn");

        if (bulletCollider != null && ship != null)
        {
            Collider shipCollider = ship.GetComponent<Collider>();

            if (shipCollider != null)
            {
                Physics.IgnoreCollision(bulletCollider, shipCollider);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
