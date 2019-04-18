using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class VelocityDetector : MonoBehaviour
{
	public float speed;
	public Vector3 velocity;

	private new Rigidbody rigidbody;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();	
	}

	private void FixedUpdate()
	{
		velocity = rigidbody.velocity;
		speed = velocity.magnitude;

	}
}