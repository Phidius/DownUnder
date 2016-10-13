using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MenuController
{

    public Image selector;
    public GameObject[] inventory = new GameObject[6];
    public Sprite emptySprite;
    public PlayerController player;

    private int primaryItem;
    private int secondaryItem;
    private static InventoryController _inventoryController;
    private Transform _inventoryContainer;
    private InventorySlot[] _slots = new InventorySlot[6];
    public static InventoryController Instance { get { return _inventoryController; } }

    protected override void Awake()
    {
        base.Awake();
        if (_inventoryController != null && _inventoryController != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _inventoryController = this;
            _inventoryContainer = GameObject.Find("InventoryContainer").transform;

            _slots = GetComponentsInChildren<InventorySlot>();
            player = GameObject.FindObjectOfType<PlayerController>();

        }
    }

    protected override void Start()
    {
        base.Start();
        UpdateInventoryIcons();
    }
    public void EquipItem()
    {
        
    }

    public bool AddInventory(GameObject item)
    {
        var result = false;
        for (var index = 0; index < inventory.Length; index++)
        {
            if (inventory[index] == null)
            {
                inventory[index] = item;
                inventory[index].GetComponent<Rigidbody>().isKinematic = true;// Prevent the object from moving around.
                inventory[index].transform.SetParent(_inventoryContainer, false);
                inventory[index].transform.localPosition = Vector3.zero;
                inventory[index].transform.localRotation = Quaternion.identity;
                inventory[index].gameObject.layer = LayerMask.NameToLayer("Weapon");

                var weapon = (Weapon)inventory[index].GetComponent(typeof(Weapon));
                if (player._weapon == null && weapon != null)
                {
                    // Equip this weapon
                    weapon.transform.SetParent(player._weaponSlot, false);
                    weapon.Equipped(true);
                    player._weapon = weapon;
                }
                if (player._item == null && weapon == null) // If this is not a weapon, it is an item
                {
                    // Player isn't holding an item - equip this one
                    item.transform.SetParent(player._itemSlot, false);
                    player._item = (Usable)item.GetComponent(typeof(Usable));
                }

                UpdateInventoryIcons();
                result = true;
                break;
            }
        }
        return result;
    }

    public bool RemoveInventory(GameObject item)
    {
        var result = false;
        for (var index = 0; index < inventory.Length; index++)
        {
            if (inventory[index] != null && inventory[index].gameObject.GetInstanceID() == item.GetInstanceID())
            {
                result = true;
                Destroy(inventory[index]);
                inventory[index] = null;
                player._item = null;

                UpdateInventoryIcons();
            }
        }
        
        return result;
    }

    public bool DropInventory(GameObject item)
    {
        var result = false;
        // TODO: turn off Kinematic so that the object behaves realistically
        return result;
    }

    public override void ActionChanged()
    {
        selector.transform.SetParent(actions[currentAction].gameObject.transform, false);
    }

    public override void ActionUsed()
    {
        var weapon = (Weapon)inventory[currentAction].GetComponent(typeof(Weapon));
        var item = (Usable)inventory[currentAction].GetComponent(typeof(Usable));
        
        if (weapon == null)
        {
            // This is a usable item
            //Move the currently equipped item into the InventoryContainer
            if (player._item != null)
            {
                player._item.transform.SetParent(_inventoryContainer, false);
                player._item.transform.localPosition = Vector3.zero;
                player._item.transform.localRotation = Quaternion.identity;
            }
            item.transform.SetParent(player._itemSlot, false);
            player._item = item;
        }
        else
        {
            //Move the currently equipped weapon into the InventoryContainer
            if (player._weapon != null)
            {
                //tempGameObject = player._weapon.gameObject;
                player._weapon.transform.SetParent(_inventoryContainer, false);
                player._weapon.transform.localPosition = Vector3.zero;
                player._weapon.transform.localRotation = Quaternion.identity;
            }
            
            // Place the selected inventory weapon in the player's hand
            weapon.transform.SetParent(player._weaponSlot, false);
            weapon.Equipped(true);
            player._weapon = weapon;
        }

        UpdateInventoryIcons();
    }

    public void UpdateInventoryIcons()
    {
        for (var index = 0; index < _slots.Length; index++)
        {
            if (inventory[index] == null)
            {
                _slots[index].SetBackground(Color.grey, false);
                _slots[index].SetIcon(emptySprite);
            }
            else
            {
                var weapon = (Weapon)inventory[index].GetComponent(typeof(Weapon));
                var usable = (Usable)inventory[index].GetComponent(typeof(Usable));

                if (weapon != null)
                {
                    if (player._weapon != null && player._weapon.gameObject.GetInstanceID() == inventory[index].GetInstanceID())
                    {
                        _slots[index].SetBackground(Color.red, true);
                    }
                    else
                    {
                        _slots[index].SetBackground(Color.red, false);
                    }
                }

                if (weapon == null)
                {
                    //print(inventory[index].GetInstanceID());
                    //if (player._item != null) {  print("Player is holding " + player._item.gameObject.GetInstanceID());}
                    if (player._item != null && player._item.gameObject.GetInstanceID() == inventory[index].GetInstanceID())
                    {
                        _slots[index].SetBackground(Color.blue, true);
                    }
                    else
                    {
                        _slots[index].SetBackground(Color.blue, false);
                    }
                }
                _slots[index].SetIcon(usable.icon);
            }
        }
    }
}
