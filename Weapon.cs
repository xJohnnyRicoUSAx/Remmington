using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Weapon : XRGrabInteractable
{
    public float breakDistance = 2.9f;

    private new Rigidbody rigidbody = null;

    private Animator animator = null;

    private GripHold gripHold = null;
    private GuardHold guardHold = null;

    private Barrel barrel = null;
    private PumpAction pumpAction = null;
    private PumpDetector pumpDetector = null;


    private XRBaseInteractor gripHand = null;
    private XRBaseInteractor guardHand = null;

    public Transform front = null;
    public Transform back = null;

    public int recoilAmmount = 25;

    private readonly Vector3 gripRotation = new Vector3(45, 0, 0);
    public float PullAmmount { get; private set; } = 0.0f;

    public int ammo = 1;
    public bool reloading = false;
    protected override void Awake()
    {
        base.Awake();
        SetupHolds();
        SetupExtras();

        onSelectEnter.AddListener(SetInitialRotation);
    }

    public void SetupHolds()
    {
        gripHold = GetComponentInChildren<GripHold>();
        gripHold.Setup(this);

        guardHold = GetComponentInChildren<GuardHold>();
        guardHold.Setup(this);
    }

    private void SetupExtras()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        barrel = GetComponentInChildren<Barrel>();
        barrel.Setup(this);

        pumpAction = GetComponentInChildren<PumpAction>();
        pumpAction.Setup(this);

        pumpDetector = GetComponentInChildren<PumpDetector>();
        pumpDetector.Setup(this);
    }


    private void OnDestroy()
    {
        onSelectEnter.RemoveListener(SetInitialRotation);
    }

    private void SetInitialRotation(XRBaseInteractor interactor)
    {
        Quaternion newRotation = Quaternion.Euler(gripRotation);
        interactor.attachTransform.localRotation = newRotation;
    }

    public void SetGripHand(XRBaseInteractor interactor)
    {
        gripHand = interactor;
        OnSelectEnter(gripHand);
    }

    public void ClearGripHand(XRBaseInteractor interactor)
    {
        gripHand = null;
        OnSelectExit(interactor);
    }

    public void SetGuardHand(XRBaseInteractor interactor)
    {
        guardHand = interactor;
    }

    public void ClearGuardHand(XRBaseInteractor interactor)
    {
        guardHand = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (gripHand && guardHand)
        {
            SetGripRotation();
            CheckDistance(gripHand, gripHold);
            CheckDistance(guardHand, guardHold);

            Vector3 pullPosition = guardHand.transform.position;
            PullAmmount = CalculatePull(pullPosition);
            Debug.Log("GettingPosition");
        }

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
    private void SetGripRotation()
    {
        Vector3 target = guardHand.transform.position - gripHold.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(target);

        Vector3 gripRotation = Vector3.zero;
        gripRotation.z = gripHand.transform.eulerAngles.z;

        lookRotation *= Quaternion.Euler(gripRotation);
        gripHand.attachTransform.rotation = lookRotation;
    }

    private void CheckDistance(XRBaseInteractor interactor, HandHold handHold)
    {
        if (interactor)
        {
            float distanceSqr = GetDistanceSqrToInteractor(interactor);

            if (distanceSqr > breakDistance)
            {
                //handHold.BreakHold(interactor);
                Debug.Log("Hold Broken");
            }
            else
            {
                FireReady();
            }
        }
    }

    private void FireReady()
    {
        if (ammo > 0)
        {
            reloading = false;
            Shoot();
        }

        else
        {
            Reload();
        }
    }

    public void Shoot()
    {
        barrel.FireSequence();
    }
    public void Reload()
    {
        reloading = true;
        AnimatePull(PullAmmount);
        Pumped();
        Racked();
        Debug.Log("ReloadReady");
    }

    public void StopReload()
    {
        reloading = false;
        ReloadAmmo();
        Debug.Log("Stopping Reload");
    }
    private void AnimatePull(float value)
    {
        if (reloading == true)
        {
            animator.SetFloat("Blend", value);
            Debug.Log("AnimatingPull");
        }
    }
    public void EmptyAmmo()
    {
        ammo = 0;
        Debug.Log("Out of Ammo");
    }

    public void ReloadAmmo()
    {
        ammo = 1;
        FireReady();
        Debug.Log("Loaded");
    }

    private void Pumped()
    {
		if (PullAmmount == 1)
		{
            pumpDetector.EjectReady();
            ReloadAmmo();
            Debug.Log("Pumping");
        }
	}

    private void Racked()
    {
        if (ammo == 1)
        {
            if (PullAmmount == 0.0f)
            {
                ResetPump();
                StopReload();
                Debug.Log("Racked");
            }
        }
    }

    public void ResetPump()
    {
        PullAmmount = 0.0f;
        Debug.Log("ResettingPump");
    }
    public void ApplyRecoil()
    {
        rigidbody.AddRelativeForce(Vector3.back * recoilAmmount, ForceMode.Impulse);
        Debug.Log("Recoiling");
    }
}
