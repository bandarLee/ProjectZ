using Photon.Pun;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public static ItemUseManager Instance;



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

    public void ApplyEffect(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Food:
                ApplyFoodEffect(item.itemName);
                break;
            case ItemType.Heal:
                ApplyHealEffect(item.itemName);
                break;
            case ItemType.Mental:
                ApplyMentalEffect(item.itemName);
                break;
            case ItemType.Weapon:
                ApplyWeaponEffect(item.itemName);
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

    public void EquipItem(Item item)
    {
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
        
            CharacterAttackAbility attackAbility = FindObjectOfType<CharacterAttackAbility>();
            CharacterGunFireAbility gunFireAbility = FindObjectOfType<CharacterGunFireAbility>();

            switch (itemName)
            {
                case "����":
                    Debug.Log("�÷��̾ ������ �����");
                    attackAbility.WeaponActive(0);
                    break;
                case "�߱���Ʈ":
                    Debug.Log("�÷��̾ �߱���Ʈ�� �����");
                    attackAbility.WeaponActive(1);
                    break;
                case "��":
                    Debug.Log("�÷��̾ ���� �����");
                    attackAbility.WeaponActive(2);
                    break;
                case "��":
                    Debug.Log("�÷��̾ ���� �����");
                    gunFireAbility.GunActive(0);
                    break;
                default:
                    Debug.LogWarning("Unknown weapon item.");
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

    private void ApplyFoodEffect(string itemName)
    {
        switch (itemName)
        {
            case "���":
                Debug.Log("����� ȸ�� 20");
                break;
            case "��":
                Debug.Log("Player hunger increased by 30.");
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

    private void ApplyWeaponEffect(string itemName)
    {
        switch (itemName)
        {
            case "����":
                Debug.Log("�÷��̾ ������ �����");
                // Player.Instance.UseWeapon(axe);
                break;
            case "��":
                Debug.Log("Player used gun.");
                // Player.Instance.UseWeapon(gun);
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
                var bookTrigger = FindObjectOfType<BookTrigger>();
                if (bookTrigger != null)
                {
                    bookTrigger.UpdateMissionText("���� ���� ������� �ð��� �߾ӿ��� 20�ʰ� ����� �巯����.");
                }
                break;
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }

    private void DecreaseItemQuantity(Item item)
    {
        var inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            string itemName = item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC ? item.uniqueId : item.itemName;

            if (inventory.itemQuantities.ContainsKey(itemName))
            {
                inventory.itemQuantities[itemName]--;
                if (inventory.itemQuantities[itemName] <= 0)
                {
                    inventory.items.Remove(itemName);
                    inventory.itemQuantities.Remove(itemName);
                }
                RemoveItemFromQuickSlots(item);
                var inventoryUI = FindObjectOfType<InventoryUI>(true);

                inventoryUI.CloseItemInfo();

            }
        }
    }

    private void RemoveItemFromQuickSlots(Item item)
    {
        var quickSlotManager = FindObjectOfType<QuickSlotManager>();
        if (quickSlotManager != null)
        {
            quickSlotManager.RemoveItemFromQuickSlots(item);
        }
    }

    public void UpdateUI()
    {
        var inventoryUI = FindObjectOfType<InventoryUI>(true);
        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventoryUI();
        }

        var quickSlotManager = FindObjectOfType<QuickSlotManager>(true);
        if (quickSlotManager != null)
        {
            quickSlotManager.UpdateQuickSlotUI();
        }
    }
}
