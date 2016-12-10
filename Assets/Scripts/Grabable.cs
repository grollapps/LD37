using UnityEngine;
using System.Collections;
using VRTK;

public class Grabable : VRTK_InteractableObject {

    private int pulsesRemaining = 0;

	// Use this for initialization
	protected override void Start () {
        base.Start();	
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public override void Grabbed(GameObject currentGrabbingObject) {
        base.Grabbed(currentGrabbingObject);
        Debug.Log("Grabbed " + currentGrabbingObject);
        //rumble appears to not be implemented yet
        VRTK_ControllerActions actions = currentGrabbingObject.GetComponent<VRTK_ControllerActions>();
        if (actions != null) {
            actions.TriggerHapticPulse((ushort)rumbleOnGrab.x, rumbleOnGrab.y, 0);
        }
    }

    public override void StartTouching(GameObject currentTouchingObject) {
        base.StartTouching(currentTouchingObject);
        //Debug.Log("Start touching " + currentTouchingObject);
        VRTK_ControllerActions actions = currentTouchingObject.GetComponent<VRTK_ControllerActions>();
        if (actions != null) {
            actions.TriggerHapticPulse((ushort)rumbleOnTouch.x, rumbleOnTouch.y, 0);
        }
    }
}
