/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollRig : MonoBehaviour
{
	private SpringJoint leftHandControlJoint;
	private SpringJoint rightHandControlJoint;

	public Joint leftFootControlJoint;
	public Joint rightFootControlJoint;

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

	Vector3 GetComputedFootPosition(Vector3 localFootTarget)
	{
		Vector3 forward = hipRb.transform.forward;
		forward.y = 0;
		forward = forward.normalized;

		return Quaternion.LookRotation(forward) * localFootTarget + hipHitPosition;
	}

	[Header("Specs")]
	[SerializeField] private float jumpVelocity = 5f;
	[SerializeField] private float speed = 3f;

	[Header("Hands")]
	public float handForce = 1000f;
	public float handsHeight = 1.0f;
	public float handsDistance = 1.0f;
	public float handsWidth = 0.2f;
	public float handsAngleMin = -45f;
	public float handsAngleMax = 45f;

	public Rigidbody leftHandRb;
	public Rigidbody rightHandRb;

	// Set aim angle between min and max
	public void SetHandsAimAngle(float value) => handsAimAngle = value;
	private float handsAimAngle;

	private Vector3 leftHandPosition;
	private Vector3 rightHandPosition;
	private Vector3 handsForward;


	// This is expected to have locked rotation
	[Space(30)]public Rigidbody hipRb;

	public LayerMask hitRayMask;
	public float hipForce = 5000f;
	public float hipHeight = 0.65f;
	public float hipZOffset = -0.1f;

	public bool Grounded => leftFoot.Grounded || rightFoot.Grounded;
	public float hipRbHorizontalDrag = 10f;
	private Vector3 hipHitPosition;

	public Rigidbody neckRb;
	public float neckForce;
	public float neckZOffset = 0.1f;

	private SmoothFloat leftHandControlWeight = new SmoothFloat();
	private SmoothFloat rightHandControlWeight = new SmoothFloat();

	public bool ControlLeftHand { get; set; }
	public bool ControlRightHand { get; set; }

	private static void EnableController(
		Joint controller, 
		Rigidbody controlledBody, 
		Vector3? targetPosition = null
	){
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


	private void Awake()
	{
		hipRb.transform.SetParent(null);
		hipRb.freezeRotation = true;
	}

	private void Start()
	{
		rightHandControlJoint = CreateJointController<SpringJoint>("rightHandControlJoint", false);
		rightHandControlJoint.transform.SetParent(hipRb.transform);

		leftHandControlJoint = CreateJointController<SpringJoint>("leftHandControlJoint", false);
		leftHandControlJoint.transform.SetParent(hipRb.transform);

		EnableController(
			rightHandControlJoint,
			rightHandRb,
			hipRb.transform.TransformPoint(rightHandPosition)
		);

		EnableController(
			leftHandControlJoint,
			leftHandRb,
			hipRb.transform.TransformPoint(leftHandPosition)
		);

		leftFootControlJoint = CreateJointController<SpringJoint>("leftFootControlJoint", false);
		leftFootControlJoint.transform.position = leftFoot.position;
		leftFootControlJoint.connectedBody = leftFoot.rigidbody;
		(leftFootControlJoint as SpringJoint).spring = footForce;
		leftFootControlJoint.gameObject.SetActive(true);

		rightFootControlJoint = CreateJointController<SpringJoint>("rightFootControlJoint", false);
		rightFootControlJoint.transform.position = rightFoot.position;
		rightFootControlJoint.connectedBody = rightFoot.rigidbody;
		(rightFootControlJoint as SpringJoint).spring = footForce;
		rightFootControlJoint.gameObject.SetActive(true);

	}



	private void FixedUpdate()
	{
		// Control hands ------------------------------------------------------------------
		ComputeHandPositions();

		ControlHand(
			leftHandControlJoint,
			leftHandControlWeight,
			hipRb.transform.TransformPoint(leftHandPosition),
			leftHandRb,
			ControlLeftHand,
			handsForward,
			handForce
		);

		ControlHand(
			rightHandControlJoint,
			rightHandControlWeight,
			hipRb.transform.TransformPoint(rightHandPosition),
			rightHandRb,
			ControlRightHand,
			handsForward,
			handForce
		);

		// Control hips etc. --------------------------------------------------------------------
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

			EnableController(leftFootControlJoint, leftFoot.rigidbody);
			EnableController(rightFootControlJoint, rightFoot.rigidbody);
		}
		else
		{
			DisableController(leftFootControlJoint);
			DisableController(rightFootControlJoint);
			hipHitPosition = hipRb.position + Vector3.down* hipHeight + hipOffsetVector;
		}

		// Apply custom drag in horizontal directions
		var velocity = hipRb.velocity;
		velocity.x /= hipRbHorizontalDrag;
		velocity.z /= hipRbHorizontalDrag;
		hipRb.velocity = velocity;

		// Control feet ----------------------------------------------------------
		leftFootControlJoint.transform.position = GetComputedFootPosition(leftFootTarget);
		rightFootControlJoint.transform.position = GetComputedFootPosition(rightFootTarget);

		// follow hipRb, it is different transform
		// Hack, should be done in camera. Or should it?
		transform.position = Vector3.Lerp (transform.position, hipHitPosition, 0.5f);
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
		if (Grounded)
		{
			hipRb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
			neckRb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
		}
	}

	////////////////////////////////////////
	/// HAND computations 				 ///
	////////////////////////////////////////

	// Hands' forward axes actually point up, so we want to rotate them additional 90 degrees
	private static readonly Quaternion handCorrection = Quaternion.Euler(90, 0, 0);
	private const float freezeHandRotationThreshold = 0.8f;

	private void ComputeHandPositions()
	{
		// Note inverting handsAngleMin, so that we can have intuitive negative inspector value
		// and correct negative value here,
		float angle = handsAimAngle < 0.0f ? 
			handsAimAngle * -handsAngleMin : 
			handsAimAngle * handsAngleMax;
		angle = handsAimAngle;
		angle *= Mathf.Deg2Rad;
		// angle *= handAimMultiplier;

		float sin = Mathf.Sin (angle);
		float cos = Mathf.Cos (angle);

		float height = handsHeight + sin * handsDistance;
		float distance = cos * handsDistance;

		leftHandPosition = new Vector3(-handsWidth, height, distance);
		rightHandPosition = new Vector3(handsWidth, height, distance);

		handsForward = hipRb.transform.TransformDirection(new Vector3(0, sin, cos));
	}


	private static void ControlHand(
		SpringJoint controlJoint, // controlJoint
		SmoothFloat controlWeight,
		Vector3 targetPosition,
		Rigidbody rigidbody,
		bool doControl,
		Vector3 direction,
		float force
	){
		controlJoint.transform.position = targetPosition;

		controlWeight.Put(doControl ? 1f : 0f);
		controlJoint.spring = Mathf.Lerp(0, force, controlWeight.Value);	

		if (controlWeight.Value > freezeHandRotationThreshold)
		{
			rigidbody.MoveRotation(Quaternion.LookRotation(direction) * handCorrection);
			rigidbody.freezeRotation = true;
		}
		else {
			rigidbody.freezeRotation = false;
		}
	}


	///////////////////////////////////////
	/// Utilities, helpers, etc.		///
	///////////////////////////////////////

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(hipHitPosition, 0.05f);

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(hipRb.position - hipHeight * Vector3.up, 0.07f);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(hipRb.transform.TransformPoint(leftHandPosition), 0.05f);
		Gizmos.DrawWireSphere(hipRb.transform.TransformPoint(rightHandPosition), 0.05f);

		if (Application.isPlaying)
		{
			Gizmos.color = new Color (1f, 0.3f, 0f);
			Gizmos.DrawWireSphere(leftHandControlJoint.transform.position, 0.07f);
			Gizmos.DrawWireSphere(rightHandControlJoint.transform.position, 0.07f);

			Gizmos.DrawLine(
				leftHandControlJoint.transform.position,
				leftHandControlJoint.transform.position + 2 * handsForward
			);

			Gizmos.DrawLine(
				rightHandControlJoint.transform.position,
				rightHandControlJoint.transform.position + 2 * handsForward
			);
		}

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.TransformPoint(leftFootTarget), 0.05f);
		Gizmos.DrawWireSphere(GetComputedFootPosition(leftFootTarget), 0.05f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.TransformPoint(rightFootTarget), 0.05f);
		Gizmos.DrawWireSphere(GetComputedFootPosition(rightFootTarget), 0.05f);
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

	public static Dictionary<String, GameObject> GetChildGameObjectDictionary(Transform root, Dictionary<String, GameObject> children = null)
	{
		children = children ?? new Dictionary<String, GameObject>();

		int childCount = root.childCount;
		for (int i = 0; i < childCount; i++)
		{
			var child = root.GetChild(i);
			children.Add(child.name, child.gameObject);
			GetChildGameObjectDictionary(child, children);
		}

		return children;
	}

	private void OnValidate()
	{
		if (hipRb != null && hipRb.freezeRotation == false)
		{
			hipRb.freezeRotation = true;
			Debug.Log($"Locked 'hipRb({hipRb.name})' rotation. It is expected behaviour.");
		}

		ComputeHandPositions();
	}

	private const string leftHandName = "Hand.L";
	private const string rightHandName = "Hand.R";

	public void FindParts(bool overridePrevious)
	{
		var children = GetChildGameObjectDictionary(transform);

		leftHandRb = GetPartConditionally(leftHandRb, leftHandName, children, overridePrevious);
		rightHandRb = GetPartConditionally(rightHandRb, rightHandName, children, overridePrevious);
	}

	private static T GetPartConditionally<T>(
		T part, 
		string partName, 
		Dictionary<string, GameObject> parts,  
		bool overridePrevious
	) where T : UnityEngine.Component
	{
		bool doGetPart = 
			(part == null || overridePrevious)
			&& parts.ContainsKey(partName);
		
		if (doGetPart)
			part = parts[partName].GetComponent<T>();

		return part;
	}
}
