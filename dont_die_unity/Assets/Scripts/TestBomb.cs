using UnityEngine;

public class TestBomb : MonoBehaviour
{
	public bool doExplode = false;
	public float power = 100;
	public float radius = 5f;
	public LayerMask ragdollMask = 0;

	private Collider [] colliders = new Collider [30];

	private void Update()
	{
		if (doExplode == false) return;

		doExplode = false;

		int collisionCount = Physics.OverlapSphereNonAlloc(
				transform.position,
				radius,
				colliders,
				ragdollMask
			);

		Debug.Log($"Exploded, collisionCount {collisionCount}");

		for (int i = 0; i < collisionCount; i ++)
		{
			Debug.Log($"Added force to {name}");
			var rb = colliders[i].GetComponent<Rigidbody>();
			if (rb != null)
				rb.AddForce((rb.position - transform.position).normalized * power, ForceMode.VelocityChange);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color (1f, 0.2f, 0f, 0.5f);
		Gizmos.DrawSphere(transform.position, radius);
	}

}