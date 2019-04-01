using UnityEngine;

public class GameManager : MonoBehaviour
{
	PlayerController playerPrefab;

	const int maxPlayers = 4;

	PlayerController [] players = new PlayerController [maxPlayers];

	private void LoadNextLevel()
	{
		// Load Level
		// Get number of players
		// get spawnpoints from level
		// instantiate players to spawn points
		// 5 .reset/initialize players
		// profit

		// 5.
		int numberOfPlayers = 2;

		for (int i = 0; i < numberOfPlayers; i++)
		{
			players[i] = Instantiate (playerPrefab);//, spawnPointPosition);

			// Get random color
			// Get controller
			// get etc..
			players[i].Initialize(new PlayerHandle (i));//, color, controller, etc....);

			players[i].OnDie += OnPlayerDie;
		}

		// Dependency injection

	}


	private void OnPlayerDie(PlayerHandle handle)
	{
		// unspawn player
		// if enough players (1) is died, end match, and someone wins

		// Do not really destroy
		Destroy(players[handle]);
		players[handle] = null;
	}
}