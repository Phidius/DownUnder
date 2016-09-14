using UnityEngine;
using System.Collections;
using System;

public class BottleHealthController : MonoBehaviour, IInteractable {

    public float healthRestored = 10;
    private Behaviour _halo;

    void Start()
    {
        _halo = GetComponent("Halo") as Behaviour;
    }

    public void Enable(bool enable)
    {
        // Always enabled
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

    public void Interact(PlayerController player)
    {
        player.Hit(-healthRestored);
        Destroy(gameObject);
    }

    public bool IsEnabled()
    {
        return true;
    }
    
}
