using UnityEngine;

public class Camera3rdPerson : MonoBehaviour
{
	public Quaternion baseRotation => transform.rotation;

	public Transform target;

	private void Start()
	{
		// This may begin game parented to target character, so that it will stay within same prefab
		transform.SetParent(null);
	}

	private void LateUpdate()
	{
		transform.position = target.position;
	}
}
