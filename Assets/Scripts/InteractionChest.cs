using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InteractionChest : Interactable
{
    public GameObject[] contents;
    public GameObject container;
    private bool _isOpened = false;
    private Animator _animator;

    public override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        //container = transform.parent.Find("Bottom/Container").gameObject;
    }

    public override void Interact(PlayerController player)
    {
        base.Interact(player);
        if (!_highlighted || _isOpened)
        {
            return;
        }

        foreach(var content in contents)
        {
            //Instantiate(content, transform.position + new Vector3(0f, .2f, 0f), Quaternion.identity);
            // Random position within this transform
            var obj = (GameObject) Instantiate(content, Vector3.zero, Quaternion.identity);//Quaternion.Euler(270f, 0f, 0f));
            if (obj.GetComponentInChildren<Weapon>())
            {
                foreach (var weapon in obj.GetComponentsInChildren<Weapon>())
                {
                    // We need to change the layer to "Interactable" as the layer "Weapon" is invisible to the Reticle (to avoid the weapon
                    // flashing when it passes between the camera and the reticle
                    weapon.gameObject.layer = LayerMask.NameToLayer("Interactable");
                }
            }
            //var collider = obj.GetComponentInChildren<Collider>();
            //if (collider != null)
            //{
            //    print(obj.name + " collider size: " + collider.bounds.size);
            //}
            var rndPosWithin = new Vector3(Random.Range(-.19f, .19f), 0.1f, Random.Range(-.65f, .65f));

            obj.transform.SetParent(container.transform, false);
            obj.transform.localPosition = rndPosWithin;
        }
        _isOpened = true;
        _animator.SetTrigger("Open");
        Enable(false);

    }

}
