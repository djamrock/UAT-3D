using UnityEngine;



public class ShooterBullet : Shooter
{
    public Transform bulletSpawnPoint;

    public GameObject bulletPrefab;

    public AudioClip shootingSound;
    public AudioSource audioSource;



    public override void Shoot()
    {
        if (bulletSpawnPoint != null && bulletPrefab != null)
        {
            // create gameObject(object, position, rotation)
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation); // creates each bullet when shoot is presses. 

            if (shootingSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootingSound);
            }
        }
    }



    public override void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Update()
    {

    }
}

