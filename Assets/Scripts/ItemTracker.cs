using UnityEngine;
using System.Collections;

public class ItemTracker : MonoBehaviour {

    private static ItemTracker instance;
    

    private GameObject usingItem;
    private bool isUsingItem = false;

	void Awake () {
        instance = this;
	}

    public static ItemTracker getInstance() {
        if (instance == null) {
            Debug.LogError("No ItemTracker instance. Make sure this script is on a GameObject in the scene");
        }
        return instance;
    }

	void Update () {

	
	}

    public bool useItem(GameObject item) {
        if (!isUsingItem) {
            usingItem = item;
            isUsingItem = true;
            return true;
        }
        return false;
    }

    public bool stopUsingItem() {
        if (isUsingItem) {
            isUsingItem = false;
            usingItem = null;
            return true;
        }
        return false;
    }
}
