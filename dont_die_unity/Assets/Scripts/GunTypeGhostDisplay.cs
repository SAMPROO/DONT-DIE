using UnityEngine;

public class GunTypeGhostDisplay : MonoBehaviour
{
	public float rotationSpeed = 180f;
	private float yRotation;
	private Quaternion startRotation;

	// Position to anchor in parents local space
	public Vector3 anchor;
	public Vector3 localPosition;

	private Vector3 finalPosition => transform.parent.TransformPoint(anchor) + localPosition;

	private void Start()
	{
		startRotation = transform.localRotation;
	}

	private void Update()
	{
		transform.position = finalPosition;
		yRotation += rotationSpeed * Time.deltaTime;
		transform.rotation = Quaternion.Euler(0, yRotation, 0) * startRotation;
	}

	private void OnValidate()
	{
		transform.position = finalPosition;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.parent.TransformPoint(anchor), 0.2f);
	}
}