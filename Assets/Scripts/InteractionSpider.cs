using UnityEngine;
using System.Collections.Generic;

public class InteractionSpider: Interactable {

    public List<GameObject> contents;
    
    public override void Start()
    {
        base.Start();
        base.Enable(false);
    }
    public override void Interact(PlayerController player)
    {
        if (!_highlighted || Vector3.Distance(player.transform.position, transform.position) > 5f)
        {
            return;
        }
        foreach (var prefab in contents)
        {
            Instantiate(prefab, transform.position + new Vector3(0f, .2f, 0f), Quaternion.identity);
        }
        contents.Clear(); // Can only get the loot once
        Enable(false);
        
    }
}
