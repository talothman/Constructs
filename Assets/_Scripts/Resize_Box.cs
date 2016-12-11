using UnityEngine;
using System.Collections;
using VRTK;

public class Resize_Box : MonoBehaviour {
	private bool rightController = true;
	private GameObject controllerRightHand;
	private Vector2 touchAxis;
	private bool rightSubscribed;
	private ControllerInteractionEventHandler touchpadAxisChanged;
	private ControllerInteractionEventHandler touchpadUntouched;
	public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress = VRTK_ControllerEvents.ButtonAlias.Touchpad_Press;

	public GameObject bucket;

	public void Awake(){
		touchpadAxisChanged = new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
		touchpadUntouched = new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

		controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
	}
	// Use this for initialization
	void Start () {

		Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
		SetControllerListeners(controllerRightHand);
	}

	private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
	{
		var controllerEvents = (VRTK_ControllerEvents)sender;
		if (moveOnButtonPress != VRTK_ControllerEvents.ButtonAlias.Undefined && !controllerEvents.IsButtonPressed(moveOnButtonPress))
		{
			touchAxis = Vector2.zero;
			return;
		}
		touchAxis = e.touchpadAxis;
		bucket.transform.localScale += new Vector3 (touchAxis.y, touchAxis.y, touchAxis.y);
	}

	private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
	{
		touchAxis = Vector2.zero;
	}

	private void SetControllerListeners(GameObject controller)
	{
		if (controller && VRTK_SDK_Bridge.IsControllerRightHand(controller))
		{
			ToggleControllerListeners(controller, rightController, ref rightSubscribed);
		}
	}

	private void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
	{
		var controllerEvent = controller.GetComponent<VRTK_ControllerEvents>();
		if (controllerEvent && toggle && !subscribed)
		{
			controllerEvent.TouchpadAxisChanged += touchpadAxisChanged;
			controllerEvent.TouchpadTouchEnd += touchpadUntouched;
			subscribed = true;
		}
		else if (controllerEvent && !toggle && subscribed)
		{
			controllerEvent.TouchpadAxisChanged -= touchpadAxisChanged;
			controllerEvent.TouchpadTouchEnd -= touchpadUntouched;
			touchAxis = Vector2.zero;
			subscribed = false;
		}
	}

	// Update is called once per frame
	void Update () {
		//print (touchAxis.y);
	}
}
