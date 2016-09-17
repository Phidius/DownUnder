using UnityEngine;
using System.Collections;

public class TreasureChest : MonoBehaviour
{
    private Animator _animator;
	// Use this for initialization
	void Start ()
	{
	    _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InteractionComplete()
    {
        Destroy(_animator);
    }
}
