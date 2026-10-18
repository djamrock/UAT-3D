using UnityEngine;



public class UFOProximityAudio : MonoBehaviour
{
    private PlayerController playerController;
    public AudioSource proximityAudio;

    public float maxDistance = 100f;
    public float minVolume = 0f;
    public float maxVolume = 1f;
    public float fadeSpeed = 2f;

    private float currentVolume = 0f;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            playerController = GameManager.instance.playerController;
        }

        if (proximityAudio == null)
        {
            proximityAudio = GetComponent<AudioSource>();
        }

        if (proximityAudio != null)
        {
            proximityAudio.loop = true;
            proximityAudio.volume = 0f;
            proximityAudio.Play();
        }
    }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.gameEnded)                                 // Stop the hum if the game has ended
        {
            StopHum();
            return;
        }
            
        if (playerController == null || playerController.pawn == null || proximityAudio == null)            // Make sure the player and audio source exist
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, playerController.pawn.transform.position);        // Calculate distance between the UFO and player

        float volumePercent = 1f - Mathf.Clamp01(distance / maxDistance);                                       // Calculate target volume based on distance

        float targetVolume = Mathf.Lerp(minVolume, maxVolume, volumePercent);

        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, fadeSpeed * Time.deltaTime);             // Smoothly adjust the volume

        proximityAudio.volume = currentVolume;


    }


    public void StopHum()
    {
        if (proximityAudio != null && proximityAudio.isPlaying)
        {
            proximityAudio.Stop();
        }
    }
}
