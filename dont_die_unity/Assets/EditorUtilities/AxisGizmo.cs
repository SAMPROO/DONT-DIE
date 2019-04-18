using UnityEngine;

public class AxisGizmo : MonoBehaviour
{
	[SerializeField] private bool selectedOnly = true;
	[SerializeField] private float size = 0.2f;

	private void OnDrawGizmos()
	{
		if (!selectedOnly)
			Draw();
	}

	private void OnDrawGizmosSelected()
	{
		if (selectedOnly)
			Draw();
	}

	private void Draw()
	{
		var position = transform.position;

		var offset = size * transform.right;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(position - offset, position + offset);

		offset = size * transform.up;
		Gizmos.color = Color.green;	
		Gizmos.DrawLine(position - offset, position + offset);

		offset = size * transform.forward;
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(position - offset, position + offset);
	}
}