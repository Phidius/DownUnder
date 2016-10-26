using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class InventoryController : MonoBehaviour
{

    protected int currentAction = 0;
    protected InventorySlot[] slots;
    protected bool verticalInUse = false;
    protected bool horizontalInUse = false;

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

    protected void Awake()
    {

        slots = GetComponentsInChildren<InventorySlot>();

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

    protected void Start()
    {
        UpdateInventoryIcons();
    }

    protected void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Play)
        {
            return; // Only allow inventory manipulation when playing (would allow any menu action to modify inventory otherwise)
        }
        var menuNavigation = CrossPlatformInputManager.GetAxisRaw("MenuActivation");
        var menuActivation = CrossPlatformInputManager.GetAxisRaw("MenuNavigation");
        if (menuNavigation != 0f)
        {
            var direction = (menuNavigation > 0)?1:-1;
            if (horizontalInUse == false)
            {
                horizontalInUse = true;
                
                currentAction = currentAction + direction;
                if (currentAction > slots.Length - 1)
                {
                    currentAction = 0;
                }
                else if (currentAction < 0)
                {
                    currentAction = slots.Length - 1;
                }
                selector.transform.SetParent(slots[currentAction].gameObject.transform, false);
            }
        }
        if (menuNavigation == 0f)
        {
            horizontalInUse = false;
        }

        if (menuActivation != 0f)
        {
            if (verticalInUse == false)
            {
                verticalInUse = true;
                if (menuActivation < 0)
                {
                    ActionUsed();
                }
                else
                {
                    DropInventory(inventory[currentAction]);
                }               
            }
        }

        if (menuActivation == 0f)
        {
            verticalInUse = false;
        }
    }

    public bool AddInventory(GameObject item)
    {
        var result = false;
        for (var index = 0; index < inventory.Length; index++)
        {
            if (inventory[index] == null && result == false)
            {
                inventory[index] = item;
                inventory[index].GetComponent<Rigidbody>().isKinematic = true;// Prevent the object from moving around.
                inventory[index].GetComponent<Collider>().isTrigger = true;
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

        if (player._item != null && player._item.gameObject.GetInstanceID() == item.GetInstanceID())
        {
            // Can't remove an equipped item
            return result;
        }

        if (player._weapon != null && player._weapon.gameObject.GetInstanceID() == item.GetInstanceID())
        {
            // Can't remove an equipped weapon
            return result;
        }

        for (var index = 0; index < inventory.Length; index++)
        {
            if (inventory[index] != null && inventory[index].gameObject.GetInstanceID() == item.GetInstanceID())
            {
                inventory[index] = null;
                item.transform.parent = null;
                item.transform.position = player._itemSlot.position;
                item.transform.localRotation = Quaternion.identity;
                item.layer = LayerMask.NameToLayer("Interactable");
                item.GetComponent<Collider>().isTrigger = false;
                item.GetComponent<Rigidbody>().isKinematic = false;
                result = true;

                UpdateInventoryIcons();
            }
        }
        
        return result;
    }
    
    public void ActionUsed()
    {
        if (inventory[currentAction] == null)
        {
            return;
        }

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
