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

	private Joint leftHandController;
	private Joint rightHandController;

	[SerializeField] private RagdollFootControl leftFoot;
	[SerializeField] private RagdollFootControl rightFoot;

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
	Vector3 neckPosition => transform.position + neck.targetPosition;

	public Transform abdomen;

	private bool handsControlled;
	public void SetHandControl(bool value)
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

	private static void SetControllerActive(Joint controller, Rigidbody controlledBody, Vector3 targetPosition)
	{
		controller.transform.position = controlledBody.transform.position;
		controller.transform.rotation = controlledBody.transform.rotation;
		controller.connectedBody = controlledBody;
		controller.gameObject.SetActive(true);
		controller.transform.position = targetPosition;
	}

	// public bool Grounded { get; private set; }
	public bool Grounded => leftFoot.Grounded || rightFoot.Grounded;

	private void Awake()
	{
		controlRb = GetComponent<Rigidbody>();

		rightHandController = CreateJointController<FixedJoint>("rightHandController");
		rightHandController.transform.SetParent(hip.rigidbody.transform);

		leftHandController = CreateJointController<FixedJoint>("leftHandController");
		leftHandController.transform.SetParent(hip.rigidbody.transform);
	}

	public float neckDistanceFromHip = 0.5f;

	private void FixedUpdate()
	{
		// Only hold hip and neck if Grounded
		// TODO: make neck target relative to hip instead of ground
		if (Grounded)
		{
			hip.rigidbody.AddForce((hipPosition - hip.rigidbody.position) * hip.force);
			// neck.rigidbody.AddForce((neckPosition - neck.rigidbody.position).y * Vector3.up * neck.force);
		}

		// Vector3 neckTargetPosition = hip.rigidbody.position + hip.rigidbody.transform.up * neckDistanceFromHip;
		// neck.rigidbody.AddForce((neckTargetPosition - neck.rigidbody.position) * neck.force);

		leftHandController.transform.rotation = Quaternion.LookRotation(transform.forward);
		rightHandController.transform.rotation = Quaternion.LookRotation(transform.forward);
	}

	private static JointType CreateJointController<JointType>(string name) where JointType : Joint
	{
		JointType joint = new GameObject(name).AddComponent<JointType>();
		joint.GetComponent<Rigidbody>().isKinematic = true;
		joint.gameObject.SetActive(false);
		return joint;
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

		if (Grounded && amount > 0)
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

	public float hipMultiplier = 1f;

	public void Jump()
	{
		// Todo: test if touching walkable perimeter, and only jump if do
		if (Grounded == false)
			return;

		hip.rigidbody.AddForce(Vector3.up * jumpPower * hipMultiplier, ForceMode.Impulse);
		neck.rigidbody.AddForce(Vector3.up * jumpPower * hipMultiplier, ForceMode.Impulse);
		
		controlRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;

		Vector3 neckTargetPosition = hip.rigidbody.position + hip.rigidbody.transform.up * neckDistanceFromHip;
		Gizmos.DrawWireSphere(neckTargetPosition, 0.05f);


		Gizmos.DrawWireSphere(hipPosition, 0.05f);

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
