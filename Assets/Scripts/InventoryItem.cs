using UnityEngine;
using System.Collections;
using VRTK;

public class InventoryItem : VRTK_InteractableObject {


    [SerializeField]
    private GameObject itemPrefab; //item that will be spawned when using this inventory item

    private GameObject itemInstance;

	protected override void Start () {
        base.Start();
	}
	
	protected override void Update () {
        base.Update();
	}

    //CurrentUsingObject will be the controller that is activating this event
    public override void StartUsing(GameObject currentUsingObject) {
        ItemTracker itt = ItemTracker.getInstance();
        if (itt.useItem(gameObject)) {
            Debug.Log("Inventory item use: " + gameObject.name);
            itemInstance = (GameObject)Instantiate(itemPrefab);
            //transition from inventory item to actual item in hand
            itt.stopUsingItem();
            Grabable g = itemInstance.GetComponent<Grabable>();
            itemInstance.transform.position = currentUsingObject.transform.position;
            g.Grabbed(currentUsingObject);
        } else {
            Debug.Log("Can't use item: " + gameObject.name);
        }
    }

    public override void StopUsing(GameObject previousUsingObject) {
        ItemTracker itt = ItemTracker.getInstance();
        itt.stopUsingItem();
        //base.StopUsing(previousUsingObject);
        Debug.Log("Inventory item stop using: " + gameObject.name);
        if (itemInstance != null) {
            itemInstance.transform.parent = null;
        }
    }
}
