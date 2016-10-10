using UnityEngine;
using System.Collections;

public class KnifeController : Weapon {
    
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            return;
        }

        var hitables = coll.GetComponents(typeof(IHitable));

        foreach (var component in hitables)
        {
            var hitable = (IHitable)component;
            print("Knife hit");
            hitable.Hit(meleeDamage);
            _state = WeaponState.Idle;
        }
    }

    public override void Throw(Vector3 target)
    {
        _state = WeaponState.Idle;
    }
}
