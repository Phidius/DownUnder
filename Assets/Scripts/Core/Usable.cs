using UnityEngine;
using UnityEngine.UI;

public class Usable : MonoBehaviour {

    public enum UsableType { Item, Weapon }

    public Image icon;

    public virtual void Equip()
    {
        
    }

    public virtual void Use()
    {
        
    }

    public virtual void Drop()
    {
        
    }
}
