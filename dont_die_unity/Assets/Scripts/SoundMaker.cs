using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource audioSrc;
    [Header("High number for smooth (500-2000)")]
    [SerializeField]
    private float volumeSmooth;
    //not in use currently
    [SerializeField]
    [Range(0.1f,1f)]
    private float volumeMax = 1f;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        if (volumeSmooth == 0)
            volumeSmooth = 1;

        if(sounds.Length==0)
        {
            Debug.Log(gameObject + " I DONT HAVE ANY SOUNDS GOOD BYE");
            Destroy(this);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        float forceTotal = collision.relativeVelocity.x + collision.relativeVelocity.y + collision.relativeVelocity.z;
        forceTotal *= forceTotal;
        Debug.Log(forceTotal+" potenssoitu");

        forceTotal /= volumeSmooth;
        forceTotal = Mathf.Clamp(forceTotal, 0.01f, volumeMax);
        Debug.Log(forceTotal);
        PlaySound(forceTotal);
    }

    private void PlaySound(float _hitStrength)
    {

        AudioClip selectedClip=sounds[Random.Range(0,sounds.Length)];
        audioSrc.PlayOneShot(selectedClip,_hitStrength);
    }
}
