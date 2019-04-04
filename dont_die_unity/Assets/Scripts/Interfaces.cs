using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void Use();
    void StartCarrying(Transform carrier);
    void StopCarrying();
}
