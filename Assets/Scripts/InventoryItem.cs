using UnityEngine;
using System.Collections;
using VRTK;

public class InventoryItem : VRTK_InteractableObject {


    [SerializeField]
    private GameObject itemPrefab; //item that will be spawned when using this inventory item

	protected override void Start () {
        base.Start();
	}
	
	protected override void Update () {
        base.Update();
	}
}
