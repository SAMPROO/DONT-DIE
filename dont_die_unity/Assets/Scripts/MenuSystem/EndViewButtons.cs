using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EndViewButtons : MonoBehaviour
{
	public UnityEvent GoToMain;

	[SerializeField] private Button goToMainButton;

	private void Start()
	{
		goToMainButton.onClick.AddListener(() => GoToMain?.Invoke());
	}

}