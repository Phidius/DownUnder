using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MenuController
{

    public Image selector;
    public Usable[] inventory = new Usable[12];
    private int primaryItem;
    private int secondaryItem;
    private static InventoryController _inventoryController;

    public static InventoryController Instance { get { return _inventoryController; } }

    private void Awake()
    {
        if (_inventoryController != null && _inventoryController != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _inventoryController = this;
        }
    }

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
