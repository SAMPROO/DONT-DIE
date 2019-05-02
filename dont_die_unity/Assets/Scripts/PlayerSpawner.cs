using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	public PlayerController playerPrefab;
	public OrbitCameraTP	cameraRigPrefab;
	public bool 			disableOnStart =  true;
	public int 				controllerIndex = 1;

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

		player.Initialize(new PlayerHandle(0), cameraRig, controller);

		if (disableOnStart)
			gameObject.SetActive(false);
	}	

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 0.1f);
	}
}