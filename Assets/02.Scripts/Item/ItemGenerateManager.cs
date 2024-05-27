using Photon.Pun;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ItemGenerateManager : MonoBehaviourPunCallbacks
{
    public ItemPresets itemPresetsContainer;
    public List<BoxInventory> allBoxInventories;
    public List<BoxTypeConfig> boxTypeConfigs;

    private void Start()
    {
        if (itemPresetsContainer == null)
        {
            Debug.LogError("ItemPresetsContainer is not set.");
            return;
        }

        allBoxInventories = FindObjectsOfType<BoxInventory>().ToList();

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var box in allBoxInventories)
            {
                GenerateItemsForBox(box);
            }
        }
    }

    public void GenerateItemsForBox(BoxInventory box)
    {
        var config = boxTypeConfigs.FirstOrDefault(c => c.boxType == box.boxType);
        if (config.boxType != box.boxType) return;

        for (int i = 0; i < config.itemCount; i++)
        {
            Item randomItem = GetRandomItem(config);
            if (randomItem != null)
            {
                photonView.RPC("AddItemToBox", RpcTarget.AllBuffered, box.photonView.ViewID, randomItem.itemName);
            }
            else
            {
                Debug.LogWarning("Item preset is empty or null. Cannot add item to box.");
            }
        }
    }

    private Item GetRandomItem(BoxTypeConfig config)
    {
        float totalProbability = config.foodProbability + config.weaponProbability + config.healProbability + config.mentalProbability + config.etcProbability;
        float randomValue = Random.Range(0, totalProbability);

        ItemType selectedType;
        if (randomValue < config.foodProbability)
        {
            selectedType = ItemType.Food;
        }
        else if (randomValue < config.foodProbability + config.weaponProbability)
        {
            selectedType = ItemType.Weapon;
        }
        else if (randomValue < config.foodProbability + config.weaponProbability + config.healProbability)
        {
            selectedType = ItemType.Heal;
        }
        else if (randomValue < config.foodProbability + config.weaponProbability + config.healProbability + config.mentalProbability)
        {
            selectedType = ItemType.Mental;
        }
        else
        {
            selectedType = ItemType.ETC;
        }

        return itemPresetsContainer.GenerateRandomItem(selectedType);
    }

    [PunRPC]
    public void AddItemToBox(int boxViewID, string itemName)
    {
        var box = PhotonView.Find(boxViewID).GetComponent<BoxInventory>();
        var item = itemPresetsContainer.GenerateRandomItem((ItemType)System.Enum.Parse(typeof(ItemType), itemName));
        if (box != null && item != null)
        {
            box.AddItem(item);
        }
    }
}
