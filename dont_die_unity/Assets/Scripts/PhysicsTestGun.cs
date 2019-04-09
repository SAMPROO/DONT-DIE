using UnityEngine;

public class PhysicsTestGun : MonoBehaviour, IWeapon
{
	public void Use()
	{
		
	}

	public void StartCarrying(Transform carrier)
	{
		// Turn off physics etc.
		transform.SetParent(carrier);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // rb.isKinematic = true;

        // isCarried = true;
	}

	public void StopCarrying()
	{
		// Turn on physics etc.
		transform.SetParent(null);
        // rb.isKinematic = false;

        // isCarried = false;
	}
}