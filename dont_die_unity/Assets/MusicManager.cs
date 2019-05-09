using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip gameMusic;
    public AudioClip victorySound;

    public void MatchStart()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = gameMusic;
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().Play();
        
    }
    public void MatchEnd()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = victorySound;
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
    }
}
