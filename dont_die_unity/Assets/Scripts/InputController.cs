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
                moveAxisXName   = $"{baseMoveAxisXName}{i + 1}",
                moveAxisYName   = $"{baseMoveAxisYName}{i + 1}",
                lookAxisXName   = $"{baseLookAxisXName}{i + 1}",
                lookAxisYName   = $"{baseLookAxisYName}{i + 1}",
                LTAxisName      = $"{baseLTAxisName}{i + 1}",
                RTAxisName      = $"{baseRTAxisName}{i + 1}",
                jumpKeyName     = $"{baseJumpKeyName}{i + 1}",
                interactKeyName = $"{baseInteractKeyName}{i + 1}"
            };
        }
        return inputControllers;
    }

    private string
        moveAxisXName = nullAxisName,
        moveAxisYName = nullAxisName,
        lookAxisXName = nullAxisName,
        lookAxisYName = nullAxisName,
        LTAxisName = nullAxisName,
        RTAxisName = nullAxisName,
        jumpKeyName = nullAxisName,
        interactKeyName = nullAxisName;
        


    public float Horizontal => Input.GetAxis(moveAxisXName);
	public float Vertical => -Input.GetAxis(moveAxisYName);

    public float LookHorizontal => Input.GetAxisRaw(lookAxisXName);
    public float LookVertical => Input.GetAxisRaw(lookAxisYName);

    public bool Focus;

    public event OneOffAction Jump;
	public event OneOffAction Fire;

	private KeyCode fireKey = KeyCode.E;

    
	// This is probably not a monobehaviour, hence it needs to be updated manually
	// Either by player class or some sort InputControllerManager
	public void UpdateController()
	{
		if (Input.GetButtonDown(jumpKeyName))
			Jump?.Invoke();

        if (Input.GetAxisRaw(RTAxisName) > 0.5f)
			Fire?.Invoke();

        if (Input.GetAxisRaw(LTAxisName) > 0.5f)
            Focus = true;
        else
            Focus = false;
	}
}

