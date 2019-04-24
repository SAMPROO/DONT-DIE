using UnityEngine;

public class Gun : Equipment
{
    public GameObject projectilePrefab;
    public Vector3 spawnOffset;

    public float startSpeed = 10;
    public float startAngle = 0;

    public int ammo = 3;

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

    private void Start()
    {
        secondsPerRound = 1f / roundPerSecond;
    }

    public override void Use()
    {
        if (ammo > 0 && secondsPerRound - (Time.time - time) <= 0/* && !isReeling*/)
        {
            projectile = Instantiate(
                projectilePrefab,
                transform.position + Quaternion.LookRotation(transform.forward, transform.up) * spawnOffset,
                transform.rotation
            );
            projectile.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(startAngle, transform.right) * transform.forward * startSpeed;

            time = Time.time;
            ammo--;

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