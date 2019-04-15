using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    // only works on stationary spikes

    public GameObject switchGameObject;
    private ISwitch iSwitch;

    public Transform movingPart;

    public float speed;
    public Vector3 positionOffset;

    private Vector3 startPos, endPos;

    private void Start()
    {
        iSwitch = switchGameObject.GetComponent<ISwitch>();

        startPos = movingPart.transform.position;
        endPos = startPos + positionOffset;
    }

    private void FixedUpdate()
    {
        if (iSwitch.State)
        {
            movingPart.position = Vector3.Lerp(movingPart.position, endPos, Time.deltaTime * speed);
        }
        else
        {
            movingPart.position = Vector3.Lerp(movingPart.position, startPos, Time.deltaTime * speed);
        }
    }
}
