using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerHUD
{

	// Use this to control slider value for example
	// public int MaxHp { set; }

	// Only set hp
	public Text hpText;
	public int Hp { set => hpText.text = $"HP: {value}"; }

	// TODO: Add icons for guns for example, do more interesting hp viewer etc.
}