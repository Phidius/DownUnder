using UnityEngine;

namespace Assets.Scripts
{
    public class BoomerangController : Weapon
    {

        public float throwSpeed;
        
        // Update is called once per frame
        private void Update ()
        {
            var step = throwSpeed * Time.deltaTime;
            if (_state == WeaponState.ThrowAway)
            {
            
                transform.position = Vector3.MoveTowards(transform.position, _target, step);
                if (Vector3.Distance(transform.position, _target) < .01f)
                {
                    // Turn around
                    //print("Weapon close to target at (" + _target + ")");
                    _state = WeaponState.ThrowReturn;
                }
            }
            else if (_state == WeaponState.ThrowReturn)
            {
                transform.position = Vector3.MoveTowards(transform.position, _weaponSlot.position, step);
                if (Vector3.Distance(transform.position, _weaponSlot.position) < .01f)
                {
                    // Return to player's "hand"
                    //print("Return to players hand");
                    ResetState();
                    transform.parent = _weaponSlot;
                    transform.localPosition = Vector3.zero; // _parent.position is the global position
                    transform.localRotation = Quaternion.identity;
                    //_parent.localScale = Vector3.one;
                
                    _animator.SetBool("Flying", false);
                    _weaponSlot.root.gameObject.GetComponent<PlayerController>().HasCaught();

                }
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            
            if (_state == WeaponState.Rest || _state == WeaponState.Idle || _state == WeaponState.Charging)
            {
                return;
            }
            if (coll.tag == "Event" || coll.tag == "Player")
            {
                return;
            }
            
            var hitables = coll.GetComponents(typeof(IHitable));
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

        public override void Throw(Vector3 target)
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
}
