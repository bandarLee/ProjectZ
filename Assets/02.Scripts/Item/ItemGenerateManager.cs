using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

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

        for (int i = 0; i < config.itemCount; i++)
        {
            Item randomItem = GetRandomItem(config);
            if (randomItem != null)
            {
                box.BoxAddItem(randomItem);
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
}
