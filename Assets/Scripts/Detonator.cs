using UnityEngine;
using System.Collections;
using VRTK;

public class Detonator : VRTK_InteractableObject {

    private bool hasFired = false;

    [SerializeField]
    private float timeBetweenFires = 3.0f;

    private float lastFireTime = 0f;

    public override void StartUsing(GameObject currentUsingObject) {
        //base.StartUsing(currentUsingObject);
        if (!hasFired) {
            fire();
        } else if (Time.time - lastFireTime > timeBetweenFires) {
            hasFired = false;
            fire();
            lastFireTime = Time.time;
        }
    }

    private void fire() {
        StartCoroutine(fireCo());
    }

    private IEnumerator fireCo() {
        hasFired = true;
        Debug.Log("Detonator firing");
        //TODO only fire connected explosives
        //prepObjs();
        GameObject[] exps = GameObject.FindGameObjectsWithTag("Explosive");
        if (exps.Length == 0) {
            Debug.Log("No explosives to detonate!");
        } else {
            for (int i = 0; i < exps.Length; i++) {
                Explosive e = exps[i].GetComponent<Explosive>();
                if (e != null) {
                    e.preDetonate();
                }
            }
                    yield return null;
            for (int i = 0; i < exps.Length; i++) {
                Explosive e = exps[i].GetComponent<Explosive>();
                if (e != null) {
                    e.detonate();
                }
            }
                    yield return null;
        }

        cleanupCrew();
    }

    private void prepObjs() {
        //ObjectPool.getInstance().swapColliders();
    }

    private void cleanupCrew() {
        Debug.Log("Cleaning up");
        ObjectPool.getInstance().cleanupAll();
    }

}
