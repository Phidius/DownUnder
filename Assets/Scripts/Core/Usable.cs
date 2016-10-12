using UnityEngine;
using UnityEngine.UI;

public class Usable : MonoBehaviour {
    
    public Sprite icon;

    protected PlayerController player;

    protected virtual void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
    }

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
