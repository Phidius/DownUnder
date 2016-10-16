using UnityEngine;
using System.Collections;

public class KnifeController : Weapon {
    
    private void OnTriggerEnter(Collider coll)
    {
        if (_state == WeaponState.Idle || _state == WeaponState.Charging)
        {
            return;
        }
        if (coll.tag == "Event" || coll.tag == "Player")
        {
            return;
        }

        var hitables = coll.GetComponents(typeof(IHitable));

        foreach (var component in hitables)
        {
            var hitable = (IHitable)component;
            hitable.Hit(meleeDamage);
            _state = WeaponState.Idle;
        }
    }

    public override void Throw(Vector3 target)
    {
        _state = WeaponState.Swing;
    }
}
