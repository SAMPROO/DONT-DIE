using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{	
	Effect,
	Player,
	Music
}

public class AudioManager : MonoBehaviour
{
	private readonly List<MeanAudioListener> listeners = new List<MeanAudioListener>();
	public AudioListener realListener;

	[SerializeField] private AudioSourceSettings playerSourceSettings;
	[SerializeField] private AudioSourceSettings effectSourceSettings;
	[SerializeField] private AudioSourceSettings musicSourceSettings;

	// Secret "simpleton"
	private static AudioManager instance; // Set in awake

	private void Awake()
	{
		instance = this;
	}

	public static void InitializeAudioSourceSettings(AudioSource source, SoundType settingType)
	{
		// Set all settings from settings
		switch (settingType)
		{
			case SoundType.Effect: 	instance.effectSourceSettings.Set(source);	break;
			case SoundType.Player: 	instance.playerSourceSettings.Set(source); 	break;
			case SoundType.Music: 	instance.musicSourceSettings.Set(source);	break;
		}
	}

	private void Update()
	{
		Vector3 meanListenerPosition = Vector3.zero;

		int listenerCount = listeners.Count;
		for(int i = 0; i < listenerCount; i++)
		{
			meanListenerPosition += listeners[i].transform.position / listenerCount;
		}

		realListener.transform.position = meanListenerPosition;
	}

	public static void PlaySoundNonSpatial(AudioClip clip)
	{
		// Play sound at real listeners position

		if (clip == null)
			Debug.Log("No sound was played");


		// Todo : play sound
	}

	public static void RegisterListener(MeanAudioListener listener)
	{
		instance.listeners.Add(listener);
	}
}
