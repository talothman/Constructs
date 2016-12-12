using UnityEngine;
using System.Collections;
using VRTK;

public class Lassoable : VRTK_InteractableObject {
    private Vector3 originalScale;

    protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
    }

	public override void Grabbed(GameObject currentGrabbingObject)
	{
        //print(IsGrabbed());
        if (!IsGrabbed() || IsSwappable())
		{
			PrimaryControllerGrab(currentGrabbingObject);
		}

		OnInteractableObjectGrabbed(SetInteractableObjectEvent(currentGrabbingObject));
	}

	private void PrimaryControllerGrab(GameObject currentGrabbingObject)
	{
		if (snappedInSnapDropZone)
		{
			ToggleSnapDropZone(storedSnapDropZone, false);
		}

		ForceReleaseGrab();
		RemoveTrackPoint();
		grabbingObjects.Add(currentGrabbingObject);
        SetTrackPoint(currentGrabbingObject);
		if (!IsSwappable())
		{
			previousIsGrabbable = isGrabbable;
			isGrabbable = false;
		}
	}

	private void ForceReleaseGrab()
	{
		var grabbingObject = GetGrabbingObject();
        //print(grabbingObject && !grabbingObject.GetComponent<VRTK_InteractGrab>().lasso);
        if (grabbingObject)
		{
			grabbingObject.GetComponent<VRTK_InteractGrab>().ForceRelease();
		}
	}

	private void RemoveTrackPoint()
	{
		if (customTrackPoint && trackPoint)
		{
			Destroy(trackPoint.gameObject);
		}
		else
		{
			trackPoint = null;
		}
		if (primaryControllerAttachPoint)
		{
			Destroy(primaryControllerAttachPoint.gameObject);
		}
	}

	private void SetTrackPoint(GameObject point)
	{
		var controllerPoint = point.transform;
		var grabScript = point.GetComponent<VRTK_InteractGrab>();

        // TODO add a list of controllerAttachPoints to VRTK_InteractGrab and query the latest one
        // check for lasso mode on from controller side and get one of the attachment points from there
        if (grabScript && grabScript.controllerAttachPoint)
        {
            controllerPoint = grabScript.controllerAttachPoint.transform;
        }

        if (AttachIsTrackObject() && precisionSnap)
		{
			trackPoint = new GameObject(string.Format("[{0}]TrackObject_PrecisionSnap_AttachPoint", gameObject.name)).transform;
			trackPoint.parent = point.transform;
			customTrackPoint = true;
			if (grabAttachMechanic == GrabAttachType.Track_Object)
			{
				trackPoint.position = transform.position;
				trackPoint.rotation = transform.rotation;
			}
			else
			{
				trackPoint.position = controllerPoint.position;
				trackPoint.rotation = controllerPoint.rotation;
			}
		}
		else
		{
			trackPoint = controllerPoint;
			customTrackPoint = false;
		}

		primaryControllerAttachPoint = new GameObject(string.Format("[{0}]Original_Controller_AttachPoint", GetGrabbingObject().name)).transform;
		primaryControllerAttachPoint.parent = transform;
		primaryControllerAttachPoint.position = trackPoint.position;
		primaryControllerAttachPoint.rotation = trackPoint.rotation;

        StartCoroutine(ScaleDownLerp());
	}

    IEnumerator ScaleDownLerp()
    {
        float t = 0.0f;

        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(originalScale, new Vector3(0.05f, 0.05f, 0.05f), t);
            t += (Time.deltaTime * 3);
            //lastSetScale = transform.localScale;
            yield return null;
        }
    }

    public override void Ungrabbed(GameObject previousGrabbingObject)
    {
        if (GetSecondaryGrabbingObject() == null)
        {
            PrimaryControllerUngrab(previousGrabbingObject);
        }
        
        OnInteractableObjectUngrabbed(SetInteractableObjectEvent(previousGrabbingObject));
    }

    private void PrimaryControllerUngrab(GameObject previousGrabbingObject)
    {
        UnpauseCollisions();
        RemoveTrackPoint();
        ResetUseState(previousGrabbingObject);
        grabbedSnapHandle = null;
        grabbingObjects.Clear();
        if (customSecondaryAction)
        {
            customSecondaryAction.OnDropAction();
        }
        LoadPreviousState();

        StartCoroutine(ScaleUpLerp());
    }

    private void UnpauseCollisions()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.detectCollisions = true;
        }
    }

    private void ResetUseState(GameObject checkObject)
    {
        var usingObjectCheck = checkObject.GetComponent<VRTK_InteractUse>();
        if (usingObjectCheck)
        {
            if (holdButtonToUse)
            {
                usingObjectCheck.ForceStopUsing();
            }
        }
    }

    IEnumerator ScaleUpLerp()
    {
        float t = 0.0f;
        Vector3 lastSetScale = transform.localScale;

        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(lastSetScale, originalScale, t);
            t += (Time.deltaTime * 3);
            yield return null;
        }
    }

    public override void StopTouching(GameObject previousTouchingObject)
    {
        base.StopTouching(previousTouchingObject);
        //if (touchingObjects.Contains(previousTouchingObject))
        //{
        //    ResetUseState(previousTouchingObject);
        //    //OnInteractableObjectUntouched(SetInteractableObjectEvent(previousTouchingObject));
        //    //touchingObjects.Remove(previousTouchingObject);
        //}
    }
}
