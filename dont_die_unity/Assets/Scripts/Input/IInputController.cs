// Button press for player input, something else for AI input
public delegate void OneOffAction();

public interface IInputController
{
    float Horizontal            { get; }
    float Vertical              { get; }

    float LookHorizontal        { get; }
    float LookVertical          { get; }

    bool Focus                  { get; }
    bool ActivateLeftHand       { get; }
    bool ActivateRightHand      { get; }
    bool HoldRagdoll            { get; }

    //event OneOffAction ActivateLeftHand;
    //event OneOffAction ActivateRightHand;
    event OneOffAction Jump;
    event OneOffAction Fire;
    event OneOffAction PickUp;
    event OneOffAction ToggleRagdoll;

    // TODO: This function should be removed from here and moved to 
    // manager and/or made monobehaviour and only in classes 
    // that actually need it
    void UpdateController();
}

//Used to represent invalid/missing controller
public class NullController : IInputController
{
    public float Horizontal => 0.0f;
    public float Vertical => 0.0f;

    public float LookHorizontal => 0.0f;
    public float LookVertical => 0.0f;

    public bool Focus => false;
    public bool ActivateLeftHand => false;
    public bool ActivateRightHand => false;
    public bool HoldRagdoll => false;
    
    public event OneOffAction Jump;
    public event OneOffAction Fire;
    public event OneOffAction PickUp;
    public event OneOffAction ToggleRagdoll;

    public void UpdateController() { }
}