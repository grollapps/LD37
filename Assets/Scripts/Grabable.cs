using UnityEngine;
using System.Collections;
using VRTK;

public class Grabable : MonoBehaviour {

    [SerializeField]
    private float grabThresholdTime = 1.5f;

    private float placementRadiusCheck = 1.0f;

    private float lastUngrabTime = 0;

    void Start() {
    }

    void Update() {
    }

    public void Grabbed(object sender, ControllerInteractionEventArgs e) {
        ItemTracker.getInstance().removeGrabbedListener(this.Grabbed);
        Grabbed(((VRTK_ControllerEvents)sender).gameObject);
    }
    public void Ungrabbed(object sender, ControllerInteractionEventArgs e) {
        ItemTracker.getInstance().removeUngrabbedListener(this.Ungrabbed);
        Ungrabbed(null);
    }

    public void Grabbed(GameObject currentGrabbingObject) {
        //if (Time.time - lastUngrabTime > grabThresholdTime) {
        ItemTracker itt = ItemTracker.getInstance();
        if (itt.useItem(gameObject)) {
            Debug.Log("Grabbed " + gameObject.name);
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.SetParent(currentGrabbingObject.transform, false);
            //Debug.Log("Grabbed " + currentGrabbingObject);
            itt.addUngrabbedListener(this.Ungrabbed);
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
        //}

    }

    public void Ungrabbed(GameObject previousGrabbingObject) {
        Debug.Log("Ungrabbed");
        ItemTracker itt = ItemTracker.getInstance();
        if (itt.stopUsingItem()) {
            itt.addGrabbedListener(this.Grabbed);
            transform.parent = null;
        }
        //gameObject.transform.SetParent(null, true);
        //lastUngrabTime = Time.time; //prevent pickup for a while so we don't regrab on accident
        Collider[] hits = Physics.OverlapSphere(transform.position, placementRadiusCheck);
        Debug.Log(hits.Length + " placement hits");
        for (int i = 0; i < hits.Length; i++) {
            GameObject go = hits[i].gameObject;
            PlacePoint goPp = go.GetComponent<PlacePoint>();
            if (goPp != null) {
                if (goPp.place(gameObject, gameObject.transform.position)) {
                    Debug.Log("Placement found");
                    break;
                }
            }
        }

    }

}
