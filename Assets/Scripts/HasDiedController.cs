using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class HasDiedController : MenuController {

    private Text _textExit;
    private string _confirmExit = "Press 'Fire' to confirm, or scroll to another choice";
    private string _initialExit;

    protected override void Start()
    {
        base.Start();
        foreach (var component in GetComponentsInChildren<Text>().Where(component => component.name == "Exit"))
        {
            _textExit = component;
            _initialExit = _textExit.text;
        }

        if (_textExit == null)
        {
            throw new UnassignedReferenceException("Exit text object not found in " + name);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && _textExit.text == _confirmExit)
        {
            GameManager.Instance.ExitGame();
        }
    }

    public override void ActionChanged()
    {
        _textExit.text = _initialExit;
    }

    public override void ActionUsed()
    {
        // May not be needed
    }
    public void ExitGame()
    {
        _textExit.text = _confirmExit;
    }
}
