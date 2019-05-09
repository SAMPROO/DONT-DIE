using UnityEngine;

public class EditorPlayerSpawner : MonoBehaviour
{
	public static bool CanSpawn = true;

	[Header("Set in prefab")]
	public PlayerController playerPrefab;
	public OrbitCameraTP	cameraRigPrefab;
	public PlayerHudScript	hudPrefab;
	public Canvas 			hudCanvas;
	public GameObject 		displayObject;

	public int 				controllerIndex = 1;


	public enum ControllerType
	{
		XBox, PS4, Null
	}

	public ControllerType controllerType;

	private void Start()
	{
		if (CanSpawn)
			Spawn();
		else 
			Destroy(gameObject);
	}

	private void Spawn()
	{
		IInputController controller;

		switch(controllerType)
		{
			case ControllerType.XBox: 	controller = InputControllerManager.CreateGamepad(false, controllerIndex); 	break;
			case ControllerType.PS4: 	controller = InputControllerManager.CreateGamepad(true, controllerIndex);	break;
			
			default: controller = new NullController(); break;
		}

		var cameraRig = Instantiate(cameraRigPrefab);
		cameraRig.SetInputController(controller);

		var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);

		// Hud won't show without canvas
		var hud = Instantiate(hudPrefab, hudCanvas.transform);
		hud.viewportRect = new Rect(0, 0, 1, 1);
		hud.Rebuild();
		
		player.Initialize(new PlayerHandle(0), cameraRig, controller, Color.clear, hud);

		displayObject.SetActive(false);
	}	

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.1f);
	}
}