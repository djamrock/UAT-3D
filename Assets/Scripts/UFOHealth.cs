
using UnityEngine;
using UnityEngine.UI;

public class UFOHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int pointsForDestroy = 100;
    public Image healthBarFill;
    public AudioClip hitSound;
    public AudioSource audioSource;



    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        healthBarFill.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth == 0)
        {
            // Award points for destroying the UFO.
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(pointsForDestroy);
            }

            Destroy(gameObject);
        }
    }



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


}
