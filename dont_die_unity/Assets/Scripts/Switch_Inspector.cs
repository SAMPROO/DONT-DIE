using System;
using UnityEngine;

public class Switch_Inspector : MonoBehaviour, ISwitch
{
    public bool state;
    public bool State
    {
        get => state;
        private set { state = value; }
    }

    [Range(0, 1)] public float range;
    public float Range
    {
        get => range;
        private set { range = value; }
    }

    public event Action OnTurnOn;
    public event Action OnTurnOff;

    private void OnValidate()
    {
        State = state;
        if (State)
            OnTurnOn?.Invoke();
        else
            OnTurnOff?.Invoke();
    }
}
