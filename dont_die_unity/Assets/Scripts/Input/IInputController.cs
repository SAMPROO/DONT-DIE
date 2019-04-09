// Button press for player input, something else for AI input
public delegate void OneOffAction();

public interface IInputController
{
    float Horizontal    { get; }
    float Vertical      { get; }

    float LookHorizontal{ get; }
    float LookVertical  { get; }

    bool Focus          { get; }

    event OneOffAction Jump;
    event OneOffAction Fire;
    event OneOffAction PickUp;

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

    // Using empty explicit implementations, we do not get stupid compiler warnings
    event OneOffAction IInputController.Jump    { add {} remove {} }
    event OneOffAction IInputController.Fire    { add {} remove {} }
    event OneOffAction IInputController.PickUp  { add {} remove {} }

    // public event OneOffAction Jump;
    // public event OneOffAction Fire;
    // public event OneOffAction PickUp;

    public void UpdateController() { }
}