using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class MenuSystem : MonoBehaviour
{
	[Header("Main Menu")]
	[SerializeField] private GameObject mainViewObject;
	[SerializeField] private Button 	mainPlayButton;
	[SerializeField] private Button 	mainExitButton;

	[Header("Player Count View")]
	[SerializeField] private GameObject playerCountViewObject;
    [SerializeField] private Button [] 	playerCountButtons;
    [SerializeField] private Button 	playerCountBackButton;

    [Header("Map Select View")]
	[SerializeField] private GameObject 		mapSelectViewObject;
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

	private GameManager gameManager;
	private EventSystem eventSystem;

	private GameObject [] allViews;

	[Header("Testing stuff")]
	public int TestViewIndex;
	public bool SwapTestView;

	private void Awake()
	{
		gameManager = GetComponent<GameManager>();
		eventSystem = GetComponentInChildren<EventSystem>();

		allViews = new []{
			mainViewObject,
			playerCountViewObject,
			mapSelectViewObject,
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
		playerCountViewObject.GetComponent<CancelEvent>().OnCancel.AddListener(SetMainMenu);

        // Map Select view
        foreach (var info in mapButtonInfos)
        {
            info.button.onClick.AddListener(() => SetMapSceneName(info.mapSceneName));
            info.button.transform.GetChild(0).gameObject.SetActive(info.locked);
        }
        mapSelectViewObject.GetComponent<CancelEvent>().OnCancel.AddListener(StartConfigureGame);

        // End view
        endGoToMainButton.onClick.AddListener(SetMainMenu);

        // Back Buttons
        playerCountBackButton.onClick.AddListener(SetMainMenu);
        mapSelectBackButton.onClick.AddListener(StartConfigureGame);

	}

	private void SubsrcribeCancels (GameObject parent, System.Action onCancelFunction)
	{
		var children = parent.GetComponentsInChildren<CancelEvent>();
		Debug.Log($"{children.Length} cancels found under {parent}");
	}

	private void Start()
	{
		SetMainMenu();
	}

	public void SetMainMenu()
	{
		SetView(mainViewObject, true);
		eventSystem.SetSelectedGameObject(mainPlayButton.gameObject);
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
				mainViewObject,
				playerCountViewObject,
				mapSelectViewObject,
				end
			};

			SetView(allViews[TestViewIndex], false);
		}
	}

	///////////////////////////////////////
	/// Configure Game                 	///
	///////////////////////////////////////

	// These are used to set up game. 'StartConfigureGame' is mapped to main menu's
	// "play button", and others to following views completing  actions
	private GameConfiguration configuration;

	private void StartConfigureGame()
	{
		configuration = new GameConfiguration();
		SetView(playerCountViewObject, true);
		eventSystem.SetSelectedGameObject(playerCountButtons[0].gameObject);
	}

	private void SetPlayerCount(int count)
	{
		configuration.playerCount = count;
		mapSelectPlayerCountText.text = count.ToString();
		SetView(mapSelectViewObject, true);
		eventSystem.SetSelectedGameObject(mapButtonInfos[0].button.gameObject);
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
