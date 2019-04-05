using System;
using UnityEngine;

[RequireComponent(typeof(RagdollCharacterDriver), typeof(DamageController))]
public class PlayerController : MonoBehaviour
{
    // These are set on Inititialize()
	private OrbitCameraTP cameraRig;
    private IInputController input;

    #if UNITY_EDITOR
    // Use these to set some values manually in editor only
    [Header ("Editor Only")]
    [SerializeField] private EditorInput editorInput = null;
    [SerializeField] private OrbitCameraTP cameraThing = null;
    [SerializeField] private bool initializeOnStart = false;
    [Space]
	#endif

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

		#if UNITY_EDITOR
		if (editorInput != null)
		{
			input = editorInput;
			if (cameraThing != null)
			{
				cameraThing.SetInputController(editorInput);
			}
		}

		if (initializeOnStart)
		{
			Initialize(new PlayerHandle(0), cameraThing, editorInput);
		}
		#endif
	}

    public void Initialize(PlayerHandle handle, OrbitCameraTP camera, IInputController inputCnt)
    {
        input = inputCnt;

        this.handle = handle;

        cameraRig = camera;
        cameraRig.anchor = transform;

        // Subscribe input events
        input.Fire += Fire;
		input.Jump += driver.Jump;
		input.PickUp += StartCarryingGun;
        
        // Initialize health systems
        hitpoints = maxHitpoints;
		damageController.TakeDamage.AddListener((damage) => Hurt((int)damage)); 
	}

	[Obsolete("Use Version that sets OrbitCameraTP directly")]
    public void Initialize(PlayerHandle handle, Camera camera, IInputController inputCnt)
    {
    	Debug.Log("Old Initialize Called");

        input = inputCnt;

        this.handle = handle;

        cameraRig = camera.GetComponent<OrbitCameraTP>();
        cameraRig.anchor = transform;

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
			cameraRig.baseRotation * Vector3.right * input.Horizontal
			+ cameraRig.baseRotation * Vector3.forward * input.Vertical;

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