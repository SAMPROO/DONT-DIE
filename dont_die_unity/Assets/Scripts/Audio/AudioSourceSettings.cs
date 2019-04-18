using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioSourceSettings : ScriptableObject
{	
	public float volume;
	public AudioMixer mixer;
	// etc...

	public void Set(AudioSource source)
	{
		source.volume = volume;
	}
}
