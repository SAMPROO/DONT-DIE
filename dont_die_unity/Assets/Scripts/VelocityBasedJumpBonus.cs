
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VelocityBasedJumpBonus : MonoBehaviour, IJumpBonus
{
	public float minBonus = 0;
	public float multiplier = 1;
	float IJumpBonus.Value => Mathf.Max(minBonus, rigidbody.velocity.y * multiplier);

	private new Rigidbody rigidbody;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}
}
