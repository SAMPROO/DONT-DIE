using UnityEngine;

public class GamepadController : IInputController
{
    public string
        moveAxisXName = null,
        moveAxisYName = null,
        lookAxisXName = null,
        lookAxisYName = null,
        LBKeyName = null,
        RBKeyName = null,
        LTAxisName = null,
        RTAxisName = null,
        jumpKeyName = null,
        interactKeyName = null,
        doRagdollKeyName = null;
        
    public float AxisInversion { get; set; } // One nice line is enough when using default getter and setter. We must use this Capital name below though.

    public float Horizontal     => Input.GetAxis(moveAxisXName);
    public float Vertical       => -Input.GetAxis(moveAxisYName);
    public float LookHorizontal => Input.GetAxisRaw(lookAxisXName) * AxisInversion; //axis inversion -1 will make it work for dualshock
    public float LookVertical   => Input.GetAxisRaw(lookAxisYName) * AxisInversion;

    // TODO: this can also use simple expression body getter
    public bool Focus { get; private set; }
    public bool ActivateLeftHand { get; private set; }
    public bool ActivateRightHand { get; private set; }
    public bool HoldRagdoll { get; private set; }

    public event OneOffAction Jump;
    public event OneOffAction Fire;
    public event OneOffAction PickUp;
    public event OneOffAction ToggleRagdoll;

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
        
        //ToggleRagdoll for testing toggle for ragdolling
        /*if (Input.GetButtonDown(doRagdollKeyName))
            ToggleRagdoll?.Invoke();
        */
        
        if (Input.GetButtonDown(doRagdollKeyName))
            HoldRagdoll = true;
        if (Input.GetButtonUp(doRagdollKeyName))
            HoldRagdoll = false;
        
        if (Input.GetButtonDown(LBKeyName))
            ActivateLeftHand = true;
        if (Input.GetButtonUp(LBKeyName))
            ActivateLeftHand = false;

        if (Input.GetButtonDown(RBKeyName))
            ActivateRightHand = true;
        if (Input.GetButtonUp(RBKeyName))
            ActivateRightHand = false;
    }
}
