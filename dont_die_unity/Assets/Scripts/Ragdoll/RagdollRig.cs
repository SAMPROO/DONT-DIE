/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
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

	public Joint leftFootController;
	public Joint rightFootController;

	[SerializeField] private RagdollFootControl leftFoot;
	[SerializeField] private RagdollFootControl rightFoot;

	[Header("Walking")]
	public float feetWidth = 0.5f;
	public float stepLength = 1.0f;
	public float footForce = 1000;
	public float footSpeed = 1;
	private bool doWalk;
	private float FootLerp()
	{
		if (doWalk == false)
			return 0;
		return Mathf.PingPong(footSpeed * Time.time, 1f) * 2f - 1f;
	}

	// foot targets in local space
	Vector3 leftFootTarget => new Vector3(-feetWidth, 0f, stepLength * FootLerp());
	Vector3 rightFootTarget => new Vector3(feetWidth, 0f, -stepLength * FootLerp());

	Vector3 getComputedFootPosition(Vector3 localFootTarget)
	{
		Vector3 forward = hipRb.transform.forward;
		forward.y = 0;
		forward = forward.normalized;

		return Quaternion.LookRotation(forward) * localFootTarget + hipHitPosition;
	}

	[Header("Specs")]
	[SerializeField] private float jumpVelocity = 5f;
	[SerializeField] private float speed = 3f;

	[Header("Controlled parts")]
	public ControlBone hip;
	public ControlBone rightHand;
	public ControlBone leftHand;

	public Rigidbody hipRb;

	Vector3 hipTargetPosition => transform.position + hip.targetPosition;

	public LayerMask hitRayMask;
	public float hipForce = 5000f;
	public float hipHeight = 0.65f;
	public float hipZOffset = -0.1f;

	public Rigidbody neckRb;
	public float neckForce;
	public float neckZOffset = 0.1f;

	private bool handsControlled;
	public void SetHandControl(bool value)
	{
		// do not set same value again
		if (handsControlled == value)
			return;

		handsControlled = value;

		if (value)
		{
			EnableController(
				rightHandController,
				rightHand.rigidbody,
				transform.TransformPoint(rightHand.targetPosition)
			);

			EnableController(
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

	private static void EnableController(Joint controller, Rigidbody controlledBody, Vector3? targetPosition = null)
	{
		controller.transform.position = controlledBody.transform.position;
		controller.transform.rotation = controlledBody.transform.rotation;
		controller.connectedBody = controlledBody;
		controller.gameObject.SetActive(true);
		controller.transform.position = targetPosition ?? Vector3.zero;
	}

	private static void DisableController(Joint controller)
	{
		controller.connectedBody = null;
		controller.gameObject.SetActive(false);		
	}

	public bool Grounded => leftFoot.Grounded || rightFoot.Grounded;

	public float controlRbHorizontalDrag = 10f;

	private void Awake()
	{
		hipRb.transform.SetParent(null);
	}

	private void Start()
	{
		rightHandController = CreateJointController<FixedJoint>("rightHandController", false);
		rightHandController.transform.SetParent(hipRb.transform);

		leftHandController = CreateJointController<FixedJoint>("leftHandController", false);
		leftHandController.transform.SetParent(hipRb.transform);

		leftFootController = CreateJointController<SpringJoint>("leftFootController", false);
		leftFootController.transform.position = leftFoot.position;
		leftFootController.connectedBody = leftFoot.rigidbody;
		(leftFootController as SpringJoint).spring = footForce;
		leftFootController.gameObject.SetActive(true);

		rightFootController = CreateJointController<SpringJoint>("rightFootController", false);
		rightFootController.transform.position = rightFoot.position;
		rightFootController.connectedBody = rightFoot.rigidbody;
		(rightFootController as SpringJoint).spring = footForce;
		rightFootController.gameObject.SetActive(true);

	}

	private Vector3 hipHitPosition;
	public bool DEBUGGrounded;

	private void FixedUpdate()
	{
		bool hipGrounded = false;
		RaycastHit hit;

		var hipOffsetVector = hipRb.transform.forward * hipZOffset;
		var hipRayOrigin = hipRb.position + hipOffsetVector;

		if (Physics.Raycast(hipRayOrigin, Vector3.down, out hit, hipHeight, hitRayMask, QueryTriggerInteraction.UseGlobal))
		{	
			hipGrounded = true;

			hipHitPosition = hit.point;

		}
	

		if (Grounded || hipGrounded)
		{
			hipRb.AddForce(hipForce * Vector3.up);

			var neckToHip = (hipRb.position + hipRb.transform.forward * neckZOffset) - neckRb.position;
			neckToHip.y = 1; // As in vector3.up, so we don't pull down, and always use full force in vertical direction
			var neckForceVector = neckToHip * neckForce;
			neckRb.AddForceAtPosition(neckForceVector, neckRb.position + 0.05f * Vector3.up);

			EnableController(leftFootController, leftFoot.rigidbody);
			EnableController(rightFootController, rightFoot.rigidbody);
		}
		else
		{
			DisableController(leftFootController);
			DisableController(rightFootController);
			hipHitPosition = hipRb.position + Vector3.down* hipHeight + hipOffsetVector;
		}

		// Apply custom drag in horizontal directions
		var velocity = hipRb.velocity;
		velocity.x /= controlRbHorizontalDrag;
		velocity.z /= controlRbHorizontalDrag;
		hipRb.velocity = velocity;

		// TODO: get aim direction from camera and use it to orient hands (and guns)
		// Keep hands in fixed rotation
		leftHandController.transform.rotation = Quaternion.LookRotation(transform.forward);
		rightHandController.transform.rotation = Quaternion.LookRotation(transform.forward);

		if (leftFootController != null)
			leftFootController.transform.position = getComputedFootPosition(leftFootTarget);
		rightFootController.transform.position = getComputedFootPosition(rightFootTarget);


		// follow hipRb, it is different transform
		transform.position = hipHitPosition;
	}

	private static JointType CreateJointController<JointType>(string name, bool setActive) where JointType : Joint
	{
		JointType joint = new GameObject(name).AddComponent<JointType>();
		joint.GetComponent<Rigidbody>().isKinematic = true;
		joint.gameObject.SetActive(setActive);
		return joint;
	}

	// Move character to direction, do this in fixed update
	public void Move(Vector3 movement, Vector3 look, float amount)
	{
		doWalk = amount > 0;

		amount *= speed;
		hipRb.MovePosition(hipRb.position + movement * amount);
		hipRb.MoveRotation(Quaternion.LookRotation(look));
	}

	public void Jump()
	{
		// TODO: ground check
		hipRb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
		neckRb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);

	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(hipHitPosition, 0.05f);

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(hipRb.position - hipHeight * Vector3.up, 0.07f);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(rightHand.targetPosition), 0.05f);
		Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(leftHand.targetPosition), 0.05f);


		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.TransformPoint(leftFootTarget), 0.05f);
		Gizmos.DrawWireSphere(getComputedFootPosition(leftFootTarget), 0.05f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.TransformPoint(rightFootTarget), 0.05f);
		Gizmos.DrawWireSphere(getComputedFootPosition(rightFootTarget), 0.05f);
	}

	// todo: Not used anymore here, can be moved to some utility class
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
