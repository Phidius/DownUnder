using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour {

    public int meleeDamage;
    public AudioClip swingSound;
    public AudioClip hitSound;

    protected Transform _parent;
    protected Transform _weaponSlot;
    protected Animator _animator;
    protected AudioSource _audioSource;

    public enum WeaponState
    {
        Rest,
        Swing,
        ThrowAway,
        ThrowReturn
    };

    public virtual void Start()
    {
        _parent = transform.parent; // Use _parent to move the Boomerang... the transform will be locked by the _animator
        _weaponSlot = _parent.parent; // Return the _parent to this transform when catching after a throw
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
}
