using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTest : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    Renderer Rend => GetComponent<Renderer>();

    public enum Mode { Toggle, Range }
    public Mode mode;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        if (mode == Mode.Toggle)
        {
            iSwitch.OnTurnOn += Toggle;
            iSwitch.OnTurnOff += Toggle;
        }

        Rend.material.color = Color.black;
    }

    private void FixedUpdate()
    {
        if (mode == Mode.Range)
        {
            Rend.material.color = Color.Lerp(Color.black, Color.white, iSwitch.Range);
        }
    }

    private void Toggle()
    {
        Rend.material.color = iSwitch.State ? Color.white : Color.black;
    }
}
