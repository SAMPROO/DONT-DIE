/*
Sampo's Game Company
Leo Tamminen
*/

using System;
using System.Collections;
using UnityEngine;

[RequireComponent(
	typeof(RagdollRig),
	typeof(DamageController),
	typeof(StatusController))]
public class PlayerController : MonoBehaviour
{
    // These are set on Inititialize()
	public PlayerHandle handle;
	private OrbitCameraTP cameraRig;
    private IInputController input;
    [NonSerialized] public PlayerHudScript hud;

    // These are fetched with GetComponent family
	public RagdollRig ragdoll {get; private set;} 
	private DamageController damageController;
	private StatusController statusController;

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
	private Vector3 lastMoveDirection = Vector3.forward;

    //by irtsa
    public bool isAlive = true;
    private bool controlRightHand;
    public bool isImmortal { get; private set; }
    public void SetImmortal(bool value)
    {
    	isImmortal = value;
        immortalDamageMultiplier = value ? 0 : 1;
    }

    // 0 is immortal 1 is mortal, this float is a damage multiplier
    private float immortalDamageMultiplier = 1;

    private Vector3 mySpawnPoint;
    private Vector3 mySpawnRot;
    //by irtsa

    private void Awake()
	{
		ragdoll = GetComponent<RagdollRig>();
		damageController = GetComponent<DamageController>();
		statusController = GetComponent<StatusController>();
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

		// These should be default values on instantiate instead of explicitly
		// setting them here
		UnSpawn();
	}

	public void ResetPlayer()
	{
		hitpoints = maxHitpoints;
        hud.SetHp(hitpoints);
        // TODO also reset damagecontroller propbably
        statusController.ResetStatus();
	}

	public void Spawn(Vector3 position, Vector3 lookDirection)
	{
        mySpawnPoint = position;
        mySpawnRot = lookDirection;
		ragdoll.SetPosition (position);	
		ragdoll.SetDirection(lookDirection);
		// this needs to be set since stupid complications in move system
		lastMoveDirection = lookDirection;
		ragdoll.ResetPose();
        // by irtsa
        ragdoll.ResetVelocity();
        //by irtsa

		characterRenderer.enabled = true;
		ragdoll.SetActive(true);

		cameraRig.SetLookDirection (lookDirection);
	}

	public void UnSpawn()
	{
		characterRenderer.enabled = false;
		ragdoll.SetActive(false);
	}

    private IEnumerator RespawnRoutine()
    {
        ragdoll.hasControl = false;
        ragdoll.ControlRightHand = false;
        ragdoll.ControlLeftHand = false;
        
        //yield return new WaitForSeconds(2);
        //UnSpawn();

        yield return new WaitForSeconds(2);
        ResetPlayer();
        Spawn(mySpawnPoint, mySpawnRot);
        yield return new WaitForSeconds(1);
        ragdoll.hasControl = true;
        ragdoll.ControlRightHand = true;
        ragdoll.ControlLeftHand = true;

        
        isAlive = true;
        yield break;
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

		// Ragdoll is moved in
        transform.position =
        	Vector3.Lerp(transform.position, ragdoll.CurrentPosition, 0.5f);
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

		if (hitpoints <= 0 && isAlive)
		{
            isAlive = false;
			OnDie?.Invoke(handle);
            StartCoroutine(RespawnRoutine());

            /*
            Spawn(mySpawnPoint, mySpawnRot);
            ResetPlayer();      
			ragdoll.hasControl = false;
			ragdoll.ControlRightHand = false;
			ragdoll.ControlLeftHand = false;
			ragdoll.Stop();
            */
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