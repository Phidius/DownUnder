using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour
{
    public int spawnLevel = 1;
    public int levelMultiplier = 10;
    public int maxSpawns = 5;
    
    private int _currentLevel = 1;
    private float _nextSpawnTime;
	// Use this for initialization
	void Start ()
	{
	    _nextSpawnTime = Time.time + (1/spawnLevel)*20;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time > _nextSpawnTime)
	    {
	        var spawners = GetComponentsInChildren(typeof (ISpawner));

	        if (spawners == null || spawners.Length == 0)
	        {
                // no more spawners
	            return;
	        }
            _nextSpawnTime = _nextSpawnTime + (1 / spawnLevel) * 5;
	        var spawnerIndex = Random.Range(0, spawners.Length);
            ((ISpawner)spawners[spawnerIndex]).Spawn();
	    }
	}
}
