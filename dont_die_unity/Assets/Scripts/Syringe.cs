using UnityEngine;

public class Syringe : MonoBehaviour
{
	[Range(0,1)]public float fill;
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
		pressPosition.z = maxFill * fill;
		pressTransform.localPosition = pressPosition;
	}

	private void OnValidate()
	{
		fillRenderer = fillTransform.GetComponent<Renderer>();
		SetFillDisplay();
	}
}