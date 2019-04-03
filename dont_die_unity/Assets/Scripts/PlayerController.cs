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
	[SerializeField] private IWeapon gun;
	[SerializeField] private Transform gunParent;
    [SerializeField] private LayerMask gunLayer;
    [SerializeField] private Transform bodyCenterPosition;

    [SerializeField] private bool collisionWithGun;

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
    public void Initialize(PlayerHandle handle, Camera camera, InputController inputCnt)
    {
        input = inputCnt;

        this.handle = handle;

        orbitCamera = camera.GetComponent<OrbitCameraTP>();
        orbitCamera.anchor = orbitAnchor;

        // Subscribe input events
        input.Fire += Fire;
		input.Jump += driver.Jump;
		input.PickUp += StartCarryingGun;
        
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
			gun.Use();
		}
		else
		{
			// 'try-shoot-with-empty-hands' animation. Surprise: nothing happens
		}
	}

	public void Hurt(float damage)
	{


		hitpoints -= Mathf.RoundToInt(damage);
		hud.Hp = hitpoints;

		if (hitpoints <= 0)
		{
			OnDie?.Invoke(handle);
		}
	}

    //Once the arm is working, we can create Physics.OverlapSphere infront of hand 
    private void StartCarryingGun()
	{
        IWeapon newGun = null;

        float sphereRadius = transform.localScale.y;

        Collider[] hitColliders = Physics.OverlapSphere(bodyCenterPosition.position, sphereRadius, gunLayer);

        float distance = sphereRadius;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            float gunDistance = Vector3.Distance(hitColliders[i].transform.position, bodyCenterPosition.position);

            if (gunDistance < distance)
            {
                newGun = hitColliders[i].GetComponentInParent<IWeapon>();
            }
        }

        if (gun == null && newGun != null)
        {
            this.gun = newGun;
            gun.StartCarrying(gunParent);
        }
        else if (gun != null && newGun != null)
        {
            StopCarryingGun();
        }
	}

	private void StopCarryingGun()
	{
		gun?.StopCarrying();
		gun = null;
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(bodyCenterPosition.position, transform.localScale.y);
    }
}