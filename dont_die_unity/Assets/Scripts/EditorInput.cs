using UnityEngine;

public class EditorInput : MonoBehaviour, IInputController
{
    public KeyCode focusKey = KeyCode.Mouse1;
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode pickupKey = KeyCode.E;

    public float Horizontal     => Input.GetAxis("Horizontal");
    public float Vertical       => Input.GetAxis("Vertical");

    public float LookHorizontal => Input.GetAxisRaw("Mouse X");
    public float LookVertical   => Input.GetAxisRaw("Mouse Y");

    public bool Focus => Input.GetKey(focusKey);

    public event OneOffAction Jump;
    public event OneOffAction Fire;
    public event OneOffAction PickUp;

    private void Update()
    {
        UpdateController();
    }

    public void UpdateController()
    {
        if (Input.GetKeyDown(fireKey)) Fire?.Invoke();
        if (Input.GetKeyDown(jumpKey)) Jump?.Invoke();
        if (Input.GetKeyDown(pickupKey)) PickUp?.Invoke();
    }
}