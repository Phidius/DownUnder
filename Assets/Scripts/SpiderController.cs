using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Rigidbody))] // Required to detect collisions
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
    public int startingHealth;
    public float runSpeed = 0.0f;
    public int damage;
    public AudioClip walkSound;
    public AudioClip deathCry;
    public AudioClip hitSound;
    public Material miniMapMaterial;
    public NavMeshAgent agent { get; private set; }

    //values for internal use
    private Animator _animator;
    private AudioSource _audioSource;
    private EndFight _endFight;
    private float _currentHealth;
    public SpiderState _state = SpiderState.Idle;
    public Transform _target;
    private float _navDelayMax = .5f;
    private float _navDelay;
    private Quaternion lookRotation;
    private InteractionSpider _interactionSpider;
    private float _spiderSenseRadius = 5.0f;

    // Use this for initialization
    void Start()
    {
        // Required components
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Child components
        var spiderSense = GetComponentInChildren<SpiderSense>();

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
            _spiderSenseRadius = spiderSense.GetComponent<SphereCollider>().radius;
        }

        _currentHealth = startingHealth;
        _navDelay = _navDelayMax + .1f;

        if (runSpeed < agent.speed)
        {
            runSpeed = agent.speed*2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_endFight.hasBegun)
        {
            agent.speed = runSpeed;
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
        }

        if (_state == SpiderState.Walking)// || _state == SpiderState.Attacking)
        {
            Vector3 desiredVelocity = Vector3.zero;
            _navDelay += Time.deltaTime;
            
            if (distance < _spiderSenseRadius)
            {
                agent.Stop();
                //find the vector pointing from our position to the _target
                var _direction = transform.position - _target.position;
                _animator.speed = 2f;
                //create the rotation we need to be in to look at the _target
                lookRotation = Quaternion.LookRotation(_direction);

                var distanceCovered = (Time.deltaTime) * runSpeed;
                var fractionalMovement = distanceCovered / distance;
                transform.position = Vector3.Lerp(transform.position, _target.position, fractionalMovement);
            }
            else
            {
                agent.Resume();
                _animator.speed = 1f;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 360 / agent.angularSpeed); // TODO: use the agent.angularSpeed instead

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
        if (distance <= agent.stoppingDistance)
        {
            var hitable = (IHitable)_target.GetComponent(typeof(IHitable));
            if (hitable != null)
            {
                hitable.Hit(damage);
            }
        }
    }

    public void RemoveCorpse()
    {
        if (_state != SpiderState.Dead)
        {
            return;
        }
        //print("Is _interactionSpider != null? " + _interactionSpider != null);  

        if (_interactionSpider != null)
        {
            //print("Is !_interactionSpider.IsEnabled()? " + !_interactionSpider.IsEnabled());
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
        if (_state == SpiderState.Dying || _state == SpiderState.Dead)
        {
            return; // No use beating a dead horse
        }
        _currentHealth -= damage;
        _audioSource.clip = hitSound;
        _audioSource.loop = false;
        _audioSource.Play();

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
        Invoke("StartWalkingSound", hitSound.length);
    }

    public void SetNextWaypoint(WayPointController waypoint)
    {
        if (_target && _target.tag != "Player")
        {
            _target = waypoint.transform;
        }

    }

    private void StartWalkingSound()
    {
        if (_state == SpiderState.Dying || _state != SpiderState.Dead)
        {
            return;
        }
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
