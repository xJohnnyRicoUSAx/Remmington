using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PumpDetector : MonoBehaviour
{

    public GameObject casingPrefab;
    public Transform casingExitLocation;

    public float casingLifetime = 10;

    public AudioClip pump;
    public AudioClip rack;

    protected Weapon weapon = null;
    public bool shellEmpty = true;
    public void Setup(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void EjectReady()
	{
        shellEmpty = true;
        Eject();
	}
    public void Eject()

        {

        if (shellEmpty == true)
            {
                GetComponent<AudioSource>().PlayOneShot(pump);
                GameObject casing;
                casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
                casing.GetComponent<Rigidbody>().AddExplosionForce(550f, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
                casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);
                Destroy(casing, casingLifetime);
                shellEmpty = false;
            }
        }
}