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
    [SerializeField] private PlayerHUD hud;

	private RagdollRig ragdoll;
	private DamageController damageController;

	public event Action<PlayerHandle> OnDie;

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
	public float handAimMultiplier = -1f;

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
    	Color color
	){
		characterRenderer.material.color = color;
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

	private void Update() 
	{
		input.UpdateController();
		ragdoll.ControlLeftHand = input.ActivateLeftHand;
		ragdoll.ControlRightHand = input.ActivateRightHand;
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

		ragdoll.SetHandsAimAngle(cameraRig.AimAngle);// * handAimMultiplier);
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

	private void ToggleRagdoll()
	{
		ragdoll.HasControl = !ragdoll.HasControl;
	}

    private void ToggleCarryGun()
	{
		// Drop if we have gun
		if (gun != null)
		{
			gun?.StopCarrying();
			gun = null;
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
        	Debug.Log(hitColliders[i].name);


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
	            	gun.StartCarrying(gunParent);
	            	return;
               	}
            }
        }
	}
}