using UnityEngine;

[RequireComponent(typeof(RagdollCharacterDriver))]
public class PlayerController : MonoBehaviour
{
	// this should instead be set from game manager etc. when creating screen
	// use this to align movement with camera
	[SerializeField]
	private new Camera3rdPerson camera;

	// this also needs to be set outside
	private InputController input = new InputController(); 

	private RagdollCharacterDriver driver;


	[SerializeField] private float speed = 3.0f;


	private void Awake()
	{
		driver = GetComponent<RagdollCharacterDriver>();
	}

	public void FixedUpdate()
	{
		Vector3 movement = 
			camera.baseRotation * Vector3.right * input.Horizontal()
			+ camera.baseRotation * Vector3.forward * input.Vertical();

		float amount = Vector3.Magnitude(movement);

		driver.Drive(movement / amount, speed * amount * Time.deltaTime);
	}
}