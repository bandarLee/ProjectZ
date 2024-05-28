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

            SyncGeneratedItemsWithRoomProperties();
        }
        else
        {
            LoadGeneratedItemsFromRoomProperties();
        }
    }

    private void SyncGeneratedItemsWithRoomProperties()
    {
        Dictionary<int, List<ItemData>> generatedItemsData = new Dictionary<int, List<ItemData>>();

        foreach (var box in allBoxInventories)
        {
            List<ItemData> boxItemsData = box.items.Select(kvp => new ItemData
            {
                ItemName = kvp.Value.itemName,
                ItemTypeString = kvp.Value.itemType.ToString(),
                UniqueId = kvp.Value.uniqueId
            }).ToList();

            generatedItemsData[box.photonView.ViewID] = boxItemsData;
        }

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "GeneratedItems", generatedItemsData }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    private void LoadGeneratedItemsFromRoomProperties()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("GeneratedItems", out var generatedItemsObject))
        {
            var generatedItemsData = (Dictionary<int, List<ItemData>>)generatedItemsObject;

            foreach (var kvp in generatedItemsData)
            {
                PhotonView boxView = PhotonView.Find(kvp.Key);
                if (boxView != null)
                {
                    BoxInventory boxInventory = boxView.GetComponent<BoxInventory>();
                    if (boxInventory != null)
                    {
                        foreach (var itemData in kvp.Value)
                        {
                            ItemType itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.ItemTypeString);
                            Item item = new Item
                            {
                                itemName = itemData.ItemName,
                                itemType = itemType,
                                uniqueId = itemData.UniqueId
                            };
                            boxInventory.AddItem(item);
                        }
                    }
                }
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
                box.AddItem(randomItem);
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

    [System.Serializable]
    public class ItemData
    {
        public string ItemName;
        public string ItemTypeString;
        public string UniqueId;
    }
}
