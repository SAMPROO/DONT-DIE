using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ConfiguredAudioSource : MonoBehaviour
{
	public SoundType type;

	private void Start()
	{
		AudioManager.InitializeAudioSourceSettings(GetComponent<AudioSource>(), type);
		Destroy(this);
	}
}
