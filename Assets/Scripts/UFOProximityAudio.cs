using UnityEngine;



public class UFOProximityAudio : MonoBehaviour
{
    public PlayerController playerController;
    public AudioSource proximityAudio;

    public float maxDistance = 100f;
    public float minVolume = 0f;
    public float maxVolume = 1f;
    public float fadeSpeed = 2f;

    private float currentVolume = 0f;

    private void Start()
    {
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
        if (playerController == null || playerController.pawn == null || proximityAudio == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, playerController.pawn.transform.position);

        float volumePercent = 1f - Mathf.Clamp01(distance / maxDistance);

        float targetVolume = Mathf.Lerp(minVolume, maxVolume, volumePercent);

        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, fadeSpeed * Time.deltaTime);

        proximityAudio.volume = currentVolume;
    }
}
