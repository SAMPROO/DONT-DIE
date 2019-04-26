using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private GameObject orbitCameraPrefab;

    private const int maxPlayers = 4;

    private PlayerController[] players = new PlayerController[maxPlayers];
    private Camera[] playerCameras = new Camera[maxPlayers];

    private GameConfiguration configuration;

    [SerializeField] private Color [] playerColors;

    private MenuSystem menuSystem;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        menuSystem = GetComponent<MenuSystem>();
    }

    // Use this from menusystem to start correct level etc.
    public void StartGame(GameConfiguration configuration)
    {
        this.configuration = configuration;

        // numberOfPlayers = configuration.playerCount;
        SceneManager.LoadScene(configuration.mapSceneName);
        SceneManager.sceneLoaded += OnLevelSceneLoaded;
    }

    // Initialize level (stored as scene), that has just been loaded. 
	// Unsubscribe itself, so we don't keep initializing level, when not supposed to
    private void OnLevelSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    	InitializeLevel();
    	SceneManager.sceneLoaded -= OnLevelSceneLoaded;
    }

    private void InitializeLevel()
	{
        // Get number of players
        // get spawnpoints from level

        // For number od players:
        // 		Instantiate players to spawn points
        // 		Reset/initialize players
        IInputController[] inputControllers = InputControllerManager.CreateControllers(configuration.playerCount);
        players = new PlayerController[maxPlayers];
        playerCameras = new Camera[maxPlayers];

        Vector3[] spawnPoints = { new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(-2, 0, 0), new Vector3(4, 0, 0) };

        for (int i = 0; i < configuration.playerCount; i++)
        {
            playerCameras[i] = Instantiate(orbitCameraPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Camera>();
            playerCameras[i].GetComponent<OrbitCameraTP>().SetInputController(inputControllers[i]);

            players[i] = Instantiate (playerPrefab, spawnPoints[i], Quaternion.identity);//.GetComponent<PlayerController>();

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
        menuSystem.Hide();
	}

	private void OnPlayerDie(PlayerHandle handle)
	{
        // unspawn player
        // if enough players (1) has died, end match, and someone wins

        for (int i = 0; i < configuration.playerCount; i++)
        {
            players[i].Destroy();
        }
		players = null;

        var endStatus = new GameEndStatus
        {
            // Add 1 to make range [1 --> 4]
            winnerNumber = handle.index + 1
        };

        menuSystem.SetEndView(endStatus);   
        SceneManager.UnloadSceneAsync("Level");
    }

    private void SetViewports(Camera[] cameraArray)
    {
        float sizeX = configuration.playerCount == 1 ? 1 : 0.5f;
        float sizeY = configuration.playerCount <= 2 ? 1 : 0.5f;

        int i = 0;
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                cameraArray[i++].rect = new Rect(0.5f * x, 0.5f * y, sizeX, sizeY);
                if (i == configuration.playerCount) return;
            }
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

// [System.Serializable]
public class GameConfiguration
{
    public int playerCount;
    public string mapSceneName;
}

public class GameEndStatus
{
    public int winnerNumber;
}