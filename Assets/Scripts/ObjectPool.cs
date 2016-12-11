using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    private static ObjectPool instance;

    class Tuple {
        public Breakable.ObjType outer;
        public Breakable.ObjType inner;
        public Vector3[][] spawnLayers; //Z layers
    }

    private List<Tuple> tuples = new List<Tuple>();

    public void Awake() {
        instance = this;
    }

    public ObjectPool getInstance() {
        if (instance == null) {
            Debug.Log("No instance for ObjectPool. Make sure it is on an object in the scene");
        }
        return instance;
    }

    public Vector3[][] getLayerData(Breakable.ObjType outerType, Breakable.ObjType innerType) {
        for (int i = 0; i < tuples.Count; i++) {
            Tuple t = tuples[i];
            if (t.outer.Equals(outerType) && t.inner.Equals(innerType)) {
                return t.spawnLayers;
            }
        }
        return null;
    }

    public void setLayerData(Breakable.ObjType outerType, Breakable.ObjType innerType, Vector3[][] data) {
        Tuple t = new Tuple();
        t.outer = outerType;
        t.inner = innerType;
        t.spawnLayers = data;
    }
}