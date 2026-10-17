using UnityEngine;




public class ShipDeath : MonoBehaviour
{


    public void Die()
    {
        Debug.Log("The ship has been destroyed!");

        // Tell the GameManager that the player has died.
        if (GameManager.instance != null)
        {
            PlayerLives playerLives = GameManager.instance.GetComponent<PlayerLives>();

            if (playerLives != null)
            {
                playerLives.HandlePlayerDeath();
            }

            else
            {
                Debug.LogError("PlayerLives component is missing!");
            }
        }

        // Disable the ship after handling its death.
        gameObject.SetActive(false);
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
