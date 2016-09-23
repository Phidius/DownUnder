using UnityEngine;
using System.Collections;
using System;

public class InteractionStamina : Interactable
{
    public float staminaRestored = 50;

    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        player.ApplyStamina(staminaRestored);
        Destroy(gameObject);
    }

}
