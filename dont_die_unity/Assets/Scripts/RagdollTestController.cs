using UnityEngine;

public class RagdollTestController : MonoBehaviour
{
	public RagdollCharacterDriver driver;

	private void FixedUpdate()
	{
		Vector3 input = Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical");

		Vector3 direction = input.normalized;
		float distance = input.magnitude * Time.deltaTime;

		driver.Drive(direction, distance);
	}
}