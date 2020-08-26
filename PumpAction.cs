using System.Collections;
using System.Collections.Specialized;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System.Configuration;

public class PumpAction : MonoBehaviour
{
    private Weapon weapon = null;

    public Transform front = null;
    public Transform back = null;

    public XRBaseInteractor pumpHand = null;

    public float pullValue = 0.0f;

    public void Setup(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public float CalculatePull(Vector3 pullPosition)

    {
        Debug.Log("Calculating Pull");
        Vector3 pullDirection = pullPosition - front.position;
        Vector3 targetDirection = back.position - front.position;
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();
        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;

        return Mathf.Clamp(pullValue, 0, 1);

    }

    public void ResetPump()
    { 
        //PullAmmount = 0.0f;
    }
}

