using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;
using GorillaLocomotion;

public class Speed : MonoBehaviour
{
    // Public

    [Header("Speed Amount")]
    public float SpeedAmount = 1.5f;

    [Header("Normal Speed Amount")]
    public float NormalSpeed = 1.1f;

    [Header("Player")]
    public Player GorillaPlayer;

    // Private

    void Update()
    {
        if (EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand))
        {
            GorillaPlayer.jumpMultiplier = SpeedAmount;
        }

        else
        {
            GorillaPlayer.jumpMultiplier = NormalSpeed;
        }
    }
}