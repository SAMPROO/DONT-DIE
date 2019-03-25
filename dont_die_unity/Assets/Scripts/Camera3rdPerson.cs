using UnityEngine;

public class Camera3rdPerson : MonoBehaviour
{
	public Quaternion baseRotation => transform.rotation;

	public Transform target;

	public void LateUpdate()
	{
		transform.position = target.position;
	}
}
