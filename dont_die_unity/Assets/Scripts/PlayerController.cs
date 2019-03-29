using UnityEngine;

[RequireComponent(typeof(RagdollCharacterDriver))]
public class PlayerController : MonoBehaviour
{
	// this should instead be set from game manager etc. when creating screen
	// use this to align movement with camera
	[SerializeField]
	private new Camera3rdPerson camera;

	// this also needs to be set outside
	private InputController inputController = new InputController(); 

	private RagdollCharacterDriver driver;


	[SerializeField] private float speed = 3.0f;
	[SerializeField] private Gun gun;
	[SerializeField] private Transform gunHandle;

	private void Awake()
	{
		driver = GetComponent<RagdollCharacterDriver>();
	}

	private void Start()
	{
		inputController.Fire += Fire;
		inputController.Jump += driver.Jump;


		StartCarryingGun(gun);
	}


	private void Update()
	{
		inputController.UpdateController();
	}

	public void FixedUpdate()
	{
		Vector3 movement = 
			camera.baseRotation * Vector3.right * inputController.Horizontal()
			+ camera.baseRotation * Vector3.forward * inputController.Vertical();

		float amount = Vector3.Magnitude(movement);

		driver.Drive(movement / amount, speed * amount * Time.deltaTime);
	}

	private void Fire()
	{
		if (gun != null)
		{
			bool didShoot = gun.Shoot();
			// todo: do recoil stuff
		}
		else
		{
			// 'try-shoot-with-empty-hands' animation. Surprise: nothing happens
		}
	}


	private void StartCarryingGun(Gun gun)
	{
		this.gun = gun;
		gun.StartCarrying(gunHandle);
	}

	private void StopCarryingGun()
	{
		gun?.StopCarrying();
		gun = null;
	}

}