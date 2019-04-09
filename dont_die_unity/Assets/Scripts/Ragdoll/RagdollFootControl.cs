/*
Sampo's Gaming Department
Leo Tamminen
*/

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollFootControl : MonoBehaviour
{
	private int collisionCount;
	private SmoothFloat smoothGrounded = new SmoothFloat(10);
	public bool Grounded => smoothGrounded.Value > 0.1f;

	private void OnCollisionEnter() => collisionCount++;
	private void OnCollisionExit() => collisionCount--;

	private void FixedUpdate()
	{
		smoothGrounded.Put(collisionCount > 0 ? 1f : 0f);
	}
}

