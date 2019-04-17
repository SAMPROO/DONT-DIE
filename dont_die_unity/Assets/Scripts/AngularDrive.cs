using UnityEngine;

public class AngularDrive : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    private HingeJoint joint;

    public float speed;

    public bool useLimits;
    public float fromAngle = 0, toAngle = 90;

    private float minAngle, maxAngle, angularRange, currentAngle;

    public enum Mode { Toggle, SpeedControl }
    public Mode mode;

    [Header("In SpeedControl mode:")]
    [Range(0, .5f)] public float deathSpot = .05f;

    private float minRange, maxRange;

    private void OnValidate()
    {
        currentAngle = Mathf.DeltaAngle(currentAngle, -transform.rotation.eulerAngles.y);

        minAngle = Mathf.Min(fromAngle, toAngle) + currentAngle;
        maxAngle = Mathf.Max(fromAngle, toAngle) + currentAngle;
        angularRange = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);

        minRange = .5f - deathSpot;
        maxRange = .5f + deathSpot;
    }

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();
        joint = GetComponent<HingeJoint>();
    }

    private void FixedUpdate()
    {
        // keep track of current angle
        currentAngle += Mathf.DeltaAngle(currentAngle, -transform.rotation.eulerAngles.y);
        //Debug.Log(currentAngle);

        float velocity = 0;

        switch (mode)
        {
            case Mode.Toggle:
                if (iSwitch.State)
                    velocity = 1;
                else
                    velocity = -1;
                break;

            case Mode.SpeedControl:
                if (iSwitch.Range > maxRange)
                    velocity = iSwitch.Range / maxRange - 1;
                else if (iSwitch.Range < minRange)
                    velocity = iSwitch.Range / minRange - 1;
                break;
        };

        if (useLimits && ((currentAngle >= maxAngle && velocity < 0) || (currentAngle <= minAngle && velocity > 0)))
        { 
            velocity = 0;
        }

        joint.motor = new JointMotor
        {
            targetVelocity = velocity * speed,
            force = joint.motor.force,
            freeSpin = joint.motor.freeSpin
        };
    }
}
