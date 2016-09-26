﻿using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class InteractionHealth : Interactable {

    public float healthRestored = 10;
    public AudioClip[] audioClips;

    private AudioSource _audioSource;

    public override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
    }
    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        if (IsEnabled() == false)
        {
            return;
        }

        var delay = 0f;
        player.Hit(-healthRestored);

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

        base.Enable(false);
        Destroy(gameObject, delay);
    }
    
}
