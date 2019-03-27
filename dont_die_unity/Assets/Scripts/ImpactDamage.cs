using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactDamage : MonoBehaviour
{
    public float damageAmount = 0;
    
    public enum Type
    {
        replace,        
        multiply        
    };

    [Tooltip("Replace velocity based damage with amount" +
        "\nMultiply velocity based damage by amount")]
    public Type type;
}
