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
        GameObject grabber = ((VRTK_ControllerEvents)sender).gameObject;
        if (testCollision(grabber)) {
            Grabbed(grabber);
        }
    }
    public void Ungrabbed(object sender, ControllerInteractionEventArgs e) {
        ItemTracker.getInstance().removeUngrabbedListener(this.Ungrabbed);
        GameObject grabber = ((VRTK_ControllerEvents)sender).gameObject;
        Ungrabbed(grabber);
    }

    public virtual void Grabbed(GameObject currentGrabbingObject) {
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
    }

    public virtual void Ungrabbed(GameObject previousGrabbingObject) {
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
            if (hits[i] != null) {
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

    private bool testCollision(GameObject other) {
        Collider[] otherC = other.GetComponentsInChildren<Collider>();
        if (otherC != null) {
            for (int i = 0; i < otherC.Length; i++) {
                Collider oc = otherC[i];
                Collider myC = GetComponent<Collider>();
                if (myC.bounds.Intersects(oc.bounds)) {
                    return true;
                }
            }
        }
        return false;
    }
}
