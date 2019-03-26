using UnityEngine;

public class Gun : MonoBehaviour
{
	public bool Shoot()
	{
		// Check if enough ammo, spawn projectile, do vfx and sfx etc.
		Debug.Log("Gun says bang");

		// Return true if there was enough ammo, so shooter can react
		// maybe return eg. Vector3? representing recoil
		return true;
	}

	public void StartCarrying(Transform carrier)
	{
		// Turn off physics etc.
		transform.SetParent(carrier);
		Debug.Log("Gun hops on");
	}

	public void StopCarrying()
	{
		// Turn on physics etc.
		transform.SetParent(null);
		Debug.Log("Gun thrown away");
	}

}