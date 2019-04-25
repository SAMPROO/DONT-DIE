using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class MenuSystem : MonoBehaviour
{
	[Header("Main Menu")]
	[SerializeField] private GameObject main;
	[SerializeField] private Button 	mainPlayButton;
	[SerializeField] private Button 	mainExitButton;

	[Header("Player Count View")]
	[SerializeField] private GameObject playerCountView;
    [SerializeField] private Button [] 	playerCountButtons;
    [SerializeField] private Button 	playerCountBackButton;

    [Header("Map Select View")]
	[SerializeField] private GameObject 		mapSelect;
	[SerializeField] private MapButtonInfo [] 	mapButtonInfos;
	[SerializeField] private Button 			mapSelectBackButton;
	[SerializeField] private Text 				mapSelectPlayerCountText;

	[System.Serializable]
	private class MapButtonInfo
	{
	    public Button   button;
	    public string   mapSceneName;
	    public bool     locked = false;
	}

	[Header("End View")]
	[SerializeField] private GameObject end;
	[SerializeField] private Button 	endGoToMainButton;
	[SerializeField] private Text 		endWinnerNumberText;


	private GameObject [] allViews;
	private GameManager gameManager;

	[Header("Testing stuff")]
	public int TestViewIndex;
	public bool SwapTestView;

	private void Awake()
	{
		gameManager = GetComponent<GameManager>();

		allViews = new []{
			main,
			playerCountView,
			mapSelect,
			end
		};

		// Main view
		mainPlayButton.onClick.AddListener(StartConfigureGame);
		mainExitButton.onClick.AddListener(gameManager.QuitGame);

		// Player Count view
		for (int i = 0; i < playerCountButtons.Length; i++)
		{
			// Add one to count since indices go [0 -> 3] and counts [1 -> 4]
			int count = i + 1;
			playerCountButtons[i].onClick.AddListener(() => SetPlayerCount(count));			
		}

        // Map Select view
        foreach (var info in mapButtonInfos)
        {
            if (info.locked == false)
            {
                info.button.onClick.AddListener(() => SetMapSceneName(info.mapSceneName));
                info.button.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                info.button.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        // End view
        endGoToMainButton.onClick.AddListener(SetMainMenu);

        // Back Buttons
        playerCountBackButton.onClick.AddListener(SetMainMenu);
        mapSelectBackButton.onClick.AddListener(StartConfigureGame);

	}

	private void Start()
	{
		SetMainMenu();
	}

	public void SetMainMenu()
	{
		SetView(main, true);
	}



	private void SetView(GameObject view, bool track)
	{
		// Hide all views
		for (int i = 0; i < allViews.Length; i++)
		{
			allViews[i].SetActive(false);
		}

		// Show selected view
		if (view != null)
		{
			view.SetActive(true);
		}
	}	

	private void OnValidate()
	{
		if (SwapTestView)
		{
			allViews = new []{
				main,
				playerCountView,
				mapSelect,
				end
			};

			SetView(allViews[TestViewIndex], false);
		}
	}

	///////////////////////////////////////
	/// Configure Game                 	///
	///////////////////////////////////////

	// These are used to set up game. 'StartConfigureGame is mapped to main menu's
	// "play button", and others to following views completing  actions
	private GameConfiguration configuration;

	public void StartConfigureGame()
	{
		configuration = new GameConfiguration();
		SetView(playerCountView, true);
	}

	private void SetPlayerCount(int count)
	{
		configuration.playerCount = count;
		mapSelectPlayerCountText.text = count.ToString();
		SetView(mapSelect, true);
	}

	private void SetMapSceneName(string mapSceneName)
	{
		configuration.mapSceneName = mapSceneName;
		SetView (null, false);
		gameManager.StartGame(configuration);
	}

	///////////////////////////////////////
	/// End view stuff                 	///
	///////////////////////////////////////

	public void SetEndView(GameEndStatus endStatus)
	{
		endWinnerNumberText.text = $"#{endStatus.winnerNumber}";
		SetView(end, false);	
	}
}
