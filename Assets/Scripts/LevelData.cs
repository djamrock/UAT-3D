using System.Collections.Generic;
using UnityEngine;



public class LevelData : MonoBehaviour
{
    public string levelName = "Test Level";

    public Vector3 levelSize = new Vector3(100f, 100f, 100f);


    // Player Movement Limit
    public float maxYPosition = 20f;
    public Transform player;

    // Spawn Points
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    public int numberOfEntitiesToSpawn = 3;
    private List<GameObject> spawnedEntities = new List<GameObject>();


    private void Awake()
    {
        SpawnPoint[] foundSpawnPoints = GetComponentsInChildren<SpawnPoint>();

        spawnPoints = new List<SpawnPoint>(foundSpawnPoints);

        Debug.Log("Found " + spawnPoints.Count + " spawn points.");
    }


    void Start()
    {
        SpawnEntities();
    }




    void SpawnEntities()
    {
        List<SpawnPoint> availablePoints = new List<SpawnPoint>(spawnPoints);       // Creates a copy of the spawn points so we can remove used points without changing the original list
        List<GameObject> spawnedPrefabs = new List<GameObject>();                   // Creates an empty list to keep track of which prefab types have been spawned

        // Spawn one of each entity type.
        foreach (SpawnPoint spawnPoint in spawnPoints)                  // Goes through each spawn point in the original spawn point list
        {
            if (spawnPoint.entityPrefab == null) continue;              // Checks if this spawn point has a prefab assigned; skips it if it does not

            // Skip this type if we already spawned it.
            if (spawnedPrefabs.Contains(spawnPoint.entityPrefab)) continue;        // Checks if this prefab type has already been spawned; skips it if it has

            // Find all spawn points for this type.
            List<SpawnPoint> matchingPoints = new List<SpawnPoint>();       // Creates an empty list to hold spawn points that use the current prefab

            foreach (SpawnPoint point in availablePoints)                   // Goes through each spawn point that is still available
            {
                if (point.entityPrefab == spawnPoint.entityPrefab)          // Checks if this point uses the same prefab as the current spawn point
                {
                    matchingPoints.Add(point);                              // Adds this point to the list of matching spawn points
                }
            }

            // Choose a random location for this type.
            if (matchingPoints.Count > 0)                                   // Checks if at least one matching spawn point was found
            {
                int randomIndex = Random.Range(0, matchingPoints.Count);    // Picks a random index from the matching spawn points list
                SpawnPoint selectedPoint = matchingPoints[randomIndex];     // Gets the spawn point at the randomly selected index

                GameObject spawnedEntity = Instantiate(selectedPoint.entityPrefab, selectedPoint.transform.position, selectedPoint.transform.rotation);        // Creates the selected prefab at the spawn point's position and rotation

                spawnedEntities.Add(spawnedEntity);

                availablePoints.Remove(selectedPoint);                      // Removes the selected point so it cannot be used again
                spawnedPrefabs.Add(spawnPoint.entityPrefab);                // Records this prefab type so another is not guaranteed in the first loop
            }
        }

        // Spawn any remaining entities at random unused points.
        int remainingEntities = numberOfEntitiesToSpawn - spawnedPrefabs.Count;         // Calculates how many more entities are needed to reach the requested total

        for (int i = 0; i < remainingEntities; i++)                                     // Repeats the loop once for each additional entity needed
        {   
            if (availablePoints.Count == 0) break;                                      // Stops the loop if there are no unused spawn points left

            int randomIndex = Random.Range(0, availablePoints.Count);                   // Picks a random index from the remaining spawn points
            SpawnPoint selectedPoint = availablePoints[randomIndex];                    // Gets the spawn point at the randomly selected index

            availablePoints.RemoveAt(randomIndex);                                      // Removes the selected point so it cannot be used again

            if (selectedPoint.entityPrefab != null)                                     // Checks if the selected point has a prefab assigned
            {
                GameObject spawnedEntity = Instantiate(selectedPoint.entityPrefab, selectedPoint.transform.position, selectedPoint.transform.rotation);        // Creates the selected prefab at the spawn point's position and rotation

                spawnedEntities.Add(spawnedEntity);
            }
        }
    }

    public void ResetEntities()
    {
        foreach (GameObject entity in spawnedEntities)
        {
            if (entity != null)
            {
                Destroy(entity);
            }
        }

        spawnedEntities.Clear();

        SpawnEntities();
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (player.position.y > maxYPosition)
        {
            Vector3 newPosition = player.position;
            newPosition.y = maxYPosition;

            player.position = newPosition;

            Rigidbody playerRb = player.GetComponent<Rigidbody>();

            if (playerRb != null && playerRb.linearVelocity.y > 0f)
            {
                Vector3 velocity = playerRb.linearVelocity; velocity.y = 0f; playerRb.linearVelocity = velocity;
            }
        }

    }



}

