using UnityEngine;
using System.Collections;

public class KnifeController : Weapon {
    
    private void OnTriggerEnter(Collider coll)
    {

        var hitables = coll.GetComponents(typeof(IHitable));

        foreach (var component in hitables)
        {
            var hitable = (IHitable)component;
            hitable.Hit(meleeDamage);
            _state = WeaponState.ThrowReturn;
        }
    }

}
