using UnityEngine;

public class InputControllerManager
{
    public class JoystickMap
    {
        public string
            baseMoveAxisXName   = null,
            baseMoveAxisYName   = null,
            baseLookAxisXName   = null,
            baseLookAxisYName   = null,
            baseJumpKeyName     = null,
            baseInteractKeyName = null,
            baseDoRagdollKeyName = null,
            baseLBKeyName       = null,
            baseRBKeyName       = null,
            baseLTAxisName      = null,
            baseRTAxisName      = null;
    }
    
    public static JoystickMap xBoneMap = new JoystickMap
    {
        //Base prefix can be removed
        baseMoveAxisXName   = "MoveX",
        baseMoveAxisYName   = "MoveY",
        baseLookAxisXName   = "LookX",
        baseLookAxisYName   = "LookY",
        baseJumpKeyName     = "A",
        baseInteractKeyName = "X",
        baseDoRagdollKeyName = "B",
        baseLBKeyName       = "LB",
        baseRBKeyName       = "RB",
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
        baseDoRagdollKeyName = "X",   //TODO Check if this is right button name
        baseLBKeyName       = "LBALT",//TODO Check if this is right button name
        baseRBKeyName       = "RBALT",//TODO Check if this is right button name
        baseLTAxisName      = "LTALT",
        baseRTAxisName      = "RTALT"
    };

    //xbone until proven dualshock
    private const string 
        dualShockName = "Wireless Controller";

    public static GamepadController CreateGamepad(bool isPSController = false, int controllerIndex = 1)
    {
        JoystickMap map = isPSController ? dualShockMap : xBoneMap;

        // Use this as long as it works, not ideal though
        //int controllerIndex = 1;

        GamepadController gamepad = new GamepadController
        {
            //integer i + 1 to match unity's own input system to the player index values
            moveAxisXName   = $"{map.baseMoveAxisXName}{controllerIndex}",
            moveAxisYName   = $"{map.baseMoveAxisYName}{controllerIndex}",
            lookAxisXName   = $"{map.baseLookAxisXName}{controllerIndex}",
            lookAxisYName   = $"{map.baseLookAxisYName}{controllerIndex}",
            LBKeyName       = $"{map.baseLBKeyName}{controllerIndex}",
            RBKeyName       = $"{map.baseRBKeyName}{controllerIndex}",
            LTAxisName      = $"{map.baseLTAxisName}{controllerIndex}",
            RTAxisName      = $"{map.baseRTAxisName}{controllerIndex}",
            jumpKeyName     = $"{map.baseJumpKeyName}{controllerIndex}",
            interactKeyName = $"{map.baseInteractKeyName}{controllerIndex}",
            doRagdollKeyName = $"{map.baseDoRagdollKeyName}{controllerIndex}",

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
            /*GamepadController gamepad = new GamepadController
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
                doRagdollKeyName = $"{map.baseDoRagdollKeyName}{i + 1}",
                //Base prefix can be removed
            };
            */

            GamepadController gamepad = CreateGamepad(isPSController, i + 1);

            // This is fine
            gamepad.AxisInversion = isPSController ? -1f : 1f;

            // Now set controller to interface array
            controllers [i] = gamepad;
        }
        return controllers;
    }
}