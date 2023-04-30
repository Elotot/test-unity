using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;
public class PlatformManager : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
 
    
    void Update()
    {
        left.SetActive(EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand));
        right.SetActive(EasyInputs.GetTriggerButtonDown(EasyHand.RightHand));
    }
}
