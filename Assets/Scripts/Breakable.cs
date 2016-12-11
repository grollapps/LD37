using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    //private Vector3[][][] spawnLayers; //Z layers
    private GameObject[][][] gridPieces; // y, x, z order
    private bool[][][] gridPieceLockedIn; // y, x, z order
    private GameObject pieceParent;

    private float myxSize;
    private float myySize;
    private float myzSize;
    private int xCount;
    private int yCount;
    private int zCount;

    private Vector3 refPoint;

    private float pxSize;
    private float pySize;
    private float pzSize;
    private float pxHalfSize;
    private float pyHalfSize;
    private float pzHalfSize;

    private List<GameObject> pieces;

    void Start() {
        myxSize = transform.localScale.x;
        myySize = transform.localScale.y;
        myzSize = transform.localScale.z;

        GameObject tmp = (GameObject)Instantiate(piecePrefab);
        pxSize = tmp.transform.localScale.x;
        pySize = tmp.transform.localScale.y;
        pzSize = tmp.transform.localScale.z;
        pxHalfSize = 0.5f * pxSize;
        pyHalfSize = 0.5f * pySize;
        pzHalfSize = 0.5f * pzSize;
        Destroy(tmp);
        //make sure scale is a mulitiple of piecePrefab size

        xCount = Mathf.FloorToInt(myxSize / pxSize);
        yCount = Mathf.FloorToInt(myySize / pySize);
        zCount = Mathf.FloorToInt(myzSize / pzSize);

        pieces = new List<GameObject>(xCount * yCount * zCount / 2); //prealloc a few

        refPoint = Vector3.Scale(new Vector3(1, -1, 1), transform.localScale / 2);

        Quaternion objRot = transform.rotation;
        Vector3 objPos = transform.position;
        //Vector3 scale = transform.localScale;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        pieceParent = new GameObject();
        pieceParent.name = "gridPieceParent";
        pieceParent.transform.position = Vector3.zero;
        pieceParent.transform.rotation = Quaternion.identity;
        //pieceParent.transform.localScale = scale;

        gridPieces = new GameObject[yCount][][];
        gridPieceLockedIn = new bool[yCount][][];
        //points ordered from (1,-1,1), -z, -x, +y
        for (int y = 0; y < yCount; y++) {

            GameObject[][] xs = new GameObject[xCount][];
            bool[][] xsLk = new bool[xCount][];
            for (int x = 0; x < xCount; x++) {

                GameObject[] zs = new GameObject[zCount];
                bool[] zsLk = new bool[zCount];
                for (int z = 0; z < zCount; z++) {
                    zs[z] = (GameObject)Instantiate(piecePrefab, getPosition(x, y, z), Quaternion.identity);
                    zs[z].name = "{" + x + "," + y + "," + z + "}";
                    zs[z].GetComponent<Collider>().enabled = false;
                    zs[z].GetComponent<Rigidbody>().isKinematic = true; //TODO ?
                    zs[z].transform.SetParent(pieceParent.transform, true);
                    zsLk[z] = true;
                }

                xs[x] = zs;
                xsLk[x] = zsLk;
            }

            gridPieces[y] = xs;
            gridPieceLockedIn[y] = xsLk;
        }

        pieceParent.transform.rotation = objRot; //all pieces should now align to the full obj
        pieceParent.transform.position = objPos;
        transform.rotation = objRot;
        transform.position = objPos;

        //disable sub pieces until they are needed
        for (int y = 0; y < yCount; y++) {
            for (int x = 0; x < xCount; x++) {
                for (int z = 0; z < zCount; z++) {
                    GameObject go = gridPieces[y][x][z];
                    go.SetActive(false);
                }
            }
        }
    }

    //Get local coords for grid cell x,y,z (ref point 0,0,0 = lc(1,-1,1))
    private Vector3 getPosition(int x, int y, int z) {
        float xPos = refPoint.x - x * pxSize;
        float yPos = refPoint.y + y * pySize;
        float zPos = refPoint.z - z * pzSize;
        //translate between reference point and piece center
        xPos -= pxHalfSize;
        yPos += pyHalfSize;
        zPos -= pzHalfSize;
        return new Vector3(xPos, yPos, zPos);
    }

    void Update() {
        //debug_event();
    }

    //bool isDivided = false;
    //private void debug_event() {
    //    if (Input.GetKey(KeyCode.Keypad0) && !isDivided) {
    //        Debug.Log("Dividing");
    //        divideAll();
    //        breakItDownByLayer(2);
    //        isDivided = true;
    //        Debug.Break();
    //    }
    //}

    //private void computeDivisions() {
    //   if (layersComputed) {
    //       return;
    //   }
    //   //assume cube for now
    //   GameObject tmp = (GameObject)Instantiate(piecePrefab, new Vector3(0, -1000, 0), Quaternion.identity);
    //   Collider c = tmp.GetComponent<Collider>();
    //   float xHalf = c.bounds.extents.x;
    //   float yHalf = c.bounds.extents.y;
    //   float zHalf = c.bounds.extents.z;
    //   Destroy(tmp);

    //   xSize = 2 * xHalf;
    //   ySize = 2 * yHalf;
    //   zSize = 2 * zHalf;

    //   Vector3 halfCubeSize = new Vector3(xHalf, -yHalf, zHalf);
    //   Collider myC = gameObject.GetComponent<Collider>();
    //   //find corner lower front corner
    //   float myx = myC.bounds.extents.x;
    //   float myy = myC.bounds.extents.y;
    //   float myz = myC.bounds.extents.z;
    //   //Vector3 frontCorner =  transform.position + new Vector3(myx, -myy, myz);
    //   Vector3 frontCorner = new Vector3(myx, -myy, myz);

    //   int maxXStep = Mathf.CeilToInt(myx * 2 / xSize);
    //   int maxYStep = Mathf.CeilToInt(myy * 2 / ySize);
    //   int maxZStep = Mathf.CeilToInt(myz * 2 / zSize);

    //   int xycount = maxXStep * maxYStep;
    //   int zcount = maxZStep;
    //   //Debug.Log(xycount + " XY subdivisions, " + zcount + " Z layers for " + gameObject.name);
    //   spawnLayers = new Vector3[zcount][];

    //   //find first cube pos
    //   Vector3 startPoint = (frontCorner - halfCubeSize);
    //   Vector3 curPos = startPoint;
    //   //lay'em out
    //   int i = 0;
    //   for (int z = 0; z < maxZStep; z++) {
    //       Vector3[] spawnPtsXY = new Vector3[xycount];
    //       for (int y = 0; y < maxYStep; y++) {
    //           for (int x = 0; x < maxXStep; x++) {
    //               curPos.x = startPoint.x - x * xSize;
    //               curPos.y = startPoint.y + y * ySize;
    //               curPos.z = startPoint.z - z * zSize;
    //               spawnPtsXY[i++] = (new Vector3(curPos.x, curPos.y, curPos.z));
    //           }
    //       }
    //       spawnLayers[z] = spawnPtsXY;
    //       i = 0;
    //   }
    //   layersComputed = true;


    //}

    // private void divideAll() {
    //     Vector3 pos = transform.position;
    //     transform.position = new Vector3(pos.x, pos.y, pos.z - 25);
    //     for (int z = 0; z < spawnLayers.Length; z++) {
    //         Vector3[] spawnPts = spawnLayers[z];
    //         for (int i = 0; i < spawnPts.Length; i++) {
    //             GameObject go = (GameObject)Instantiate(piecePrefab, spawnPts[i], transform.rotation);
    //         }
    //     }

    // }

    private float getForce(Vector3 forceDir, float force, Vector3 forceOrigin, Vector3 forceTarget) {
        float dist = (forceTarget - forceOrigin).sqrMagnitude;
        return force / dist;
    }

    //Returned game objects will be of type piecePrefab (if any)
    public List<GameObject> breakItDown(Vector3 forceDir, float force, Vector3 forceOrigin) {
        if (fullComputed) {
            return null;
        }

        //figure out which blocks in the grid will be hit by at least the min activation power
        for (int y = 0; y < yCount; y++) {
            for (int x = 0; x < xCount; x++) {
                for (int z = 0; z < zCount; z++) {
                    GameObject go = gridPieces[y][x][z];
                    if (gridPieceLockedIn[y][x][z]) {
                        float forceAtCenter = getForce(forceDir, force, forceOrigin, go.transform.position);
                        if (forceAtCenter > minActivePwr) {
                            gridPieceLockedIn[y][x][z] = false;
                            go.SetActive(true); //TODO?
                            go.GetComponent<Collider>().enabled = true;
                            go.GetComponent<Rigidbody>().isKinematic = false;
                            pieces.Add(go);
                        } else {
                            //TODO group into sub object
                            go.SetActive(true); //TODO?
                            go.GetComponent<Collider>().enabled = true;
                            go.GetComponent<Rigidbody>().isKinematic = true; //hold in position
                        }
                    }

                }
            }
        }

        gameObject.SetActive(false);

        //if (spawnLayers == null) {
        //    computeDivisions();
        //}
        //int numPenLayers = Mathf.Min(spawnLayers[0].Length, Mathf.CeilToInt(Mathf.Abs(penDist) / zSize)); //round up/down/all around?
        //breakItDownByLayer(numPenLayers);
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

    public void cleanup() {
        pieces.Clear();
        fullComputed = false;
    }

    //private void breakItDownByLayer(int numPenLayers) {
    //    Vector3 pos = transform.position;
    //    GameObject parent = new GameObject();
    //    parent.name = "breakable parent";
    //    parent.transform.position = transform.position;
    //    parent.transform.rotation = transform.rotation;
    //    int pieceCount = numPenLayers * spawnLayers[0].Length;
    //    //Debug.Log("Piececount=" + pieceCount);
    //    pieces = new GameObject[pieceCount];

    //    //make pieces
    //    int pidx = 0;
    //    for (int z = 0; z < numPenLayers & z < spawnLayers.Length; z++) {
    //        Vector3[] spawnPts = spawnLayers[z];
    //        for (int i = 0; i < spawnPts.Length; i++) {
    //            GameObject go = (GameObject)Instantiate(piecePrefab, spawnPts[i], Quaternion.identity);
    //            go.GetComponent<Rigidbody>().isKinematic = true;
    //            go.transform.SetParent(parent.transform, false);
    //            pieces[pidx++] = go;
    //        }
    //    }

    //    //make solid
    //    float xscale = transform.localScale.x;
    //    float yscale = transform.localScale.y;
    //    float zscale = transform.localScale.z;
    //    if (numPenLayers < zscale) {
    //        Vector3 startOffset = new Vector3(0, 0, -(numPenLayers) * zSize);
    //        GameObject scaleParent = new GameObject();
    //        scaleParent.transform.localScale = new Vector3(1, 1, zscale);
    //        scaleParent.transform.rotation = transform.rotation;
    //        //scaleParent.transform.position = gameObject.transform.position + new Vector3(0, 0, zscale / 2); //edge 

    //        GameObject solid = (GameObject)Instantiate(gameObject, Vector3.zero, Quaternion.identity);
    //        solid.GetComponent<Rigidbody>().isKinematic = true;
    //        solid.transform.SetParent(scaleParent.transform, true);
    //        //solid.transform.SetParent(scaleParent.transform, false);
    //        //solid.transform.position = transform.position;
    //        solid.transform.position = scaleParent.transform.position + new Vector3(0, 0, zscale / 2);

    //        float newScale = zscale - numPenLayers * zSize;
    //        scaleParent.transform.localScale = new Vector3(1, 1, newScale); //assume whole number of scale units per block
    //        solid.transform.position = Vector3.zero;
    //        //solid.transform.localRotation = transform.localRotation;
    //        scaleParent.transform.DetachChildren();
    //        solid.GetComponent<Rigidbody>().isKinematic = false; //
    //        //solid.transform.SetParent(parent.transform, true); //doesn't work correctly - moves obj over?
    //        Destroy(scaleParent);
    //    }

    //    //parent.transform.localRotation = transform.rotation; //match to original rotation of the object we are replacing
    //    for (int i = 0; i < pieces.Length; i++) {
    //        if (pieces[i] != null) {
    //            pieces[i].GetComponent<Rigidbody>().isKinematic = false;
    //        }
    //    }

    //    //this is the piece that was replaced...put this somewhere else for now
    //    gameObject.GetComponent<Rigidbody>().isKinematic = true;
    //    transform.position = new Vector3(pos.x, pos.y, pos.z - 1000);
    //    //Destroy(gameObject);
    //}

    public float getMinActivePwr() {
        return minActivePwr;
    }
}
