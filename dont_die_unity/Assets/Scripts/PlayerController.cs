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
    [NonSerialized] public PlayerHudScript hud;

    // These are fetched with GetComponent family
	public RagdollRig ragdoll;
	private DamageController damageController;

	// Scorable events
	public event Action<PlayerHandle> OnDie;
    public event Action<PlayerHandle, int> OnChangeHealth;
    public event Action<PlayerHandle, int> OnAreaScore;

    // This is used to access character model's instantiated material and set color
    public Renderer characterRenderer;

	[SerializeField] private BaseGun gun;
	[SerializeField] private Transform gunParent;
    [SerializeField] private LayerMask gunLayer;

    [SerializeField] private float pickUpRange;

	[Header("Health")]
	[SerializeField] private int maxHitpoints = 100;
    public int hitpoints { get; private set; }

	[SerializeField] private Transform rightHandTransform;
	[SerializeField] private Transform leftHandTransform;
	public float handAimMultiplier = 1f;
	public float minHandsAngle = -45f;
	public float maxHandsAngle = 45f;

	public bool Grounded => ragdoll.Grounded;

    //by irtsa
    private bool controlRightHand;
    public void SetImmortal(bool value) 
        => immortalDamageMultiplier = value ? 0 : 1;

    private float immortalDamageMultiplier;
    //by irtsa

    private void Awake()
	{
		ragdoll = GetComponentInChildren<RagdollRig>();
		ragdoll.transform.position = transform.position;

		damageController = GetComponent<DamageController>();
	}

	// Set mandatory values to playercontroller
    public void Initialize(
    	PlayerHandle handle, 
    	OrbitCameraTP cameraRig, 
    	IInputController input,
    	Color color,
    	PlayerHudScript hud
	){
        this.handle = handle;
		this.hud 	= hud;
        this.input 	= input;

        this.cameraRig = cameraRig;
        cameraRig.anchor = transform;

        // Color clear designates no change
		if (color != Color.clear)
			characterRenderer.material.color = color;
		
        // Initialize health systems
        hitpoints = maxHitpoints;
		damageController.TakeDamage.AddListener((damage) => Hurt((int)damage)); 
		hud.SetHp(maxHitpoints);

        // Subscribe input events
        input.Fire += Fire;
		input.Jump += ragdoll.Jump;
		input.PickUp += ToggleCarryGun;
		input.ToggleRagdoll += ToggleRagdoll;
        
	}

	private void Update() 
	{
        
        input.UpdateController();

        controlRightHand = input.ActivateRightHand || (gun != null && input.Focus);

        if (gun != null && ragdoll.ControlRightHand != controlRightHand)
            gun.SetColliders(input.ActivateRightHand);


        ragdoll.ControlLeftHand = input.ActivateLeftHand;    
        ragdoll.ControlRightHand = controlRightHand;

        

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

		bool doFocus = input.ActivateLeftHand || controlRightHand; //right hand has a variable because left trigger works as ActivateRightHand too if carrying Gun
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
	}

	public void Hurt(float damage)
	{
        damage *= immortalDamageMultiplier;
		hitpoints -= Mathf.RoundToInt(damage);
		hud.SetHp(hitpoints);

        OnChangeHealth?.Invoke(handle, Mathf.RoundToInt(damage));

		if (hitpoints <= 0)
		{
			OnDie?.Invoke(handle);

			ragdoll.hasControl = false;
			ragdoll.ControlRightHand = false;
			ragdoll.ControlLeftHand = false;
			ragdoll.Stop();
		}
	}

    // Use this to get points from Gameworld locations
    // (e.g king of the hill)
    public void GetAreaScore(int amount)
    {
        OnAreaScore?.Invoke(handle, amount);
    }

	public void Stop()
	{
		ragdoll.ControlRightHand = false;
		ragdoll.ControlLeftHand = false;
		ragdoll.Stop();

		// Unsubscribe events
        input.Fire -= Fire;
		input.Jump -= ragdoll.Jump;
		input.PickUp -= ToggleCarryGun;
		input.ToggleRagdoll -= ToggleRagdoll;
	}

	private void ToggleRagdoll()
	{
		ragdoll.hasControl = !ragdoll.hasControl;
	}

    private void ToggleCarryGun()
	{
		// Drop if we have gun
		if (gun != null)
		{
			gun?.StopCarrying();
			gun = null;

			hud.SetEquippedIcon(null);

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
               	gun = hitColliders[i].GetComponentInParent<BaseGun>();
               	if (gun != null)
               	{
	            	gun.StartCarrying(gunParent.GetComponent<Rigidbody>(), -90);
					hud.SetEquippedIcon(gun.HudIcon);
					hud.SetAmmo(gun.Ammo);

	            	return;
               	}
            }
        }
	}
}