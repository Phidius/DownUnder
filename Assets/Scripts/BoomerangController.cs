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
    private Transform _parent;
    private Transform _weaponSlot;
    
	// Use this for initialization
	void Start ()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _parent = transform.parent; // Use _parent to move the Boomerang... the transform will be locked by the _animator
	    _weaponSlot = _parent.parent; // Return the _parent to this transform when catching after a throw
    }
	
	// Update is called once per frame
	void Update () {
        var step = throwSpeed * Time.deltaTime;
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
            _parent.position = Vector3.MoveTowards(_parent.position, _weaponSlot.position, step);
            if (Vector3.Distance(_parent.position, _weaponSlot.position) < .01f)
            {
                // Return to player's "hand"
                _state = BoomerangState.Rest;
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
        if (collider.tag == "Event" || _state == BoomerangState.Rest)
        {
            return;
        }
        var hitables = collider.GetComponents(typeof(IHitable));

        if (hitables == null || hitables.Length == 0)
        {
            if (_state == BoomerangState.ThrowAway)
            {
                // Hitting anything without a hitable component simply causes the projectile to return
                _state = BoomerangState.ThrowReturn;
            }
            return;
        }

        foreach (var component in hitables)
        {
            var hitable = (IHitable) component;
            hitable.Hit(meleeDamage);
            _state = BoomerangState.ThrowReturn;
        }

        //var spiderController = collider.GetComponent<SpiderController>();

        //if (spiderController)
        //{
        //    spiderController.ApplyDamage(meleeDamage);
        //    _state = BoomerangState.ThrowReturn;
        //    _audioSource.clip = hitSound;
        //    _audioSource.Play();
        //    return;
        //}

        //var trapdoorController = collider.GetComponent<TrapdoorController>();
        //if (trapdoorController)
        //{
        //    trapdoorController.ApplyDamage(meleeDamage);
        //    _state = BoomerangState.ThrowReturn;
        //    _audioSource.clip = hitSound;
        //    _audioSource.Play();
        //    return;
        //}

    }
    public BoomerangState GetState()
    {
        return _state;
    }

    public void Swing()
    {
        if (_state == BoomerangState.Rest)
        {
            _state = BoomerangState.Swing;
            _animator.SetTrigger("Swing");
            Invoke("ResetState", _animator.GetCurrentAnimatorStateInfo(0).length);

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
            _parent.parent = null;
            _parent.position = position;
            _target = target;
            _state = BoomerangState.ThrowAway;
            _animator.SetBool("Flying", true);
            _audioSource.clip = swingSound;
            _audioSource.Play();

        }
    }
}
