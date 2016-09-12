using UnityEngine;
using System.Collections;

public class BottleHealthController : MonoBehaviour {

    public float healthRestored = 10;

	public void Use(PlayerController player)
    {
        player.ApplyDamage(-healthRestored);
        Destroy(gameObject);
    }
}
