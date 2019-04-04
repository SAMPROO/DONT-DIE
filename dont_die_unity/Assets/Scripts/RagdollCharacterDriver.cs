using System;
using UnityEngine;

public class RagdollCharacterDriver : MonoBehaviour
{
	[Serializable]
	private class ControlBone
	{
		public Rigidbody rigidbody;
		public Vector3 targetPosition;
		public float force;
	}

	[Header("Specs")]
	[SerializeField] private float jumpPower = 5f;
	[SerializeField] private float speed = 3f;
	[SerializeField] private float moveForce;
	[SerializeField] private float stabilityForce;
	[SerializeField] private float hipStabilityForce;


	[Header("Controlled parts")]
	[SerializeField] private ControlBone hip;
	[SerializeField] private ControlBone neck;
	[SerializeField] private ControlBone head;

	[SerializeField] private Rigidbody controlRb;

	Vector3 hipPosition => controlRb.position + hip.targetPosition;
	Vector3 headPosition => controlRb.position + neck.targetPosition;


	private void FixedUpdate()
	{
		hip.rigidbody.AddForce((hipPosition - hip.rigidbody.position) * hip.force);
		neck.rigidbody.AddForce((headPosition - neck.rigidbody.position) * neck.force);

		head.rigidbody.MoveRotation(Quaternion.Inverse(hip.rigidbody.rotation));
		// head.rigidbody.transform.localRotation = Quaternion.identity;
	}

	// Move character to direction, do this in fixed update
	public void Drive(Vector3 direction, float amount)
	{
		// Enforce this to be used only in fixed update
		if (Time.inFixedTimeStep == false)
		{
			Debug.LogError("Use RagdollCharacterDriver.Drive only in FixedUpdate");				
			return;
		}

		if (amount > 0)
		{
			amount *= speed;
			controlRb.MovePosition(controlRb.position + direction * amount);
		}
	}

	public void Jump()
	{
		// Todo: test if touching walkable perimeter, and only jump if do
		controlRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
		Debug.Log("Ragdoll do jump");
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;

		Gizmos.DrawWireSphere(hipPosition, 0.05f);
		Gizmos.DrawWireSphere(headPosition, 0.05f);

	}
}
