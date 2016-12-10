using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {

    [SerializeField]
    private float effectRadius = 10.0f;
    [SerializeField]
    private float blastPressure = 100.0f;

    private bool fragCalculated = false;
    private bool hasExploded = false;

    void Start() {

    }

    void Update() {
        debug_event();

    }

    public void debug_event() {
        if (Input.GetKey(KeyCode.Space)) {
            detonate();
            hasExploded = true;
        }
    }

    public void detonate() {
        if (!hasExploded) {
            if (!fragCalculated) {
                Debug.Log("CalcFrag: " + gameObject.name);
                Collider[] bigHits = Physics.OverlapSphere(transform.position, effectRadius);
                for (int i = 0; i < bigHits.Length; i++) {
                    GameObject go = bigHits[i].gameObject;
                    ForceReceiver frec = go.GetComponent<ForceReceiver>();
                    if (frec != null) {
                        Vector3 dirTowardsObj = go.transform.position - this.transform.position;
                        float dist = dirTowardsObj.magnitude;
                        dirTowardsObj /= dist;
                        frec.calcFrag(dirTowardsObj, blastPressure, dist);
                    }
                }
            }

            //this will now find all new pieces
            Debug.Log("Explode: " + gameObject.name);
            Collider[] hits = Physics.OverlapSphere(transform.position, effectRadius);
            for (int i = 0; i < hits.Length; i++) {
                GameObject go = hits[i].gameObject;
                ForceReceiver frec = go.GetComponent<ForceReceiver>();
                if (frec != null) {
                    Vector3 dirTowardsObj = go.transform.position - this.transform.position;
                    float dist = dirTowardsObj.magnitude;
                    dirTowardsObj /= dist;
                    frec.takeHit(dirTowardsObj, blastPressure, dist);
                }
            }
        } 

    }
}
