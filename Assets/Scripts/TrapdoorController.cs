using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class TrapdoorController : MonoBehaviour, IHitable
{
    public float initialHealthMultiplier = 2f;
    public int initialSpawnsMultiplier = 2;
    public float initialSpawnDelay = 20f;
    public AudioClip hitSound;
    public GameObject spawnPrefab;

    public float _currentHealth;
    public float _currentSpawns;
    private float _currentDelay;

    private float _nextSpawnTime = 0f;
    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    private List<GameObject> _spawns = new List<GameObject>();
    private int _gameDifficulty = -1;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();
        if (!_meshRenderer)
        {
            // The MeshRenderer is removed on destruction so that the player can fall through
            throw new NotImplementedException(name + " does not have a MeshRenderer");
        }
        _gameDifficulty = (int)(GameManager.Instance.GetDifficulty());
        _currentHealth = initialHealthMultiplier * (_gameDifficulty + 1);
        _currentSpawns = initialSpawnsMultiplier * (_gameDifficulty + 1);
        _currentDelay = (initialSpawnDelay /= (_gameDifficulty + 1));
    }

    void Update()
    {
        if (GameManager.Instance.IsGamePaused())
        {
            return;
        }

        if (_gameDifficulty != (int) (GameManager.Instance.GetDifficulty()))
        {
            // calculate maxHealth based on previous game difficulty
            var maxHealth = (initialHealthMultiplier * (_gameDifficulty + 1));
            var healthRatio = 1f;
            if (_currentHealth > 0)
            {
                healthRatio = _currentHealth / maxHealth;
            }
            _gameDifficulty = (int) (GameManager.Instance.GetDifficulty());
            // Recalculate the maxHealth based on current game difficulty
            maxHealth = (initialHealthMultiplier * (_gameDifficulty + 1));
            // Recalculate health and spawn rate
            _currentHealth = healthRatio*maxHealth;
            _currentSpawns = initialSpawnsMultiplier*(_gameDifficulty + 1);
            _currentDelay = (initialSpawnDelay / (_gameDifficulty + 1));
        }

        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        if (_spawns.Count > _currentSpawns)
        {
            // Last In, Last Out
            for (var index = _spawns.Count - 1; index > _currentSpawns - 1; index--)
            {
                var spawn = _spawns[index];
                if (spawn != null)
                {
                    // Destroy this spawn before removing from the list.
                    Destroy(spawn);
                }
                _spawns.RemoveAt(index);
            }
        } else if (_spawns.Count < _currentSpawns)
        {
            // Add some blanks, let them spawn in due time
            for (var index = _spawns.Count; index < _currentSpawns; index++)
            {
                _spawns.Add(null);
            }
        }

        if (Time.time > _nextSpawnTime)
        {
            for (var counter = 0;counter < _spawns.Count;counter++)
            {
                var spawn = _spawns[counter];
                if (spawn == null)
                {
                    _nextSpawnTime += _currentDelay;
                    _spawns[counter] = Spawn();
                    break;
                }
            }
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
            color.a = _currentHealth / (initialHealthMultiplier * (_gameDifficulty + 1));
            _meshRenderer.material.color = color;
        }
    }

    public GameObject Spawn()
    {
        GameObject result = null;
        if (spawnPrefab)
        {
            var spawn = (GameObject)Instantiate(spawnPrefab, transform.position, Quaternion.identity);
            result = spawn;
            var spiderController = spawn.GetComponent<SpiderController>();
            var patrolable = (IPatrolable)spawn.GetComponent(typeof (IPatrolable));
            
            var waypoint = WayPointController.SetWayPoint(patrolable);
            spiderController.Spawn(waypoint.transform);
        }
        return result;
    }
}
