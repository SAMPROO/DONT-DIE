using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class FallDamageTester : MonoBehaviour
{
    public Text damageText;

    public float playerMinFallHeight;

    private void Update()
    {
        Vector3 raycastPosition = transform.position + new Vector3(.5f, 1, -.5f);

        if (Physics.Raycast(raycastPosition, Vector3.down, out RaycastHit hit))
        {
            float height = hit.distance;

            float minDamage = Mathf.Sqrt(2 * Physics.gravity.magnitude * playerMinFallHeight);

            float damage = Mathf.Sqrt(2 * Physics.gravity.magnitude * height);
            damage -= minDamage;
            damage = Mathf.Max(damage, 0);

            damageText.text = damage.ToString("0.0");
        }
        else damageText.text = "NULL";
    }
}
