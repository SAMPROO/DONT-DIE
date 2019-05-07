using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject holoModel;
    public GameObject holoAxis;
    public Text spawnTimerText;
    public float spinRate;
    public int respawnDelay;
    public float currentDelay=0;
    public bool weaponOnPad = false;
    public GameObject[] weapons;
    public GameObject currentWeapon;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponOnPad==false)
        {
            if(currentDelay<=1)
            {
                currentWeapon = Instantiate(weapons[Random.Range(0, weapons.Length)], holoModel.transform.position, holoModel.transform.rotation);
                currentDelay = respawnDelay;
                weaponOnPad = true;
                holoModel.SetActive(false);
            }
            else
            {
                currentDelay -= 1 * Time.deltaTime;
                holoAxis.transform.Rotate(0, spinRate, 0);
                spawnTimerText.text = ((int)currentDelay).ToString();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentWeapon)
        {
            weaponOnPad = false;
            currentWeapon = null;
            holoModel.SetActive(true);
        }
            
    }
}
