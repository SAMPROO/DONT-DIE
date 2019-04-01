using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float health = 100f;

    public Text healthText;

    private void Start()
    {
        healthText.text = ((int)health).ToString();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthText.text = ((int)health).ToString();
    }
}
