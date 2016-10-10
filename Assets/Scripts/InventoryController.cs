using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MenuController
{

    public Image selector;
    private int primaryItem;
    private int secondaryItem;

    public void EquipItem()
    {
        
    }

    public override void ActionChanged()
    {


        selector.transform.SetParent(actions[currentAction].gameObject.transform, false);
        print(actions[currentAction] + " selected");
    }

    public override void ActionUsed()
    {
        print(actions[currentAction] + " used");
    }
}
