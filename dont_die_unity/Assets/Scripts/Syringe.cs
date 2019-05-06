using UnityEngine;

public class Syringe : MonoBehaviour
{
	[Range(0,1), SerializeField] private float fill;
	public float Fill
	{
		get => fill;
		set
		{
			fill = value;
			SetFillDisplay();
		}
	}
	public float maxFill;

	// public Color fillColor;
	public Gradient fillColor;
	public Transform fillTransform;
	private Renderer fillRenderer;

	public Transform pressTransform;

	private void SetFillDisplay()
	{
		var fillScale = fillTransform.localScale;
		fillScale.z = fill;
		fillTransform.localScale = fillScale;

		fillRenderer.material.color = fillColor.Evaluate(fill);

		var pressPosition = pressTransform.localPosition;
		pressPosition.z = -1 * maxFill * fill;
		pressTransform.localPosition = pressPosition;
	}

	private void OnValidate()
	{
		fillRenderer = fillTransform.GetComponent<Renderer>();
		SetFillDisplay();
	}
}