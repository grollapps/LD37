using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

    [SerializeField]
    private GameObject piecePrefab;

    [SerializeField]
    private GameObject solidPrefab;

    [SerializeField]
    private float minActivePwr = 25f;

    private bool isDivided = false;

    //private Vector3[] spawnPtsXY; //XY layer
    private Vector3[][] spawnLayers; //Z layers

    private float xSize;
        private float ySize;
        private float zSize;

    void Start() {
        computeDivisions();
    }

    void Update() {
        debug_event();
    }

    private void debug_event() {
        if (Input.GetKey(KeyCode.D) && !isDivided) {
            Debug.Log("Dividing");
            //divideAll();
            breakItDown2(2);
            isDivided = true;
        }
    }

    private void computeDivisions() {
        //assume cube for now
        GameObject tmp = (GameObject)Instantiate(piecePrefab, new Vector3(0, -1000, 0), Quaternion.identity);
        Collider c = tmp.GetComponent<Collider>();
        float xHalf = c.bounds.extents.x;
        float yHalf = c.bounds.extents.y;
        float zHalf = c.bounds.extents.z;
        Destroy(tmp);

        xSize = 2 * xHalf;
        ySize = 2 * yHalf;
        zSize = 2 * zHalf;

        Vector3 halfCubeSize = new Vector3(xHalf, -yHalf, zHalf);
        Collider myC = gameObject.GetComponent<Collider>();
        //find corner lower front corner
        float myx = myC.bounds.extents.x;
        float myy = myC.bounds.extents.y;
        float myz = myC.bounds.extents.z;
        Vector3 frontCorner =  transform.position + new Vector3(myx, -myy, myz);

        int maxXStep = Mathf.CeilToInt(myx * 2 / xSize);
        int maxYStep = Mathf.CeilToInt(myy * 2 / ySize);
        int maxZStep = Mathf.CeilToInt(myz * 2 / zSize);

        int xycount = maxXStep * maxYStep;
        int zcount = maxZStep;
        Debug.Log(xycount + " XY subdivisions, " + zcount + " Z layers for " + gameObject.name);
        spawnLayers = new Vector3[zcount][];

        //find first cube pos
        Vector3 startPoint = (frontCorner - halfCubeSize);
        Vector3 curPos = startPoint;
        //lay'em out
        int i = 0;
        for (int z = 0; z < maxZStep; z++) {
            Vector3[] spawnPtsXY = new Vector3[xycount];
            for (int y = 0; y < maxYStep; y++) {
                for (int x = 0; x < maxXStep; x++) {
                    curPos.x = startPoint.x - x * xSize;
                    curPos.y = startPoint.y + y * ySize;
                    curPos.z = startPoint.z - z * zSize;
                    spawnPtsXY[i++] = (new Vector3(curPos.x, curPos.y, curPos.z));
                }
            }
            spawnLayers[z] = spawnPtsXY;
            i = 0;
        }


    }

    private void divideAll() {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 25);
        for (int z = 0; z < spawnLayers.Length; z++) {
            Vector3[] spawnPts = spawnLayers[z];
            for (int i = 0; i < spawnPts.Length; i++) {
                GameObject go = (GameObject)Instantiate(piecePrefab, spawnPts[i], transform.rotation);
            }
        }

    }

    //assume pen is +z
    public void breakItDown(float penDist) {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 25);
        int numPenLayers = Mathf.Min(spawnLayers.Length, Mathf.CeilToInt(penDist / zSize)); //round up/down/all around?
        for (int z = 0; z < numPenLayers; z++) {
            Vector3[] spawnPts = spawnLayers[z];
            for (int i = 0; i < spawnPts.Length; i++) {
                GameObject go = (GameObject)Instantiate(piecePrefab, spawnPts[i], transform.rotation);
            }
        }
    }

    public void breakItDown2(int numPenLayers) {
        Vector3 pos = transform.position;
        GameObject parent = new GameObject();
        parent.transform.position = transform.position;
        for (int z = 0; z < numPenLayers; z++) {
            Vector3[] spawnPts = spawnLayers[z];
            for (int i = 0; i < spawnPts.Length; i++) {
                GameObject go = (GameObject)Instantiate(piecePrefab, spawnPts[i], Quaternion.identity);
                go.transform.SetParent(parent.transform, true);
            }
        }
        float xscale = transform.localScale.x;
        float yscale = transform.localScale.y;
        float zscale = transform.localScale.z;
        if (numPenLayers < zscale) {
            Vector3 spos = new Vector3(pos.x, pos.y, pos.z - (numPenLayers-1) * zSize);
            GameObject solid = (GameObject)Instantiate(gameObject, spos, transform.rotation);
            solid.transform.localScale = new Vector3(xscale, yscale, zscale - numPenLayers*zSize); //assume whole number of scale units per block
            solid.transform.localPosition = spos;
            solid.transform.SetParent(parent.transform, true);
            Debug.Break();
        }

        parent.transform.localRotation = transform.rotation; //match to original rotation of the object we are replacing

        transform.position = new Vector3(pos.x, pos.y, pos.z - 250); //put this somewhere else for now
    }

    public float getMinActivePwr() {
        return minActivePwr;
    }
}
