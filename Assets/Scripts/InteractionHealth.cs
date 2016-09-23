using UnityEngine;

public class InteractionHealth : Interactable {

    public float healthRestored = 10;

    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        player.Hit(-healthRestored);
        Destroy(gameObject);
    }
    
}
