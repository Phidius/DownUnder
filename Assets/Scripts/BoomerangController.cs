﻿using UnityEngine;

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
                if (Vector3.Distance(transform.position, _weaponSlot.position) < .01f)
                {
                    print("Return to player's hand");
                    // Return to player's "hand"
                    ResetState();
                    _parent.parent = _weaponSlot;
                    _parent.localPosition = Vector3.zero; // _parent.position is the global position
                    _parent.localRotation = Quaternion.identity;
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
    }
}
