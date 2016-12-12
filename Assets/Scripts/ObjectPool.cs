using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    private static ObjectPool instance;

    private List<ForceReceiver> forceReceivers = new List<ForceReceiver>(2048);
    private List<Breakable> breakables = new List<Breakable>(1024);

    //class Tuple {
    //    public Breakable.ObjType outer;
    //    public Breakable.ObjType inner;
    //    public Vector3[][] spawnLayers; //Z layers
    //}

    //private List<Tuple> tuples = new List<Tuple>();

    public void Awake() {
        instance = this;
    }

    public static ObjectPool getInstance() {
        if (instance == null) {
            Debug.Log("No instance for ObjectPool. Make sure it is on an object in the scene");
        }
        return instance;
    }

    public void AddForceReceivers(IEnumerable<GameObject> frs) {
        foreach (GameObject go in frs) {
            ForceReceiver fr = go.GetComponent<ForceReceiver>();
            if (fr != null) {
                forceReceivers.Add(fr);
            }
            Breakable br = go.GetComponent<Breakable>();
            if (br != null) {
                breakables.Add(br);
            }
        }
        //Debug.Log("Total objects in pool: " + forceReceivers.Count);
    }

    public void cleanupAll() {
        StartCoroutine(cleanupAllCo());
    }

    private IEnumerator cleanupAllCo() {
        int blockSize = 500;
        for (int i = 0; i < breakables.Count; i++) {
            if (breakables[i] != null) {
                breakables[i].cleanup();
            }
            if ((i + 1) % blockSize == 0) {
                yield return null;
            }
        }
        for (int i = 0; i < forceReceivers.Count; i++) {
            if (forceReceivers[i] != null) {
                forceReceivers[i].cleanup();
            }
            if ((i + 1) % blockSize == 0) {
                yield return null;
            }
        }

        //perfModeOn();
    }

//    public void swapColliders() {
//        for (int i = 0; i < forceReceivers.Count; i++) {
//            if (forceReceivers[i] != null) {
//                forceReceivers[i].forceCollider.enabled = true;
//                forceReceivers[i].forceRidgidbody.isKinematic = false;
//            }
//        }
//    }

//    public void perfModeOn() {
//        for (int i = 0; i < forceReceivers.Count; i++) {
//            if (forceReceivers[i] != null) {
//                forceReceivers[i].forceCollider.enabled = false;
//                forceReceivers[i].forceRidgidbody.isKinematic = true;
//            }
//        }
//    }
}
