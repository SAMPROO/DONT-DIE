using System;
using UnityEngine;

[RequireComponent(typeof(RagdollCharacterDriver), typeof(DamageController))]
public class PlayerController : MonoBehaviour
{
    // this should instead be set from game manager etc. when creating screen
    // use this to align movement with camera
    [SerializeField] private OrbitCameraTP orbitCamera;
    [SerializeField] private Transform orbitAnchor;

    // this also needs to be set outside
    private InputController input = new InputController(); 
	
	private RagdollCharacterDriver driver;
	private DamageController damageController;
	// [SerializeField] private CharacterHealth health;

	private PlayerHandle handle;
	public event Action<PlayerHandle> OnDie;

	[SerializeField] private float speed = 3.0f;
	[SerializeField] private Gun gun;
	[SerializeField] private Transform gunParent;

	// This is really not serializable thing, but it is injected from hudmanager or similar
	[SerializeField] private PlayerHUD hud;
 
	[Header("Health")]
	[SerializeField] private int maxHitpoints = 100;
	private int hitpoints;


	private void Awake()
	{
		driver = GetComponent<RagdollCharacterDriver>();
		damageController = GetComponent<DamageController>();
	}

	//private void Start ()
	//{
	//	// Initialize in builder scene only
	//	if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "CharacterBuilder")
	//	{
	//		Initialize(new PlayerHandle(0), "No idea");
	//	}
	//}

	public void Initialize(PlayerHandle handle, Camera camera)
	{
		this.handle = handle;

        orbitCamera = camera.GetComponent<OrbitCameraTP>();
        orbitCamera.anchor = orbitAnchor;

        // Subscribe input events
        input.Fire += Fire;
		input.Jump += driver.Jump;

		// Initialize health systems
		hitpoints = maxHitpoints;
		damageController.TakeDamage.AddListener((damage) => Hurt((int)damage)); 
	}

	private void Update()
	{
		input.UpdateController();
	}

	private void FixedUpdate()
	{
		Vector3 movement = 
			orbitCamera.baseRotation * Vector3.right * input.Horizontal
			+ orbitCamera.baseRotation * Vector3.forward * input.Vertical;

		float amount = Vector3.Magnitude(movement);

		driver.Drive(movement / amount, amount * Time.deltaTime);
	}

	private void Fire()
	{
		if (gun != null)
		{
			/*bool didShoot = */gun.Shoot();
		}
		else
		{
			// 'try-shoot-with-empty-hands' animation. Surprise: nothing happens
		}
	}

	public void Hurt(float damage)
	{
		Debug.Log($"[{name}]: Hurt ({damage})");


		hitpoints -= Mathf.RoundToInt(damage);
		hud.Hp = hitpoints;

		if (hitpoints <= 0)
		{
			OnDie?.Invoke(handle);
			Debug.Log($"[{name}]: died");
		}
	}


	private void StartCarryingGun(Gun gun)
	{
		this.gun = gun;
		gun.StartCarrying(gunParent);
	}

	private void StopCarryingGun()
	{
		gun?.StopCarrying();
		gun = null;
	}


}