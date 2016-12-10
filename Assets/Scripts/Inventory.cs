using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory : MonoBehaviour {

    [SerializeField]
    private GameObject attachTo;

    [SerializeField]
    private List<GameObject> itemSlotGameObjects;

    [SerializeField]
    private List<GameObject> inventoryPrefabs; //index corresponds to itemSlot index

	void Start () {
        gameObject.transform.SetParent(attachTo.transform, false);

        if (inventoryPrefabs.Count > itemSlotGameObjects.Count) {
            Debug.LogError("More inventory than slots available");
        }
        for(int i = 0; i < inventoryPrefabs.Count; i++) {
            GameObject inv = Instantiate(inventoryPrefabs[i]);
            GameObject slot = itemSlotGameObjects[i];
            //GameObject scaleHolder = new GameObject();
            //scaleHolder.name = inv.name + " parent";
            Quaternion rot = slot.transform.rotation;
            Vector3 loc = slot.transform.position;
            inv.transform.SetParent(slot.transform, true);
            inv.transform.rotation = rot * inv.transform.rotation;
            inv.transform.position = loc;
            //scaleHolder.transform.SetParent(slot.transform, false);
        }

	}
	
	void Update () {
	
	}
}
