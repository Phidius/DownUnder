using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Usable))]
public abstract class Weapon : MonoBehaviour {

    public int meleeDamage;
    public AudioClip swingSound;
    public AudioClip hitSound;
    public float maxThrowDistance = 50f;
    public float throwSpeed;

    protected float _throwDistance = 0f;
    protected Image _throwImage;
    protected float _throwWindupSpeed = 40f; // This is currently tied to the Windup animation duration

    protected Transform _parent;
    protected Vector3 _startingPoint;
    protected Transform _weaponSlot;
    protected Animator _animator;
    protected AudioSource _audioSource;
    public WeaponState _state = WeaponState.Idle;
    protected Vector3 _target;
    protected Interactable _interactable;

    public enum WeaponState
    {
        Idle,
        Charging,
        Swing,
        ThrowAway
        ,ThrowReturn
    };

    public virtual void Awake()
    {
        _parent = transform; // Use _parent to move the Weapon... the transform will be locked by the _animator
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _interactable = (Interactable)GetComponent(typeof(Interactable));
    }

    public virtual void Update()
    { 
        // Very odd... the Camera.main doesn't seem to contain the "Throw" image either during the Wake or Start method
        // of this class.  So, we'll set it on the first Update if it's null.
        if (_throwImage == null)
        {
            foreach (var component in Camera.main.GetComponentsInChildren<Image>())
            {
                if (component.name == "Throw")
                {
                    _throwImage = component;
                    break;
                }
            }
            return;
        }
        var throwScale = _throwImage.transform.localScale;
        throwScale.x = 1f;
        if (maxThrowDistance <= 0f)
        {
            throwScale.y = 0;

            _throwImage.transform.localScale = throwScale;
            return; // The remaining code deals with scaling the _throwImage based on the distance it will be thrown.
        }

        if (GetState() == Weapon.WeaponState.Charging)
        {
            _throwDistance += (_throwWindupSpeed * Time.deltaTime);
            _throwDistance = Mathf.Clamp(_throwDistance, 0f, 50f);

            if (_throwDistance > maxThrowDistance * .2f)
            {
                _throwImage.color = Color.green;
            }
            else
            {
                _throwImage.color = Color.grey;
            }
        }

        throwScale.y = _throwDistance / maxThrowDistance;

        _throwImage.transform.localScale = throwScale;


    }
    public virtual void Equipped(bool equipped)
    {
        _state = WeaponState.Idle;
        if (equipped)
        {
            _interactable.Enable(false);
        }
        else
        {
            _interactable.Enable(true);
        }
        _weaponSlot = transform.parent; // Return the _parent to this transform when catching after a throw
    }

    public virtual WeaponState GetState()
    {
        return _state;
    }

    public void PlaceInHand()
    {
        transform.parent = _weaponSlot;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
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
        if (_throwDistance > maxThrowDistance * .2f && maxThrowDistance > 0f) // TODO: base this on the player's collider, perhaps?
        {
            // Throw the weapon
            var targetDistance = Vector3.Distance(transform.position, target);
            var fractionThrow = (_throwDistance < targetDistance) ? _throwDistance / targetDistance : 1f;
            _target = Vector3.Lerp(transform.position, target, fractionThrow);
            _animator.SetBool("Flying", true);

            _parent.parent = null;
            _parent.rotation = Quaternion.identity;
            _state = WeaponState.ThrowAway;
            _audioSource.clip = swingSound;
            _audioSource.Play();
        }
        else
        {
            // Don't throw the weapon - just reset it to Idle.
            _state = WeaponState.Idle;
        }

        _throwDistance = 0f;
    }
    
}
