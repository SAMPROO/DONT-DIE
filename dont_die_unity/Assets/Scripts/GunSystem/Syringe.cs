using UnityEngine;

public class Syringe : MonoBehaviour
{
	[Range(0,1), SerializeField] private float fill;
	public float Fill
	{
		get => fill;
		set
		{
			fill = Mathf.Clamp01(value);
			SetFillDisplay();
		}
	}

	public Gradient fillColor;
	public Transform fillTransform;
	private Renderer fillRenderer;

	public Transform pressTransform;
	public float maxPressMovement;

	private void Start()
	{
		fillRenderer = fillTransform.GetComponent<Renderer>();
		SetFillDisplay();
	}
	
	private void SetFillDisplay()
	{
		var fillScale = fillTransform.localScale;
		fillScale.z = fill;
		fillTransform.localScale = fillScale;

		fillRenderer.material.color = fillColor.Evaluate(fill);

		var pressPosition = pressTransform.localPosition;
		pressPosition.z = -1 * maxPressMovement * fill;
		pressTransform.localPosition = pressPosition;
	}
}