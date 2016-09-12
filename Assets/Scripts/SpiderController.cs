using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class SpiderController : MonoBehaviour
{
    public enum SpiderState
    {
        Idle,
        Climbing,
        Walking,
        Attacking,
        Dying
    };

    //values that will be set in the Inspector
    public int startingHealth;
    public int damage;
    public AudioClip deathCry;
    public Material miniMapMaterial;
    public NavMeshAgent agent { get; private set; }

    //values for internal use
    private Animator _animator;
    private Rigidbody _rigidBody;
    private AudioSource _audioSource;
    private int _currentHealth;
    private SpiderState _state = SpiderState.Idle;
    public Transform _target;
    private float _navDelayMax = .5f;
    private float _navDelay;
    private Quaternion lookRotation;
    private IInteractable _interactionSpider;

    // Use this for initialization
    void Start ()
	{
        // Required components
        agent = GetComponentInChildren<NavMeshAgent>();
        _animator = GetComponent<Animator>();
	    _rigidBody = GetComponent<Rigidbody>();
	    _audioSource = GetComponent<AudioSource>();

        // Optional components
        _interactionSpider = (IInteractable)GetComponent(typeof(IInteractable));
        agent.updateRotation = false;
        agent.updatePosition = true;

        _currentHealth = startingHealth;
        _navDelay = _navDelayMax + .1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentHealth <= 0)
        {
            if (_state != SpiderState.Dying)
            {
                _state = SpiderState.Dying;
                _rigidBody.velocity = Vector3.zero;
                _audioSource.clip = deathCry;
                _audioSource.loop = false;
                _audioSource.PlayDelayed(.5f);
                _animator.SetTrigger("IsDieing");
                if (_interactionSpider != null)
                {
                    _interactionSpider.Enable(true);
                }
            }
            else if (_interactionSpider!= null)
            {
                RemoveCorpse();
            }
            return;
        }
        if (!_target || _state == SpiderState.Dying)
        {
            _rigidBody.velocity = Vector3.zero;
            return;
        }

        var distance = Vector3.Distance(transform.position, _target.position);

        if (distance < agent.stoppingDistance && _state != SpiderState.Attacking)
        {
            _state = SpiderState.Attacking;
            _animator.SetTrigger("IsAttacking");
        }

        if (_state == SpiderState.Walking || _state == SpiderState.Attacking)
        {
            Vector3 desiredVelocity = Vector3.zero;
            _navDelay += Time.deltaTime;

            //print(name + " is " + distance + "m away");
            if (distance < 5f)
            {
                //find the vector pointing from our position to the _target
                var _direction = transform.position - _target.position;

                //create the rotation we need to be in to look at the _target
                lookRotation = Quaternion.LookRotation(_direction);

                //var velocity = new Vector3(_direction.x*agent.speed*-1f, _rigidBody.velocity.y,
                //    _direction.z*agent.speed*-1f);

                //_rigidBody.velocity = velocity;
                //print("Distance:" + distance + ", transform:" + transform.position + ", _target:" + _target.position + ", setVelocity:" + velocity);

                var distanceCovered = (Time.deltaTime) * agent.speed;
                var fractionalMovement = distanceCovered / distance;
                transform.position = Vector3.Lerp(transform.position, _target.position, fractionalMovement);
            }
            else
            {
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
                    lookRotation *= Quaternion.Euler(Vector3.up*180); // Stupid blender model gets imported facing the wrong way...
                }

                _rigidBody.velocity = desiredVelocity;
            }
            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 360 / agent.angularSpeed); // TODO: use the agent.angularSpeed instead

        }
        else
        {
            _rigidBody.velocity = Vector3.zero;
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
    public void ApplyDamage(int damage)
    {
        _currentHealth -= damage;
    }

    public void DoDamage()
    {
        var distance = Vector3.Distance(transform.position, _target.position);
        if (distance <= agent.stoppingDistance)
        {
            _target.GetComponent<PlayerController>().ApplyDamage(damage);
        }
    }

    public void RemoveCorpse()
    {
        if (_interactionSpider!= null)
        {
            if (!_interactionSpider.IsEnabled())
            {
                Destroy(gameObject);
            }
        } else
        {
            Destroy(gameObject);    
        }
           
    }

    public void Spawn(Transform target)
    {
        _target = target;
        var child = transform.Find("MiniMapIcon").gameObject;
        child.GetComponent<MeshRenderer>().material = miniMapMaterial;
        _animator.SetTrigger("IsActive");
    }

    public void SetActive()
    {
        _state = SpiderState.Walking;
    }
    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
