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

        // 지도
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

        if ((item.itemType == ItemType.Gun) || (item.itemType == ItemType.ETC && item.itemName == "열쇠"))
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
            case "고기":
                Debug.Log("고기를 들었음");
                characterItemAbility.ItemActive("고기");
                break;
            case "사과":
                Debug.Log("사과를 들었음");
                characterItemAbility.ItemActive("사과");
                break;
            case "통조림":
                Debug.Log("통조림을 들었음");
                characterItemAbility.ItemActive("통조림");
                break;
            case "수박":
                Debug.Log("수박을 들었음");
                characterItemAbility.ItemActive("수박");
                break;
            case "토마토":
                Debug.Log("토마토를 들었음");
                characterItemAbility.ItemActive("토마토");
                break;
            case "빵":
                Debug.Log("빵을 들었음");
                characterItemAbility.ItemActive("빵");
                break;
            case "김밥":
                Debug.Log("김밥을 들었음");
                characterItemAbility.ItemActive("김밥");
                break;
            case "초밥":
                Debug.Log("초밥을 들었음");
                characterItemAbility.ItemActive("초밥");
                break;
            case "바나나":
                Debug.Log("바나나를 들었음");
                characterItemAbility.ItemActive("바나나");
                break;
            case "아몬드":
                Debug.Log("아몬드를 들었음");
                characterItemAbility.ItemActive("아몬드");
                break;
            case "블루베리":
                Debug.Log("블루베리를 들었음");
                characterItemAbility.ItemActive("블루베리");
                break;
            case "감자칩":
                Debug.Log("감자칩을 들었음");
                characterItemAbility.ItemActive("감자칩");
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
                characterItemAbility.ItemActive("구급상자");
                break;
            case "진통제":
                Debug.Log("진통제를 들었음");
                characterItemAbility.ItemActive("진통제");
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
                characterItemAbility.ItemActive("술");
                break;
            case "주사기":
                Debug.Log("주사기를 들었음");
                characterItemAbility.ItemActive("주사기");
                break;
            case "물":
                Debug.Log("물을 들었음");
                characterItemAbility.ItemActive("물");
                break;
            case "맥주":
                Debug.Log("맥주를 들었음");
                characterItemAbility.ItemActive("맥주");
                break;
            case "위스키":
                Debug.Log("위스키를 들었음");
                characterItemAbility.ItemActive("위스키");
                break;
            case "에너지드링크_포도맛":
                Debug.Log("에너지드링크_포도맛을 들었음");
                characterItemAbility.ItemActive("에너지드링크_포도맛");
                break;
            case "에너지드링크_레몬맛":
                Debug.Log("에너지드링크_레몬맛을 들었음");
                characterItemAbility.ItemActive("에너지드링크_레몬맛");
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
            case "손전등":
                Debug.Log("Player found a flashlight.");
                break;
            case "지도":
                Debug.Log("Player found a map.");
                characterItemAbility.ItemActive("지도");
                break;
            case "지도조각 1":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("지도조각 1");
                break;
            case "지도조각 2":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("지도조각 2");
                break;
            case "지도조각 3":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("지도조각 3");
                break;
            case "지도조각 4":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("지도조각 4");
                break;
            case "지도조각 5":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("지도조각 5");
                break;
            case "지도조각 6":
                Debug.Log("Player found a map.");
                //characterItemAbility.ItemActive("지도조각 6");
                break;
            case "열쇠":
                Debug.Log("Player found a key.");
                characterItemAbility.ItemActive("열쇠");
                break;
            case "책":
                Debug.Log("Player used a Book.");
                characterItemAbility.ItemActive("책");
                break;
            case "책2":
                Debug.Log("Player used a Book.");
                characterItemAbility.ItemActive("책");
                break;
            case "디스크1":
                characterItemAbility.ItemActive("디스크1");
                break;
            case "디스크2":
                characterItemAbility.ItemActive("디스크1");
                break;
            case "디스크3":
                characterItemAbility.ItemActive("디스크1");
                break;
            case "세계수 씨앗":
                characterItemAbility.ItemActive("세계수 씨앗");
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
                characterItemAbility.ItemActive("총알");
                break;
            default:
                Debug.LogWarning("Unknown Consumable item.");
                break;
        }
    }
    private void EquipStatBook(string itemName)
    {
        characterItemAbility.ItemActive("책");
        switch (itemName)
        {
            case "스탯북_이동속도":
                Debug.Log("이속북 들었음");
                break;
            case "스탯북_공격력":
                Debug.Log("공격북 들었음");
                break;
            case "스탯북_점프력":
                Debug.Log("점프북 들었음");
                break;
            case "스탯북_체력":
                Debug.Log("체력북 들었음");
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
            case "사과":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "통조림":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 20;
                break;
            case "수박":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "토마토":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "빵":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "김밥":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "초밥":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "바나나":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 10;
                break;
            case "아몬드":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 5;
                break;
            case "블루베리":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Hunger += 5;
                break;
            case "감자칩":
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
            case "구급상자":
                Debug.Log("플레이어 체력 +20.");
                Character.LocalPlayerInstance.Stat.Health += 20;
                break;
            case "진통제":
                Debug.Log("플레이어 체력 +10.");
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
            case "술":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 15;
                break;
            case "주사기":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental = Character.LocalPlayerInstance.Stat.MaxMental;
                break;
            case "물":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 5;
                break;
            case "맥주":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 10;
                break;
            case "위스키":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 15;
                break;
            case "에너지드링크_포도맛":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Mental += 10;
                break;
            case "에너지드링크_레몬맛":
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
            case "손전등":
                Debug.Log("Player used a flashlight.");
                ToggleFlashLight();
                break;
            case "지도":
                Debug.Log("Player used a map.");
                ToggleMap();
                break;
            case "지도조각 1":
                Debug.Log("Player registers the map1.");
                map.RegisterMapPiece(0);
                DecreaseItemQuantity(item);
                break;
            case "지도조각 2":
                Debug.Log("Player registers the map2.");
                map.RegisterMapPiece(1);
                DecreaseItemQuantity(item);
                break;
            case "지도조각 3":
                Debug.Log("Player registers the map3.");
                map.RegisterMapPiece(2);
                DecreaseItemQuantity(item);
                break;
            case "지도조각 4":
                Debug.Log("Player registers the map4.");
                map.RegisterMapPiece(3);
                DecreaseItemQuantity(item);
                break;
            case "지도조각 5":
                Debug.Log("Player registers the map5.");
                map.RegisterMapPiece(4);
                DecreaseItemQuantity(item);
                break;
            case "지도조각 6":
                Debug.Log("Player registers the map6.");
                map.RegisterMapPiece(5);
                DecreaseItemQuantity(item);
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
                if (!isDisplayingText)
                {
                    Debug.Log("Player used a Book.");
                    isDisplayingText = true;
                    uI_BookText.DisplayText("소의 뿔이 사라지는 시간에 중앙에서 모습을 드러낸다.", () => {
                        isDisplayingText = false;
                    });
                }
                break;
            case "책2":
                if (!isDisplayingText)
                {
                    Debug.Log("Player used a Book.");
                    isDisplayingText = true;
                    uI_BookText.DisplayText("죽음의 도시에서만 존재한다.", () => {
                        isDisplayingText = false;
                    });
                }
                break;
            case "디스크1":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk1");
                    uI_DiskText.DisplayText_1("가장 깊은 곳에 마지막 생명이 숨쉬고 있다.");
                }
                break;
            case "디스크2":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk2");
                    uI_DiskText.DisplayText_2("마지막 생명과 함께 최후의 섬으로 가라.");
                }
                break;
            case "디스크3":
                if (computerTrigger.isPlayerInTrigger)
                {
                    Debug.Log("Player used a Disk3");
                    uI_DiskText.DisplayText_3("<The Last Yggdrasil>.");
                }
                break;
            case "세계수 씨앗":
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
            case "스탯북_이동속도":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.MoveSpeed += 0.1f;
                isDisplayingText = true;
                uI_BookText.DisplayText("빨리 뛰는 법 ... 다리를 빠르게 움직인다 ", () => {
                    isDisplayingText = false;
                });
                break;
            case "스탯북_공격력":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.Damage += 0.1f;
                isDisplayingText = true;
                uI_BookText.DisplayText("세게 때리는 법 ... 세게 팔을 휘두른다.", () => {
                    isDisplayingText = false;
                });
                break;
            case "스탯북_점프력":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.JumpPower += 0.1f;
                isDisplayingText = true;
                uI_BookText.DisplayText("높이 뛰는 법 ... 신발을 좋은걸 신는다.", () => {
                    isDisplayingText = false;
                });
                break;
            case "스탯북_체력":
                DecreaseItemQuantity(item);
                Character.LocalPlayerInstance.Stat.MaxHealth += 10;
                isDisplayingText = true;
                uI_BookText.DisplayText("체력이 좋아지는 법 ... 운동을 한다.", () => {
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
        if (item.itemType == ItemType.ETC && item.itemName == "열쇠")
        {
            Debug.Log("열쇠는 UseItem 메서드에서 사용되지 않습니다.");
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
