using UnityEngine;

public class PlayerLives : MonoBehaviour
{

    public GameObject[] lifeIcons;  // variable set as an array to have multiple values (the life icons in the top right), the [] makes it an array

    public int lives = 3;

    public GameObject playerPawnPrefab;

    public AsteroidSpawner asteroidSpawner;

    public float respawnDelay = 1.5f;

    public GameObject gameplayAudioListener;

    public AudioListener cameraAudioListener;

    public PlayerController playerController;

    public GameObject GameOverScreenStateObject;


    public void UpdateLivesUI()         // Updates the life icons in the top right of the gameplay UI
    {
        if (lifeIcons == null)          
        {
            return;
        }

        for (int i = 0; i < lifeIcons.Length; i += 1)       // Loops through each life icon, activates the icon if the position in the array is less than the number of lives remaining
        {                                                   // and hides the icon if the life has been lost
            if (lifeIcons[i] != null)                       // Life 1, 2 and 3 are assigned in the Inspector, number of lives are editable in Inpsector, just have to add more Icons if more lives are added
            {
                lifeIcons[i].SetActive(i < lives);
            }
        }
    }
    public void HandlePlayerDeath()                     // Runs when the player pawn is destroyed, removes a life and decides whether the player should respawn or the game should end
    {
        if (GameManager.instance.gameEnded)
        {
            return;
        }

        lives = Mathf.Max(0, lives - 1);            // Decreases lives by 1. Math.Max compares 1st # to 2nd # , returns larger (prevents lives going negative)

        UpdateLivesUI();                            // Calls above to update UI

        if (playerController != null)               // Clears the controller while pawn is dead waiting respawn so no errors occur
        {
            playerController.pawn = null;
        }

        if (asteroidSpawner != null)                // Clears the ships position after its destroyed while waiting to respawn so spawning asteroids arent using the old location
        {
            asteroidSpawner.starShipPawn = null;
        }

        if (lives <= 0)                                         // If player has no lives left, game over happens
        {
            GameManager.instance.gameEnded = true;
            GameManager.instance.ActivateGameOverScreen();      // Activates Game Over Screen
            Time.timeScale = 0f;                                // Pauses game time, essentially everything gameplay wise stops
        }
       
        else                                                                       // If lives still exist, player respawns. Delay is editable in Inspector
        {
            Invoke(nameof(RespawnPlayer), respawnDelay);
        }
    }



    public void RespawnPlayer()
    {
        // Check that the player prefab is assigned.
        if (playerPawnPrefab == null)
        {
            Debug.LogError("Player Pawn Prefab is not assigned!");
            return;
        }

        // Check that the player controller is assigned.
        if (playerController == null)
        {
            Debug.LogError("Player Controller is not assigned!");
            return;
        }

        // Set the ship's respawn position in the 3D world.
        Vector3 spawnPosition = new Vector3(0f, 3f, 0f);

        // Create a new ship at the spawn position.
        GameObject newShip = Instantiate(
            playerPawnPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Get the Pawn component from the new ship.
        Pawn newPawn = newShip.GetComponent<Pawn>();

        // Check that the new ship has a Pawn component.
        if (newPawn == null)
        {
            Debug.LogError("Player Pawn Prefab must have a Pawn component!");
            Destroy(newShip);
            return;
        }

        // Give the new ship to the player controller.
        playerController.pawn = newPawn;

        // Update the camera to follow the new ship.
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (cameraFollow != null)
        {
            cameraFollow.target = newShip.transform;
        }

        Debug.Log("Player respawned successfully!");
    }


    public void ClearExistingPlayerShips()
    {
        StarShipPawn[] existingShips = FindObjectsByType<StarShipPawn>(FindObjectsInactive.Include);

        foreach (StarShipPawn ship in existingShips)
        {
            if (ship != null)
            {
                Destroy(ship.gameObject);
            }
        }

        if (playerController != null)
        {
            playerController.pawn = null;
        }

    }
}
