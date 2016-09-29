using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Interactable : MonoBehaviour {

    /*
     * This class provides the base behavior of all IInteractables.
     * Classes that inherit from it are free to modify any and all of it's behavior - specifically, the "Interact" method - but don't need to.
     * For example, 
     */
    protected bool _highlighted = false;

    private Renderer _renderer;
    private Material[] _materials;

    private bool _enable = true;
    
    public virtual void Start()
    {
        // Use the current object to find the first renderer in the children
        _renderer = GetComponentInChildren<Renderer>();
        _materials = _renderer.materials;
        foreach (var mat in _materials)
        {
            // Enable the ability to set the "_EmissionColor"
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

    }

    public virtual void Enable(bool enable)
    {
        _enable = enable;
    }
    public virtual bool IsEnabled()
    {
        return _enable;
    }

    public virtual void Highlight(PlayerController player, bool show)
    {
        if (IsEnabled() && show && Vector3.Distance(player.transform.position, transform.position) < 5f)
        {
            _highlighted = true;

            for (var index = 0; index < _materials.Length; index++)
            {
                _materials[index].SetColor("_EmissionColor", Color.green);
            }
        }
        else
        {
            _highlighted = false;
            for (var index = 0; index < _materials.Length; index++)
            {
                _materials[index].SetColor("_EmissionColor", Color.black);
            }

        }
    }

    public virtual bool IsHighlighted()
    {
        return _highlighted;
    }

    public virtual void Interact(PlayerController player)
    {
        if (_highlighted == false || _enable == false)
        {
            return;
        }
    }

}
