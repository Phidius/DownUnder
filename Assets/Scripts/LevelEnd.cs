using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class LevelEnd : MonoBehaviour
{
    public AudioClip winningSound;

    private AudioSource _audioSource;
    private bool _levelFinished = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider collider)
    {
        var playerController = collider.GetComponent <PlayerController>();

        if (playerController && !_levelFinished)
        {
            _levelFinished = true;
            _audioSource.clip = winningSound;
            _audioSource.loop = false;
            _audioSource.volume = 0.9f;
            _audioSource.Play();
            playerController.FinishLevel();
            
        }
    }
}
