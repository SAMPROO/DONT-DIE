using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip victorySound;
    public float baseVolume=0.4f;

    public void PlayMenu(float _volume = 0.4f)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().volume=_volume;
        GetComponent<AudioSource>().clip = menuMusic;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();

    }

    public void PlayStart(float _volume = 0.4f)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().volume = _volume;
        GetComponent<AudioSource>().clip = gameMusic;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
        
    }

    public void PlayEnd(float _volume = 0.4f)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().volume = _volume;
        GetComponent<AudioSource>().clip = victorySound;
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
    }
}
