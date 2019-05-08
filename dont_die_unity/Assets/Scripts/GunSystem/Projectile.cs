using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	public float launchSpeed = 5f;

	public void Launch()
	{
        OnLaunch?.Invoke();

        GetComponent<Rigidbody>().AddForce(transform.forward * launchSpeed, ForceMode.VelocityChange);
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