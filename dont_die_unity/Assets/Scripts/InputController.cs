using UnityEngine;

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

    void UpdateController();
}

public class JoystickMap
{
    public string
        baseMoveAxisXName   = null,
        baseMoveAxisYName   = null,
        baseLookAxisXName   = null,
        baseLookAxisYName   = null,
        baseJumpKeyName     = null,
        baseInteractKeyName = null,
        baseLTAxisName      = null,
        baseRTAxisName      = null;
}

public class InputControllerManager
{

    public static JoystickMap xBoneMap = new JoystickMap
    {
        //Base prefix can be removed
        baseMoveAxisXName   = "MoveX",
        baseMoveAxisYName   = "MoveY",
        baseLookAxisXName   = "LookX",
        baseLookAxisYName   = "LookY",
        baseJumpKeyName     = "A",
        baseInteractKeyName = "X",
        baseLTAxisName      = "LT",
        baseRTAxisName      = "RT"
    };

    public static JoystickMap dualShockMap = new JoystickMap
    {
        //Base prefix can be removed
        baseMoveAxisXName   = "MoveX",
        baseMoveAxisYName   = "MoveY",
        baseLookAxisXName   = "LT",
        baseLookAxisYName   = "RT",
        baseJumpKeyName     = "X",
        baseInteractKeyName = "A",
        baseLTAxisName      = "LTALT",
        baseRTAxisName      = "RTALT"
    };

    //xbone until proven dualshock
    private const string 
        dualShockName = "Wireless Controller";

    public static IInputController[] CreateControllers(int playersCount)
    {
        var controllerNames = Input.GetJoystickNames();
        int controllerCount = controllerNames.Length;

        IInputController[] controllers = new IInputController[playersCount];

        for (int i = 0; i < playersCount; i++)
        {
            
            //if i exceeds controllerCount then a controller will become a nullcontroller
            if(i >= controllerCount)
            {
                controllers[i] = new NullController();
                continue;
            }

            JoystickMap map = controllerNames[i] == dualShockName ? dualShockMap : xBoneMap;
           
            controllers[i] = new GamepadController
            {
                //integer i + 1 to match unity's own input system to the player index values
                moveAxisXName   = $"{map.baseMoveAxisXName}{i + 1}",
                moveAxisYName   = $"{map.baseMoveAxisYName}{i + 1}",
                lookAxisXName   = $"{map.baseLookAxisXName}{i + 1}",
                lookAxisYName   = $"{map.baseLookAxisYName}{i + 1}",
                LTAxisName      = $"{map.baseLTAxisName}{i + 1}",
                RTAxisName      = $"{map.baseRTAxisName}{i + 1}",
                jumpKeyName     = $"{map.baseJumpKeyName}{i + 1}",
                interactKeyName = $"{map.baseInteractKeyName}{i + 1}",
                //Base prefix can be removed
            };
        }
        return controllers;
    }

}

//Used to represent invalid/missing controller
public class NullController : IInputController
{

    public float Horizontal => 0.0f;
    public float Vertical => 0.0f;

    public float LookHorizontal => 0.0f;
    public float LookVertical => 0.0f;

    public bool Focus => false;

    public event OneOffAction Jump;
    public event OneOffAction Fire;
    public event OneOffAction PickUp;

    public void UpdateController() { }

}



public class GamepadController : IInputController
{

    public string
        moveAxisXName   = null,
        moveAxisYName   = null,
        lookAxisXName   = null,
        lookAxisYName   = null,
        LTAxisName      = null,
        RTAxisName      = null,
        jumpKeyName     = null,
        interactKeyName = null;
        

    public float Horizontal     => Input.GetAxis(moveAxisXName);
	public float Vertical       => -Input.GetAxis(moveAxisYName);

    public float LookHorizontal => Input.GetAxisRaw(lookAxisXName);
    public float LookVertical   => Input.GetAxisRaw(lookAxisYName);

    public bool Focus { get; private set; }

    public event OneOffAction Jump;
	public event OneOffAction Fire;
	public event OneOffAction PickUp;

    private bool fireEventTriggered = false;

    public float triggerDeadzone = 0.7f;
    
	// This is probably not a monobehaviour, hence it needs to be updated manually
	// Either by player class or some sort InputControllerManager
	public void UpdateController()
	{
		if (Input.GetButtonDown(jumpKeyName))
        {
            Jump?.Invoke();
        }
			

        //fire input
        float rightTriggerValue = Input.GetAxisRaw(RTAxisName);
        if (rightTriggerValue > triggerDeadzone && fireEventTriggered == false)
        {
            Fire?.Invoke();
            fireEventTriggered = true;
        }
        else if (rightTriggerValue < triggerDeadzone)
        {
            fireEventTriggered = false;
        }

        //focus input
        if (Input.GetAxisRaw(LTAxisName) > triggerDeadzone)
            Focus = true;
        else
            Focus = false;

        if (Input.GetButtonDown(interactKeyName))
            PickUp?.Invoke();

	}

}

