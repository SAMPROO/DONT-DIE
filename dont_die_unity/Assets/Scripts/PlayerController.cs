/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using UnityEngine;

[RequireComponent(typeof(RagdollCharacterDriver), typeof(DamageController))]
public class PlayerController : MonoBehaviour
{
    // These are set on Inititialize()
	private PlayerHandle handle;
	private OrbitCameraTP cameraRig;
    private IInputController input;

    // This is really not serializable thing, but it is injected from hudmanager or similar
    [SerializeField] private PlayerHUD hud;

	private RagdollCharacterDriver driver;
	private DamageController damageController;

	public event Action<PlayerHandle> OnDie;

	[SerializeField] private float speed = 3.0f;
	[SerializeField] private IWeapon gun;
	[SerializeField] private Transform gunParent;
    [SerializeField] private LayerMask gunLayer;
    // [SerializeField] private Transform bodyCenterPosition;
    [SerializeField] private float pickUpRange;

    [SerializeField] private bool collisionWithGun;

	[Header("Health")]
	[SerializeField] private int maxHitpoints = 100;
	private int hitpoints;

	[SerializeField] private Transform rightHandTransform;
	[SerializeField] private Transform leftHandTransform;

	public bool Grounded => driver.Grounded;

	private void Awake()
	{
		driver = GetComponent<RagdollCharacterDriver>();
		damageController = GetComponent<DamageController>();
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
		input.PickUp += TogglePickup;
        
        // Initialize health systems
        hitpoints = maxHitpoints;
		damageController.TakeDamage.AddListener((damage) => Hurt((int)damage)); 
	}

	[Obsolete("Use Version that sets OrbitCameraTP directly")]
    public void Initialize(PlayerHandle handle, Camera camera, IInputController controller)
    {
    	// just redirect to new one
    	Initialize (handle, camera.GetComponent<OrbitCameraTP>(), controller);
	}

	private void Update() 
	{
		input.UpdateController();
		driver.ControlRightHand = input.Focus;
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
    private void TogglePickup()
	{
		// Drop if we have gun
		if (gun != null)
		{
			StopCarryingGun();
		}

		// Check if new gun is nearby and pick int. Use main transform now, since hands are not controlled
        // Vector3 handPosition = rightHandTransform.position;
        Vector3 handPosition = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(
        	handPosition,
        	pickUpRange,
        	gunLayer
    	);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            float distanceToGun = Vector3.Distance(
            	hitColliders[i].transform.position,
            	handPosition
        	);

            if (distanceToGun < pickUpRange)
            {
            	// TODO: not likey this at all. Things in gun (or rather pickup) layer should have right component in itself and not parent
               	gun = hitColliders[i].GetComponentInParent<IWeapon>();
               	if (gun != null)
               	{
	            	gun.StartCarrying(gunParent);
	            	return;
               	}
            }
        }


		/*
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
        */
	}

	private void StopCarryingGun()
	{
		gun?.StopCarrying();
		gun = null;
	}

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //     Gizmos.DrawWireSphere(bodyCenterPosition.position, transform.localScale.y);
    // }
}