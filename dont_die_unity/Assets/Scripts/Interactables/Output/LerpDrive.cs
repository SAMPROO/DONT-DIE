using UnityEngine;

public class LerpDrive : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    [Space]
    public float speed;

    [Space]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public enum Mode { Toggle, SpeedControl, PingPong }
    [Space]
    public Mode mode;

    [Header("In SpeedControl mode:")]
    [Range(0, .5f)] public float deathZone = .05f;

    private Vector3 startPos, endPos;
    private Quaternion startRot, endRot;
    private float value, minValue, maxValue;

    private bool pingPongDir = true;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        startPos = transform.localPosition;
        endPos = startPos + positionOffset;

        startRot = transform.localRotation;
        endRot.eulerAngles = startRot.eulerAngles + rotationOffset;

        minValue = .5f - deathZone;
        maxValue = .5f + deathZone;
    }

    private void FixedUpdate()
    {
        switch (mode)
        {
            case Mode.Toggle:

                if (iSwitch.State)
                {
                    value += speed * Time.deltaTime;
                }
                else
                {
                    value -= speed * Time.deltaTime;
                }

                break;

            case Mode.SpeedControl:

                if (iSwitch.Range > maxValue)
                {
                    value += (iSwitch.Range / maxValue - 1) * speed * Time.deltaTime;
                }
                else if (iSwitch.Range < minValue)
                {
                    value -= (1 - iSwitch.Range / minValue) * speed * Time.deltaTime;
                }

                break;

            case Mode.PingPong:

                if (iSwitch.State)
                {
                    if (pingPongDir)
                    {
                        value += speed * Time.deltaTime;
                    }
                    else
                    {
                        value -= speed * Time.deltaTime;
                    }

                    if (value >= 1 || value <= 0)
                    {
                        pingPongDir = !pingPongDir;
                    }
                }

                break;
        };

        value = Mathf.Clamp(value, 0, 1);

        transform.localPosition = Vector3.Lerp(startPos, endPos, value);
        transform.localRotation = Quaternion.Lerp(startRot, endRot, value);
    }
}
