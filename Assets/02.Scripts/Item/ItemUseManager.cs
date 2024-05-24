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

    private void ApplyFoodEffect(string itemName)
    {
        switch (itemName)
        {
            case "Meat":
                Debug.Log("Player hunger increased by 20.");
                // 실제 로직은 주석 처리
                // PlayerStatus.Instance.IncreaseHunger(20);
                break;
            case "Bread":
                Debug.Log("Player hunger increased by 30.");
                // 실제 로직은 주석 처리
                // PlayerStatus.Instance.IncreaseHunger(30);
                break;
            // 다른 음식 아이템의 효과도 추가
            default:
                Debug.LogWarning("Unknown food item.");
                break;
        }
    }

    private void ApplyHealEffect(string itemName)
    {
        switch (itemName)
        {
            case "Medkit":
                Debug.Log("Player health increased by 50.");
                // 실제 로직은 주석 처리
                // PlayerStatus.Instance.IncreaseHealth(50);
                break;
            case "Bandage":
                Debug.Log("Player health increased by 20.");
                // 실제 로직은 주석 처리
                // PlayerStatus.Instance.IncreaseHealth(20);
                break;
            // 다른 치료 아이템의 효과도 추가
            default:
                Debug.LogWarning("Unknown heal item.");
                break;
        }
    }

    private void ApplyMentalEffect(string itemName)
    {
        switch (itemName)
        {
            case "Book":
                Debug.Log("Player mental state improved.");
                // 실제 로직은 주석 처리
                // PlayerStatus.Instance.IncreaseMentalState(20);
                break;
            case "Toy":
                Debug.Log("Player happiness increased.");
                // 실제 로직은 주석 처리
                // PlayerStatus.Instance.IncreaseHappiness(15);
                break;
            // 다른 정신력 아이템의 효과도 추가
            default:
                Debug.LogWarning("Unknown mental item.");
                break;
        }
    }

    private void ApplyWeaponEffect(string itemName)
    {
        // 무기 아이템 사용 시의 효과 처리
        switch (itemName)
        {
            case "도끼":
                Debug.Log("Player equipped with sword.");
                // 실제 로직은 주석 처리
                // Player.Instance.EquipWeapon(sword);
                break;
            case "Gun":
                Debug.Log("Player equipped with bow.");
                // 실제 로직은 주석 처리
                // Player.Instance.EquipWeapon(bow);
                break;
            // 다른 무기 아이템의 효과도 추가
            default:
                Debug.LogWarning("Unknown weapon item.");
                break;
        }
    }

    private void ApplyEtcEffect(string itemName)
    {
        switch (itemName)
        {
            case "Map":
                Debug.Log("Player found a map.");
                // 실제 로직은 주석 처리
                // Player.Instance.ShowMap();
                break;
            case "Key":
                Debug.Log("Player found a key.");
                // 실제 로직은 주석 처리
                // Player.Instance.CollectKey();
                break;
            // 다른 기타 아이템의 효과도 추가
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }
}
