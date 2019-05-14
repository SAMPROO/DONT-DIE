using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingHill : MonoBehaviour
{
    public float tickDelay = 1;
    public float tick=0;
    public float tickRate = 1;
    public int tickAmplitude=1;
    public List<PlayerController> players;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tick < tickDelay)
        {
            tick += tickRate * Time.deltaTime;
        }
        else
        {
            foreach(PlayerController player in players)
            {
                player.GetAreaScore(tickAmplitude);
            }
            tick = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            players.Add(other.GetComponentInParent<StatusHelper>().pc);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            players.Remove(other.GetComponentInParent<StatusHelper>().pc);

        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
