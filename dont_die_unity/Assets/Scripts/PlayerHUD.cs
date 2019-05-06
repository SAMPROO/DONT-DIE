using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
	public enum Alignment { TopLeft, TopRight, BottomLeft, BottomRight }
	public Alignment alignment;

	[SerializeField] private VerticalLayoutGroup mainPanel;
	
	[SerializeField] private HorizontalLayoutGroup hpPanel;
	[SerializeField] private Text hpText;
	[SerializeField] private Image hpImage;

	[SerializeField] private HorizontalLayoutGroup ammoPanel;
	[SerializeField] private Text ammoText;
	[SerializeField] private Image ammoImage;

	public Sprite emptyGunHudIcon;

	public void SetHp(int value) => hpText.text = value.ToString();
	public void SetAmmo(int value)
	{
		if (value == int.MaxValue)
			ammoText.text = "Inf.";
		else
			ammoText.text = value.ToString();
	}

	// Set equipped gun icon. Setting null changes to default icon and
	// hides ammo count display
	public void SetEquippedIcon(Sprite icon)
	{
		bool hasEquipped = icon != null;
		ammoImage.sprite = hasEquipped ? icon : emptyGunHudIcon;
		ammoText.gameObject.SetActive(hasEquipped);
	}

	public Rect viewportRect;
	public Color color = Color.white;
	public int rowHeight = 40;
	public int textLength = 60;
	public Vector2Int spacing = new Vector2Int (5, 5);
	public Vector2Int padding = new Vector2Int (10, 10);

	private void OnEnable()
	{
		BuildLayout();
	}

	public void Rebuild()
	{
		BuildLayout();
	}

	private void BuildLayout()
	{
		// Set root hud object (not actual root transform necessarily)
		mainPanel.spacing = spacing.y;
		mainPanel.childAlignment = GetMainAlignment(alignment);
		
		mainPanel.padding.left 		= padding.x;
		mainPanel.padding.right 	= padding.x;
		mainPanel.padding.top 		= padding.y;
		mainPanel.padding.bottom 	= padding.y;

		// this is needed to immediately update new padding to layoutgroups
		LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

		var rectTransform = transform as RectTransform;
		rectTransform.anchorMin = viewportRect.position;
		rectTransform.anchorMax = viewportRect.position + viewportRect.size;
		rectTransform.offsetMin = Vector2Int.zero;
		rectTransform.offsetMax = Vector2Int.zero;

		// Set child component positions
		SetComponentLayout(hpPanel, hpText, hpImage);
		SetComponentLayout(ammoPanel, ammoText, ammoImage);
	}


	private void SetComponentLayout(HorizontalLayoutGroup panel, Text text, Image image)
	{
		panel.spacing = spacing.x;
		var panelTransform = panel.transform as RectTransform;
		panelTransform.sizeDelta = new Vector2(panelTransform.sizeDelta.x, rowHeight);
		panel.childAlignment = GetChildAlignment(alignment);

		text.fontSize = rowHeight;
		text.color = color;
		var textTransform = text.transform as RectTransform;
		textTransform.sizeDelta = new Vector2(textLength, rowHeight);

		image.color = color;
		var imageTransform = image.transform as RectTransform;
		imageTransform.sizeDelta = new Vector2(rowHeight, rowHeight);
	}

	private static TextAnchor GetMainAlignment(Alignment alignment)
	{
		switch (alignment)
		{
			case Alignment.BottomLeft: 	return TextAnchor.LowerLeft;
			case Alignment.BottomRight:	return TextAnchor.LowerRight;
			case Alignment.TopLeft: 	return TextAnchor.UpperLeft;
			case Alignment.TopRight:	return TextAnchor.UpperRight;
		}

		// return default
		return (TextAnchor) 0;
	}

	private static TextAnchor GetChildAlignment(Alignment alignment)
	{
		switch (alignment)
		{
			case Alignment.BottomLeft:
			case Alignment.TopLeft:
				return TextAnchor.MiddleLeft;

			case Alignment.BottomRight:
			case Alignment.TopRight:
				return TextAnchor.MiddleRight;
		}

		return TextAnchor.MiddleLeft;
	}


	private void OnValidate()
	{
		BuildLayout();
	}
}