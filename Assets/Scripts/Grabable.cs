using UnityEngine;
using System.Collections;
using VRTK;

public class Grabable : VRTK_InteractableObject {

    [SerializeField]
    private float grabThresholdTime = 1.5f;

    private float lastUngrabTime = 0;

	// Use this for initialization
	protected override void Start () {
        base.Start();	
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public override void Grabbed(GameObject currentGrabbingObject) {
        if (Time.time - lastUngrabTime > grabThresholdTime) {
            //base.Grabbed(currentGrabbingObject);
            ItemTracker itt = ItemTracker.getInstance();
            if (itt.useItem(gameObject)) {
                gameObject.transform.SetParent(currentGrabbingObject.transform, false);
                Debug.Log("Grabbed " + currentGrabbingObject);
            } else {
                Debug.Log("Couldn't grab " + gameObject.name);
            }
            //rumble appears to not be implemented yet
            //VRTK_ControllerActions actions = currentGrabbingObject.GetComponent<VRTK_ControllerActions>();
            //if (actions != null) {
            //actions.TriggerHapticPulse((ushort)rumbleOnGrab.y, rumbleOnGrab.x, 0.001f); //TODO only trigger on grab
            //} else {
            //Debug.Log("Actions null");
            //}
        }

    }

    public override void Ungrabbed(GameObject previousGrabbingObject) {
        Debug.Log("Ungrabbed");
        ItemTracker itt = ItemTracker.getInstance();
        itt.stopUsingItem();
        //base.Ungrabbed(previousGrabbingObject);
        gameObject.transform.SetParent(null, true);
        lastUngrabTime = Time.time; //prevent pickup for a while so we don't regrab on accident
    }

    public override void StartUsing(GameObject currentUsingObject) {
        base.StartUsing(currentUsingObject);
        Debug.Log("Start using called");
    }

    public override void StopUsing(GameObject previousUsingObject) {
        base.StopUsing(previousUsingObject);
        Debug.Log("Stop using called");
    }

    public override void StartTouching(GameObject currentTouchingObject) {
        base.StartTouching(currentTouchingObject);
        //Debug.Log("Start touching " + currentTouchingObject);
        VRTK_ControllerActions actions = currentTouchingObject.GetComponent<VRTK_ControllerActions>();
        if (actions != null) {
            //x = duration, y = force
            //Debug.Log("rumble " + rumbleOnTouch);
            actions.TriggerHapticPulse((ushort)rumbleOnTouch.y, rumbleOnTouch.x, 1.5f);
        } else {
            Debug.Log("Actions null");
        }
    }
}
