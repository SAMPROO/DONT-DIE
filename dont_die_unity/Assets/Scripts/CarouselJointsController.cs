using UnityEngine;

public class CarouselJointsController : MonoBehaviour
{
	public float breakForce = float.PositiveInfinity;
	public float massScale = 1f;
	public float connectedMassScale = 1f;

	private void Start()
	{
		Set<FixedJoint>();
	}

	private void Set<T> () where T : Joint
	{
		foreach (var joint in GetComponents<T>())
		{
			joint.breakForce = breakForce;
			joint.massScale = massScale;
			joint.connectedMassScale = connectedMassScale;
		}
	}
}