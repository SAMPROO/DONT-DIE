using UnityEngine;

public class LinearDrive : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    private ConfigurableJoint joint;

    public float speed;

    public float leftLimit, rightLimit;

    public enum Mode { Toggle, SpeedControl }
    public Mode mode;

    [Header("In SpeedControl mode:")]
    [Range(0, .5f)] public float deathZone = .1f;

    private float minRange, maxRange;

    private void OnValidate()
    {
        leftLimit = Mathf.Max(0, leftLimit);
        rightLimit = Mathf.Max(0, rightLimit);
    }

    private void Start()
    {
        minRange = .5f - deathZone;
        maxRange = .5f + deathZone;

        iSwitch = switchGameObject.GetComponent<ISwitch>();
        joint = GetComponent<ConfigurableJoint>();

        // set anchor to the middle of linear range;
        joint.autoConfigureConnectedAnchor = false;

        if (joint.connectedBody)
            joint.connectedAnchor = transform.localPosition;
        
        joint.anchor = (rightLimit - leftLimit) / 2 * Vector3.left; 

        // set the linear limit
        joint.linearLimit = new SoftJointLimit { limit = (leftLimit + rightLimit) / 2 };
    }

    private void FixedUpdate()
    {
        //Debug.Log("Position: " + transform.localPosition.x);

        float velocity = 0;

        switch (mode)
        {
            case Mode.Toggle:
                if (iSwitch.State)
                    velocity = -1;
                else
                    velocity = 1;
                break;

            case Mode.SpeedControl:
                if (iSwitch.Range > maxRange)
                    velocity = iSwitch.Range / maxRange - 1;
                else if (iSwitch.Range < minRange)
                    velocity = iSwitch.Range / minRange - 1;
                break;
        };

        joint.targetVelocity = Vector3.right * velocity * speed;
    }
}
