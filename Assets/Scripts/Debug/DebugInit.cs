using UnityEngine;
using System.Collections;

public class DebugInit : MonoBehaviour {

    [SerializeField]
    private GameObject startPos;

    [SerializeField]
    private GameObject camObj;

	// Use this for initialization
	void Start () {
        Debug.Log("Putting head at debug location " + startPos.transform.position);
        float height = 1.1f;
        Vector3 newPos = new Vector3(startPos.transform.position.x, height, startPos.transform.position.z);
        camObj.transform.position = newPos;
        Quaternion rot = Quaternion.AngleAxis(180, Vector3.up);
        camObj.transform.rotation = rot * startPos.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
