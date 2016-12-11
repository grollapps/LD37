using UnityEngine;
using System.Collections;
using VRTK;

public class Detonator : VRTK_InteractableObject {

    private bool hasFired = false;

    public override void StartUsing(GameObject currentUsingObject) {
        base.StartUsing(currentUsingObject);
        if (!hasFired) {
            fire();
        }
    }

    private void fire() {
        hasFired = true;
        Debug.Log("Detonator firing");
        //TODO only fire connected explosives

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
            for (int i = 0; i < exps.Length; i++) {
                Explosive e = exps[i].GetComponent<Explosive>();
                if (e != null) {
                    e.detonate();
                }
            }
        }

    }

}
