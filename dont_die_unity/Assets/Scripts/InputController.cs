using UnityEngine;

// Button press for player input, something else for AI input
public delegate void OneOffAction();

public class InputController
{
	public float Horizontal => Input.GetAxis("Horizontal");
	public float Vertical => Input.GetAxis("Vertical");

	public event OneOffAction Jump;
	public event OneOffAction Fire;

	private KeyCode jumpKey = KeyCode.Space;
	private KeyCode fireKey = KeyCode.E;

	// This is probably not a monobehaviour, hence it needs to be updated manually
	// Either by player class or some sort InputControllerManager
	public void UpdateController()
	{
		if (Input.GetKeyDown(jumpKey))
			Jump?.Invoke();

		if (Input.GetKeyDown(fireKey))
			Fire?.Invoke();
	}
}

