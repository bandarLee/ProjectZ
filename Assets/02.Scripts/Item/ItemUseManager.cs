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
            case "���":
                Debug.Log("��⸦ �����");
                break;
            case "��":
                Debug.Log("���������");
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
            case "���޻���":
                Debug.Log("������ �����");
                break;
            case "������":
                Debug.Log("�������� �����");
                break;
            case "�ش�":
                Debug.Log("�ش븦 �����");
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
            case "��":
                Debug.Log("���� �����");
                break;
            case "å":
                Debug.Log("å�� �����");
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
                case "����":
                    Debug.Log("�÷��̾ ������ �����");
                    attackAbility.WeaponActive(0);
                    
                    break;
                case "��Ʈ":
                    Debug.Log("�÷��̾ �߱���Ʈ�� �����");
                    attackAbility.WeaponActive(1);
                break;
                case "��":
                    Debug.Log("�÷��̾ ���� �����");
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
            case "��_������":
                Debug.Log("�÷��̾ ���� �����");
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
            case "����":
                Debug.Log("Player found a map.");

                break;
            case "����":
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
            case "�Ѿ�":
                Debug.Log("�Ѿ��� �����");
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
            case "���":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 30; 
                break;
            case "��":
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
            case "���޻���":
                Debug.Log("Player health increased by 50.");
                break;
            case "������":
                Debug.Log("Player health increased by 20.");
                break;
            case "�ش�":
                Debug.Log("�÷��̾� ü�� ȸ�� 10");
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
            case "��":
                Debug.Log("�÷��̾��� ���ŷ��� �ö󰬴�");
                // PlayerStatus.Instance.IncreaseMentalState(20);
                break;
            case "å":
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
            case "����":
                Debug.Log("�÷��̾ ������ �����");
                // Player.Instance.UseWeapon(axe);
                break;
            case "��Ʈ":
                Debug.Log("�÷��̾ �߱���Ʈ�� �����");
                // Player.Instance.UseWeapon(bat);
                break;
            case "��":
                Debug.Log("�÷��̾ ���� �����");
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
            case "����":
                Debug.Log("Player used a map.");
                // Player.Instance.UseMap();
                break;

            case "����":
                Debug.Log("Player used a key.");
                var policeTrigger = FindObjectOfType<PoliceTrigger>();
                if (policeTrigger != null)
                {
                    policeTrigger.UseKeyToOpenDoor();
                }
                DecreaseItemQuantity(item);
                break;

            case "å":
                Debug.Log("Player used a Book");
                if (hintLog != null)
                {
                    hintLog.UpdateHintText("���� ���� ������� �ð��� �߾ӿ��� 20�ʰ� ����� �巯����.");
                }
                break;

            case "��ũ1":
                if (hintLog != null && computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk1");
                    hintLog.UpdateHintText("���� ���� ���� ������ ������ ������ �ִ�.");
                    DecreaseItemQuantity(item);
                }
                break;

            case "��ũ2":
                if (hintLog != null && computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk2");
                    hintLog.UpdateHintText("������ ����� �Բ� ������ ������ ����.");
                    DecreaseItemQuantity(item);
                }
                break;

            case "��ũ3":
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
                Debug.LogError("������ ��������");
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
