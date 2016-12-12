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

    private bool fullComputed = false;

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

    //void Start() {
    void Awake() {
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

    void Start() {
        for (int y = 0; y < yCount; y++) {
            for (int x = 0; x < xCount; x++) {
                ObjectPool.getInstance().AddForceReceivers(gridPieces[y][x]);
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
    }

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
                    if (go != null) {
                        //if (gridPieceLockedIn[y][x][z]) { //this prevents pieces from being hit twice
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
                        //}
                    }

                }
            }
        }

        gameObject.SetActive(false);

        fullComputed = true;
        return pieces; 
    }

    public void cleanup() {
        pieces.Clear();
        //lots of pieces now, optimize for perf. note some is done by forcereceiver cleanup
        float maxDist = 100 * 100;
        for (int y = 0; y < yCount; y++) {
            for (int x = 0; x < xCount; x++) {
                for (int z = 0; z < zCount; z++) {
                    GameObject go = gridPieces[y][x][z];
                    if (go != null) {
                        if ((go.transform.position - gameObject.transform.position).sqrMagnitude > maxDist) {
                            //go.GetComponent<Collider>().enabled = false;
                            //go.GetComponent<Rigidbody>().isKinematic = true;
                            Destroy(go);
                        }
                    }

                }
            }
        }
        fullComputed = false;
    }


    public float getMinActivePwr() {
        return minActivePwr;
    }
}
