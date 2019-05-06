/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using UnityEngine;

[RequireComponent(typeof(DamageController))]
public class PlayerController : MonoBehaviour
{
    // These are set on Inititialize()
	private PlayerHandle handle;
	private OrbitCameraTP cameraRig;
    private IInputController input;

    // This is really not serializable thing, but it is injected from hudmanager or similar
    [SerializeField] private PlayerHud hud;

	private RagdollRig ragdoll;
	private DamageController damageController;

	public event Action<PlayerHandle> OnDie;

	// This is used to access characters instantiated material and set color
	public Renderer characterRenderer;

	[SerializeField] private Equipment gun;
	[SerializeField] private Transform gunParent;
    [SerializeField] private LayerMask gunLayer;

    [SerializeField] private float pickUpRange;

	[Header("Health")]
	[SerializeField] private int maxHitpoints = 100;
	private int hitpoints;

	[SerializeField] private Transform rightHandTransform;
	[SerializeField] private Transform leftHandTransform;
	public float handAimMultiplier = 1f;
	public float minHandsAngle = -45f;
	public float maxHandsAngle = 45f;

	public bool Grounded => ragdoll.Grounded;

	private void Awake()
	{
		ragdoll = GetComponentInChildren<RagdollRig>();
		ragdoll.transform.position = transform.position;

		damageController = GetComponent<DamageController>();
	}

	// This version also sets color.
    public void Initialize(
    	PlayerHandle handle, 
    	OrbitCameraTP camera, 
    	IInputController inputCnt,
    	Color color,
    	PlayerHud hud
	){
		characterRenderer.material.color = color;
		this.hud = hud;
		Initialize(handle, camera, inputCnt);
	}

    public void Initialize(
    	PlayerHandle handle,
    	OrbitCameraTP camera,
    	IInputController inputCnt
	){
        input = inputCnt;

        this.handle = handle;

        cameraRig = camera;
        cameraRig.anchor = transform;

        // Subscribe input events
        input.Fire += Fire;
		input.Jump += ragdoll.Jump;
		input.PickUp += ToggleCarryGun;
		input.ToggleRagdoll += ToggleRagdoll;
        
        // Initialize health systems
        hitpoints = maxHitpoints;
		damageController.TakeDamage.AddListener((damage) => Hurt((int)damage)); 
	}

	public void Initialize(
    	PlayerHandle handle,
    	OrbitCameraTP cameraRig,
    	IInputController input,
    	PlayerHud hud
	){
		this.hud = hud;
		Initialize(handle, cameraRig, input);
	}

	private void OnDestroy()
	{
		Destroy(cameraRig.gameObject);
	}

	private void Update() 
	{
		input.UpdateController();
		
		ragdoll.ControlLeftHand = input.ActivateLeftHand;
		ragdoll.ControlRightHand = input.ActivateRightHand;

		// Only grab if we are not carrying gun
		ragdoll.CanGrab = gun == null;
	}

	private Vector3 lastMoveDirection = Vector3.forward;

	private void FixedUpdate()
	{
		Vector3 movement = 
			cameraRig.BaseRotation * Vector3.right * input.Horizontal
			+ cameraRig.BaseRotation * Vector3.forward * input.Vertical;

		float amount = Vector3.Magnitude(movement);
		Vector3 moveDirection = movement / amount;

		if (amount > 0)
			lastMoveDirection = moveDirection;

		bool doFocus = input.ActivateLeftHand || input.ActivateRightHand;
		Vector3 lookDirection = 
			doFocus ? 
			cameraRig.BaseRotation * Vector3.forward : 
			lastMoveDirection;
		
		ragdoll.MoveWithVelocity(
			lastMoveDirection,
			lookDirection, 
			amount);

		float handsAngle = Mathf.Clamp(cameraRig.AimAngle * handAimMultiplier, minHandsAngle, maxHandsAngle);
		ragdoll.SetHandsAimAngle(handsAngle);
	}

	private void Fire()
	{
		if (gun != null)
		{
			gun.Use();
			hud.SetAmmo(gun.Ammo);
		}
		else
		{
			// 'try-shoot-with-empty-hands' animation. Surprise: nothing happens
		}
	}

	public void Hurt(float damage)
	{
		hitpoints -= Mathf.RoundToInt(damage);
		hud.SetHp(hitpoints);

		if (hitpoints <= 0)
		{
			OnDie?.Invoke(handle);
		}
	}

	private void ToggleRagdoll()
	{
		ragdoll.hasControl = !ragdoll.hasControl;
	}

	// Use until GunInfo gets properly implemented
	public GunInfo BACKUPGunInfo;

    private void ToggleCarryGun()
	{
		// Drop if we have gun
		if (gun != null)
		{
			gun?.StopCarrying();
			gun = null;

			hud.SetEquipped(null);

			return;
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
               	gun = hitColliders[i].GetComponentInParent<Equipment>();
               	if (gun != null)
               	{
	            	gun.StartCarrying(gunParent.GetComponent<Rigidbody>(), -90);

	            	var gunInfoObject = gun.GetComponent<GunInfoObject>();
	            	if (gunInfoObject != null)
					{
						hud.SetEquipped(gunInfoObject.gunInfo);
					}	
					else 
					{
						hud.SetEquipped(BACKUPGunInfo);
					}

					hud.SetAmmo(gun.Ammo);

	            	return;
               	}
            }
        }
	}
}