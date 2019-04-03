using UnityEngine;

// Button press for player input, something else for AI input
public delegate void OneOffAction();

public class InputController
{
    private static string
        nullAxisName = "NullAxis",
        baseMoveAxisXName   = "MoveX",
        baseMoveAxisYName   = "MoveY",
        baseLookAxisXName   = "LookX",
        baseLookAxisYName   = "LookY",
        baseJumpKeyName     = "A",
        baseInteractKeyName = "X",
        baseLTAxisName      = "LT",
        baseRTAxisName      = "RT";




    public static InputController[] CreateControllers(int playersCount)
    {
        InputController[] inputControllers = new InputController[playersCount];

        for (int i = 0; i < playersCount; i++)
        {
            inputControllers[i] = new InputController
            {
                //integer i + 1 to match unity's own input system to the player index values
                moveAxisXName   = $"{baseMoveAxisXName}{i + 1}",
                moveAxisYName   = $"{baseMoveAxisYName}{i + 1}",
                lookAxisXName   = $"{baseLookAxisXName}{i + 1}",
                lookAxisYName   = $"{baseLookAxisYName}{i + 1}",
                LTAxisName      = $"{baseLTAxisName}{i + 1}",
                RTAxisName      = $"{baseRTAxisName}{i + 1}",
                jumpKeyName     = $"{baseJumpKeyName}{i + 1}",
                interactKeyName = $"{baseInteractKeyName}{i + 1}",
                myIndex         = i
                
            };
        }
        return inputControllers;
    }

    int myIndex=-1;
    private string
        moveAxisXName   = nullAxisName,
        moveAxisYName   = nullAxisName,
        lookAxisXName   = nullAxisName,
        lookAxisYName   = nullAxisName,
        LTAxisName      = nullAxisName,
        RTAxisName      = nullAxisName,
        jumpKeyName     = nullAxisName,
        interactKeyName = nullAxisName;
        


    public float Horizontal     => Input.GetAxis(moveAxisXName);
	public float Vertical       => -Input.GetAxis(moveAxisYName);

    public float LookHorizontal => Input.GetAxisRaw(lookAxisXName);
    public float LookVertical   => Input.GetAxisRaw(lookAxisYName);

    public bool Focus;

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

