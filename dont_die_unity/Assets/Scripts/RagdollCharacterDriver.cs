using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollCharacterDriver : MonoBehaviour
{
	[SerializeField] private float jumpPower = 5f;
	[SerializeField] private float speed = 3f;

	private new Rigidbody rigidbody;

	[SerializeField] private Rigidbody hipRb;
	[SerializeField] private Rigidbody headRb;



	[SerializeField] private float hipHeight = 0.5f;
	Vector3 hipPosition => transform.position + new Vector3(0, hipHeight, 0);
	[SerializeField] private float headHeight = 2.0f;
	Vector3 headPosition => transform.position + new Vector3(0, headHeight, 0);

	public float moveForce;
	public float stabilityForce;

	public float abdomenDamp = 0.5f;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();
	}

	private void FixedUpdate()
	{
		hipRb.AddForce((hipPosition - hipRb.position) * stabilityForce);
		headRb.AddForce ((headPosition - headRb.position) * stabilityForce);
	}

	// Move character to direction, do this in fixed update
	public void Drive(Vector3 direction, float amount)
	{
		// Enforce this to be used only in fixed update
	#if UNITY_EDITOR
		if (!Time.inFixedTimeStep)
			Debug.LogError("Use RagdollCharacterDriver.Drive only in FixedUpdate");				
	#endif

		if (amount > 0)
		{
			amount *= speed;

			transform.position += direction * amount;
			// hipRb.AddForce(moveForce * direction * amount);

			Debug.Log($"[{name}]: Added force ({(moveForce * direction * amount).magnitude})");

			Debug.DrawRay(hipPosition, moveForce * direction * amount, Color.cyan);


			// rigidbody.MovePosition(transform.position + direction * amount);
			// rigidbody.MoveRotation(Quaternion.LookRotation (direction, Vector3.up));
		}
	}

	public void Jump()
	{
		// Todo: test if touching walkable perimeter, and only jump if do
		rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
		Debug.Log("Ragdoll do jump");
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;

		Gizmos.DrawWireSphere(hipPosition, 0.05f);
	}

}
