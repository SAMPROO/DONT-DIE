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

    [SerializeField] private float speed = 3.0f;
	[SerializeField] private Equipment gun;

	[SerializeField] private Transform gunParent;
    [SerializeField] private LayerMask gunLayer;

    [SerializeField] private float pickUpRange;

	[Header("Health")]
	[SerializeField] private int maxHitpoints = 100;
	private int hitpoints;

	[SerializeField] private Transform rightHandTransform;
	[SerializeField] private Transform leftHandTransform;

	public bool Grounded => ragdoll.Grounded;

	private void Awake()
	{
		ragdoll = GetComponentInChildren<RagdollRig>();
		ragdoll.transform.position = transform.position;

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
		input.Jump += ragdoll.Jump;
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

		ragdoll.SetRightHandControl(input.Focus);

		// ragdoll.rightHand.active = input.Focus;
		// ragdoll.leftHand.active = input.Focus;
	}

	private void FixedUpdate()
	{
		Vector3 movement = 
			cameraRig.baseRotation * Vector3.right * input.Horizontal
			+ cameraRig.baseRotation * Vector3.forward * input.Vertical;

		float amount = Vector3.Magnitude(movement);
		ragdoll.Move(movement / amount, amount * Time.deltaTime);

		// transform.position = ragdoll.transform.position;
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

    private void TogglePickup()
	{
        Equipment lastGun = null;

        Debug.Log("Gun:::::" + gun);

        // Drop if we have gun
		if (gun != null)
		{
			lastGun = StopCarryingGun();
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
               	if (gun != lastGun)
               	{
	            	gun.StartCarrying(gunParent);
	            	return;
               	}
                else gun = null;
            }
        }
	}

	private Equipment StopCarryingGun()
	{
		gun?.StopCarrying();
        var lastGun = gun;
		gun = null;
        return lastGun;
	}

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //     Gizmos.DrawWireSphere(bodyCenterPosition.position, transform.localScale.y);
    // }
}

public class SmoothFloat
{
	private int index;
	private float []array;
	public readonly int length;

	// Set min and max to clamp values when putting in
	public SmoothFloat (int arrayLength = 10, float? min = null, float? max = null)
	{
		array = new float [arrayLength];
		index = 0;
		length = arrayLength;

		// Make one null check here, so we don't have to check everytime
		if (min == null || max == null)
		{
			Put = PutImplement;	
		}
		else 
		{
			float _min = min ?? 0.0f;
			float _max = max ?? 1.0f;
			_max = Mathf.Max(_min, _max);

			Put = (value) =>
			{
				value = Mathf.Clamp(value, _min, _max);
				PutImplement(value);
			};
		}
	}

	public System.Action<float> Put;

	private void PutImplement(float value)
	{
		array[index] = value;
		index = (index + 1) % length;
	}

	public float Get()
	{
		return array.Average();	
	}
}