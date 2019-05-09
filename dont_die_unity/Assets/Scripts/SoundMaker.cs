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

    [Header("Manual mode")]
    public bool manualMode = false;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        if (volumeSmooth == 0)
            volumeSmooth = 1;

        if(sounds.Length==0)
        {
            Debug.Log(sounds.Length);
            Debug.Log(gameObject + " I DONT HAVE ANY SOUNDS GOOD BYE");
            Destroy(this);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(manualMode==false)
        {
            CalculateImpact(collision.relativeVelocity);
        }
        
    }

    private void PlaySound(float _hitStrength)
    {

        AudioClip selectedClip=sounds[Random.Range(0,sounds.Length)];
        audioSrc.PlayOneShot(selectedClip,_hitStrength);

    }

    public void CalculateImpact(Vector3 impact)
    {
        float forceTotal = impact.x + impact.y + impact.z;
        forceTotal *= forceTotal;
        //Debug.Log(forceTotal+" potenssoitu");

        forceTotal /= volumeSmooth;
        forceTotal = Mathf.Clamp(forceTotal, 0.01f, volumeMax);
        //Debug.Log(forceTotal);
        PlaySound(forceTotal);
    }
    public void CalculateImpactSimple(float impact)
    {
        float forceTotal = impact;
        forceTotal *= forceTotal;
        //Debug.Log(forceTotal+" potenssoitu");

        forceTotal /= volumeSmooth;
        forceTotal = Mathf.Clamp(forceTotal, 0.01f, volumeMax);
        //Debug.Log(forceTotal);
        PlaySound(forceTotal);
    }
}
