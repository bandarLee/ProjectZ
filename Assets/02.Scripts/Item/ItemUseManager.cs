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
                ApplyEtcEffect(item.itemName);
                break;
            default:
                Debug.LogWarning("Unknown item type.");
                break;
        }
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
                // 실제 로직은 주석 처리
                // Player.Instance.EquipWeapon(axe);

                break;
            case "총":
                Debug.Log("Player equipped with gun.");
                // 실제 로직은 주석 처리
                // Player.Instance.EquipWeapon(gun);
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


    private void ApplyFoodEffect(string itemName)
    {
        switch (itemName)
        {
            case "고기":
                Debug.Log("배고픔 회복 20");
                break;
            case "빵":
                Debug.Log("Player hunger increased by 30.");
                break;
            default:
                Debug.LogWarning("Unknown food item.");
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
                Debug.Log("Player mental state improved.");
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

    private void ApplyWeaponEffect(string itemName)
    {
        switch (itemName)
        {
            case "도끼":
                Debug.Log("플레이어가 도끼를 사용함");
                // Player.Instance.UseWeapon(axe);
                break;
            case "총":
                Debug.Log("Player used gun.");
                // Player.Instance.UseWeapon(gun);
                break;
            default:
                Debug.LogWarning("Unknown weapon item.");
                break;
        }
    }

    private void ApplyEtcEffect(string itemName)
    {
        switch (itemName)
        {
            case "지도":
                Debug.Log("Player used a map.");
                // Player.Instance.UseMap();
                break;
            case "열쇠":
                Debug.Log("Player used a key.");
                break;
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }
}
