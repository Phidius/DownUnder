﻿using UnityEngine;
using System.Collections;

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
    private Transform _parent; // The transform that will be moving through world space - this.transform will only move in local space, never through the world
    private Transform _weaponSpot; // The transform that the _parent will reside in while at rest
    
	// Use this for initialization
	void Start ()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _parent = transform.parent;
        _weaponSpot = _parent.transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
        var step = throwSpeed * Time.deltaTime;
        //print(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        if (_state == BoomerangState.ThrowAway)
	    {            
            _parent.position = Vector3.MoveTowards(_parent.position, _target, step);
	        if (Vector3.Distance(_parent.position, _target) < .01f)
	        {
	            // Turn around
                _state = BoomerangState.ThrowReturn;
	        }
        }
        else if (_state == BoomerangState.ThrowReturn)
        {
            _parent.position = Vector3.MoveTowards(_parent.position, _weaponSpot.position, step);
            if (Vector3.Distance(_parent.position, _weaponSpot.position) < .01f)
            {
                // Return to player's "hand"
                _state = BoomerangState.Rest;
                _parent.parent = _weaponSpot;
                //_parent.position = Vector3.zero;
                _parent.localPosition = Vector3.zero;
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
            Invoke("ResetState", _animator.GetCurrentAnimatorStateInfo(0).length);
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
            var position = transform.root.position; // Model's position in world space
            _parent.parent = null; // Free the Boomerang transform to move through world space
            _parent.position = position; // But first, move it to the starting point
            _target = target;
            _state = BoomerangState.ThrowAway;
            _audioSource.clip = swingSound;
            _audioSource.Play();
            _animator.SetBool("Flying", true);

        }
    }
}
