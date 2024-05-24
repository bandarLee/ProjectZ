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
                // ���� ������ �ּ� ó��
                // PlayerStatus.Instance.IncreaseHunger(20);
                break;
            case "Bread":
                Debug.Log("Player hunger increased by 30.");
                // ���� ������ �ּ� ó��
                // PlayerStatus.Instance.IncreaseHunger(30);
                break;
            // �ٸ� ���� �������� ȿ���� �߰�
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
                // ���� ������ �ּ� ó��
                // PlayerStatus.Instance.IncreaseHealth(50);
                break;
            case "Bandage":
                Debug.Log("Player health increased by 20.");
                // ���� ������ �ּ� ó��
                // PlayerStatus.Instance.IncreaseHealth(20);
                break;
            // �ٸ� ġ�� �������� ȿ���� �߰�
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
                // ���� ������ �ּ� ó��
                // PlayerStatus.Instance.IncreaseMentalState(20);
                break;
            case "Toy":
                Debug.Log("Player happiness increased.");
                // ���� ������ �ּ� ó��
                // PlayerStatus.Instance.IncreaseHappiness(15);
                break;
            // �ٸ� ���ŷ� �������� ȿ���� �߰�
            default:
                Debug.LogWarning("Unknown mental item.");
                break;
        }
    }

    private void ApplyWeaponEffect(string itemName)
    {
        // ���� ������ ��� ���� ȿ�� ó��
        switch (itemName)
        {
            case "����":
                Debug.Log("Player equipped with sword.");
                // ���� ������ �ּ� ó��
                // Player.Instance.EquipWeapon(sword);
                break;
            case "Gun":
                Debug.Log("Player equipped with bow.");
                // ���� ������ �ּ� ó��
                // Player.Instance.EquipWeapon(bow);
                break;
            // �ٸ� ���� �������� ȿ���� �߰�
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
                // ���� ������ �ּ� ó��
                // Player.Instance.ShowMap();
                break;
            case "Key":
                Debug.Log("Player found a key.");
                // ���� ������ �ּ� ó��
                // Player.Instance.CollectKey();
                break;
            // �ٸ� ��Ÿ �������� ȿ���� �߰�
            default:
                Debug.LogWarning("Unknown etc item.");
                break;
        }
    }
}
