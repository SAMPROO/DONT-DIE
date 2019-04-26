using UnityEngine;
using UnityEngine.UI;

/*
This class provides way of nicely formatting all entries in credits
view from single settings.
*/

public class CreditsFormatter : MonoBehaviour
{
	[System.Serializable]
	private class CreditEntry
	{
		public string firstName;
		public string lastName;
		public string job;
	}

	[SerializeField] private CreditEntry [] entries;
	[SerializeField] private Transform textObjectsParent;
	[SerializeField] private Text [] textObjects;

	[Header("Text Settings")]
	public Font font;
	public FontStyle fontStyle;
	public int fontSize;
	public int lineSpacing;
	public Color textColor;
	public HorizontalWrapMode horizontalOverflow;
	public VerticalWrapMode verticalOverflow;

	private void OnValidate()
	{
		EnsureTextObjectCount();
		int count = entries.Length;
		for (int i = 0; i < count; i++)
		{
			SetFormattedText(entries[i], textObjects[i]);
			SetTextSettings(textObjects[i]);
		}
	}

	private static void SetFormattedText(CreditEntry entry, Text textObject)
	{
		string formattedText = $"{entry.firstName} {entry.lastName} - {entry.job}";
		textObject.text = formattedText;
		textObject.name = $"{entry.firstName}";
	}

	private Text CreateTextObjectWithSettings()
	{
		Text textObject = new GameObject(
				"No name yet", 
				typeof(RectTransform), 
				typeof(CanvasRenderer)
			).AddComponent<Text>();

		SetTextSettings(textObject);

		return textObject;
	}

	private void SetTextSettings(Text textObject)
	{
		textObject.font = font;
		textObject.fontStyle = fontStyle;
		textObject.fontSize = fontSize;
		textObject.lineSpacing = lineSpacing;
		textObject.color = textColor;
		textObject.horizontalOverflow = horizontalOverflow;
		textObject.verticalOverflow = verticalOverflow;

		// Add more if wanted
	}

	private void EnsureTextObjectCount()
	{
		int entryCount = entries.Length;

		// We're good
		if (textObjects.Length == entryCount)
			return;

		// Too few, add more
		if (textObjects.Length < entryCount)
		{
			var newTextObjects = new Text[entryCount];

			for (int i = 0; i < textObjects.Length; i++)
			{
				newTextObjects[i] = textObjects[i];
			}

			for (int i = textObjects.Length; i < entryCount; i++)
			{
				newTextObjects[i] = CreateTextObjectWithSettings();
				newTextObjects[i].transform.parent = textObjectsParent;
			}

			textObjects = newTextObjects;
		}

		// Too many, remove some
		if (textObjects.Length > entryCount)
		{
			var newTextObjects = new Text[entryCount];

			for (int i = 0; i < entryCount; i++)
			{
				newTextObjects[i] = textObjects[i];
			}

			for (int i = entryCount; i < textObjects.Length; i++)
			{
				Destroy(textObjects[i].gameObject);
			}

			textObjects = newTextObjects;
		}
	}

}
