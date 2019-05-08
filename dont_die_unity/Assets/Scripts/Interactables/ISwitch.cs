using System;

public interface ISwitch
{
    event Action OnTurnOn;
    event Action OnTurnOff;
    bool State { get; }
    float Range { get; }
}
