using UnityEngine;
using System.Collections;

public class PlacePoint : MonoBehaviour {

    [SerializeField]
    private float maxSnapDist = 0.1f;

    //we will compare against squared distance so take the sqrt on maxSnapDist to compensate
    private float maxSnapDistSq;

	void Start () {
        maxSnapDistSq = Mathf.Sqrt(maxSnapDist);
	
	}
	
	void Update () {
	
	}

    public bool place(GameObject item, Vector3 location) {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        Collider c = GetComponent<Collider>();
        //closest point to location on this object
        Vector3 closestPt = c.ClosestPointOnBounds(location);
        Vector3 hitVec = closestPt - location;
        float dist = hitVec.magnitude;
        Vector3 hitDir = hitVec / dist; //points toward this object's inside
        float sqDist = Vector3.SqrMagnitude(location - closestPt); //TODO redundant
        Debug.Log("location=" + location + ", closestPt=" + closestPt +", sqDist=" + sqDist + ", maxDistSq=" + maxSnapDistSq);
        if (sqDist <= maxSnapDistSq) {
            //close enough to connect, find the exact spot on the incoming collider
            Vector3 otherConnectPt = closestPt; //incoming object's
            Vector3 myConnectPt = closestPt; //this object's
            Collider itemC = item.GetComponent<Collider>();
            float maxRayLen = maxSnapDist + 5;
            Debug.Log("maxRayLen=" + maxRayLen);
            Vector3 pos = item.transform.position;
            pos += (pos - closestPt);
            item.transform.position = pos;
            //bool result = itemC.Raycast(new Ray(closestPt, location), out hitInfo, maxRayLen);
            //bool result = Physics.Linecast(location, closestPt, out hitInfo);
            RaycastHit[] results =Physics.RaycastAll(new Ray(closestPt, -hitDir), maxRayLen);
            Debug.Log("NumResults=" + results.Length);
            bool found = false;
            if (results.Length == 0) {
                Debug.Log("Missed a raycast"); //shouldn't happen
                //result = itemC.Raycast(new Ray(closestPt, location), out hitInfo, itemC.bounds.max.sqrMagnitude + maxSnapDist);
                //Debug.Log("opposite result=" + result);
            } else {
                found = false;
                for (int i = 0; i < results.Length; i++) {
                    if (results[i].collider.Equals(itemC)) {
                        otherConnectPt = results[i].point;
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    Debug.Log("Didn't find incoming collider with raycast");
                }
            }

            //find exact spot on this collider
            results = Physics.RaycastAll(new Ray(location, hitDir), maxRayLen);
            //result = c.Raycast(new Ray(location, closestPt), out hitInfo, maxRayLen);
            if (results.Length == 0) {
                Debug.Log("Another miss");
            } else {
                found = false;
                for (int i = 0; i < results.Length; i++) {
                    if (results[i].collider.Equals(c)) {
                        myConnectPt = results[i].point;
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    Debug.Log("Didn't find my collider with raycast");
                }
            }
            //} else {
                rb.isKinematic = true;
                rb.useGravity = false;
            itemC.enabled = false;
            //rb.Sleep();
            item.transform.position = myConnectPt - (otherConnectPt - item.transform.position);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.mass = 0;

            Debug.Log("myConnectPt=" + myConnectPt + ", otherConnectPt=" + otherConnectPt + " result=" + item.transform.position);
                //align normals?
                return true;
            //}
        }
        Debug.Log("Not placed, dist too high: " + location + ", " + closestPt + ", dist=" + Vector3.SqrMagnitude(location - closestPt));
        return false;
    }
}
