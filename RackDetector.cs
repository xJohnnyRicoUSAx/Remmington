using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class RackDetector : MonoBehaviour
{
    private Weapon weapon = null;
    private PumpAction pumpAction = null;
    public AudioClip rack;

    public void Setup(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void Setup(PumpAction pumpAction)
    {
        this.pumpAction = pumpAction;
    }
    void OnTriggerEnter(Collider other)
    {
        if (weapon.reloading == true)
        {
            if (other.CompareTag("Pump"))
            {
                GetComponent<AudioSource>().PlayOneShot(rack);
                pumpAction.ResetPump();
                weapon.StopReload();
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pump"))

            GetComponent<AudioSource>().PlayOneShot(rack);

    }
}