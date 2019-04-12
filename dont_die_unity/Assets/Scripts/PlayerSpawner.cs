using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	public PlayerController playerPrefab;
	public OrbitCameraTP	cameraRigPrefab;
	public bool 			disableOnStart =  true;

	private void Start()
	{
		var controller = InputControllerManager.CreateGamepad();
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