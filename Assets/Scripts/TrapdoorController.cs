using System;
using UnityEngine;
using System.Collections;

public class TrapdoorController : MonoBehaviour
{
    public float initialHealth = 10;
    public GameObject barrier;

    private float _currentHealth;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _currentHealth = initialHealth;
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
            Destroy(barrier);
            Destroy(_meshRenderer);
        }
    }

    public void ApplyDamage(int damage)
    {
        _currentHealth -= damage;
        if (_meshRenderer)
        {
            var color = _meshRenderer.material.color;
            color.a = _currentHealth / initialHealth;
            _meshRenderer.material.color = color;
        }
    }

    public float GetHealth()
    {
        return _currentHealth;
    }
}
