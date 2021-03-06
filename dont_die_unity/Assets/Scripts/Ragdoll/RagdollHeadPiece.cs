using System;
using UnityEngine;

public class RagdollHeadPiece : MonoBehaviour
{
	public float velocityThreshold;
	public float sqrVelocityThreshold;

	public event Action OnImpact;

	private void OnCollisionEnter(Collision collision)
	{
		// Let's not hit ourselves
		if (collision.collider.transform.root == transform.root)
			return;

		if (collision.relativeVelocity.sqrMagnitude > sqrVelocityThreshold)
		{
			OnImpact?.Invoke();

			AudioManager.PlaySoundNonSpatial(null);
		}
	}

	private void OnValidate()
	{
		sqrVelocityThreshold = velocityThreshold * velocityThreshold;
	}
}