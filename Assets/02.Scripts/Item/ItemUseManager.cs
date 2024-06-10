using Photon.Pun;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public static ItemUseManager Instance;
    public UseComputerTrigger computerTrigger;
    public UI_HintLog hintLog;
    public QuickSlotManager quickSlotManager;
    public InventoryUI inventoryUI;
    public UI_Gunfire uI_Gunfire;
    public CharacterAttackAbility attackAbility;
    public CharacterGunFireAbility gunFireAbility;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        computerTrigger = FindObjectOfType<UseComputerTrigger>();
        attackAbility = Character.LocalPlayerInstance._attackability; 
        gunFireAbility = Character.LocalPlayerInstance._gunfireAbility;
    }

    public void ApplyEffect(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Food:
                ApplyFoodEffect(item);
                break;
            case ItemType.Heal:
                ApplyHealEffect(item.itemName);
                DecreaseItemQuantity(item);

                break;
            case ItemType.Mental:
                ApplyMentalEffect(item.itemName);
                DecreaseItemQuantity(item);

                break;
            case ItemType.Weapon:
                ApplyWeaponEffect(item.itemName, item);
                break;
            case ItemType.ETC:
                ApplyEtcEffect(item.itemName, item);
                break;
            default:
                Debug.LogWarning("Unknown item type.");
                break;
        }

        UpdateUI();
    }
    public void UseConsumable(Item item)
    {
        if(item.itemType == ItemType.Consumable)
        {
            DecreaseItemQuantity(item);
        }
        UpdateUI();

    }

    public void EquipItem(Item item)
    {
        attackAbility.DeactivateAllWeapons();
        gunFireAbility.DeactivateAllGuns();

        switch (item.itemType)
        {
            case ItemType.Food:
                EquipFood(item.itemName);
                break;
            case ItemType.Heal:
                EquipHeal(item.itemName);
                break;
            case ItemType.Mental:
                EquipMental(item.itemName);
                break;
            case ItemType.Weapon:
                EquipWeapon(item.itemName);
                break;
            case ItemType.ETC:
                EquipEtc(item.itemName);
                break;
            case ItemType.Consumable:
                EquipConsumable(item.itemName);
                break;
            case ItemType.Gun:
                EquipGun(item.itemName);
                break;
            default:
                Debug.LogWarning("This item type cannot be equipped.");
                break;
        }
    }

    private void EquipFood(string itemName)
    {
        switch (itemName)
        {
            case "고기":
                Debug.Log("고기를 들었음");
                break;
            case "빵":
                Debug.Log("빵을들었음");
                break;
            default:
                Debug.LogWarning("Unknown food item.");
                break;
        }
    }

    private void EquipHeal(string itemName)
    {
        switch (itemName)
        {
            case "구급상자":
                Debug.Log("구상을 들었음");
                break;
            case "진통제":
                Debug.Log("진통제를 들었음");
                break;
            case "붕대":
                Debug.Log("붕대를 들었음");
                break;
            default:
                Debug.LogWarning("Unknown heal item.");
                break;
        }
    }

    private void EquipMental(string itemName)
    {
        switch (itemName)
        {
            case "술":
                Debug.Log("술을 들었음");
                break;
            case "책":
                Debug.Log("책을 들었음");
                break;

            default:
                Debug.LogWarning("Unknown mental item.");
                break;
        }
    }

    private void EquipWeapon(string itemName)
    {


        switch (itemName)
            {
                case "도끼":
                    Debug.Log("플레이어가 도끼를 들었음");
                    attackAbility.WeaponActive(0);
                    
                    break;
                case "배트":
                    Debug.Log("플레이어가 야구배트를 들었음");
                    attackAbility.WeaponActive(1);
                break;
                case "삽":
                    Debug.Log("플레이어가 삽을 들었음");
                    attackAbility.WeaponActive(2);
                    
                break;


            default:
                    Debug.LogWarning("Unknown weapon item.");
                    break;
            }
        
           
    }
    private void EquipGun(string itemName)
    {


        switch (itemName)
        {
            case "총_라이플":
                Debug.Log("플레이어가 총을 들었음");
                gunFireAbility.GunActive(0);
                break;
          

            default:
                Debug.LogWarning("Unknown Gun item.");
                break;
        }


    }

    private void EquipEtc(string itemName)
    {
        switch (itemName)
        {
            case "지도":
                Debug.Log("Player found a map.");

                break;
            case "열쇠":
                Debug.Log("Player found a key.");

                break;
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }
    private void EquipConsumable(string itemName)
    {
        switch (itemName)
        {
            case "총알":
                Debug.Log("총알을 들었음");
                break;
            

            default:
                Debug.LogWarning("Unknown Consumable item.");
                break;
        }
    }
    private void ApplyFoodEffect(Item item)
    {
        switch (item.itemName)
        {
            case "고기":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 30; 
                break;
            case "빵":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 20;
                break;
            default:
                break;
        }
    }

    private void ApplyHealEffect(string itemName)
    {
        switch (itemName)
        {
            case "구급상자":
                Debug.Log("Player health increased by 50.");
                break;
            case "진통제":
                Debug.Log("Player health increased by 20.");
                break;
            case "붕대":
                Debug.Log("플레이어 체력 회복 10");
                break;
            default:
                Debug.LogWarning("Unknown heal item.");
                break;
        }
    }

    private void ApplyMentalEffect(string itemName)
    {
        switch (itemName)
        {
            case "술":
                Debug.Log("플레이어의 정신력이 올라갔다");
                // PlayerStatus.Instance.IncreaseMentalState(20);
                break;
            case "책":
                Debug.Log("Player happiness increased.");
                // PlayerStatus.Instance.IncreaseHappiness(15);
                break;
            default:
                Debug.LogWarning("Unknown mental item.");
                break;
        }
    }

    private void ApplyWeaponEffect(string itemName, Item item)
    {
        switch (itemName)
        {
            case "도끼":
                Debug.Log("플레이어가 도끼를 사용함");
                // Player.Instance.UseWeapon(axe);
                break;
            case "배트":
                Debug.Log("플레이어가 야구배트를 사용함");
                // Player.Instance.UseWeapon(bat);
                break;
            case "삽":
                Debug.Log("플레이어가 삽을 사용함");
                // Player.Instance.UseWeapon(shovel);
                break;

            default:
                Debug.LogWarning("Unknown weapon item.");
                break;
        }
    }

    private void ApplyEtcEffect(string itemName, Item item)
    {
        switch (itemName)
        {
            case "지도":
                Debug.Log("Player used a map.");
                // Player.Instance.UseMap();
                break;

            case "열쇠":
                Debug.Log("Player used a key.");
                var policeTrigger = FindObjectOfType<PoliceTrigger>();
                if (policeTrigger != null)
                {
                    policeTrigger.UseKeyToOpenDoor();
                }
                DecreaseItemQuantity(item);
                break;

            case "책":
                Debug.Log("Player used a Book");
                if (hintLog != null)
                {
                    hintLog.UpdateHintText("소의 뿔이 사라지는 시간에 중앙에서 20초간 모습을 드러낸다.");
                }
                break;

            case "디스크1":
                if (hintLog != null && computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk1");
                    hintLog.UpdateHintText("가장 깊은 곳에 마지막 생명이 숨쉬고 있다.");
                    DecreaseItemQuantity(item);
                }
                break;

            case "디스크2":
                if (hintLog != null && computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk2");
                    hintLog.UpdateHintText("마지막 생명과 함께 최후의 섬으로 가라.");
                    DecreaseItemQuantity(item);
                }
                break;

            case "디스크3":
                if (hintLog != null && computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk3");
                    hintLog.UpdateHintText("<The Last Yggdrasil>");
                    DecreaseItemQuantity(item);
                }
                break;

            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }

    private void DecreaseItemQuantity(Item item)
    {
        Inventory inventory = Character.LocalPlayerInstance.gameObject.GetComponent<Inventory>();
        if (inventory != null && inventory.pv.IsMine)
        {
            string itemName = item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC ? item.uniqueId : item.itemName;

            if (inventory.itemQuantities.ContainsKey(itemName))
            {
                inventory.itemQuantities[itemName]--;
                Debug.LogError("아이템 갯수감소");
                if (inventory.itemQuantities[itemName] <= 0)
                {
                    inventory.items.Remove(itemName);
                    inventory.itemQuantities.Remove(itemName);
                    quickSlotManager.currentEquippedItem = null;
                    quickSlotManager.RemoveItemFromQuickSlots(item);

                    inventoryUI.CloseItemInfo();

                }


            }
        }
    }


    public void UseItem(Item item, float duration)
    {
        StartCoroutine(uI_Gunfire.UseItemWithTimer(duration, () => ApplyEffect(item)));
    }
    public void UpdateUI()
    {

        inventoryUI.UpdateInventoryUI();
        


        quickSlotManager.UpdateQuickSlotUI();
        
    }
}
