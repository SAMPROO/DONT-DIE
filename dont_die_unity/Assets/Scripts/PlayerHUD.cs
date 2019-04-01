using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHUD
{
	public Text hpText;

	public void SetHp(int hp)
	{
		hpText.text = $"HUD HP: {hp}";
	}
}