using UnityEngine;
using System.Collections;
using VRTK;

public class ItemTracker : MonoBehaviour {

    private static ItemTracker instance;
    

    private GameObject usingItem;
    private bool isUsingItem = false;

    private GameObject controllerGo1;
    private GameObject controllerGo2;

    private VRTK_ControllerEvents controller1;
    private VRTK_ControllerEvents controller2;



    void Awake() {
        instance = this;
        initController();
    }

    private void initController() {
        controllerGo1 = GameObject.Find("Controller (left)");
        controllerGo2 = GameObject.Find("Controller (right)");
        if (controllerGo1 != null) {
            controller1 = controllerGo1.GetComponent<VRTK_ControllerEvents>();
        }
        if (controllerGo2 != null) {
            controller2 = controllerGo2.GetComponent<VRTK_ControllerEvents>();
        }
    }


    public static ItemTracker getInstance() {
        if (instance == null) {
            Debug.LogError("No ItemTracker instance. Make sure this script is on a GameObject in the scene");
        }
        return instance;
    }

	void Update () {
	}

    private void checkControllers() {
        if (controller1 == null || controller2 == null) {
            initController();
        }
    }

    public void addGrippedListener(ControllerInteractionEventHandler gripHandler) {
        checkControllers();
        controller1.GripPressed += gripHandler;
        controller2.GripPressed += gripHandler;
    }

    public void removeGrippedListener(ControllerInteractionEventHandler ungripHandler) {
        checkControllers();
        controller1.GripPressed -= ungripHandler;
        controller2.GripPressed -= ungripHandler;
    }

    public void addGrabbedListener(ControllerInteractionEventHandler grabHandler) {
        checkControllers();
        controller1.TriggerPressed += grabHandler;
        controller2.TriggerPressed += grabHandler;
    }

    public void addUngrabbedListener(ControllerInteractionEventHandler ungrabHandler) {
        checkControllers();
        controller1.TriggerPressed += ungrabHandler;
        controller2.TriggerPressed += ungrabHandler;
    }

    public void removeGrabbedListener(ControllerInteractionEventHandler grabHandler) {
        checkControllers();
        controller1.TriggerPressed -= grabHandler;
        controller2.TriggerPressed -= grabHandler;
    }
    public void removeUngrabbedListener(ControllerInteractionEventHandler ungrabHandler) {
        checkControllers();
        controller1.TriggerPressed -= ungrabHandler;
        controller2.TriggerPressed -= ungrabHandler;
    }

    //TODO separate hands
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
