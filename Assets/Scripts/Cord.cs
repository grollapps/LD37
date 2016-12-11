using UnityEngine;
using System.Collections;

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
    }

    public override void Ungrabbed(GameObject previousGrabbingObject) {
        base.Ungrabbed(previousGrabbingObject);
        Cord next = grabNew();
        if (next != null) {
            next.Grabbed(previousGrabbingObject);
        }
    }

    private Cord grabNew() {
        GameObject newSection = (GameObject)Instantiate(this.gameObject);
        newSection.name = "cord " + Time.time;
        newSection.transform.position = transform.position;
        newSection.transform.rotation = transform.rotation;
        Cord newSectCord = newSection.GetComponent<Cord>();

        newSectCord.lastPiece = this.gameObject;
        this.nextPiece = newSection;
        //if (lastPiece == null) {
            //first piece in chain
            lastPiece = this.gameObject;
        //}
        lineRenderer.SetPosition(0, lastPiece.transform.position);
        lineRenderer.SetPosition(1, nextPiece.transform.position);

        return newSectCord;
    }
}