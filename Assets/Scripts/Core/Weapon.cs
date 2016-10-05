using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Interactable))]
public abstract class Weapon : MonoBehaviour {

    public int meleeDamage;
    public AudioClip swingSound;
    public AudioClip hitSound;

    protected Transform _parent;
    protected Vector3 _startingPoint;
    protected Transform _weaponSlot;
    protected Animator _animator;
    protected AudioSource _audioSource;
    public WeaponState _state = WeaponState.Rest;
    protected Vector3 _target;
    protected Interactable _interactable;

    public enum WeaponState
    {
        Rest,
        Idle,
        Charging,
        Swing,
        ThrowAway
        ,ThrowReturn
    };

    public virtual void Awake()
    {
        _parent = transform; // Use _parent to move the Weapon... the transform will be locked by the _animator
        _weaponSlot = transform.parent; // Return the _parent to this transform when catching after a throw
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _interactable = (Interactable)GetComponent(typeof(Interactable));

    }

    public virtual void Equipped(bool equipped)
    {
        if (equipped)
        {
            ResetState();
            _interactable.Enable(false);
        }
        else
        {
            _state = WeaponState.Rest;
            _interactable.Enable(true);

        }
    }

    public virtual WeaponState GetState()
    {
        return _state;
    }

    public void ResetState()
    {
        //print("Reset State");
        _state = WeaponState.Idle;

    }

    public virtual void Charge()
    {
        _state = WeaponState.Charging;
    }

    public virtual void Swing()
    {
        if (_state == WeaponState.Charging)
        {
            _state = WeaponState.Swing;

            _audioSource.clip = swingSound;
            _audioSource.Play();
        }

    }

    public virtual void Throw(Vector3 target)
    {
        _animator.SetBool("Flying", true);
        _target = target;

        _parent.parent = null;
        _parent.rotation = Quaternion.identity;
        _state = WeaponState.ThrowAway;
        _audioSource.clip = swingSound;
        _audioSource.Play();
    }
    
}
