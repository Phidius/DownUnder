using UnityEngine;
using System.Collections;
using System;

public class BottleStaminaController : MonoBehaviour
{
    public float staminaRestored = 50;

    public void Use(PlayerController player)
    {
        player.ApplyStamina(staminaRestored);
        Destroy(gameObject);
    }
}
