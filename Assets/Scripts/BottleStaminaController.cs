using UnityEngine;
using System.Collections;
using System;

public class BottleStaminaController : MonoBehaviour, IInteractable
{
    public float staminaRestored = 50;
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
        player.ApplyStamina(staminaRestored);
        Destroy(gameObject);
    }

    public bool IsEnabled()
    {
        return true;
    }
    
}
