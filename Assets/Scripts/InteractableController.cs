using UnityEngine;
using System.Collections.Generic;

public class InteractableController : MonoBehaviour {

    public List<GameObject> lootItems;
    public bool isInteractable = true;
    private Behaviour _halo;
    
	// Use this for initialization
	void Start () {
        _halo = GetComponent("Halo") as Behaviour;
    }
	
    public void EnableHalo(bool enable)
    {
        if (isInteractable)
        {
            _halo.enabled = enable;
        }
    }

    public List<GameObject> GetLoot()
    {
        var result = new List<GameObject>(lootItems);
        lootItems.Clear(); // Can only get the loot once
        return result;
    }

    public int GetLootSize()
    {
        return lootItems.Count;     
    }
}
