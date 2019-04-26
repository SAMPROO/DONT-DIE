using UnityEngine;
using UnityEditor;

public static class Checkings
{
	[MenuItem("!Dont Die/Check Player Spawnpoints")]
	private static void Test()
	{
		var spawnPoints = Object.FindObjectsOfType<PlayerSpawnPoint>();
		Debug.Log($"{spawnPoints.Length} PlayerSpawnPoints found");
	}
}