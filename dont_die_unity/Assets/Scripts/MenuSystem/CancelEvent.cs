using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CancelEvent : MonoBehaviour
{
	public UnityEvent OnCancel;

	public static string cancelButtonName = "Cancel";

	private void Update()
	{
		if (Input.GetButtonDown(cancelButtonName))
			OnCancel?.Invoke();
	}
}