using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class MenuController : MonoBehaviour {

    protected int currentAction = 0;
    protected Button[] actions;
    protected bool verticalInUse = false;
    protected bool horizontalInUse = false;

    // Use this for initialization
    protected virtual void Start () {
        actions = GetComponentsInChildren<Button>();
        actions[currentAction].Select();
    }

    protected virtual void Update()
    {
        if (CrossPlatformInputManager.GetAxisRaw("Vertical") != 0)
        {
            if (verticalInUse == false)
            {
                verticalInUse = true;
                //EventSystem.current.SetSelectedGameObject(null);
                actions[currentAction].enabled = false;
                actions[currentAction].enabled = true;
                currentAction = currentAction + 1;
                if (currentAction > actions.Length - 1)
                {
                    currentAction = 0;
                }
                actions[currentAction].Select();
            }
        }
        if (CrossPlatformInputManager.GetAxisRaw("Vertical") == 0)
        {
            verticalInUse = false;
        }
        if (CrossPlatformInputManager.GetAxisRaw("Horizontal") != 0)
        {
            if (horizontalInUse == false)
            {
                horizontalInUse = true;
                UseAction();
            }
        }
        if (CrossPlatformInputManager.GetAxisRaw("Horizontal") == 0)
        {
            horizontalInUse = false;
        }
    }
    public virtual void SetActive(bool active)
    {
        SetActive(active);
    }

    public virtual bool GetActive()
    {
        return GetActive();
    }

    protected virtual void UseAction()
    {
        if (actions.Length > currentAction)
        {
            actions[currentAction].onClick.Invoke();
        }

    }
    
}
