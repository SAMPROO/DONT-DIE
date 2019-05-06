// using UnityEngine;

// public class Projectile : MonoBehaviour
// {
// 	public float speed;
// 	public bool explodeSometimes;

// 	public void Launch()
// 	{
// 		GetComponent<Rigidbody>() /// add force....
// 	}

// 	public event UnityEvent OnLaunch;
// }

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