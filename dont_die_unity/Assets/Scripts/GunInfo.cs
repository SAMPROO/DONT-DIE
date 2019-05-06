using UnityEngine;

[CreateAssetMenu]
public class GunInfo : ScriptableObject
{
	// Icon to show on player's hud
	public Sprite hudIcon;

	// These will be something other than GameObjects...
	public GameObject projectilePrefab;
	public GameObject projectileGhostPrefab;
}