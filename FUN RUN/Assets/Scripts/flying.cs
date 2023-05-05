using UnityEngine;
using easyInputs;

public class flying : MonoBehaviour
{
    public Rigidbody GorillaPlayer;
    public int Force = 20;
    public Transform FlyDirection; // the transform where the force is applied from

    void Update()
    {
        if (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand))
        {
            GorillaPlayer.AddForceAtPosition(FlyDirection.forward.normalized * Force, FlyDirection.position, ForceMode.Acceleration);
        }
    }
}