using System;

public interface ISwitch
{
    event Action OnTurnOn;
    event Action OnTurnOff;
    bool Status { get; }
}
