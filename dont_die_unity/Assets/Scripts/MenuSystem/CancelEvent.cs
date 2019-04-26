using UnityEngine;
using UnityEngine.Events;

public class CancelEvent : MonoBehaviour
{
	public UnityEvent OnCancel;

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			OnCancel?.Invoke();
		}
	}
}