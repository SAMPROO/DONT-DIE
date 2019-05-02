using UnityEngine;

public class RagdollHandGrab : MonoBehaviour
{
	public float radius = 0.2f;
	public Vector3 offset;

	private FixedJoint grabJoint;
	private SphereCollider grabTrigger;

	private void Start()
	{
		grabTrigger = gameObject.AddComponent<SphereCollider>();
		grabTrigger.radius = radius;
		grabTrigger.isTrigger = true;
		grabTrigger.center = offset;
	}

	public void SetGrab(bool value)
	{
		// If we release hold, destroy any joint
		if (value == false && grabJoint != null)
			Destroy(grabJoint);

		grabTrigger.enabled = value;
	}

	private void OnTriggerEnter(Collider other)
	{
		// Dont grab self or if already grabbing something
		if (other.transform.root == transform.root || grabJoint != null)
			return;

		// Also only grab rigidbodies now
		var otherRigidbody = other.GetComponent<Rigidbody>();
		if (otherRigidbody != null)
		{
			grabJoint = gameObject.AddComponent<FixedJoint>();
			grabJoint.connectedBody = otherRigidbody;
		}
	}

	private void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.TransformPoint(offset), radius);
	}
}