using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviour
{
    public float health = 1000f;

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
