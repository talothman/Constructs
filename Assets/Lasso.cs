using UnityEngine;
using System.Collections.Generic;
using VRTK;

public class Lasso : MonoBehaviour {

    private VRTK_InteractTouch interactTouch;
    private VRTK_ControllerActions controllerActions;
    private VRTK_ControllerEvents controllerEvents;

    private List<GameObject> grabbedObjectList;

    private void Awake()
    {
        if (GetComponent<VRTK_InteractTouch>() == null)
        {
            Debug.LogError("VRTK_InteractGrab is required to be attached to a Controller that has the VRTK_InteractTouch script attached to it");
            return;
        }

        interactTouch = GetComponent<VRTK_InteractTouch>();
        controllerActions = GetComponent<VRTK_ControllerActions>();
        controllerEvents = GetComponent<VRTK_ControllerEvents>();

        grabbedObjectList = new List<GameObject>();
    }

    private void OnEnable()
    {
        GetComponent<VRTK_ControllerEvents>().AliasGrabOn += new ControllerInteractionEventHandler(DoGrabObject);
        GetComponent<VRTK_ControllerEvents>().AliasGrabOff += new ControllerInteractionEventHandler(DoReleaseObject);
    }

    private void OnDisable()
    {
        GetComponent<VRTK_ControllerEvents>().AliasGrabOn -= new ControllerInteractionEventHandler(DoGrabObject);
        GetComponent<VRTK_ControllerEvents>().AliasGrabOff -= new ControllerInteractionEventHandler(DoReleaseObject);
    }

    private void DoGrabObject(object sender, ControllerInteractionEventArgs e)
    {
        AttemptGrabObject();
    }

    private void DoReleaseObject(object sender, ControllerInteractionEventArgs e)
    {
        //AttemptReleaseObject();
    }

    private void AttemptGrabObject()
    {
        var objectToGrab = GetGrabbableObject();

        if (objectToGrab != null)
        {
            grabbedObjectList.Add(objectToGrab);
            InitPrimaryGrab(objectToGrab.GetComponent<VRTK_InteractableObject>());
            print(grabbedObjectList.Count);
        }

        
        //if (objectToGrab != null)
        //{
        //    IncrementGrabState();
        //    var initialGrabAttempt = false;
        //    var objectToGrabScript = objectToGrab.GetComponent<VRTK_InteractableObject>();

        //    if (grabbedObject == null && IsObjectGrabbable(interactTouch.GetTouchedObject()))
        //    {
        //        initialGrabAttempt = CheckGrabAttempt(objectToGrabScript);

        //    }

        //    undroppableGrabbedObject = (grabbedObject && grabbedObject.GetComponent<VRTK_InteractableObject>() && !grabbedObject.GetComponent<VRTK_InteractableObject>().IsDroppable() ? grabbedObject : null);
        //    grabbedObjectList.Add(undroppableGrabbedObject);
        //    grabbedObjectIndex++;

        //    if (grabbedObject && initialGrabAttempt)
        //    {
        //        var rumbleAmount = grabbedObject.GetComponent<VRTK_InteractableObject>().rumbleOnGrab;
        //        if (!rumbleAmount.Equals(Vector2.zero))
        //        {
        //            controllerActions.TriggerHapticPulse((ushort)rumbleAmount.y, rumbleAmount.x, 0.05f);
        //        }
        //    }
        //}
        //else
        //{
        //    grabPrecognitionTimer = Time.time + grabPrecognition;
        //}
    }

    private GameObject GetGrabbableObject()
    {
        GameObject obj = interactTouch.GetTouchedObject();
        if (obj != null && interactTouch.IsObjectInteractable(obj))
        {
            return obj;
        }
        return null;
        //return grabbedObject;
    }

    private void InitPrimaryGrab(VRTK_InteractableObject currentGrabbedObject)
    {
        currentGrabbedObject.SaveCurrentState();
        currentGrabbedObject.Grabbed(gameObject);
        currentGrabbedObject.ZeroVelocity();
        currentGrabbedObject.ToggleHighlight(false);
        currentGrabbedObject.ToggleKinematic(false);

        //Pause collisions (if allowed on object) for a moment whilst sorting out position to prevent clipping issues
        currentGrabbedObject.PauseCollisions();

        if (currentGrabbedObject.grabAttachMechanic == VRTK_InteractableObject.GrabAttachType.Child_Of_Controller)
        {
            currentGrabbedObject.ToggleKinematic(true);
        }
    }
}
