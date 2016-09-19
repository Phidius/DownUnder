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
    //private Material _outlinedMaterial;
    private Material[] _materials;
    public List<Material> _highlightedMaterials;

    private bool _enable = true;
    
    public virtual void Start()
    {
        var _outlinedMaterial = (Material)Resources.Load("Outlined_Material", typeof(Material));
        // Use the current object to find the MeshRenderer by default
        _renderer = GetComponentInChildren<Renderer>();
        _materials = _renderer.materials;

        _highlightedMaterials = _materials.ToList<Material>();
        _highlightedMaterials.Add(_outlinedMaterial);
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
            _renderer.materials = _highlightedMaterials.ToArray<Material>();
        }
        else
        {
            _highlighted = false;
            if (_renderer != null)
            {
                _renderer.materials = _materials;
            }
        }
    }

    public virtual void Interact(PlayerController player)
    {
        if (_highlighted == false)
        {
            return;
        }
    }

}
