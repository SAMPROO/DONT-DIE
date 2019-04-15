using UnityEngine;

public class SetTimeScale : MonoBehaviour
{
	public bool setTime = true;
	public float timeScale = 1.0f;

	private void Awake()
	{
		if (setTime)
			Time.timeScale = timeScale;
	}

	private void OnValidate()
	{
		timeScale = Mathf.Max(0.0f, timeScale);

		if (Application.isPlaying && setTime)
			Time.timeScale = timeScale;
	}
}