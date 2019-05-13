/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RagdollRig : MonoBehaviour
{
	public bool startWithControl = true;
	[NonSerialized] public bool hasControl;

	private bool isControlled => 
		concussionState != ConcussionState.Concussion 
		&& isGoingTooFast == false
		&& hasControl;

	// This should be close to walking speed. It is used as critical
	// speed after which ragdoll loses controllability
	public float horizontalSpeedThreshold = 5f;
	private float sqrHorizontalSpeedThreshold;
	private bool isGoingTooFast;

	private bool GoesTooFast()
	{
		Vector3 velocity = hipRb.velocity;
		if (velocity.y > jumpVelocity)
			return true;

		float xzSqrSpeed = velocity.x * velocity.x + velocity.z * velocity.z;
		if (xzSqrSpeed > sqrHorizontalSpeedThreshold)
			return true;

		return false;
	}

	[Header("Walking")]
	public float feetWidth = 0.5f;
	public float stepLength = 1.0f;
	public float footForce = 1000;
	public float footSpeed = 1;
	private bool doWalk;

	private Vector3 leftFootPosition;
	private Vector3 rightFootPosition;

	// This is called when one step is taken
	public UnityEvent OnTakeStep;
	private bool tookStep;
	private const float footLerpStepThreshold = 0.95f;
	private const float footLerpStepResetThreshold = 0.05f;

	private void ComputeFootPositions()
	{
		Vector3 forward = hipRb.transform.forward;
		forward.y = 0;
		forward = forward.normalized;
		var rotation = Quaternion.LookRotation(forward);

		// TODO: Add knees
		// Get cyclic value in range [-1 ... 1], so that we can move legs back and forth
		// Set to zero aka standing point when not walking
		float footLerp = doWalk ?
			Mathf.PingPong(footSpeed * Time.time, 1f) * 2f - 1f:
			0f;

		// Two if clauses to track steps
		float absFootLerp = Mathf.Abs(footLerp);
		if (tookStep == false && absFootLerp > footLerpStepThreshold)
		{
			// This is used for sounds now, so only call when grounded.
			// For other stuff this may not be valid
			if (Grounded)
				OnTakeStep?.Invoke();
			tookStep = true;
		}

		if (tookStep == true && absFootLerp < footLerpStepResetThreshold)
		{
			tookStep = false;
		}


		// foot targets in local space
		Vector3 leftFootTarget = new Vector3(-feetWidth, 0f, stepLength * footLerp);
		Vector3 rightFootTarget = new Vector3(feetWidth, 0f, -stepLength * footLerp);

		leftFootPosition = rotation * leftFootTarget + hipRayHitPosition;
		rightFootPosition = rotation * rightFootTarget + hipRayHitPosition;
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

	public RagdollHandGrab leftGrab;
	public RagdollHandGrab rightGrab;

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

	public bool Grounded { get; private set; }
	public float hipRbHorizontalDrag = 10f;
	private Vector3 hipRayHitPosition;
	public Vector3 CurrentPosition => hipRayHitPosition;


	[Header("Neck")]
	public Rigidbody neckRb;
	public float neckForce;
	public float neckZOffset = 0.1f;

	private SmoothFloat leftHandControlWeight = new SmoothFloat();
	private SmoothFloat rightHandControlWeight = new SmoothFloat();

	public bool ControlLeftHand { get; set; }
	public bool ControlRightHand { get; set; }
	public bool CanGrab { get; set; }

	[Header("Head and Concussion")]
	public float concussionVelocityThreshold;
	public float concussionTime = 5f;
	public float concussionInvulnerabilityTime = 1f;
	private enum ConcussionState { None, Concussion, Invulnerable }
	private ConcussionState concussionState;
	[SerializeField] private ParticleSystem concussionVFX;


	private Rigidbody [] allRigidbodies = null;
	private Vector3 [] startPositions = null;
	private Quaternion [] startRotations = null;

    [Header("Other Status things")]
    public ParticleSystem healVFX;
    public ParticleSystem damageVFX;
    private IEnumerator DoConcussion()
	{
		if (concussionState != ConcussionState.None)
			yield break;

		// Set concussion
		concussionVFX.gameObject.SetActive(true);
		concussionVFX.Play();
		concussionState = ConcussionState.Concussion;	
		yield return new WaitForSeconds	(concussionTime);

		// Set invulnerability. Also set hasControl to false, so that we stay
		// on ground until player presses button
		concussionVFX.gameObject.SetActive(false);
		concussionState = ConcussionState.Invulnerable;
		hasControl = false;
		yield return new WaitForSeconds (concussionInvulnerabilityTime);

		// Unset conscussion
		concussionState = ConcussionState.None;
	}

	// Sets ragdolls position so that given position is assumed ground level
	// and hip (and other parts) are positioned accordinlgy
	public void SetPosition(Vector3 position)
	{
		ResetVelocity();
		Debug.Log($"Set ragdoll position to {position}");
		hipRb.transform.position = position + Vector3.up * hipHeight;
	}

	public void SetDirection(Vector3 lookDirection)
	{
		ResetVelocity();
		hipRb.transform.rotation = Quaternion.LookRotation(lookDirection);
	}

	// Reset velocities for all rigidbodies in ragdoll
	public void ResetVelocity()
	{
		int count = allRigidbodies.Length;
		for (int i = 0; i < count; i++)
		{
			allRigidbodies[i].velocity = Vector3.zero;
		}
	}

	// Reset pose of all bones to starting poses.
	// Does not affect hip since it is root bone.
	public void ResetPose()
	{
		ResetVelocity();
		int count = allRigidbodies.Length;
		for (int i = 1; i < count; i ++)
		{
			allRigidbodies[i].transform.localRotation = startRotations[i];
		}
	}

	public void SetActive(bool value)
		=> hipRb.gameObject.SetActive(value);

	private void Awake()
	{
		var rigidbodiesInChildren = GetComponentsInChildren<Rigidbody>();
		int childCount = rigidbodiesInChildren.Length;

		allRigidbodies = new Rigidbody 	[childCount + 1];
		startPositions = new Vector3 	[childCount + 1];
		startRotations = new Quaternion	[childCount + 1];

		allRigidbodies [0] = hipRb;
		startPositions [0] = hipRb.transform.position;
		startRotations [0] = hipRb.transform.rotation;
		
		for (int i = 0; i < childCount; i++)
		{
			allRigidbodies [i + 1] = rigidbodiesInChildren[i];
			startPositions [i + 1] = rigidbodiesInChildren[i].transform.localPosition;
			startRotations [i + 1] = rigidbodiesInChildren[i].transform.localRotation;
		}
	}

	private void Start()
	{
		sqrHorizontalSpeedThreshold = horizontalSpeedThreshold * horizontalSpeedThreshold;
		hasControl = startWithControl;

		foreach (var item in GetComponentsInChildren<RagdollHeadPiece>())
		{
			item.OnImpact += () => StartCoroutine(DoConcussion());
		}

		hipRb.transform.SetParent(null);
		hipRb.freezeRotation = true;
		
		leftHandControlJoint = CreateHandControlJoint(
			"leftHandControlJoint",
			leftHandRb,
			hipRb.transform,
			leftHandPosition
		);

		rightHandControlJoint = CreateHandControlJoint(
			"rightHandControlJoint",
			rightHandRb,
			hipRb.transform,
			rightHandPosition
		);
	}

	private void FixedUpdate()
	{
		isGoingTooFast = GoesTooFast();

		RaycastHit hit;

		var hipOffsetVector = hipRb.transform.forward * hipZOffset;
		var hipRayOrigin = hipRb.position + hipOffsetVector;

		if (Physics.Raycast(hipRayOrigin, Vector3.down, out hit, hipHeight, hitRayMask, QueryTriggerInteraction.UseGlobal))
		{	
			Grounded = true;
			hipRayHitPosition = hit.point;
		}
		else
		{
			Grounded = false;
			hipRayHitPosition = hipRayOrigin + Vector3.down * hipHeight;
		}

		if (isControlled)
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

			if (Grounded)
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
				hipRayHitPosition = hipRb.position + Vector3.down* hipHeight + hipOffsetVector;
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

		leftGrab.SetGrab (isControlled && CanGrab && ControlLeftHand);
		rightGrab.SetGrab (isControlled && CanGrab && ControlRightHand);


		// follow hipRb, it is different transform
		// Hack, should be done in camera. Or should it?
		// transform.position = Vector3.Lerp (transform.position, hipRayHitPosition, 0.5f);
	}



	// Move character to direction, do this in fixed update only.
	// NOTE: Do not use delta time for amount, instead pass [0 ... 1] value of input.
	// amount will be multiplied with ragdoll's walkSpeed.
	// moveDirection and lookDirection should be normalized
	public void MoveWithVelocity(Vector3 moveDirection, Vector3 lookDirection, float amount)
	{
		if (enabled == false)
			return;

		// Walk if we are grounded or our velocity is smaller than walk speed.
		// This way explosions etc. can apply force through regular physics,
		// And we can still manouver slightly in air
		if (isControlled) 
		{
			if (Grounded || hipRb.velocity.magnitude < walkSpeed)
			{
				float speed = amount * walkSpeed;
				hipRb.velocity = new Vector3(
					moveDirection.x * speed,
					hipRb.velocity.y,
					moveDirection.z * speed
				);	
			}
			
			// Allow rotation in all cases
			var hipRotation = Quaternion.RotateTowards(
				hipRb.rotation, 
				Quaternion.LookRotation(lookDirection),
				turnSpeed * Time.deltaTime);
			hipRb.MoveRotation(hipRotation);
		}
		
		doWalk = isControlled && amount > 0.001f;
	}
	
	// Stop all movement
	public void Stop()
	{
		doWalk = false;
		hipRb.velocity = Vector3.zero;
	}

	public void Jump()
	{
		if (enabled == false)
			return;

		if (isControlled && Grounded)
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

	private static SpringJoint CreateHandControlJoint(string name, Rigidbody controlledBody, Transform parent, Vector3 localPosition)
	{
		var joint = new GameObject(name).AddComponent<SpringJoint>();
		joint.GetComponent<Rigidbody>().isKinematic = true;

		joint.gameObject.SetActive(false);
		joint.transform.position = controlledBody.transform.position;
		joint.transform.rotation = controlledBody.transform.rotation;
		joint.connectedBody = controlledBody;
		joint.gameObject.SetActive(true);
		joint.transform.position = parent.TransformPoint(localPosition);

		joint.transform.SetParent(parent);

		return joint;
	}

	/*
	Utilities, helpers, etc.

	These are not neede in game at all, they kinda help build stuff in editor
	*/	

	#if UNITY_EDITOR

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(hipRayHitPosition, 0.05f);

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

	#endif
}