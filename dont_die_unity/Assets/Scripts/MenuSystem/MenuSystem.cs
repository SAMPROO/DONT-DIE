using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class MenuSystem : MonoBehaviour
{
	[SerializeField] private MenuInputSetter menuInputSetter;

	[Header("Main Menu")]
	[SerializeField] private GameObject mainViewObject;
	[SerializeField] private Button 	mainPlayButton;
	[SerializeField] private Button 	mainCreditsButton;
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
	[SerializeField] private GameObject endViewObject;
	[SerializeField] private Button 	endGoToMainButton;
	[SerializeField] private Text 		endWinnerNumberText;

	[Header("Credits View")]
	[SerializeField] private GameObject creditsViewObject;
	[SerializeField] private Button 	creditsBackButton;

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

		allViews = GetAllViews();

		// Main view
		mainPlayButton.onClick.AddListener(StartConfigureGame);
		mainCreditsButton.onClick.AddListener(SetCreditsView);
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
        	if (info.locked == false)
            	info.button.onClick.AddListener(() => SetMapSceneName(info.mapSceneName));
            info.button.transform.GetChild(0).gameObject.SetActive(info.locked);
        }
        mapSelectViewObject.GetComponent<CancelEvent>().OnCancel.AddListener(StartConfigureGame);

        // End view
        endGoToMainButton.onClick.AddListener(SetMainMenu);

        // Credits view
        creditsViewObject.GetComponent<CancelEvent>().OnCancel.AddListener(SetMainMenu);

        // Back Buttons
        playerCountBackButton.onClick.AddListener(SetMainMenu);
        mapSelectBackButton.onClick.AddListener(StartConfigureGame);
        creditsBackButton.onClick.AddListener(SetMainMenu);
	}

	public void Hide()
	{
		SetView (null);
	}

	public void SetMainMenu()
	{
		SetView(mainViewObject);
		eventSystem.SetSelectedGameObject(mainPlayButton.gameObject);
		menuInputSetter.DetectAndSet();
	}

	private void SetView(GameObject view)
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

	private GameObject [] GetAllViews()
	{
		return new [] {
			mainViewObject,
			playerCountViewObject,
			mapSelectViewObject,
			endViewObject,
			creditsViewObject
		};
	}

	private void OnValidate()
	{
		if (SwapTestView)
		{
			allViews = GetAllViews();
			SetView(allViews[TestViewIndex]);
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
		SetView(playerCountViewObject);
		eventSystem.SetSelectedGameObject(playerCountButtons[0].gameObject);
	}

	private void SetPlayerCount(int count)
	{
		configuration.playerCount = count;
		mapSelectPlayerCountText.text = count.ToString();
		SetView(mapSelectViewObject);
		eventSystem.SetSelectedGameObject(mapButtonInfos[0].button.gameObject);
	}

	private void SetMapSceneName(string mapSceneName)
	{
		configuration.mapSceneName = mapSceneName;
		gameManager.StartGame(configuration);
	}

	///////////////////////////////////////
	/// End view stuff                 	///
	///////////////////////////////////////

	public void SetEndView(GameEndStatus endStatus)
	{
		endWinnerNumberText.text = $"#{endStatus.winnerNumber}";
		SetView(endViewObject);	
		eventSystem.SetSelectedGameObject(endGoToMainButton.gameObject);
	}

	///////////////////////////////////////
	/// Other Stuff 					///
	///////////////////////////////////////

	private void SetCreditsView()
	{
		SetView(creditsViewObject);
		eventSystem.SetSelectedGameObject(creditsBackButton.gameObject);
	}
}
