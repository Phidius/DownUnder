using UnityEngine;
using System.Collections;

public class UsableFosters : Usable {

    public float staminaRestored = 50;
    public AudioClip[] audioClips;

    private AudioSource _audioSource;

    protected override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
    }

    public override void Use()
    {
        print("Give " + player.name + " some stamina");
        var delay = 0f;
        player.ApplyStamina(staminaRestored);

        if (audioClips.Length > 0)
        {
            var index = 0;
            if (GameManager.Instance.HasBeenPlayed(this.name))
            {
                // Play the first clip the first time, otherwise select at random
                index = Random.Range(0, audioClips.Length);
            }

            if (audioClips[index] == null)
            {
                // Not all sound clips have to be present... skip playing this one if it's null.
                Debug.Log("Null sound clip in " + name + "... no sound will be played");
            }
            else
            {
                _audioSource.loop = false;
                delay = audioClips[index].length;
                _audioSource.clip = audioClips[index];
                _audioSource.Play();
            }
            GameManager.Instance.BeenPlayed(this.name);

        }
        Invoke("DelayedRemoval", delay);
    }

    private void DelayedRemoval()
    {
        InventoryController.Instance.RemoveInventory(this.gameObject);
    }
}
