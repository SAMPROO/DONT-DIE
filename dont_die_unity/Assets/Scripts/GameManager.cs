using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private const int maxPlayers = 4;

    private PlayerController[] players = new PlayerController[maxPlayers];
    private readonly string[] sceneNames = {"Start", "Level", "End"};
    private int sceneIndex = 0;

    private int numberOfPlayers = 2;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneNames[sceneIndex]);

            if (sceneIndex == 1)
            {
                InitializeLevel();
            }

            sceneIndex++;
            sceneIndex %= sceneNames.Length;
        }

        
    }

    private void InitializeLevel()
	{
        // Load Level
        // Get number of players
        // get spawnpoints from level
        // instantiate players to spawn points
        // 5 .reset/initialize players
        // profit


		// 5.
        players = new PlayerController[maxPlayers];

        Vector3[] spawnPoints = { new Vector3(0, 0, 0), new Vector3(2, 0, 0) };

		for (int i = 0; i < numberOfPlayers; i++)
		{
			players[i] = Instantiate (playerPrefab, spawnPoints[i], Quaternion.identity, transform).GetComponent<PlayerController>();
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
        //Destroy(players[handle]);
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Destroy(players[i].gameObject);
        }

		players = null;
        SceneManager.LoadScene("End");

    }
}