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
	public bool startWithControl = true;
	private bool hasControl;
	public bool HasControl
	{ 
		get => hasControl; 
		set {
			if (concussionState == ConcussionState.Concussion)
				hasControl = false;
			else
				hasControl = value;
		} 
	}

	[Header("Walking")]
	public float feetWidth = 0.5f;
	public float stepLength = 1.0f;
	public float footForce = 1000;
	public float footSpeed = 1;
	private bool doWalk;

	private Vector3 leftFootPosition;
	private Vector3 rightFootPosition;

	private void ComputeFootPositions()
	{
		Vector3 forward = hipRb.transform.forward;
		forward.y = 0;
		forward = forward.normalized;
		var rotation = Quaternion.LookRotation(forward);

		// Get cyclic value in range [-1 ... 1], so that we can move legs back and forth
		// Set to zero aka standing point when not walking
		float footLerp = doWalk ?
			Mathf.PingPong(footSpeed * Time.time, 1f) * 2f - 1f:
			0f;

		// foot targets in local space
		Vector3 leftFootTarget = new Vector3(-feetWidth, 0f, stepLength * footLerp);
		Vector3 rightFootTarget = new Vector3(feetWidth, 0f, -stepLength * footLerp);

		leftFootPosition  = rotation * leftFootTarget + hipHitPosition;
		rightFootPosition  = rotation * rightFootTarget + hipHitPosition;
	}


	public float walkSpeed = 3f;
	public float turnSpeed = 360f;
	public float jumpVelocity = 5f;

	private Joint leftFootControlJoint;
	private Joint rightFootControlJoint;

	[SerializeField] private RagdollFootControl leftFoot;
	[SerializeField] private RagdollFootControl rightFoot;

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

	private SpringJoint leftHandControlJoint;
	private SpringJoint rightHandControlJoint;



	// This is expected to have locked rotation
	[Header("Hip")]
	public Rigidbody hipRb;
	public LayerMask hitRayMask;
	public float hipForce = 5000f;
	public float hipHeight = 0.65f;
	public float hipZOffset = -0.1f;

	
	public bool Grounded => leftFoot.Grounded || rightFoot.Grounded;
	public float hipRbHorizontalDrag = 10f;
	private Vector3 hipHitPosition;

	[Header ("Neck")]
	public Rigidbody neckRb;
	public float neckForce;
	public float neckZOffset = 0.1f;

	private SmoothFloat leftHandControlWeight = new SmoothFloat();
	private SmoothFloat rightHandControlWeight = new SmoothFloat();

	public bool ControlLeftHand { get; set; }
	public bool ControlRightHand { get; set; }

	[Header("Head and Concussion")]
	public float concussionVelocityThreshold;
	public float concussionTime = 5f;
	public float concussionInvulnerabilityTime = 1f;
	private enum ConcussionState { None, Concussion, Invulnerable }
	private ConcussionState concussionState;
	[SerializeField] private ParticleSystem concussionVFX;

	private IEnumerator DoConcussion()
	{
		if (concussionState != ConcussionState.None)
			yield break;

		// Set concussion
		concussionVFX.gameObject.SetActive(true);
		concussionVFX.Play();
		HasControl = false;
		concussionState = ConcussionState.Concussion;	
		yield return new WaitForSeconds	(concussionTime);

		// Set invulnerability
		concussionVFX.gameObject.SetActive(false);
		concussionState = ConcussionState.Invulnerable;
		yield return new WaitForSeconds (concussionInvulnerabilityTime);

		concussionState = ConcussionState.None;
	}

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

	private void Start()
	{
		hasControl = startWithControl;

		foreach (var item in GetComponentsInChildren<RagdollHeadPiece>())
		{
			item.OnImpact += () => StartCoroutine(DoConcussion());
		}
	
		hipRb.transform.SetParent(null);
		hipRb.freezeRotation = true;
		
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
	}

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
		else
		{
			hipHitPosition = hipRayOrigin + Vector3.down * hipHeight;
		}

		// SetFootControlsActive((Grounded || hipGrounded) && HasControl);

		if (HasControl)
		{
			hipRb.freezeRotation = true;

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

			if (Grounded || hipGrounded)
			{
				hipRb.AddForce(hipForce * Vector3.up);

				var neckToHip = (hipRb.position + hipRb.transform.forward * neckZOffset) - neckRb.position;
				neckToHip.y = 1; // As in vector3.up, so we don't pull down, and always use full force in vertical direction
				var neckForceVector = neckToHip * neckForce;
				neckRb.AddForceAtPosition(neckForceVector, neckRb.position + 0.05f * Vector3.up);
			}
			else
			{
				// Set this to ray's end.
				hipHitPosition = hipRb.position + Vector3.down* hipHeight + hipOffsetVector;
			}

			// Control feet ----------------------------------------------------------
			ComputeFootPositions();

			leftFoot.rigidbody.MovePosition(leftFootPosition);
			rightFoot.rigidbody.MovePosition(rightFootPosition);
		}
		else
		{
			hipRb.freezeRotation = false;
		}



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

	// Move character to direction, do this in fixed update only.
	// NOTE: Do not use delta time for amount, instead pass [0 ... 1] value of input.
	// moveVelocity and lookDirection should be normalized
	public void MoveWithVelocity(Vector3 moveVelocity, Vector3 lookDirection, float amount)
	{
		// Walk conditionally. 'doWalk' is also used for foot animation
		if (HasControl)
		{
			Vector3 velocity = hipRb.velocity;
			float ySpeed = velocity.y;
			velocity = moveVelocity * amount * walkSpeed;
			velocity.y = ySpeed;
			hipRb.velocity = velocity;

			var hipRotation = Quaternion.RotateTowards(
				hipRb.rotation, 
				Quaternion.LookRotation(lookDirection),
				turnSpeed * Time.deltaTime);
			hipRb.MoveRotation(hipRotation);
		}
		
		doWalk = HasControl && amount > 0.001f;
	}
	
	public void Jump()
	{
		if (HasControl && Grounded)
		{
			float jumpBonus = Mathf.Max(leftFoot.JumpBonusValue, rightFoot.JumpBonusValue);
			float currentJumpVelocity = jumpVelocity + jumpBonus;
			
			hipRb.AddForce(Vector3.up * currentJumpVelocity, ForceMode.VelocityChange);
			neckRb.AddForce(Vector3.up * currentJumpVelocity, ForceMode.VelocityChange);
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
		float angle = handsAimAngle * Mathf.Deg2Rad;
		float sin = Mathf.Sin (angle);
		float cos = Mathf.Cos (angle);
		float height = handsHeight + sin * handsDistance;
		float distance = cos * handsDistance;

		leftHandPosition = new Vector3(-handsWidth, height, distance);
		rightHandPosition = new Vector3(handsWidth, height, distance);
		handsForward = hipRb.transform.TransformDirection(new Vector3(0, sin, cos));
	}

	private static void ControlHand(
		SpringJoint	controlJoint,
		SmoothFloat controlWeight,
		Vector3 	targetPosition,
		Rigidbody 	rigidbody,
		bool 		doControl,
		Vector3 	direction,
		float 		force
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
		Gizmos.DrawWireSphere(leftFootPosition, 0.05f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(rightFootPosition, 0.05f);
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

		foreach (var item in GetComponentsInChildren<RagdollHeadPiece>())
		{
			item.velocityThreshold = concussionVelocityThreshold;
			item.sqrVelocityThreshold = concussionVelocityThreshold * concussionVelocityThreshold;
		}
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