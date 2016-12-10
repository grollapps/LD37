using UnityEngine;
using System.Collections;

public class PlacePoint : MonoBehaviour {

    [SerializeField]
    private float maxSnapDist = 0.05f;

    private float maxSnapDistSq;

	void Start () {
        maxSnapDistSq = maxSnapDist * maxSnapDist;
	
	}
	
	void Update () {
	
	}

    public bool placeAt(GameObject item, Vector3 location) {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        Collider c = GetComponent<Collider>();
        Vector3 attachPt = c.ClosestPointOnBounds(location);
        if (Vector3.SqrMagnitude(location - attachPt) <= maxSnapDistSq) {
            item.transform.position = attachPt;
            return true;
        }
        return false;
    }
}
