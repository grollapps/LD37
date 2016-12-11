using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosive : MonoBehaviour {

    [SerializeField]
    private float effectRadius = 10.0f;
    [SerializeField]
    private float blastPressure = 100.0f;
    [SerializeField]
    private GameObject particleSystemObj;

    private List<Collider> affectedColliders = new List<Collider>(1024);
    private bool fragCalculated = false;
    private bool hasExploded = false;

    void Start() {

    }

    void Update() {
        //debug_event();

    }

    //public void debug_event() {
    //if (!hasExploded && Input.GetKey(KeyCode.Space)) {
    //detonate();
    //}
    //}

    public void preDetonate() {
        if (!fragCalculated) {
            fragCalculated = true;
            Debug.Log("FragCalc");
            StartCoroutine(calcFrag());
            Debug.Log("FragCalcDone");
        }
    }

    public void detonate() {

        if (!hasExploded) {
            hasExploded = true;
            //this will now find all new pieces
            Debug.Log("Explode: " + gameObject.name);
            if (particleSystemObj != null) {
                GameObject psObj = (GameObject)Instantiate(particleSystemObj);
                psObj.transform.position = transform.position;
                ParticleSystem p = psObj.GetComponent<ParticleSystem>();
                p.Play();
                Destroy(psObj, p.startLifetime + p.duration + 0.5f);
            }
            StartCoroutine(doBlast());
            Debug.Log("ExplodeCalcDone");
            Destroy(gameObject);
        }

    }

    private IEnumerator calcFrag() {
        Collider[] bigHits = Physics.OverlapSphere(transform.position, effectRadius);
        if (bigHits.Length > 0) {

            int maxLoopSize = Mathf.Clamp(bigHits.Length / 10, 50, 150);
            for (int i = 0; i < bigHits.Length; i++) {
                GameObject go = bigHits[i].gameObject;
                ForceReceiver frec = go.GetComponent<ForceReceiver>();
                if (frec != null) {
                    Vector3 dirTowardsObj = (go.transform.position - this.transform.position).normalized;
                    //float dist = dirTowardsObj.magnitude;
                    //dirTowardsObj /= dist;
                    List<Collider> frags = frec.calcFrag(dirTowardsObj, blastPressure, this.transform.position);
                    affectedColliders.AddRange(frags);
                }
                if (i + 1 % maxLoopSize == 0) {
                    yield return null;
                }
            }
        }
    }

    private IEnumerator doBlast() {
        int numBigguns = affectedColliders.Count;
        if (numBigguns > 0) {
            int maxLoopSize = Mathf.Clamp(affectedColliders.Count / 10, 25, 100);
            for (int i = 0; i < affectedColliders.Count; i++) {
                GameObject go = affectedColliders[i].gameObject;
                ForceReceiver frec = go.GetComponent<ForceReceiver>();
                if (frec != null) {
                    Vector3 dirTowardsObj = go.transform.position - this.transform.position;
                    float dist = dirTowardsObj.magnitude;
                    dirTowardsObj /= dist;
                    frec.takeHit(dirTowardsObj, blastPressure, dist);
                }
                if (i + 1 % maxLoopSize == 0) {
                    yield return null;
                }
            }
            for (int i = 0; i < affectedColliders.Count; i++) {
                GameObject go = affectedColliders[i].gameObject;
                ForceReceiver frec = go.GetComponent<ForceReceiver>();
                if (frec != null) {
                    frec.hitsComplete();
                }
            }
        }
    }
}
