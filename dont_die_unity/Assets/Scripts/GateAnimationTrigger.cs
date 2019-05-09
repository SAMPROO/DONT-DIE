using UnityEngine;

public class GateAnimationTrigger : MonoBehaviour
{
    public GameObject switchGameObject;
    private Animator animator;
    private bool used = false;
    private ISwitch iSwitch;


    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (iSwitch.State && used == false)
        {
            used = true;
            animator.Play("Gate");
        }
        // else
            // Indicate that switch is broken with a sound or something
    }
}
