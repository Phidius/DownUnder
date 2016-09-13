using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class TrapdoorController : MonoBehaviour, IHitable, ISpawner
{
    public float initialHealth = 10;
    public AudioClip hitSound;
    public GameObject spawnPrefab;

    private float _currentHealth;
    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;

    void Start()
    {
        _currentHealth = initialHealth;
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();
        if (!_meshRenderer)
        {
            // The MeshRenderer is removed on destruction so that the player can fall through
            throw new NotImplementedException(name + " does not have a MeshRenderer");
        }
    }

    void Update()
    {
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public float GetHealth()
    {
        return _currentHealth;
    }

    public void Hit(float damage)
    {
        _currentHealth -= damage;
        _audioSource.clip = hitSound;
        _audioSource.Play();
        if (_meshRenderer)
        {
            var color = _meshRenderer.material.color;
            color.a = _currentHealth / initialHealth;
            _meshRenderer.material.color = color;
        }
    }

    public void Spawn()
    {
        var spawn = (GameObject) Instantiate(spawnPrefab, transform.position, Quaternion.identity);
    }
}
