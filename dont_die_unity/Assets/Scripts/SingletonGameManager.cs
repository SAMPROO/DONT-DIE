using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonGameManager : MonoBehaviour
{
    private static SingletonGameManager instance;
    public static SingletonGameManager Instance { get; private set; }

    [SerializeField]
    private GameObject playerPrefab, orbitCameraPrefab;

    private const int maxPlayers = 4;

    private PlayerController[] players = new PlayerController[maxPlayers];
    private Camera[] playerCameras = new Camera[maxPlayers];

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

        // load start scene
        LoadNextLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
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
        playerCameras = new Camera[maxPlayers];

        Vector3[] spawnPoints = { new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(-2, 0, 0), new Vector3(4, 0, 0) };

		for (int i = 0; i < numberOfPlayers; i++)
		{
            playerCameras[i] = Instantiate (orbitCameraPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Camera>();

            players[i] = Instantiate (playerPrefab, spawnPoints[i], Quaternion.identity).GetComponent<PlayerController>();

			// Get controller, camera, hud, random color, etc
			players[i].Initialize(new PlayerHandle (i), playerCameras[i]);//, color, controller, etc....);

			players[i].OnDie += OnPlayerDie;
		}

        SetViewports(playerCameras);
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

    public void SetViewports(Camera[] cameraArray)
    {
        float sizeX = numberOfPlayers == 1 ? 1 : 0.5f;
        float sizeY = numberOfPlayers <= 2 ? 1 : 0.5f;

        int i = 0;
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                cameraArray[i++].rect = new Rect(0.5f * x, 0.5f * y, sizeX, sizeY);
                if (i == numberOfPlayers) return;
            }
        }
    }
}