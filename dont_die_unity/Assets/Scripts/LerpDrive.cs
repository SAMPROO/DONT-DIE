﻿using UnityEngine;

public class LerpDrive : MonoBehaviour
{
    public GameObject switchGameObject;
    private ISwitch iSwitch;

    [Space]
    public float speed;

    [Space]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public enum Mode { Toggle, SpeedControl }
    [Space]
    public Mode mode;

    [Header("In SpeedControl mode:")]
    [Range(0, .5f)] public float deathSpot = .05f;

    private Vector3 startPos, endPos;
    private Quaternion startRot, endRot;
    private float range, minRange, maxRange;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        startPos = transform.localPosition;
        endPos = startPos + positionOffset;

        startRot = transform.localRotation;
        endRot.eulerAngles = startRot.eulerAngles + rotationOffset;
    }

    private void OnValidate()
    {
        minRange = .5f - deathSpot;
        maxRange = .5f + deathSpot;
    }

    private void FixedUpdate()
    {
        switch (mode)
        {
            case Mode.Toggle:

                if (iSwitch.State)
                {
                    range += speed * Time.deltaTime;
                }
                else
                {
                    range -= speed * Time.deltaTime;
                }

                break;

            case Mode.SpeedControl:

                if (iSwitch.Range > maxRange)
                {
                    range += (iSwitch.Range / maxRange - 1) * speed * Time.deltaTime;
                }
                else if (iSwitch.Range < minRange)
                {
                    range -= (1 - iSwitch.Range / minRange) * speed * Time.deltaTime;
                }

                break;
        };

        range = Mathf.Clamp(range, 0, 1);

        transform.localPosition = Vector3.Lerp(startPos, endPos, range);
        transform.localRotation = Quaternion.Lerp(startRot, endRot, range);
    }
}
