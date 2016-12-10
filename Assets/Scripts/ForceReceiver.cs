using UnityEngine;
using System.Collections;

public class ForceReceiver : MonoBehaviour {

    [SerializeField]
    private int numForceFrames = 4;
    private int remainingForceFrames;

    private bool forceCalculated = false;
    private bool used = false;
    private Vector3 force = Vector3.zero;
    private Vector3 forcePos = Vector3.zero;


    void Start() {
        remainingForceFrames = numForceFrames;
    }

    void FixedUpdate() {

        if (!used && forceCalculated) {
            if (numForceFrames > 0) {
                applyForce(force, forcePos); //TODO does the force pos need to change per frame?
                numForceFrames--;
            } else {
                forceCalculated = false;
                used = true;
            }
        }

    }

    private void applyForce(Vector3 force, Vector3 forcePos) {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        //Debug.Log("Apply f=" + force + " at p=" + forcePos);
        rb.AddForceAtPosition(force, forcePos);
    }

    public void takeHit(Vector3 forceDir, float force, float distFromSrc) {
        //Debug.Log("Hit taken: force=" + force + " dist=" + distFromSrc);
        //TODO
        this.force = forceDir * (force / distFromSrc);
        this.forcePos = transform.position;
        forceCalculated = true; //TODO
    }
}
