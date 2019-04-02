using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonGameManager : MonoBehaviour
{
    private static SingletonGameManager instance;
    public static SingletonGameManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    [SerializeField]
    private GameObject playerPrefab;

    private const int maxPlayers = 4;

    private PlayerController[] players = new PlayerController[maxPlayers];
    private readonly string[] sceneNames = {"Start", "Level", "End"};
    private int sceneIndex = 0;

    private int numberOfPlayers = 2;

    private void Awake()
    {
        if (SingletonGameManager.Instance == null)
        {
            Instance = this;
        }
        else if (SingletonGameManager.Instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        // Default LoadSceneMode is Single, but lets be explicit
        SceneManager.LoadScene(sceneNames[sceneIndex], LoadSceneMode.Single);

        if (sceneIndex == 1)
        {
            // Scene loading completes next frame, so we need to subscribe event.
            // Also we need to unsubscribe, so let's use proper function.            	
            SceneManager.sceneLoaded += OnLevelSceneLoaded;
        }

        sceneIndex++;
        sceneIndex %= sceneNames.Length;
    }


    // Kinda hacky feeling function, but whatever
    private void OnLevelSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    	InitializeLevel();

    	// Unsubscribe itself, so we don't keep initializing level, when not supposed to
    	SceneManager.sceneLoaded -= OnLevelSceneLoaded;
    }

    private void InitializeLevel()
	{
        // Get number of players
        // get spawnpoints from level

		// For number od players:
        // 		Instantiate players to spawn points
        // 		Reset/initialize players
        players = new PlayerController[maxPlayers];

        Vector3[] spawnPoints = { new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(-2, 0, 0), new Vector3(4, 0, 0) };

		for (int i = 0; i < numberOfPlayers; i++)
		{
			players[i] = Instantiate (playerPrefab, spawnPoints[i], Quaternion.identity).GetComponent<PlayerController>();

			// Get controller, camera, hud, random color, etc
			players[i].Initialize(new PlayerHandle (i));//, color, controller, etc....);

			players[i].OnDie += OnPlayerDie;
		}
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

    public void SetPlayerCount(int playerCount)
    {
        numberOfPlayers = playerCount;
        Debug.Log("Number of players: " + numberOfPlayers);
        LoadNextLevel();
    }
}