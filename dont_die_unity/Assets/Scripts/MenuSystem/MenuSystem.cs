using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class MenuSystem : MonoBehaviour
{
	public MenuView	main;
	public MenuView	playerCountView;
	public MenuView	mapSelect;
	public MenuView	end;

	private MenuView [] allViews;
	private MenuView lastView = null;

	private GameManager gameManager;

	private const int maxTrackedPreviousViews = 10;
	private MenuView [] trackedViews = new MenuView[maxTrackedPreviousViews];
	private int trackedViewIndex;

	private void Awake()
	{
		gameManager = GetComponent<GameManager>();

		allViews = new []{
			main,
			playerCountView,
			mapSelect,
			end
		};

	}

	private void Start()
	{
		// Track first view manually
		SetView(main, false);
		trackedViews[0] = main;
	}

	// publically only expose this versiom where tracking is must
	public void SetView(MenuView view)
	{
		SetView(view, true);
	}

	private void SetView(MenuView view, bool track)
	{
		// Hide all views
		for (int i = 0; i < allViews.Length; i++)
		{
			allViews[i].gameObject.SetActive(false);
		}

		// Show selected view
		if (view != null)
		{
			view.gameObject.SetActive(true);
		
			if (track)
				SetTrackedView(view);	
		}
	}	

	public int TestViewIndex;
	public bool SwapTestView;

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

	public void SetPreviousView()
	{
		// If we ever try to go back from zero, we have miscreated menuview
		// traversing hierarchy, and should fix that instead of zero or null
		// checking here.

		trackedViewIndex -= 1;
		SetView(trackedViews[trackedViewIndex], false);
	}

	private void SetTrackedView(MenuView view)
	{
		trackedViewIndex += 1;
		trackedViews [trackedViewIndex] = view;

		if (trackedViewIndex >= maxTrackedPreviousViews)
		{
			Debug.LogError("Cannot track this many views");
			trackedViewIndex -= 1; // keep first ones in tracklist, only not track new ones
		}
	}


	// These are used to set up game
	private GameConfiguration configuration = new GameConfiguration();

	public void SetPlayerCount(int count)
	{
		configuration.playerCount = count;
		SetView(mapSelect, true);
	}

	public void SetMapSceneName(string mapSceneName)
	{
		configuration.mapSceneName = mapSceneName;
		SetView (null);
		gameManager.StartGame(configuration);
	}
}
