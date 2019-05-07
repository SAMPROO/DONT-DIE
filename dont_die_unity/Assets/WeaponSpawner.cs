using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject holoModel;
    public GameObject holoAxis;
    public Text spawnTimerText;
    public Image timerCircle;
    public float spinRate;
    public int respawnDelay;
    public float currentDelay=0;
    public bool weaponOnPad = false;
    public GameObject[] weapons;
    public GameObject currentWeapon;
    public Material noGun;
    public Material yesGun;
    public GameObject colorRing;


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
                colorRing.GetComponent<MeshRenderer>().material = yesGun;
            }
            else
            {
                currentDelay -= 1 * Time.deltaTime;
                holoAxis.transform.Rotate(0, spinRate*Time.deltaTime, 0);
                spawnTimerText.text = ((int)currentDelay).ToString();
                Debug.Log((respawnDelay - currentDelay) / respawnDelay);
                timerCircle.fillAmount = (respawnDelay - currentDelay) / respawnDelay;
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
            colorRing.GetComponent<MeshRenderer>().material = noGun;
        }
            
    }
}
