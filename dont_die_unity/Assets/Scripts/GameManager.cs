using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject playerPrefab, orbitCameraPrefab;

    private const int maxPlayers = 4;

    private PlayerController[] players = new PlayerController[maxPlayers];
    private Camera[] playerCameras = new Camera[maxPlayers];

    private readonly string[] sceneNames = {"Start", "MapSelection", "Level", "End"};
    private int sceneIndex = 0;

    private int numberOfPlayers = 2;

    [SerializeField] private Color [] playerColors;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            Instance = this;
        }
        else if (GameManager.Instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

        // load start scene
        // LoadNextLevel();
    }

    public void LoadNextLevel(string sceneName = null)
    {
        if (sceneName == null)
        {
            // Default LoadSceneMode is Single, but lets be explicit
            SceneManager.LoadScene(sceneNames[sceneIndex], LoadSceneMode.Single);
        }
        else
        {
            // Loads level that was selected in MapSelection scene
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        if (sceneIndex == 1)
        {
            // Scene loading completes next frame, so we need to subscribe event.
            // Also we need to unsubscribe, so let's use proper function.            	
            SceneManager.sceneLoaded += OnLevelSceneLoaded;
        }

        sceneIndex++;
        sceneIndex %= sceneNames.Length;
    }

    public void StartGame(GameConfiguration configuration)
    {
        numberOfPlayers = configuration.playerCount;
        SceneManager.LoadScene(configuration.mapSceneName);
        SceneManager.sceneLoaded += OnLevelSceneLoaded;
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
        IInputController[] inputControllers = InputControllerManager.CreateControllers(numberOfPlayers);
        players = new PlayerController[maxPlayers];
        playerCameras = new Camera[maxPlayers];

        Vector3[] spawnPoints = { new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(-2, 0, 0), new Vector3(4, 0, 0) };

        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerCameras[i] = Instantiate(orbitCameraPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Camera>();
            playerCameras[i].GetComponent<OrbitCameraTP>().SetInputController(inputControllers[i]);

            players[i] = Instantiate (playerPrefab, spawnPoints[i], Quaternion.identity).GetComponent<PlayerController>();

			// Get controller, camera, hud, random color, etc
			players[i].Initialize(
                new PlayerHandle (i), 
                playerCameras[i].GetComponent<OrbitCameraTP>(),
                inputControllers[i],
                playerColors [i]
            );//, color, controller, etc....);
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
            players[i].Destroy();
        }

		players = null;
        // SceneManager.LoadScene("End");
        SceneManager.UnloadScene("Level");
        GetComponent<MenuSystem>().SetEndView();   
    }

    public void SetPlayerCount(int playerCount)
    {
        numberOfPlayers = playerCount;
        SceneManager.LoadScene("MapSelection", LoadSceneMode.Single);
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

    public void QuitGame()
    {
        Debug.Log("Quit game");
    }
}

[System.Serializable]
public class GameConfiguration
{
    public int playerCount;
    public string mapSceneName;
}