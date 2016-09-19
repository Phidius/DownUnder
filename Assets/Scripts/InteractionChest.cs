﻿using UnityEngine;

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
        // Don't remove the gameobject by calling base.Interact(player);
        if (_isOpened)
        {
            return;
        }
        foreach(var content in contents)
        {
            //Instantiate(content, transform.position + new Vector3(0f, .2f, 0f), Quaternion.identity);
            // Random position within this transform
            Vector3 rndPosWithin;

            
            rndPosWithin = new Vector3(Random.Range(-.38f, .38f), 0.1f, Random.Range(-1.3f, 1.3f));
            //rndPosWithin = container.transform.TransformPoint(rndPosWithin * .5f);

            var obj = (GameObject)Instantiate(content, Vector3.zero, Quaternion.Euler(270f, 0f, 0f));
            obj.transform.parent = container.transform;
            obj.transform.localPosition = rndPosWithin;
        }
        _isOpened = true;
        _animator.SetTrigger("Open");

    }

}
