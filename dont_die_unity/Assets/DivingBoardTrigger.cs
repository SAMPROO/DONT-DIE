using UnityEngine;

public class DivingBoardTrigger : MonoBehaviour
{
    public Animator anim;
    public float minVelocityForTrigger = 5;
    // Start is called before the first frame update
    

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other);
        Debug.Log("vel: " + other.relativeVelocity.magnitude);
        if (other.relativeVelocity.magnitude >= minVelocityForTrigger)
        {
            anim.SetTrigger("PlayAnimation");
        }
    }
}
