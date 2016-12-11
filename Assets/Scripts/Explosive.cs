using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosive : MonoBehaviour {

    [SerializeField]
    private float effectRadius = 10.0f;
    [SerializeField]
    private float blastPressure = 100.0f;

    private List<Collider> affectedColliders = new List<Collider>(1024);
    private bool fragCalculated = false;
    private bool hasExploded = false;

    void Start() {

    }

    void Update() {
        //debug_event();

    }

    public void debug_event() {
        if (!hasExploded && Input.GetKey(KeyCode.Space)) {
            detonate();
        }
    }

    public void preDetonate() {
        if (!fragCalculated) {
            fragCalculated = true;
            //Debug.Log("FragCalc");
            StartCoroutine(calcFrag());
        }
    }

    public void detonate() {

        if (!hasExploded) {
            hasExploded = true;
            //this will now find all new pieces
            //Debug.Log("Explode: " + gameObject.name);
            StartCoroutine(doBlast());
            //            Destroy(gameObject);
        }

    }

    private IEnumerator calcFrag() {
        Collider[] bigHits = Physics.OverlapSphere(transform.position, effectRadius);
        for (int i = 0; i < bigHits.Length; i++) {
            affectedColliders.Add(bigHits[i]);
            GameObject go = bigHits[i].gameObject;
            ForceReceiver frec = go.GetComponent<ForceReceiver>();
            if (frec != null) {
                Vector3 dirTowardsObj = go.transform.position - this.transform.position;
                float dist = dirTowardsObj.magnitude;
                dirTowardsObj /= dist;
                frec.calcFrag(dirTowardsObj, blastPressure, dist);
            }
            if (i % 5 == 0) {
                yield return null;
            }
        }
    }

    private IEnumerator doBlast() {
        //Collider[] hits = Physics.OverlapSphere(transform.position, effectRadius);
        int numBigguns = affectedColliders.Count;
        for (int c = 0; c < numBigguns; c++) {
            GameObject parent = affectedColliders[c].gameObject;
            Collider[] chillun = parent.GetComponentsInChildren<Collider>();
            affectedColliders.AddRange(chillun);
        }
        //for (int i = 0; i < hits.Length; i++) {
        for (int i = 0; i < affectedColliders.Count; i++) {
            //GameObject go = hits[i].gameObject;
            GameObject go = affectedColliders[i].gameObject;
            ForceReceiver frec = go.GetComponent<ForceReceiver>();
            if (frec != null) {
                Vector3 dirTowardsObj = go.transform.position - this.transform.position;
                float dist = dirTowardsObj.magnitude;
                dirTowardsObj /= dist;
                frec.takeHit(dirTowardsObj, blastPressure, dist);
            }
            if (i % 5 == 0) {
                yield return null;
            }
        }
    }
}
