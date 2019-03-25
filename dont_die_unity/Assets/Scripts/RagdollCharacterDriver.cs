using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RagdollCharacterDriver : MonoBehaviour
{
	private new Rigidbody rigidbody;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();		
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
			rigidbody.MovePosition(transform.position + direction * amount);
			rigidbody.MoveRotation(Quaternion.LookRotation (direction, Vector3.up));
		}
	}
}
