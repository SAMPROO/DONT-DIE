using UnityEngine;

public class ProjectileLauncher : MonoBehaviour, IWeapon
{
    public Transform projectileOrigin;
    public GameObject projectilePrefab;

    public float initialVelocity;
    public float accuracy;
    public float lifetime;

    public float roundPerSecond = 3;
    private float secondsPerRound;

    public int ammo = 30;

    [Space]
    public bool debug;
    public int resolution;
    public float reticleDistance;
    public LayerMask reticleRaycastMask;

    private float time;

    private Rigidbody rb;

    private bool isCarried;

    private void Start()
    {
        secondsPerRound = 1f / roundPerSecond;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (debug && isCarried)
        {
            AccuracyReticle(resolution, reticleDistance);
        }
    }

    public void Use()
	{
        if (ammo > 0 && secondsPerRound - (Time.time - time) <= 0)
        {
            Debug.Log("Gun says \"Bang!\"");

            GameObject bullet = Instantiate(projectilePrefab, projectileOrigin.position, Quaternion.identity);

            Vector3 direction = (Vector3)Random.insideUnitCircle + Vector3.forward * accuracy;
            direction = Quaternion.FromToRotation(Vector3.forward, projectileOrigin.forward) * direction;

            bullet.GetComponent<Rigidbody>().velocity = direction.normalized * initialVelocity;

            Destroy(bullet, lifetime);

            time = Time.time;

            ammo--;

            if (ammo == 0) Debug.Log("Gun says \"I'm out!\"");
        }
        else
        {
            Debug.Log("Gun says \"Click!\"");
        }

        // Return true if there was enough ammo, so shooter can react
        // maybe return eg. Vector3? representing recoil

        // or just add force from projectileOrigin backwards if shot and when ammo==0: play a sound "click" 
    }

    public void StartCarrying(Transform carrier)
	{
		// Turn off physics etc.
		transform.SetParent(carrier);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rb.isKinematic = true;

        isCarried = true;

        Debug.Log("Gun hops on");
	}

	public void StopCarrying()
	{
		// Turn on physics etc.
		transform.SetParent(null);
        rb.isKinematic = false;

        isCarried = false;

        Debug.Log("Gun thrown away");
	}

    private void AccuracyReticle(int CircleVertexCount, float reticleDistance)
    {
        float segmentWidth = Mathf.PI * 2f / CircleVertexCount;
        float angle = 0f;
        float distanceMultiplier = reticleDistance / accuracy;

        Vector3 v0, v1, r0, r1;

        v0 = GetPointOnCircle(angle, distanceMultiplier);
        r0 = Raycast(v0);

        for (int i = 1; i < CircleVertexCount + 1; i++)
        {
            angle -= segmentWidth;

            v1 = GetPointOnCircle(angle, distanceMultiplier);
            r1 = Raycast(v1);

            Debug.DrawLine(v0, v1, Color.green);
            Debug.DrawLine(r0, r1, Color.blue);

            v0 = v1;
            r0 = r1;
        }
    }

    private Vector3 GetPointOnCircle(float angle, float distanceMultiplier)
    {
        Vector3 vertex = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        vertex += Vector3.up * accuracy;
        vertex = Quaternion.FromToRotation(Vector3.up, projectileOrigin.forward) * vertex;
        vertex = distanceMultiplier * vertex;
        vertex += projectileOrigin.position;

        return vertex;
    }

    private Vector3 Raycast(Vector3 vertex)
    {
        Vector3 origin = projectileOrigin.position;
        Vector3 direction = (vertex - projectileOrigin.position).normalized;

        float distance = initialVelocity * lifetime;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, reticleRaycastMask))
        {
            return hit.point;
        }

        return origin + direction * distance;
    }
}