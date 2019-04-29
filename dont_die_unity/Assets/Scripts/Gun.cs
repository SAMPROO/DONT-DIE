﻿using UnityEngine;

public class Gun : Equipment
{
    public GameObject projectilePrefab;
    public Vector3 spawnOffset;
    public Vector3 projectileRotationOffset;
    private Quaternion projectileRotationOffset2;

    public float startSpeed = 10;
    public float startAngle = 0;

    // Used to limit the firerate so player can't spam fire
    public float roundPerSecond = 3;

    //[Header("Reuseable aka projectiles are reeled back in")]
    //public bool reuseable;
    //public int maxNodeCount;
    //private int nodeCount;
    //private bool isReeling;

    private float secondsPerRound;
    private float time;

    private GameObject projectile;

    //Sounds
    private AudioSource audioSrc;
    public AudioClip[] gunSounds;
    public AudioClip[] noAmmoSounds;

    private void Start()
    {
        secondsPerRound = 1f / roundPerSecond;
        audioSrc = GetComponent<AudioSource>();
        if (gunSounds.Length < 1)
            Debug.Log("No sounds");
        projectileRotationOffset2 = Quaternion.Euler(projectileRotationOffset.x, projectileRotationOffset.y, projectileRotationOffset.z);
    }

    public override void Use()
    {
        if (Ammo > 0 && secondsPerRound - (Time.time - time) <= 0/* && !isReeling*/)
        {
            projectile = Instantiate(
                projectilePrefab,
                transform.position + Quaternion.LookRotation(transform.forward, transform.up) * spawnOffset,
                transform.rotation * projectileRotationOffset2
            );
            projectile.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(startAngle, transform.right) * transform.forward * startSpeed;
            AudioClip selectedClip = gunSounds[Random.Range(0, gunSounds.Length)];
            audioSrc.PlayOneShot(selectedClip);
            time = Time.time;
            Ammo--;

            //isReeling = reuseable;

            // Debug.Log("Gun says \"Bang!\"");
        }
        //else if (reuseable && isReeling && projectile != null)
        //{
            
        //}
        // else
        // {
        //     Debug.Log("Gun says \"Click!\"");
        // }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Draw a sphere at the projectile spawn position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Quaternion.LookRotation(transform.forward, transform.up) * spawnOffset, .05f);
    }
}