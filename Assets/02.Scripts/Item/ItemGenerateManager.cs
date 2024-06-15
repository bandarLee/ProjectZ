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
            itemPresetsContainer = FindObjectOfType<ItemPresets>();
        }


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
        float totalProbability = config.defaultProbability + config.foodProbability + config.weaponProbability + config.healProbability + config.mentalProbability + config.etcProbability + config.gunProbability + config.consumeProbability;
        float randomValue = Random.Range(0, totalProbability);

        ItemType selectedType;
        if(randomValue < config.defaultProbability)
        {
            Debug.LogError("아이템 생성 이상함");
        }
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
        else if (randomValue < config.foodProbability + config.weaponProbability + config.healProbability + config.mentalProbability + config.gunProbability)
        {
            selectedType = ItemType.Gun;
        }
        else if (randomValue < config.foodProbability + config.weaponProbability + config.healProbability + config.mentalProbability + config.gunProbability + config.consumeProbability)
        {
            selectedType = ItemType.Consumable;
        }
        else
        {
            selectedType = ItemType.ETC;
        }

        return itemPresetsContainer.GenerateRandomItem(selectedType);
    }
}
