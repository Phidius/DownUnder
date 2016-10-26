using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class SpiderController : MonoBehaviour, IHitable, IPatrolable
{
    public enum SpiderState
    {
        Idle,
        Climbing,
        Walking,
        Attacking,
        Dying,
        Dead
    };

    //values that will be set in the Inspector
    public float initialHealthMultiplier = 1;
    public float initialRunSpeedMultiplier = 3f;
    public float initialWalkSpeedMultiplier = 2f;
    public int initialDamageMultiplier = 1;
    public AudioClip walkSound;
    public AudioClip deathCry;
    public AudioClip hitSound;
    public Material miniMapMaterial;
    public NavMeshAgent agent { get; private set; }

    //values for internal use
    private Animator _animator;
    private AudioSource _audioSource;
    private EndFight _endFight;
    public float _currentHealth = 0;
    public float _runSpeed = 0;
    public float _damage = 0;
    public SpiderState _state = SpiderState.Idle;
    public Transform _target;
    private float _navDelayMax = .5f;
    private float _navDelay;
    private Quaternion lookRotation;
    private InteractionSpider _interactionSpider;
    public float _spiderSenseRadius = 5.0f;
    public int _gameDifficulty = -1;
    private ParticleSystem _webShooter;

    // Use this for initialization
    void Start()
    {
        // Required components
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Child components
        var spiderSense = GetComponentInChildren<SpiderSense>();
        _webShooter = GetComponentInChildren<ParticleSystem>();

        if (_webShooter)
        {
            print(_webShooter);
        }
        
        // Optional components
        _interactionSpider = (InteractionSpider)GetComponent(typeof(Interactable));

        // Global components
        _endFight = GameObject.FindObjectOfType<EndFight>();

        if (_endFight == null)
        {
            Debug.Log("This level is missing an EndFight!");
        }

        agent.updateRotation = false;
        agent.updatePosition = true;
        

        if (spiderSense)
        {
            _spiderSenseRadius = spiderSense.GetComponent<SphereCollider>().radius * 2;
        }

        _navDelay = _navDelayMax + .1f;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsGamePaused())
        {
            return;
        }

        if (_gameDifficulty != (int)(GameManager.Instance.GetDifficulty()))
        {
            // calculate maxHealth based on previous game difficulty
            var maxHealth = (initialHealthMultiplier*(_gameDifficulty + 1));
            var healthRatio = 1f;
            if (_currentHealth > 0)
            {
                healthRatio = _currentHealth/maxHealth;
            }
            _gameDifficulty = (int)(GameManager.Instance.GetDifficulty());
            // Recalculate the maxHealth based on current game difficulty
            maxHealth = (initialHealthMultiplier * (_gameDifficulty + 1));
            // Recalculate health and spawn rate
            _currentHealth = healthRatio*maxHealth;
            _runSpeed = initialRunSpeedMultiplier * (_gameDifficulty + 1);

            agent.speed = initialWalkSpeedMultiplier * (_gameDifficulty + 1);
            _damage = initialDamageMultiplier * (_gameDifficulty + 1);
        }

        if (_endFight.hasBegun)
        {
            agent.speed = _runSpeed;
            _target = _endFight.target;
        }
        if (_state == SpiderState.Dead)
        {
            RemoveCorpse();
            return;
        }

        if (!_target)
        {
            return;
        }

        var distance = Vector3.Distance(transform.position, _target.position);

        if (distance < agent.stoppingDistance && _state != SpiderState.Attacking)
        {
            _state = SpiderState.Attacking;
            _animator.speed = 1f;
            _animator.SetTrigger("IsAttacking");
            if (_webShooter != null)
            {
                _webShooter.Play();
            }
        }

        if (_state == SpiderState.Walking)// || _state == SpiderState.Attacking)
        {
            var desiredVelocity = Vector3.zero;
            _navDelay += Time.deltaTime;
            
            if (distance < _spiderSenseRadius)
            {
                agent.Stop();
                //find the vector pointing from our position to the _target
                var _direction = transform.position - _target.position;
                _animator.speed = 2f;
                //create the rotation we need to be in to look at the _target
                lookRotation = Quaternion.LookRotation(_direction);

                var distanceCovered = (Time.deltaTime) * _runSpeed;
                var fractionalMovement = distanceCovered / distance;
                transform.position = Vector3.Lerp(transform.position, _target.position, fractionalMovement);
            }
            else
            {
                agent.Resume();
                _animator.speed = 1f;
                _navDelayMax = .5f;

                if (distance < _spiderSenseRadius * 3)
                {
                    _navDelayMax = 1f;
                }
                else if (distance < _spiderSenseRadius*6)
                {
                    _navDelayMax = 2f;
                }
                else if (distance < _spiderSenseRadius*10)
                {
                    _navDelayMax = 3f;
                }

                if (_navDelay > _navDelayMax)
                {
                    _navDelay = 0f;
                    agent.SetDestination(_target.position);
                }

                desiredVelocity = agent.desiredVelocity;
                // Get the direction and speed from the agent to get to the destination

                //create the rotation we need to be in to look at the _target
                if (desiredVelocity != Vector3.zero)
                {
                    lookRotation = Quaternion.LookRotation(desiredVelocity);
                    lookRotation *= Quaternion.Euler(Vector3.up * 180); // Stupid blender model gets imported facing the wrong way...
                }
                
            }
            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 360 / agent.angularSpeed);

        }
    }


    public SpiderState GetState()
    {
        return _state;
    }

    public void ResetState()
    {
        // Only reset the state if the spider is attacking... otherwise, the spider might come back to life after an attack!
        if (_state == SpiderState.Attacking)
        {
            _state = SpiderState.Walking;
        }

    }

    public void DoDamage()
    {
        var distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= agent.stoppingDistance * 1.5f)
        {
            var hitable = (IHitable)_target.GetComponent(typeof(IHitable));
            if (hitable != null)
            {
                hitable.Hit(_damage);
            }
        }
    }

    public void RemoveCorpse()
    {
        if (_state != SpiderState.Dead)
        {
            return;
        }

        if (_interactionSpider != null)
        {
            if (!_interactionSpider.IsEnabled())
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Spawn(Transform target)
    {
        _target = target;
        var child = transform.Find("MiniMapIcon").gameObject;
        child.GetComponent<MeshRenderer>().material = miniMapMaterial;
        var animator = GetComponent<Animator>();

        animator.SetTrigger("IsActive");
    }

    public void SetActive()
    {
        _state = SpiderState.Walking;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetDead()
    {
        _state = SpiderState.Dead;

        if (_interactionSpider != null)
        {
            _interactionSpider.Enable(true);
        }
    }

    public void Hit(float damage)
    {
        if (_state == SpiderState.Dying || _state == SpiderState.Dead || _state == SpiderState.Idle)
        {
            return; // No use beating a dead horse
        }

        _currentHealth -= damage;
        if (damage > 0)
        {
            _audioSource.clip = hitSound;
            _audioSource.loop = false;
            _audioSource.Play();
            //_particle.Play();
            Invoke("FinishHit", hitSound.length);
        }

        if (_currentHealth <= 0)
        {
            _state = SpiderState.Dying;
            agent.Stop();
            Invoke("StartDyingSound", hitSound.length);
            _animator.speed = 1f;
            _animator.SetTrigger("IsDieing");

            if (_interactionSpider != null)
            {
                RemoveCorpse();
            }
            return;
        }
        
    }

    public void SetNextWaypoint(WayPointController waypoint)
    {
        if (_target && _target.tag != "Player")
        {
            _target = waypoint.transform;
        }

    }

    private void FinishHit()
    {
        if (_state == SpiderState.Dying || _state != SpiderState.Dead)
        {
            return;
        }
        //_particle.Stop();
        _audioSource.clip = walkSound;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void StartDyingSound()
    {
        _audioSource.clip = deathCry;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    public Transform CurrentTarget()
    {
        return _target;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
