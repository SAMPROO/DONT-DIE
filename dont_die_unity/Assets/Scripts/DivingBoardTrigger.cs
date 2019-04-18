using UnityEngine;

public class DivingBoardTrigger : MonoBehaviour
{
    public Animator anim;
    public float minVelocityForTrigger = 5;
    // Start is called before the first frame update
    

    private void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.magnitude >= minVelocityForTrigger)
        {
            anim.SetTrigger("PlayAnimation");
        }
    }
}
