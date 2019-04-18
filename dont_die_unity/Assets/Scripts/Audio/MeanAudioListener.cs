using UnityEngine;

public class MeanAudioListener : MonoBehaviour
{
	private void Awake()
	{
		AudioManager.RegisterListener(this);
	}
}