using UnityEngine;
using System.Collections;
using VRTK;

public class ItemTracker : MonoBehaviour {

    private static ItemTracker instance;
    

    private GameObject usingItem;
    private bool isUsingItem = false;

    private GameObject controllerGo;

    private VRTK_ControllerEvents controller;



    void Awake() {
        instance = this;
        initController();
    }

    private void initController() {
        controllerGo = GameObject.Find("Controller (right)");
        controller = controllerGo.GetComponent<VRTK_ControllerEvents>();
    }


    public static ItemTracker getInstance() {
        if (instance == null) {
            Debug.LogError("No ItemTracker instance. Make sure this script is on a GameObject in the scene");
        }
        return instance;
    }

	void Update () {
	}

    public void addGrabbedListener(ControllerInteractionEventHandler grabHandler) {
        if (controller == null) {
            initController();
        }
        controller.TriggerPressed += grabHandler;
    }

    public void addUngrabbedListener(ControllerInteractionEventHandler ungrabHandler) {
        if (controller == null) {
            initController();
        }
        controller.TriggerPressed += ungrabHandler;
    }

    public void removeGrabbedListener(ControllerInteractionEventHandler grabHandler) {
        if (controller == null) {
            initController();
        }
        controller.TriggerPressed -= grabHandler;
    }
    public void removeUngrabbedListener(ControllerInteractionEventHandler ungrabHandler) {
        if (controller == null) {
            initController();
        }
        controller.TriggerPressed -= ungrabHandler;
    }

    public bool useItem(GameObject item) {
        if (!isUsingItem) {
            usingItem = item;
            isUsingItem = true;
            return true;
        } else {
            Debug.Log("Still using item: " + usingItem);
            return false;
        }
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
