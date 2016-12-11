using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

    public enum ObjType {
        BIG, MED, SMALL
    }

    [SerializeField]
    private ObjType type;

    [SerializeField]
    private GameObject piecePrefab;

    [SerializeField]
    private float minActivePwr = 25f;

    private bool layersComputed = false;
    private bool fullComputed = false;

    //private Vector3[] spawnPtsXY; //XY layer
    private Vector3[][] spawnLayers; //Z layers

    private float xSize;
    private float ySize;
    private float zSize;

    private GameObject[] pieces;

    void Start() {
        computeDivisions();
    }

    void Update() {
        //debug_event();
    }

    //private void debug_event() {
        //if (Input.GetKey(KeyCode.Keypad0) && !isDivided) {
            //Debug.Log("Dividing");
            //divideAll();
            //breakItDownByLayer(2);
            //isDivided = true;
        //}
    //}

    private void computeDivisions() {
        if (layersComputed) {
            return;
        }
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
        //Vector3 frontCorner =  transform.position + new Vector3(myx, -myy, myz);
        Vector3 frontCorner = new Vector3(myx, -myy, myz);

        int maxXStep = Mathf.CeilToInt(myx * 2 / xSize);
        int maxYStep = Mathf.CeilToInt(myy * 2 / ySize);
        int maxZStep = Mathf.CeilToInt(myz * 2 / zSize);

        int xycount = maxXStep * maxYStep;
        int zcount = maxZStep;
        //Debug.Log(xycount + " XY subdivisions, " + zcount + " Z layers for " + gameObject.name);
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
        layersComputed = true;


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

    //assume pen is +z. Returned game objects will be of type piecePrefab (if any)
    public GameObject[] breakItDown(float penDist) {
        if (fullComputed) {
            return null;
        }
        if (spawnLayers == null) {
            computeDivisions();
        }
        int numPenLayers = Mathf.Min(spawnLayers[0].Length, Mathf.CeilToInt(Mathf.Abs(penDist) / zSize)); //round up/down/all around?
        breakItDownByLayer(numPenLayers);
        fullComputed = true;
//        if (pieces != null) {
//            for (int i = 0; i < pieces.Length; i++) {
//                if (pieces[i] != null) {
//                    Breakable brb = pieces[i].GetComponent<Breakable>();
//                    if (brb != null) {
//                        brb.breakItDown(penDist); //TODO calc correct value
//                    }
//                }
//            }
        //}
        return pieces;
    }

    private void breakItDownByLayer(int numPenLayers) {
        Vector3 pos = transform.position;
        GameObject parent = new GameObject();
        parent.name = "breakable parent";
        parent.transform.position = transform.position;
        int pieceCount = numPenLayers * spawnLayers[0].Length;
        //Debug.Log("Piececount=" + pieceCount);
        pieces = new GameObject[pieceCount];

        //make pieces
        int pidx = 0;
        for (int z = 0; z < numPenLayers & z < spawnLayers.Length; z++) {
            Vector3[] spawnPts = spawnLayers[z];
            for (int i = 0; i < spawnPts.Length; i++) {
                GameObject go = (GameObject)Instantiate(piecePrefab, spawnPts[i], Quaternion.identity);
                go.GetComponent<Rigidbody>().isKinematic = true;
                go.transform.SetParent(parent.transform, false);
                pieces[pidx++] = go;
            }
        }

        //make solid
        float xscale = transform.localScale.x;
        float yscale = transform.localScale.y;
        float zscale = transform.localScale.z;
        if (numPenLayers < zscale) {
            Vector3 startOffset = new Vector3(0, 0, -(numPenLayers) * zSize);
            GameObject scaleParent = new GameObject();
            scaleParent.transform.localScale = new Vector3(1, 1, zscale);
            scaleParent.transform.position = gameObject.transform.position + new Vector3(0, 0, zscale / 2); //edge 

            GameObject solid = (GameObject)Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            solid.GetComponent<Rigidbody>().isKinematic = true;
            solid.transform.SetParent(scaleParent.transform, true);
            solid.transform.position = transform.position;

            float newScale = zscale - numPenLayers * zSize;
            scaleParent.transform.localScale = new Vector3(1, 1, newScale); //assume whole number of scale units per block
            solid.transform.localRotation = transform.localRotation;
            scaleParent.transform.DetachChildren();
            solid.GetComponent<Rigidbody>().isKinematic = false; //
            //solid.transform.SetParent(parent.transform, true); //doesn't work correctly - moves obj over?
            Destroy(scaleParent);
        }

        parent.transform.localRotation = transform.rotation; //match to original rotation of the object we are replacing
        for (int i = 0; i < pieces.Length; i++) {
            if (pieces[i] != null) {
                pieces[i].GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        //this is the piece that was replaced...put this somewhere else for now
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 1000); 
        //Destroy(gameObject);
    }

    public float getMinActivePwr() {
        return minActivePwr;
    }
}
