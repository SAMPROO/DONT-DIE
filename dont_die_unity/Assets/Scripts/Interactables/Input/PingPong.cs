using System;
using UnityEngine;

public class PingPong : MonoBehaviour, ISwitch
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    private bool state;
    public bool State
    {
        get => state;
        private set { state = value; }
    }

    private float range = .5f;
    public float Range
    {
        get => range;
        private set { range = value; }
    }

    public event Action OnTurnOn;
    public event Action OnTurnOff;

    public float delay;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        iSwitch.OnTurnOn += BeginPingPong;
        iSwitch.OnTurnOff += StopPingPong;
    }

    private void BeginPingPong()
    {
        InvokeRepeating(nameof(PingPongState), 0, delay);
    }

    private void StopPingPong()
    {
        CancelInvoke(nameof(PingPongState));
        Range = .5f;
    }

    private void PingPongState()
    {
        Range = Range != 1 ? 1 : 0;
        State = !State;
    }
}
