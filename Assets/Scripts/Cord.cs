using UnityEngine;
using System.Collections;
using VRTK;

[RequireComponent(typeof(LineRenderer))]
public class Cord : Grabable {

    private LineRenderer lineRenderer;

    private GameObject lastPiece;
    private GameObject nextPiece;

    void Start() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        nextPiece = this.gameObject;
    }

    void Update() {
        if (nextPiece != null && lineRenderer != null && lastPiece != null) {
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, lastPiece.transform.position);
            lineRenderer.SetPosition(1, nextPiece.transform.position);
        }
    }

    public override void Grabbed(GameObject currentGrabbingObject) {
        base.Grabbed(currentGrabbingObject);
        ItemTracker.getInstance().addGrippedListener(this.Gripped);
    }

    public override void Ungrabbed(GameObject previousGrabbingObject) {
        base.Ungrabbed(previousGrabbingObject);
        Cord next = getNextCord(false);
        if (next != null) {
            next.Grabbed(previousGrabbingObject);
        }
        //don't allow mid line cuts
        ItemTracker.getInstance().removeGrippedListener(this.Gripped);
    }

    private Cord getNextCord(bool stopStrand) {
        Cord newSectCord = null;
        if (!stopStrand) {
            GameObject newSection = (GameObject)Instantiate(this.gameObject);
            newSection.name = "cord " + Time.time;
            newSection.transform.position = transform.position;
            newSection.transform.rotation = transform.rotation;
            newSectCord = newSection.GetComponent<Cord>();

            newSectCord.lastPiece = this.gameObject;
            this.nextPiece = newSection;
        } else {
            nextPiece = this.gameObject;
        }
        lastPiece = this.gameObject;
        lineRenderer.SetPosition(0, lastPiece.transform.position);
        lineRenderer.SetPosition(1, nextPiece.transform.position);

        return newSectCord;
    }

    public void Gripped(object sender, ControllerInteractionEventArgs e) {
        GameObject gripper = ((VRTK_ControllerEvents)sender).gameObject;
        ItemTracker.getInstance().removeGrippedListener(this.Gripped);
        base.Ungrabbed(gripper);
        getNextCord(true);
    }
}