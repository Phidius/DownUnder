using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class BoomerangController : MonoBehaviour
{
    public enum BoomerangState
    {
        Rest,
        Swing,
        ThrowAway,
        ThrowReturn
    };

    public int meleeDamage;
    public float throwSpeed;
    public AudioClip swingSound;
    public AudioClip hitSound;

    private Animator _animator;
    private AudioSource _audioSource;
    private BoomerangState _state = BoomerangState.Rest;
    private Vector3 _target;
    private GameObject _parent;
    private Vector3 _offset;
    
	// Use this for initialization
	void Start ()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _parent = transform.parent.gameObject;
        _offset = transform.localPosition; //new Vector3(0.11f, -0.1f, 0.4f);
	}
	
	// Update is called once per frame
	void Update () {
        var step = throwSpeed * Time.deltaTime;
        if (_state == BoomerangState.ThrowAway)
	    {
            
            transform.position = Vector3.MoveTowards(transform.position, _target, step);
	        if (Vector3.Distance(transform.position, _target) < .01f)
	        {
	            // Turn around
                _state = BoomerangState.ThrowReturn;
	        }
        }
        else if (_state == BoomerangState.ThrowReturn)
        {
            transform.position = Vector3.MoveTowards(transform.position, _parent.transform.position + _offset, step);
            if (Vector3.Distance(transform.position, _parent.transform.position + _offset) < .01f)
            {
                // Return to player's "hand"
                _state = BoomerangState.Rest;
                transform.parent = _parent.transform;
                transform.localPosition = _offset;
                _animator.SetBool("Flying", false);

            }
        }
	}


    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "SpiderSense")
        {
            return;
        }

        var spiderController = collider.GetComponent<SpiderController>();

        if (spiderController && _state != BoomerangState.Rest)
        {
            spiderController.ApplyDamage(meleeDamage);
            _state = BoomerangState.ThrowReturn;
            _audioSource.clip = hitSound;
            _audioSource.Play();
            return;
        }

        var trapdoorController = collider.GetComponent<TrapdoorController>();
        if (trapdoorController && _state != BoomerangState.Rest)
        {
            trapdoorController.ApplyDamage(meleeDamage);
            _state = BoomerangState.ThrowReturn;
            _audioSource.clip = hitSound;
            _audioSource.Play();
            return;
        }

        if (_state == BoomerangState.ThrowAway)
        {
            // No sound effect when hitting terrain
            _state = BoomerangState.ThrowReturn;
        }

    }
    public BoomerangState GetState()
    {
        return _state;
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    var spiderController = collision.collider.GetComponent<SpiderController>();
    //    var playerController = GetComponent<Collider>().GetComponent<PlayerController>();

    //    if (spiderController && _state != BoomerangState.Rest)
    //    {
    //        spiderController.ApplyDamage(meleeDamage);
    //        _audioSource.clip = hitSound;
    //        _audioSource.Play();
    //    }
    //    _state = BoomerangState.ThrowReturn;
    //    if (playerController)
    //    {
    //        if (_state == BoomerangState.ThrowReturn)
    //        {
    //            // Return to player's "hand"
    //            _animator.SetBool("Flying", false);
    //            _rigidBody.velocity = Vector3.zero;
    //            _state = BoomerangState.Rest;
    //            transform.parent = _parent.transform;
    //            transform.localPosition = _offset;
    //        }
    //    }
    //    else
    //    {
    //        _state = BoomerangState.ThrowReturn;
    //    }

    //}

    public void Swing()
    {
        if (_state == BoomerangState.Rest)
        {
            _animator.SetTrigger("Swing");
            _state = BoomerangState.Swing;

            _audioSource.clip = swingSound;
            _audioSource.Play();
        }

    }
    
    public void ResetState()
    {
        _state = BoomerangState.Rest;
    }

    public void Throw(Vector3 target)
    {
        if (_state == BoomerangState.Rest)
        {
            var position = transform.root.position;
            _animator.SetBool("Flying", true);
            transform.parent = null;
            transform.position = position;
            _target = target;
            _state = BoomerangState.ThrowAway;
            _audioSource.clip = swingSound;
            _audioSource.Play();

        }
    }
}
