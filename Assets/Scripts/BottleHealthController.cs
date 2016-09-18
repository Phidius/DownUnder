using UnityEngine;

public class BottleHealthController : Interactable {

    public float healthRestored = 10;

    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        player.Hit(-healthRestored);
    }
    
}
