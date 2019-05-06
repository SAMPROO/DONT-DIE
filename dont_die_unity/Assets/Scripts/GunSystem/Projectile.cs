using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	public float lauchSpeed = 5f;

	public void Launch()
	{
		GetComponent<Rigidbody>().AddForce(transform.forward * lauchSpeed, ForceMode.VelocityChange);
	}

	public UnityEvent OnLaunch;
}

// [RequireComponent(typeof(Projectile))]
// public class GrowingDuck : MonoBehaviour
// {
// 	void Awake()
// 	{
// 		GetComponent<Projectile>().OnLaunch += StartGrowing();
// 	}
// }


// public interface IProjectile
// {
// 	void Launch();
// }