using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class TrapdoorController : MonoBehaviour, IHitable
{
    public float initialHealth = 4f;
    public int maxSpawns = 3;
    public float spawnDelay = 20f;
    public AudioClip hitSound;
    public GameObject spawnPrefab;

    private float _currentHealth;
    private float _nextSpawnTime;
    private MeshRenderer _meshRenderer;
    private AudioSource _audioSource;
    private GameObject[] _spawns;

    void Start()
    {
        _spawns =  new GameObject[maxSpawns];
        _currentHealth = initialHealth;
        _nextSpawnTime = Time.time + spawnDelay;
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


        if (Time.time > _nextSpawnTime)
        {
            for (var counter = 0;counter < _spawns.Length;counter++)
            {
                var spawn = _spawns[counter];
                //print(spawn);
                if (spawn == null)
                {
                    _nextSpawnTime = _nextSpawnTime + spawnDelay;
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
            color.a = _currentHealth / initialHealth;
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
