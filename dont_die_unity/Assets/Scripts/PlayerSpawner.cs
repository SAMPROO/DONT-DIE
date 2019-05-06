using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	[Header("Set in prefab")]
	public PlayerController playerPrefab;
	public OrbitCameraTP	cameraRigPrefab;
	public PlayerHud		hudPrefab;

	public bool 			disableOnStart =  true;
	public int 				controllerIndex = 1;

	[Header("Set in scene")]
	public Canvas 			hudCanvas = null;

	public enum ControllerType
	{
		XBox, PS4, Null
	}

	public ControllerType controllerType;

	private void Start()
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

		// if we have canvas to display hud, instantiate it
		if (hudCanvas == null)
		{
			player.Initialize(new PlayerHandle(0), cameraRig, controller);
		}
		else 
		{
			var hud = Instantiate(hudPrefab, hudCanvas.transform);
			hud.viewportRect = new Rect(0, 0, 1, 1);
			hud.Rebuild();
			
			player.Initialize(new PlayerHandle(0), cameraRig, controller, hud);
		}


		if (disableOnStart)
			gameObject.SetActive(false);
	}	

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.1f);
	}
}