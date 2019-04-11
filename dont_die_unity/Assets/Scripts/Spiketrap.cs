using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiketrap : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    Renderer Rend => GetComponent<Renderer>();

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();
        iSwitch.OnTurnOn += ToggleSpikes;
        iSwitch.OnTurnOff += ToggleSpikes;
    }

    private void ToggleSpikes()
    {
        if (iSwitch.State)
        {
            Debug.Log("Spikes up!");
            Rend.material.color = Color.green;
        }
        else
        {
            Debug.Log("Spikes down!");
            Rend.material.color = Color.red;
        }
    }
}
