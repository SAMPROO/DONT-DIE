using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyInArch : MonoBehaviour
{
	private new Rigidbody rigidbody;

	public float speedThreshold = 2f;
	public float SqrSpeedThreshold => speedThreshold * speedThreshold;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		bool tooSlow = rigidbody.velocity.sqrMagnitude < SqrSpeedThreshold;
		if (tooSlow || rigidbody.isKinematic)
			return;

		var targetRotation = Quaternion.LookRotation(rigidbody.velocity);
		var rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f);
		rigidbody.MoveRotation(rotation);
	}
}