using UnityEngine;
using System.Collections;

public class InteractionInventory: Interactable
{
    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        if (_highlighted)
        {
            _highlighted = false;
            InventoryController.Instance.AddInventory(gameObject);
        }
    }
}
