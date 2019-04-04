using UnityEngine;

public class RagdollTestController : MonoBehaviour
{
	public RagdollCharacterDriver driver;


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			driver.Jump();
		}
	}

	private void FixedUpdate()
	{
		Vector3 input = Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical");

		Vector3 direction = input.normalized;
		float distance = input.magnitude * Time.deltaTime;

		driver.Drive(direction, distance);

	}
}

/*
public class PlayerTestBuilder : MonoBehaviour
{
	[SerializeField] private PlayerController playerPrefab;
	[SerializeField] private OrbitCameraTP cameraPrefab;
	[SerializeField] private Vector3 spawnPoint;

	public void Start()
	{


		var cameraRig = Instantiate(cameraPrefab, spawnPoint, Quaternion.identity);
		cameraRig
        // playerCameras[i] = Instantiate(orbitCameraPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Camera>();
        playerCameras[i].GetComponent<OrbitCameraTP>().SetInputController(inputControllers[i]);

        players[i] = Instantiate (playerPrefab, spawnPoints[i], Quaternion.identity).GetComponent<PlayerController>();

		// Get controller, camera, hud, random color, etc
		players[i].Initialize(new PlayerHandle (i), playerCameras[i],inputControllers[i]);//, color, controller, etc....);
		players[i].OnDie += OnPlayerDie;
	}
}
*/