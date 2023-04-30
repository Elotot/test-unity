using UnityEngine;
using easyInputs;

public class MonkeGrab : MonoBehaviour
{
    public Collider LeftHand;
    public Collider RightHand;
    public GameObject Grabbable;

    private bool _isGrabbing = false;
    private Transform _originalParent;

    void Start()
    {
        _originalParent = Grabbable.transform.parent;
    }

    void Update()
    {
        if (!_isGrabbing && (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) || EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand)))
        {
            Collider handCollider = null;
            if (LeftHand.bounds.Intersects(Grabbable.GetComponent<Collider>().bounds))
            {
                handCollider = LeftHand;
            }
            else if (RightHand.bounds.Intersects(Grabbable.GetComponent<Collider>().bounds))
            {
                handCollider = RightHand;
            }

            if (handCollider != null)
            {
                _isGrabbing = true;
                Grabbable.transform.SetParent(handCollider.transform);
                Grabbable.GetComponent<Collider>().enabled = false;
            }
        }
        else if (_isGrabbing && (EasyInputs.GetTriggerButtonDown(EasyHand.RightHand) || EasyInputs.GetTriggerButtonDown(EasyHand.LeftHand)))
        {
            _isGrabbing = false;
            Grabbable.transform.SetParent(_originalParent);
            Grabbable.GetComponent<Collider>().enabled = true;
        }
    }
}
