using UnityEngine;
using System.Collections;

public class InteractionInventory: Interactable
{
    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        InventoryController.Instance.AddInventory(gameObject);
    }
}
