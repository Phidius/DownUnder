using UnityEngine;
using System.Collections;

public class BoomerangController : Weapon
{

    public float throwSpeed;

    private WeaponState _state = WeaponState.Rest;
    private Vector3 _target;
    
	// Update is called once per frame
	void Update () {
        var step = throwSpeed * Time.deltaTime;
        if (_state == WeaponState.ThrowAway)
	    {
            
            _parent.position = Vector3.MoveTowards(_parent.position, _target, step);
	        if (Vector3.Distance(_parent.position, _target) < .01f)
	        {
	            // Turn around
                _state = WeaponState.ThrowReturn;
	        }
        }
        else if (_state == WeaponState.ThrowReturn)
        {
            _parent.position = Vector3.MoveTowards(_parent.position, _weaponSlot.position, step);
            if (Vector3.Distance(_parent.position, _weaponSlot.position) < .01f)
            {
                // Return to player's "hand"
                _state = WeaponState.Rest;
                _parent.parent = _weaponSlot;
                _parent.localPosition = Vector3.zero; // _parent.position is the global position
                _parent.localRotation = Quaternion.identity;
                _parent.localScale = Vector3.one;
                
                _animator.SetBool("Flying", false);

            }
        }
	}


    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Event" || _state == WeaponState.Rest)
        {
            return;
        }
        var hitables = collider.GetComponents(typeof(IHitable));

        if (hitables == null || hitables.Length == 0)
        {
            if (_state == WeaponState.ThrowAway)
            {
                // Hitting anything without a hitable component simply causes the projectile to return
                _state = WeaponState.ThrowReturn;
            }
            return;
        }

        foreach (var component in hitables)
        {
            var hitable = (IHitable) component;
            hitable.Hit(meleeDamage);
            _state = WeaponState.ThrowReturn;
        }
    }

    public WeaponState GetState()
    {
        return _state;
    }

    public void Swing()
    {
        if (_state == WeaponState.Rest)
        {
            _state = WeaponState.Swing;
            _animator.SetTrigger("Swing");
            Invoke("ResetState", _animator.GetCurrentAnimatorStateInfo(0).length);

            _audioSource.clip = swingSound;
            _audioSource.Play();
        }

    }

    public void Throw(Vector3 target)
    {
        if (_state == WeaponState.Rest)
        {
            var position = transform.root.position;
            _parent.parent = null;
            _parent.position = position;
            _target = target;
            _state = WeaponState.ThrowAway;
            _animator.SetBool("Flying", true);
            _audioSource.clip = swingSound;
            _audioSource.Play();

        }
    }
    
    public void ResetState()
    {
        _state = WeaponState.Rest;
    }
}
