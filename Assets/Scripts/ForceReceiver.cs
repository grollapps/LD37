using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForceReceiver : MonoBehaviour {

    [SerializeField]
    private Breakable.ObjType objType = Breakable.ObjType.MED;

    [SerializeField]
    private int numForceFrames = 4;
    private int remainingForceFrames;

    private bool forceGathered = false;
    private bool forceCalculated = false;
    private bool used = false;
    private bool cashedOut = false;
    private List<Vector3> forces = new List<Vector3>();
    private List<Vector3> forcePoses = new List<Vector3>();
    private Vector3 force = Vector3.zero;
    private Vector3 forcePos = Vector3.zero;


    void Start() {
        remainingForceFrames = numForceFrames;
    }

    void Update() {

        if (used && !cashedOut) {
            cashedOut = true;
            cashout();
        }
    }

    void FixedUpdate() {

        if (!used && forceCalculated) {
            if (numForceFrames > 0) {
                applyForce(force, forcePos); //TODO does the force pos need to change per frame?
                numForceFrames--;
            } else {
                //forceCalculated = false;
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
    public List<Collider> calcFrag(Vector3 forceDir, float force, float distFromSrc) {
        //Debug.Log("Hit taken: force=" + force + " dist=" + distFromSrc);
        //this.force = forceDir * (force / distFromSrc);
        //this.forcePos = transform.position;

        Breakable brb = GetComponent<Breakable>();
        List<Collider> fragResults;
        if (brb == null) {
            fragResults = new List<Collider>(1); //no subs
        } else {
            if (objType.Equals(Breakable.ObjType.BIG)) {
                fragResults = new List<Collider>(1024);
            } else {
                fragResults = new List<Collider>(10);
            }
        }

        if (brb != null) {
            //this object is breakable. Use the force to break it up.
            float minPwrToBreak = brb.getMinActivePwr();
            float falloffDist = Mathf.Sqrt(force / minPwrToBreak);
            //Debug.Log("Falloff dist=" + falloffDist + ", distFromSrc=" + distFromSrc/2);
            GameObject[] fragments = brb.breakItDown(falloffDist - distFromSrc / 2);
            if (fragments != null) {
                for (int i = 0; i < fragments.Length; i++) {
                    if (fragments[i] != null) {
                        ForceReceiver subFr = fragments[i].GetComponent<ForceReceiver>();
                        if (subFr != null) {
                            List<Collider> subFrC = subFr.calcFrag(forceDir, force, distFromSrc);
                            fragResults.AddRange(subFrC);
                        }
                    }
                }
            } else {
                fragResults.Add(gameObject.GetComponent<Collider>()); //breakable didn't break
            }
        } else {
            //one fragment: this object
            fragResults.Add(gameObject.GetComponent<Collider>());
        }

        return fragResults;

    }

    /// <summary>
    /// Send a force from another object into this object
    /// </summary>
    public void takeHit(Vector3 forceDir, float force, float distFromSrc) {
        //Debug.Log("Hit taken: force=" + force + " dist=" + distFromSrc);
        if (!forceGathered) {
            forces.Add(forceDir * (force / distFromSrc));
            forcePoses.Add(transform.position);
        }

    }

    public void hitsComplete() {
        forceGathered = true;
        if (forces.Count > 0) {
            Vector3 forceSum = Vector3.zero;
            for (int i = 0; i < forces.Count; i++) {
                forceSum += forces[i];
            }
            this.force = forceSum / forces.Count;

            Vector3 forcePosSum = Vector3.zero;
            for (int i = 0; i < forcePoses.Count; i++) {
                forcePosSum += forcePoses[i];
            }
            this.forcePos = forcePosSum / forcePoses.Count;
        }
        forceCalculated = true;

    }

    private void cashout() {
        ValueItem v = gameObject.GetComponent<ValueItem>();
        if (v != null) {
            ScoreTracker.getInstance().addCashAndExp(v.cashValue, v.expValue);
        }
    }
}
