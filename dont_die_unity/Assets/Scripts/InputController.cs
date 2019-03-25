using UnityEngine;

// Button press for player input, something else for AI input
public delegate void OneOffAction();

public class InputController
{
	public float Horizontal() => Input.GetAxis("Horizontal");
	public float Vertical() => Input.GetAxis("Vertical");

	public event OneOffAction jump;
	public event OneOffAction fire;
}