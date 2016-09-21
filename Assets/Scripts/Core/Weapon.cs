using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour {

    public int meleeDamage;
    public AudioClip swingSound;
    public AudioClip hitSound;

    protected Transform _parent;
    protected Vector3 _startingPoint;
    protected Transform _weaponSlot;
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected WeaponState _state = WeaponState.Rest;
    protected Vector3 _target;

    public enum WeaponState
    {
        Rest,
        Idle,
        Swing,
        ThrowAway,
        ThrowReturn
    };

    public virtual void Start()
    {
        _parent = transform.parent; // Use _parent to move the Weapon... the transform will be locked by the _animator
        _weaponSlot = _parent.parent; // Return the _parent to this transform when catching after a throw
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

    }

    public virtual void Equipped(bool equipped)
    {
        if (equipped)
        {
            _state = WeaponState.Idle;
        }
        else
        {
            _state = WeaponState.Rest;

        }
        //_animator.SetBool("Equipped", equipped);
        GetComponent<Animator>().SetBool("Equipped", equipped);
    }

    public virtual WeaponState GetState()
    {
        return _state;
    }
    public virtual void Swing()
    {
        if (_state == WeaponState.Idle)
        {
            _state = WeaponState.Swing;
            _animator.SetTrigger("Swing");
            Invoke("ResetState", _animator.GetCurrentAnimatorStateInfo(0).length);

            _audioSource.clip = swingSound;
            _audioSource.Play();
        }

    }

    public virtual void Throw(Vector3 target)
    {
        if (_state == WeaponState.Idle)
        {
            //var position = transform.root.position;
            _parent.parent = null;
            _parent.rotation = Quaternion.identity;
            //_parent.position = position;
            _target = target;
            _state = WeaponState.ThrowAway;
            _animator.SetBool("Flying", true);
            _audioSource.clip = swingSound;
            _audioSource.Play();

        }
    }
}
