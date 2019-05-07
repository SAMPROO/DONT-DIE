using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StickyProjectile : MonoBehaviour
{
	public float startSpeed = 10f;
	public float stickingForce = 100f;
	public float duration = 5f;

	public enum DurationStart { OnLaunch, OnHit }
	public DurationStart startDuration = DurationStart.OnHit;

	public enum EndAction { None, Disable, Destroy }
	public EndAction endAction;

	public bool connectToRigidbodiesOnly = false;

	private new Rigidbody rigidbody;
	private FixedJoint stickedJoint = null;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
 		rigidbody.AddForce(transform.forward * startSpeed, ForceMode.VelocityChange);

		if (startDuration == DurationStart.OnLaunch)
			StartCoroutine(UnSpawn());
	}

	private void OnDisable()
	{
		if (stickedJoint != null)
			Destroy(stickedJoint);

		stickedJoint = null;
	}

	private void OnCollisionEnter(Collision collision)
	{
		// Do not stick to other stuff
		if(stickedJoint != null)
			return;

		if (startDuration == DurationStart.OnHit)
			StartCoroutine(UnSpawn());


		// Check if other collider has rigidbody, and stick to it if wanted
		var otherBody = collision.collider.GetComponent<Rigidbody>();

		if (otherBody != null)
		{
			stickedJoint = gameObject.AddComponent<FixedJoint>();
			stickedJoint.breakForce = stickingForce;
			stickedJoint.connectedBody = otherBody;
		}
		else if (connectToRigidbodiesOnly == false)
		{
			stickedJoint = gameObject.AddComponent<FixedJoint>();
			stickedJoint.breakForce = stickingForce;
		}
	}

	private IEnumerator UnSpawn()
	{
		if(endAction == EndAction.None)
			yield break;

		yield return new WaitForSeconds (duration);

		switch(endAction)
		{
			case EndAction.Disable: gameObject.SetActive(false); 	break;
			case EndAction.Destroy: Destroy(gameObject);			break;
		}
	}
}