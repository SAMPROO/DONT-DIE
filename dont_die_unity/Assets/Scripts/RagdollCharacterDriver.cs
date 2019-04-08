/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollCharacterDriver : MonoBehaviour
{
	[Serializable]
	private class ControlBone
	{
		public bool active = true;
		public Rigidbody rigidbody;
		public Vector3 targetPosition;
		public float force;

		// transformPosition is drivers current position, it is used as an base offset
		public void ControlWithOffset(Vector3 transformPosition)
		{
			float currentForce = active ? force : 0f;
			rigidbody.AddForce(
				(transformPosition + targetPosition - rigidbody.position) * currentForce
			);
		}
	}

	[Header("Specs")]
	[SerializeField] private float jumpPower = 5f;
	[SerializeField] private float speed = 3f;

	[Header("Controlled parts")]
	[SerializeField] private ControlBone hip;
	[SerializeField] private ControlBone neck;
	[SerializeField] private ControlBone head;
	[SerializeField] private ControlBone rightHand;

	private Rigidbody controlRb;

	Vector3 hipPosition => controlRb.position + hip.targetPosition;
	Vector3 headPosition => controlRb.position + neck.targetPosition;

	public Transform abdomen;

	public bool ControlRightHand
	{
		get => rightHand.active;
		set => rightHand.active = value;
	}

	public bool Grounded { get; private set; }

	private void Awake()
	{
		controlRb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		hip.rigidbody.AddForce((hipPosition - hip.rigidbody.position) * hip.force);
		neck.rigidbody.AddForce((headPosition - neck.rigidbody.position) * neck.force);

		// Sometimes inverse works. What is going on here?
		head.rigidbody.MoveRotation(hip.rigidbody.rotation);
		// head.rigidbody.MoveRotation(Quaternion.Inverse(hip.rigidbody.rotation));

		rightHand.ControlWithOffset(controlRb.position);
	}

	// TODO: Orient dude towards move direction

	private void OnCollisionEnter(Collision collision)
	{
		Grounded = true;	
	}

	private void OnCollisionExit()
	{
		Grounded = false;
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
			controlRb.MoveRotation(Quaternion.LookRotation(direction));

		}
		hip.rigidbody.MoveRotation(controlRb.rotation);

		Debug.DrawRay (transform.position + Vector3.up, transform.forward * 2.5f, Color.red);
		Debug.DrawRay (transform.position + Vector3.up, hip.rigidbody.transform.forward * 2.5f, Color.cyan);
		Debug.DrawRay (abdomen.position, abdomen.forward * 2.5f, Color.green);
	}

	public void Jump()
	{
		// Todo: test if touching walkable perimeter, and only jump if do
		if (Grounded == false)
			return;

		controlRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
		Debug.Log("Jump");
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;

		Gizmos.DrawWireSphere(hipPosition, 0.05f);
		Gizmos.DrawWireSphere(headPosition, 0.05f);
		Gizmos.DrawWireSphere(controlRb.position + rightHand.targetPosition, 0.05f);

	}
}
