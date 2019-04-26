using UnityEngine;

// Merely a tag to find all these when game loads
public class PlayerSpawnPoint : MonoBehaviour
{
	public Vector3 Position => transform.position;
	public Quaternion Orientation {
		get {
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward = forward.normalized;

			return Quaternion.LookRotation(forward, Vector3.up);
		}
	}

	// private void Awake()
	// {
	// 	gameObject.SetActive(false);
	// }
}