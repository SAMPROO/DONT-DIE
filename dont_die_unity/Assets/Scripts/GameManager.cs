using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController   playerPrefab;
    [SerializeField] private OrbitCameraTP      orbitCameraPrefab;
    [SerializeField] private PlayerHud          hudPrefab;

    private PlayerController[] players;
    private GameConfiguration configuration;

    [SerializeField] private Color [] playerColors;
    [SerializeField] private Canvas hudCanvas;

    private MenuSystem menuSystem;
    private const string menuSceneName = "MenuViewer";

    private void Awake()
    {
        DontDestroyOnLoad(this);
        menuSystem = GetComponent<MenuSystem>();
    }

    private void Start()
    {
        menuSystem.SetMainMenu();
        SceneManager.LoadScene(menuSceneName);
    }

    // Use this from menu system to start correct level etc.
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
        menuSystem.Hide();

    	SceneManager.sceneLoaded -= OnLevelSceneLoaded;
    }

    private void InitializeLevel()
	{
        IInputController[] inputControllers = InputControllerManager.CreateControllers(configuration.playerCount);
        players = new PlayerController[configuration.playerCount];

        // Get number of viewport rectangle sizes depending on player count
        var viewRects = GetViewRects(configuration.playerCount);

        // Find spawn points and hide them from view. We can't unactivate them from elsewhere before used here.
        var spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < configuration.playerCount; i++)
        {
            OrbitCameraTP orbitCamera = Instantiate(
                orbitCameraPrefab,
                spawnPoints[i].Position,
                spawnPoints[i].Orientation
            );
            
            orbitCamera.SetInputController(inputControllers[i]);
            orbitCamera.GetCamera().rect = viewRects[i];

            PlayerHud hud = Instantiate(
                hudPrefab,
                hudCanvas.transform
            );

            hud.viewportRect = viewRects[i];
            hud.Rebuild();

            players[i] = Instantiate (
                playerPrefab, 
                spawnPoints[i].Position, 
                spawnPoints[i].Orientation 
            );

			// Get controller, camera, hud, random color, etc
			players[i].Initialize(
                new PlayerHandle (i), 
                orbitCamera,
                inputControllers[i],
                playerColors [i],
                hud
            );

			players[i].OnDie += OnPlayerDie;
		}
	}

	private void OnPlayerDie(PlayerHandle handle)
	{
        // unspawn players
        // if enough players (1) has died, end match, and someone wins

        for (int i = 0; i < configuration.playerCount; i++)
        {
            Destroy(players[i].gameObject);
        }
		players = null;

        var endStatus = new GameEndStatus
        {
            // Add 1 to make range [1 --> 4]
            winnerNumber = handle.index + 1
        };

        menuSystem.SetEndView(endStatus);   
        SceneManager.LoadScene(menuSceneName);
    }

    private static Rect [] GetViewRects (int count)
    {
        if (count < 1 || 4 < count)
        {
            Debug.LogError($"{count} number of players not supported. Count must be 1 - 4.");
        }

        float sizeX = count == 1 ? 1 : 0.5f;
        float sizeY = count <= 2 ? 1 : 0.5f;

        Rect[] rects = new Rect[count];

        switch (count)
        {
            case 1:
                rects[0] = new Rect(0, 0, sizeX, sizeY);
                break;
            case 2:
                rects[0] = new Rect(0, 0, sizeX, sizeY);
                rects[1] = new Rect(0.5f, 0, sizeX, sizeY);
                break;

            // Notice that rects [0] and [1] are now on top row, so their y-coordinate
            // is different from above
            case 3:
                rects[0] = new Rect(0, 0.5f, sizeX, sizeY);
                rects[1] = new Rect(0.5f, 0.5f, sizeX, sizeY);
                rects[2] = new Rect(0, 0, sizeX, sizeY);
                break;
            case 4:
                rects[0] = new Rect(0, 0.5f, sizeX, sizeY);
                rects[1] = new Rect(0.5f, 0.5f, sizeX, sizeY);
                rects[2] = new Rect(0, 0, sizeX, sizeY);
                rects[3] = new Rect(0.5f, 0, sizeX, sizeY);
                break;
        }

        return rects;
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

public class GameConfiguration
{
    public int playerCount;
    public string mapSceneName;
}

public class GameEndStatus
{
    public int winnerNumber;
}