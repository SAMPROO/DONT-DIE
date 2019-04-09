/*
Sampo's Gaming Department
Leo Tamminen
*/

using UnityEngine;


[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class RagdollFootControl : MonoBehaviour
{
	private int collisionCount;// { get; private set; } = 0;
	public bool Grounded => smoothGrounded.Value > 0.1f; //collisionCount > 0;

	private void OnCollisionEnter() => collisionCount++;
	private void OnCollisionExit() => collisionCount--;

	private SmoothFloat smoothGrounded = new SmoothFloat(10);

	private void FixedUpdate()
	{
		smoothGrounded.Put(collisionCount > 0 ? 1f : 0f);
	}
}

