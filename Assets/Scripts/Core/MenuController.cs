using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public abstract class MenuController : MonoBehaviour {

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
        if (CrossPlatformInputManager.GetAxisRaw("MenuNavigation") != 0f)
        {
            var direction = (int) CrossPlatformInputManager.GetAxisRaw("MenuNavigation");
            if (verticalInUse == false)
            {
                verticalInUse = true;

                actions[currentAction].enabled = false;
                actions[currentAction].enabled = true;
                currentAction = currentAction + direction;
                if (currentAction > actions.Length - 1)
                {
                    currentAction = 0;
                }
                else if (currentAction < 0)
                {
                    currentAction = actions.Length - 1;
                }
                actions[currentAction].Select();
                print("MenuController - ActionChanged");
                ActionChanged();
            }
        }
        if (CrossPlatformInputManager.GetAxisRaw("MenuNavigation") == 0f)
        {
            verticalInUse = false;
        }
        if (CrossPlatformInputManager.GetAxisRaw("MenuActivation") != 0f)
        {
            if (horizontalInUse == false)
            {
                horizontalInUse = true;
                UseAction();
            }
        }
        if (CrossPlatformInputManager.GetAxisRaw("MenuActivation") == 0f)
        {
            horizontalInUse = false;
        }
    }

    public abstract void ActionChanged();

    public abstract void ActionUsed();

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
