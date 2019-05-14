using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameArea : MonoBehaviour
{

    public Transform[] spawnPositions;
    public List<Transform> playersOutOfBounds;
    public List<int> playersOutOfBoundsTime;
    public int timeAllowed;
    private int positionsAmount; 
    public float currentTime;

    private void Awake()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        spawnPositions = new Transform[points.Length];
        positionsAmount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            spawnPositions[i] = points[i].transform;
        }
    }

    void Update()
    {
        if (currentTime >= 1f)
        {
            currentTime = 0f;

            if (playersOutOfBounds.Count > 0)
            {
                CheckPlayers();
            }
        }
        else
        {
            currentTime += 1f * Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (other.transform.parent.GetComponent<StatusHelper>().rr.gameObject.GetComponent<PlayerController>().isAlive)
            {
                playersOutOfBounds.Add(other.transform.parent.transform);
                playersOutOfBoundsTime.Add(timeAllowed);
            }
            
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < playersOutOfBounds.Count; i++)
            {
                if (playersOutOfBounds[i] == other.transform.parent.transform)
                {
                    playersOutOfBounds.RemoveAt(i);
                    playersOutOfBoundsTime.RemoveAt(i);
                }
                
            }
            
        }
    }

    private void ReturnPlayer(Transform _player)
    {
        Debug.Log(positionsAmount);
        _player.position = spawnPositions[Random.Range(0, positionsAmount)].position + new Vector3(0, 3, 0);
    }

    private void CheckPlayers()
    {
        for(int i=0;i<playersOutOfBounds.Count;i++)
        {
            playersOutOfBoundsTime[i] -= 1;
            Debug.Log(playersOutOfBounds + " " + playersOutOfBoundsTime);
            if(playersOutOfBoundsTime[i]<=0)
            {
                ReturnPlayer(playersOutOfBounds[i]);
                playersOutOfBounds.RemoveAt(i);
                playersOutOfBoundsTime.RemoveAt(i);
            }
        }
    }
}
