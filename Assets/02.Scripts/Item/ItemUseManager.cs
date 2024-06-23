using Photon.Pun;
using System.Collections;
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
    public Map map;
    public Light FlashLight;
    public GameObject MapImage;

    private bool isMapActive = false;
    private bool isFlashLightActive = false;
    private bool isDisplayingText = false;

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

        FlashLight.enabled = false;
        MapImage.SetActive(false); 

        // ����
        if (map == null)
        {
            map = FindObjectOfType<Map>();
            if(map == null)
            {
                Debug.LogError("Map component not found!!!!!!!!!!");
            }
        }
        if (MapImage == null)
        {
            Debug.LogError("MapImage GameObject is not assigned!");
        }
    }

    public void ApplyEffect(Item item)
    {

        if ((item.itemType == ItemType.Gun) || (item.itemType == ItemType.ETC && item.itemName == "����"))
        {
            return;
        }
        Character.LocalPlayerInstance._animator.SetBool("isPullOut", false);
        


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
            case ItemType.StatBook:
                ApplyStatBookEffect(item);
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
        if (item.itemType == ItemType.Gun)
        {
            Character.LocalPlayerInstance._animator.SetInteger("UsingHand", 2);
        }
        else if (item.itemType == ItemType.Weapon)
        {
            return;
        }
        else
        {
            Character.LocalPlayerInstance._animator.SetInteger("UsingHand", 1);
            StartCoroutine(TimeDelayHand());

        }
    }
    IEnumerator TimeDelayHand()
    {
        yield return new WaitForSeconds(0.5f);
        Character.LocalPlayerInstance._animator.SetInteger("UsingHand", 0);
    }
    public void EquipItem(Item item)
    {
        characterItemAbility.DeactivateAllItems();
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
            case ItemType.StatBook:
                EquipStatBook(item.itemName);
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
            case "���":
                Debug.Log("����� �����");
                characterItemAbility.ItemActive("���");
                break;
            case "������":
                Debug.Log("�������� �����");
                characterItemAbility.ItemActive("������");
                break;
            case "����":
                Debug.Log("������ �����");
                characterItemAbility.ItemActive("����");
                break;
            case "�丶��":
                Debug.Log("�丶�並 �����");
                characterItemAbility.ItemActive("�丶��");
                break;
            case "��":
                Debug.Log("���� �����");
                characterItemAbility.ItemActive("��");
                break;
            case "���":
                Debug.Log("����� �����");
                characterItemAbility.ItemActive("���");
                break;
            case "�ʹ�":
                Debug.Log("�ʹ��� �����");
                characterItemAbility.ItemActive("�ʹ�");
                break;
            case "�ٳ���":
                Debug.Log("�ٳ����� �����");
                characterItemAbility.ItemActive("�ٳ���");
                break;
            case "�Ƹ��":
                Debug.Log("�Ƹ�带 �����");
                characterItemAbility.ItemActive("�Ƹ��");
                break;
            case "��纣��":
                Debug.Log("��纣���� �����");
                characterItemAbility.ItemActive("��纣��");
                break;
            case "����Ĩ":
                Debug.Log("����Ĩ�� �����");
                characterItemAbility.ItemActive("����Ĩ");
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
                characterItemAbility.ItemActive("���޻���");
                break;
            case "������":
                Debug.Log("�������� �����");
                characterItemAbility.ItemActive("������");
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
            case "�ֻ��":
                Debug.Log("�ֻ�⸦ �����");
                characterItemAbility.ItemActive("�ֻ��");
                break;
            case "��":
                Debug.Log("���� �����");
                characterItemAbility.ItemActive("��");
                break;
            case "����":
                Debug.Log("���ָ� �����");
                characterItemAbility.ItemActive("����");
                break;
            case "����Ű":
                Debug.Log("����Ű�� �����");
                characterItemAbility.ItemActive("����Ű");
                break;
            case "�������帵ũ_������":
                Debug.Log("�������帵ũ_�������� �����");
                characterItemAbility.ItemActive("�������帵ũ_������");
                break;
            case "�������帵ũ_�����":
                Debug.Log("�������帵ũ_������� �����");
                characterItemAbility.ItemActive("�������帵ũ_�����");
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
            case "������":
                Debug.Log("Player found a flashlight.");
                break;
            case "����":
                Debug.Log("Player found a map.");
                characterItemAbility.ItemActive("����");
                break;
            case "�������� 1":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("�������� 1");
                break;
            case "�������� 2":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("�������� 2");
                break;
            case "�������� 3":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("�������� 3");
                break;
            case "�������� 4":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("�������� 4");
                break;
            case "�������� 5":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("�������� 5");
                break;
            case "�������� 6":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("�������� 6");
                break;
            case "����":
                Debug.Log("Player found a key.");
                characterItemAbility.ItemActive("����");
                break;
            case "å":
                Debug.Log("Player used a Book.");
                characterItemAbility.ItemActive("å");
                break;
            case "å2":
                Debug.Log("Player used a Book.");
                characterItemAbility.ItemActive("å");
                break;
            case "��ũ1":
                characterItemAbility.ItemActive("��ũ1");
                break;
            case "��ũ2":
                characterItemAbility.ItemActive("��ũ1");
                break;
            case "��ũ3":
                characterItemAbility.ItemActive("��ũ1");
                break;
            case "����� ����":
                characterItemAbility.ItemActive("����� ����");
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
                characterItemAbility.ItemActive("�Ѿ�");
                break;
            default:
                Debug.LogWarning("Unknown Consumable item.");
                break;
        }
    }
    private void EquipStatBook(string itemName)
    {
        characterItemAbility.ItemActive("å");
        switch (itemName)
        {
            case "���Ⱥ�_�̵��ӵ�":
                Debug.Log("�̼Ӻ� �����");
                break;
            case "���Ⱥ�_���ݷ�":
                Debug.Log("���ݺ� �����");
                break;
            case "���Ⱥ�_������":
                Debug.Log("������ �����");
                break;
            case "���Ⱥ�_ü��":
                Debug.Log("ü�º� �����");
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
            case "���":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "������":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 20;
                break;
            case "����":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "�丶��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "���":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "�ʹ�":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "�ٳ���":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "�Ƹ��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 5;
                break;
            case "��纣��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 5;
                break;
            case "����Ĩ":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
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
                Debug.Log("�÷��̾� ü�� +20.");
                Character.LocalPlayerInstance.Stat.Health += 20;
                break;
            case "������":
                Debug.Log("�÷��̾� ü�� +10.");
                Character.LocalPlayerInstance.Stat.Health += 10;
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
                Character.LocalPlayerInstance.Stat.Mental += 15;
                break;
            case "�ֻ��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental = Character.LocalPlayerInstance.Stat.MaxMental;
                break;
            case "��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 5;
                break;
            case "����":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 10;
                break;
            case "����Ű":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 15;
                break;
            case "�������帵ũ_������":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 10;
                break;
            case "�������帵ũ_�����":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 10;
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
            case "������":
                Debug.Log("Player used a flashlight.");
                ToggleFlashLight();
                break;
            case "����":
                Debug.Log("Player used a map.");
                ToggleMap();
                break;
            case "�������� 1":
                Debug.Log("Player registers the map1.");
                map.RegisterMapPiece(0);
                DecreaseItemQuantity(item);
                break;
            case "�������� 2":
                Debug.Log("Player registers the map2.");
                map.RegisterMapPiece(1);
                DecreaseItemQuantity(item);
                break;
            case "�������� 3":
                Debug.Log("Player registers the map3.");
                map.RegisterMapPiece(2);
                DecreaseItemQuantity(item);
                break;
            case "�������� 4":
                Debug.Log("Player registers the map4.");
                map.RegisterMapPiece(3);
                DecreaseItemQuantity(item);
                break;
            case "�������� 5":
                Debug.Log("Player registers the map5.");
                map.RegisterMapPiece(4);
                DecreaseItemQuantity(item);
                break;
            case "�������� 6":
                Debug.Log("Player registers the map6.");
                map.RegisterMapPiece(5);
                DecreaseItemQuantity(item);
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
                if (!isDisplayingText)
                {
                    Debug.Log("Player used a Book.");
                    isDisplayingText = true;
                    uI_BookText.DisplayText("���� ���� ������� �ð��� �߾ӿ��� ����� �巯����.", () => {
                        isDisplayingText = false;
                    });
                }
                break;
            case "å2":
                if (!isDisplayingText)
                {
                    Debug.Log("Player used a Book.");
                    isDisplayingText = true;
                    uI_BookText.DisplayText("������ ���ÿ����� �����Ѵ�.", () => {
                        isDisplayingText = false;
                    });
                }
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
            case "����� ����":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a LastSeed");
                    
                }
                break;
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }
    private void ApplyStatBookEffect(Item item)
    {
        switch (item.itemName)
        {
            case "���Ⱥ�_�̵��ӵ�":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.MoveSpeed += 0.1f;
                isDisplayingText = true;
                uI_BookText.DisplayText("���� �ٴ� �� ... �ٸ��� ������ �����δ� ", () => {
                    isDisplayingText = false;
                });
                break;
            case "���Ⱥ�_���ݷ�":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Damage += 0.1f;
                isDisplayingText = true;
                uI_BookText.DisplayText("���� ������ �� ... ���� ���� �ֵθ���.", () => {
                    isDisplayingText = false;
                });
                break;
            case "���Ⱥ�_������":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.JumpPower += 0.1f;
                isDisplayingText = true;
                uI_BookText.DisplayText("���� �ٴ� �� ... �Ź��� ������ �Ŵ´�.", () => {
                    isDisplayingText = false;
                });
                break;
            case "���Ⱥ�_ü��":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.MaxHealth += 10;
                isDisplayingText = true;
                uI_BookText.DisplayText("ü���� �������� �� ... ��� �Ѵ�.", () => {
                    isDisplayingText = false;
                });
                break;
        }
    }
    public void DecreaseItemQuantity(Item item)
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
                    characterItemAbility.DeactivateAllItems();

                    inventory.items.Remove(itemName);
                    inventory.itemQuantities.Remove(itemName);
                    if (quickSlotManager.currentEquippedItem.itemType != ItemType.Gun)
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
        if(item.itemType != ItemType.Gun)
        {
            HandCount(item);
            StartCoroutine(uI_Gunfire.UseItemWithTimer(duration, () => ApplyEffect(item)));
        }
        if (item.itemType == ItemType.ETC && item.itemName == "����")
        {
            Debug.Log("����� UseItem �޼��忡�� ������ �ʽ��ϴ�.");
            return;
        }
    }

    public void MapExit()
    {
        map.mapController.IsMapActive = false;

        MapImage.SetActive(false);
        Character.LocalPlayerInstance._characterRotateAbility.SetMouseLock(true);

    }


    private void ToggleMap()
    {
        MapImage.SetActive(true);
        map.mapController.IsMapActive = true;
        map.mapController.IconInactive(false);

            map.OpenMap();
            Character.LocalPlayerInstance._characterRotateAbility.SetMouseLock(false);

    }

    private void ToggleFlashLight()
    {
        characterItemAbility.ToggleFlashlight();
    }

    public void UpdateUI()
    {
        inventoryUI.UpdateInventoryUI();
        quickSlotManager.UpdateQuickSlotUI();
    }
}
