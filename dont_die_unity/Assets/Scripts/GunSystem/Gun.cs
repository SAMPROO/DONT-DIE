using UnityEngine;

public class Gun : BaseGun
{
    public Projectile projectilePrefab;
    public Vector3 spawnOffset;
    public Vector3 projectileRotationOffset;
    private Quaternion projectileRotationOffset2;

    public float startSpeed = 10;
    public float startAngle = 0;

    // Used to limit the firerate so player can't spam fire
    public float roundPerSecond = 3;
    private float secondsPerRound;
    private float lastFiredTime;

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
        if (Ammo > 0 && secondsPerRound - (Time.time - lastFiredTime) <= 0/* && !isReeling*/)
        {
            var projectile = Instantiate(
                projectilePrefab,
                transform.position + Quaternion.LookRotation(transform.forward, transform.up) * spawnOffset,
                transform.rotation * projectileRotationOffset2
            );
            projectile.Launch();
            lastFiredTime = Time.time;
            Ammo--;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Draw a sphere at the projectile spawn position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Quaternion.LookRotation(transform.forward, transform.up) * spawnOffset, .05f);
    }
}