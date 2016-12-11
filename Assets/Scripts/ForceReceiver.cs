using UnityEngine;
using System.Collections;

public class ForceReceiver : MonoBehaviour {

    [SerializeField]
    private Breakable.ObjType objType = Breakable.ObjType.MED;

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

    /// <summary>
    /// Send a force from another object into this object. Calculates fragments that may result
    /// but does not move anything.
    /// </summary>
    public void calcFrag(Vector3 forceDir, float force, float distFromSrc) {
        //Debug.Log("Hit taken: force=" + force + " dist=" + distFromSrc);
        this.force = forceDir * (force / distFromSrc);
        this.forcePos = transform.position;

        Breakable brb = GetComponent<Breakable>();
        if (brb != null) {
            //this object is breakable. Use the force to break it up.
            float minPwrToBreak = brb.getMinActivePwr();
            float falloffDist = Mathf.Sqrt(force / minPwrToBreak);
            //Debug.Log("Falloff dist=" + falloffDist + ", distFromSrc=" + distFromSrc/2);
            brb.breakItDown(falloffDist - distFromSrc/2);
        }

    }

    /// <summary>
    /// Send a force from another object into this object
    /// </summary>
    public void takeHit(Vector3 forceDir, float force, float distFromSrc) {
        //Debug.Log("Hit taken: force=" + force + " dist=" + distFromSrc);
        this.force = forceDir * (force / distFromSrc);
        this.forcePos = transform.position;

        forceCalculated = true; //TODO
    }
}
