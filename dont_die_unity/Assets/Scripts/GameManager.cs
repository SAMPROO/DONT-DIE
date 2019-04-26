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
        IInputController[] inputControllers = InputControllerManager.CreateControllers(configuration.playerCount);
        players = new PlayerController[maxPlayers];
        playerCameras = new Camera[maxPlayers];

        // Find spawnpoints and hide them from view. We can't hide them from elsewhere before used here.
        var spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < configuration.playerCount; i++)
        {
            playerCameras[i] = Instantiate(orbitCameraPrefab).GetComponent<Camera>();
            playerCameras[i].GetComponent<OrbitCameraTP>().SetInputController(inputControllers[i]);

            players[i] = Instantiate (
                playerPrefab, 
                spawnPoints[i].Position, 
                spawnPoints[i].Orientation 
            );

			// Get controller, camera, hud, random color, etc
			players[i].Initialize(
                new PlayerHandle (i), 
                playerCameras[i].GetComponent<OrbitCameraTP>(),
                inputControllers[i],
                playerColors [i]
            );

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