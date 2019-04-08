using UnityEngine;

// Button press for player input, something else for AI input
public delegate void OneOffAction();




public interface IInputController
{
    // //Should be improved
    // float AxisInversion { get; set; }
    // //Should be improved
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
        baseLookAxisXName   = "LookXAlt",
        baseLookAxisYName   = "LookYAlt",
        baseJumpKeyName     = "B",
        baseInteractKeyName = "A",
        baseLTAxisName      = "LTALT",
        baseRTAxisName      = "RTALT"
    };

    //xbone until proven dualshock
    private const string 
        dualShockName = "Wireless Controller";

    public static IInputController CreateGamepad(bool isPSController = false)
    {
        JoystickMap map = isPSController ? dualShockMap : xBoneMap;

        // Use this as long as it works, not ideal though
        int controllerIndex = 1;

        GamepadController gamepad = new GamepadController
        {
            //integer i + 1 to match unity's own input system to the player index values
            moveAxisXName   = $"{map.baseMoveAxisXName}{controllerIndex}",
            moveAxisYName   = $"{map.baseMoveAxisYName}{controllerIndex}",
            lookAxisXName   = $"{map.baseLookAxisXName}{controllerIndex}",
            lookAxisYName   = $"{map.baseLookAxisYName}{controllerIndex}",
            LTAxisName      = $"{map.baseLTAxisName}{controllerIndex}",
            RTAxisName      = $"{map.baseRTAxisName}{controllerIndex}",
            jumpKeyName     = $"{map.baseJumpKeyName}{controllerIndex}",
            interactKeyName = $"{map.baseInteractKeyName}{controllerIndex}",

            AxisInversion   = isPSController ? -1f : 1f
        };
        return gamepad;
    }

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

            // Create gamepad controller
            bool isPSController = controllerNames[i] == dualShockName;

            JoystickMap map = isPSController ? dualShockMap : xBoneMap;
            
            // Construct new controller as concrete class and not interface
            GamepadController gamepad = new GamepadController
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

            // This is fine
            gamepad.AxisInversion = isPSController ? -1f : 1f;

            // Now set controller to interface array
            controllers [i] = gamepad;
        }
        return controllers;
    }

}

//Used to represent invalid/missing controller
public class NullController : IInputController
{
    // //Should be improved I think
    // public float AxisInversion
    // {
    //     get { return axisInversion; }
    //     set { axisInversion = value; }
    // }
    // private float axisInversion;
    // //Should be improved
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
        
    // //Should be improved
    // public float AxisInversion
    // {
    //     get { return axisInversion; }
    //     set { axisInversion = value; }
    // }
    // private float axisInversion;
    // //Should be improved
    public float AxisInversion { get; set; } // One nice line is enough when using default getter and setter. We must use this Capital name below though.

    public float Horizontal     => Input.GetAxis(moveAxisXName);
    public float Vertical       => -Input.GetAxis(moveAxisYName);
    public float LookHorizontal => Input.GetAxisRaw(lookAxisXName) * AxisInversion; //axis inversion -1 will make it work for dualshock
    public float LookVertical   => Input.GetAxisRaw(lookAxisYName) * AxisInversion;

    // TODO: this can also use simple expression body getter
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

        //// added because fire button doesnt work on ps4 controller (currently)
        //if (Input.GetKeyDown(KeyCode.F))
        //    Fire?.Invoke();
    }

}
