using UnityEngine;
using System.Collections.Generic;

public class InteractionSpider : Interactable
{

    public List<GameObject> contents;

    public override void Start()
    {
        base.Start();
        base.Enable(false);
    }
    public override void Interact(PlayerController player)
    {
        if (!_highlighted)
        {
            return;
        }
        if (contents.Count < 1)
        {
            return;
        }
        // Determine what (if anything) will be dropped
        var index = Random.Range(0, contents.Count);
        if (contents[index] != null)
        {
            Instantiate(contents[index], transform.position + new Vector3(0f, .2f, 0f), Quaternion.identity);
        }
        contents.Clear(); // Can only get the loot once
        Enable(false);

    }
}
