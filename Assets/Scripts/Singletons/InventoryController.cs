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

    public bool AddInventory(GameObject gameObject)
    {
        var result = false;
        for (var index = 0; index < inventory.Length; index++)
        {
            if (inventory[index] == null)
            {
                inventory[index] = gameObject;
                gameObject.transform.SetParent(_inventoryContainer, false);
                gameObject.transform.localPosition = Vector3.zero;
                UpdateInventoryIcons();
                result = true;
                break;
            }
        }
        return false;
    }

    public override void ActionChanged()
    {
        selector.transform.SetParent(actions[currentAction].gameObject.transform, false);
    }

    public override void ActionUsed()
    {
        var weapon = (Weapon)inventory[currentAction].GetComponent(typeof(Weapon));

        if (weapon != null)
        {
            // Store the equipped weapon in a temporary variable
            var tempGameObject = player._weapon.gameObject;

            tempGameObject.transform.SetParent(_inventoryContainer, false);
            tempGameObject.transform.localPosition = Vector3.zero;

            // Place the selected inventory weapon in the player's hand
            weapon.transform.SetParent(player._weaponSlot, false);
            weapon.Equipped(true);
            player._weapon = weapon;
            UpdateInventoryIcons();
        }
    }

    public void UpdateInventoryIcons()
    {
        for (var index = 0; index < _slots.Length; index++)
        {
            if (inventory[index] == null)
            {
                _slots[index].SetBackground(Color.grey);
                _slots[index].SetIcon(emptySprite);
            }
            else
            {
                var usable = (Usable)inventory[index].GetComponent(typeof(Usable));
                print(player.gameObject.name);
                print(player._weapon);

                if (player._weapon != null && player._weapon.GetInstanceID() == inventory[index].GetInstanceID())
                {
                    print(player._weapon.GetInstanceID() + " and " + inventory[index].GetInstanceID());

                    _slots[index].SetBackground(Color.red);
                }
                else
                {
                    print("null and " + inventory[index].GetInstanceID());

                    _slots[index].SetBackground(Color.grey);
                }
                
                _slots[index].SetIcon(usable.icon);
            }

        }
    }
}
