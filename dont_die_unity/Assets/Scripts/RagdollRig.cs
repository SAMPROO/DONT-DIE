/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollRig : MonoBehaviour
{
	[Serializable]
	public class ControlBone
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

		public void ControlWithTransform(Matrix4x4 localToWorld)
		{
			float currentForce = active ? force : 0f;

			Vector3 target = localToWorld.MultiplyPoint(targetPosition);

			rigidbody.AddForce ((target - rigidbody.position) * currentForce);
		}
	}

	private FixedJoint rightHandController;
	private FixedJoint leftHandController;


	[Header("Specs")]
	[SerializeField] private float jumpPower = 5f;
	[SerializeField] private float speed = 3f;

	[Header("Controlled parts")]
	public ControlBone hip;
	public ControlBone neck;
	public ControlBone head;
	public ControlBone rightHand;
	public ControlBone leftHand;

	private Rigidbody controlRb;

	Vector3 hipPosition => transform.position + hip.targetPosition;
	Vector3 headPosition => transform.position + neck.targetPosition;

	public Transform abdomen;

	private bool handsControlled;
	public void SetRightHandControl(bool value)
	{
		// do not set same value again
		if (handsControlled == value)
			return;

		handsControlled = value;

		if (value)
		{
			SetControllerActive(
				rightHandController,
				rightHand.rigidbody,
				transform.TransformPoint(rightHand.targetPosition)
			);

			SetControllerActive(
				leftHandController,
				leftHand.rigidbody,
				transform.TransformPoint(leftHand.targetPosition)
			);
		}

		else
		{
			rightHandController.connectedBody = null;
			leftHandController.connectedBody = null;
			rightHandController.gameObject.SetActive(value);
			leftHandController.gameObject.SetActive(value);
		}
	}

	private static void SetControllerActive(FixedJoint controller, Rigidbody controlledBody, Vector3 targetPosition)
	{
		controller.transform.position = controlledBody.transform.position;
		controller.transform.rotation = controlledBody.transform.rotation;
		controller.connectedBody = controlledBody;
		controller.gameObject.SetActive(true);
		controller.transform.position = targetPosition;
	}

	public bool Grounded { get; private set; }

	private void Awake()
	{
		controlRb = GetComponent<Rigidbody>();

		rightHandController = CreateJointController("rightHandController");
		rightHandController.transform.SetParent(hip.rigidbody.transform);

		leftHandController = CreateJointController("leftHandController");
		leftHandController.transform.SetParent(hip.rigidbody.transform);

	}

	[Range(0f, 1f)] public float hipDamping = 0.5f;

	private void FixedUpdate()
	{
		hip.rigidbody.AddForce((hipPosition - hip.rigidbody.position) * hip.force);
		neck.rigidbody.AddForce((headPosition - neck.rigidbody.position).y * Vector3.up * neck.force);

		leftHandController.transform.rotation = Quaternion.LookRotation(transform.forward);
		rightHandController.transform.rotation = Quaternion.LookRotation(transform.forward);
	}

	private FixedJoint CreateJointController(string name)
	{
		var joint = new GameObject(name, typeof(FixedJoint)).GetComponent<FixedJoint>();
		joint.GetComponent<Rigidbody>().isKinematic = true;
		joint.gameObject.SetActive(false);
		return joint;
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
	public void Move(Vector3 direction, float amount)
	{
		// Enforce this to be used only in fixed update
		if (Time.inFixedTimeStep == false)
		{
			Debug.LogError("Use RagdollRig.Move only in FixedUpdate");				
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
		Debug.DrawRay (hipPosition, hip.rigidbody.transform.forward * 2.5f, Color.cyan);
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

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(rightHand.targetPosition), 0.05f);
		Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(leftHand.targetPosition), 0.05f);

	}

	public static Transform [] GetRecursiveChildren(Transform parent)
	{
		var children = new List<Transform>();

		int childCount = parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			var child = parent.GetChild(i);
			children.Add(child);
			children.AddRange(GetRecursiveChildren(child));
		}

		return children.ToArray();
	}
}
