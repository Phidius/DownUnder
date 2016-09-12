using UnityEngine;
using System.Collections.Generic;
using System;

public class InteractionSpider : MonoBehaviour, IInteractable {

    public List<GameObject> lootItems;
    public bool isInteractable = true;
    private Behaviour _halo;
    
	// Use this for initialization
	void Start () {
        _halo = GetComponent("Halo") as Behaviour;
    }
	
    public List<GameObject> GetLoot()
    {
        var result = new List<GameObject>(lootItems);
        lootItems.Clear(); // Can only get the loot once
        Enable(false);
        return result;
    }
    
    public void Interact(PlayerController player)
    {
        foreach (var prefab in lootItems)
        {
            var loot = Instantiate(prefab, transform.position, Quaternion.identity);
        }
        lootItems.Clear(); // Can only get the loot once
        Enable(false);
    }
    
    public void Enable(bool enable)
    {
        isInteractable = enable;
    }

    public bool IsEnabled()
    {
        return isInteractable;
    }

    public void Highlight(PlayerController player, bool show)
    {
        if (IsEnabled() && show && Vector3.Distance(player.transform.position, transform.position) < 5f)
        {
            _halo.enabled = true;
        }
        else
        {
            _halo.enabled = false;
        }
    }
}
