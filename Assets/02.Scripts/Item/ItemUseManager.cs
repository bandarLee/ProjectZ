using Photon.Pun;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public static ItemUseManager Instance;
    public UseComputerTrigger computerTrigger;
    public QuickSlotManager quickSlotManager;
    public InventoryUI inventoryUI;
    public UI_Gunfire uI_Gunfire;
    public CharacterAttackAbility attackAbility;
    public CharacterGunFireAbility gunFireAbility;
    private CharacterItemAbility characterItemAbility;
    public UI_BookText uI_BookText; 
    public UI_DiskText uI_DiskText;

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
        if (quickSlotManager == null)
        {
            quickSlotManager = FindObjectOfType<QuickSlotManager>();
        }
        characterItemAbility = Character.LocalPlayerInstance.GetComponent<CharacterItemAbility>();
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
                ApplyMentalEffect(item);
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
        if (item.itemType == ItemType.Consumable)
        {
            DecreaseItemQuantity(item);
        }
        UpdateUI();
    }

    private void HandCount(Item item)
    {
        characterItemAbility.UnUsingHandAnimation();
        if (item.itemType == ItemType.Gun)
        {
            characterItemAbility.TwoHandAnimation();
        }
        else if (item.itemType == ItemType.Weapon)
        {
            return;
        }
        else
        {
            characterItemAbility.OneHandAnimation();
        }
    }

    public void EquipItem(Item item)
    {
        characterItemAbility.DeactivateAllItems();
        attackAbility.DeactivateAllWeapons();
        gunFireAbility.DeactivateAllGuns();
        HandCount(item);

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
                characterItemAbility.ItemActive("���");
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
                characterItemAbility.ItemActive("��");
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
            case "å":
                Debug.Log("Player used a Book.");
                uI_BookText.DisplayText("���� ���� ������� �ð��� �߾ӿ��� 20�ʰ� ����� �巯����.");
                break;
            case "��ũ1":
                break;
            case "��ũ2":
                break;
            case "��ũ3":
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
                Character.LocalPlayerInstance.Stat.Hunger += 20;
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

    private void ApplyMentalEffect(Item item)
    {
        switch (item.itemName)
        {
            case "��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 20;
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
                uI_BookText.DisplayText("���� ���� ������� �ð��� �߾ӿ��� 20�ʰ� ����� �巯����.");
                break;
            case "��ũ1":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk1");
                    uI_DiskText.DisplayText_1("���� ���� ���� ������ ������ ������ �ִ�.");
                }
                break;
            case "��ũ2":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk2");
                    uI_DiskText.DisplayText_2("������ ����� �Բ� ������ ������ ����.");
                }
                break;
            case "��ũ3":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk3");
                    uI_DiskText.DisplayText_3("<The Last Yggdrasil>.");
                }
                break;
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }

    private void DecreaseItemQuantity(Item item)
    {
        Inventory inventory = Inventory.Instance;
        if (inventory != null && Character.LocalPlayerInstance.PhotonView.IsMine)
        {
            string itemName = item.itemType == ItemType.Weapon || item.itemType == ItemType.ETC ? item.uniqueId : item.itemName;

            if (inventory.itemQuantities.ContainsKey(itemName))
            {
                inventory.itemQuantities[itemName]--;

                if (inventory.itemQuantities[itemName] <= 0)
                {
                    inventory.items.Remove(itemName);
                    inventory.itemQuantities.Remove(itemName);
                    if(quickSlotManager.currentEquippedItem.itemType != ItemType.Gun)
                    {
                        quickSlotManager.currentEquippedItem = null;
                    }
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
