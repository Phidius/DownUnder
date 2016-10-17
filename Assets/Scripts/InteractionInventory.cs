using UnityEngine;
using System.Collections;

public class InteractionInventory: Interactable
{
    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        if (_highlighted)
        {
            InventoryController.Instance.AddInventory(gameObject);
        }
    }
}
