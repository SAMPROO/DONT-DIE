using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayStepSound : MonoBehaviour
{
	private AudioSource audioSource;

	[SerializeField] private AudioClip [] clips;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void Play()
	{
        
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)],0.14f);
	}
}