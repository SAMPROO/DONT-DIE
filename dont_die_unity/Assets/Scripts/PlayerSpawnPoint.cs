using UnityEngine;

[ExecuteInEditMode]
public class PlayerSpawnPoint : MonoBehaviour
{
	public Vector3 Position => transform.position;
	public Vector3 Forward => transform.forward;
	public Quaternion Orientation {
		get {
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward = forward.normalized;

			return Quaternion.LookRotation(forward, Vector3.up);
		}
	}

	private void Update()
	{
		// Snap to horizontal orientation
		transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
	}
}