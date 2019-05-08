using UnityEngine;

/*
Set Rigidbodys center of mass to local position on Start
*/
[RequireComponent(typeof(Rigidbody))]
public class CenterOfMass : MonoBehaviour
{
	public Vector3 localPosition;

	private void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = localPosition;
	}

	private void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.TransformPoint(localPosition), 0.15f);
	}
}