using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMakerTrigger : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource audioSrc;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        PlaySound();
    }

    private void PlaySound()
    {

        AudioClip selectedClip = sounds[Random.Range(0, sounds.Length)];
        audioSrc.PlayOneShot(selectedClip);
    }
}
