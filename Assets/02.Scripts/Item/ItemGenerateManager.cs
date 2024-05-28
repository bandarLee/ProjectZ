using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ItemGenerateManager : MonoBehaviourPunCallbacks
{
    public ItemPresets itemPresetsContainer;
    public List<BoxInventory> allBoxInventories;
    public List<BoxTypeConfig> boxTypeConfigs;

    private Dictionary<int, List<ItemData>> generatedItemsData = new Dictionary<int, List<ItemData>>();

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
            StartCoroutine(LoadGeneratedItemsAfterDelay()); 
        }
    }

    public void GenerateItemsForBox(BoxInventory box)
    {
        var config = boxTypeConfigs.FirstOrDefault(c => c.boxType == box.boxType);

        List<ItemData> boxItemsData = new List<ItemData>();
        for (int i = 0; i < config.itemCount; i++)
        {
            Item randomItem = GetRandomItem(config);
            if (randomItem != null)
            {
                ItemData itemData = new ItemData
                {
                    ItemName = randomItem.itemName,
                    ItemTypeString = randomItem.itemType.ToString(),
                    UniqueId = randomItem.uniqueId,
                    IconPath = randomItem.iconPath,
                    ItemEffect = randomItem.itemEffect,
                    ItemDescription = randomItem.itemDescription
                };
                boxItemsData.Add(itemData);
                box.AddItem(randomItem);
            }
            else
            {
                Debug.LogWarning("Item preset is empty or null. Cannot add item to box.");
            }
        }
        generatedItemsData[box.photonView.ViewID] = boxItemsData;
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

    private void SyncGeneratedItemsWithRoomProperties()
    {
        Hashtable roomProperties = new Hashtable
        {
            { "GeneratedItemsData", SerializationUtils.Serialize(generatedItemsData) }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }
    private IEnumerator LoadGeneratedItemsAfterDelay()
    {
        yield return new WaitForSeconds(1f); 
        LoadGeneratedItemsFromRoomProperties();
    }
    private void LoadGeneratedItemsFromRoomProperties()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("GeneratedItemsData", out object serializedData))
        {
            generatedItemsData = SerializationUtils.Deserialize<Dictionary<int, List<ItemData>>>((byte[])serializedData);

            foreach (var box in allBoxInventories)
            {
                if (generatedItemsData.TryGetValue(box.photonView.ViewID, out List<ItemData> itemDataList))
                {
                    foreach (var itemData in itemDataList)
                    {
                        string iconPath = itemData.IconPath;
                        Debug.Log($"Loading icon from path: {iconPath}");
                        Sprite icon = Resources.Load<Sprite>(iconPath);
                        if (icon == null)
                        {
                            Debug.LogError($"Failed to load icon at path: {iconPath}");
                        }
                        else
                        {
                            Debug.Log($"Successfully loaded icon at path: {iconPath}");
                        }

                        box.AddItem(new Item
                        {
                            itemName = itemData.ItemName,
                            itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemData.ItemTypeString),
                            uniqueId = itemData.UniqueId,
                            icon = icon,
                            itemEffect = itemData.ItemEffect,
                            itemDescription = itemData.ItemDescription
                        });
                    }
                }
            }
        }
    }


    [System.Serializable]
    public struct ItemData
    {
        public string ItemName;
        public string ItemTypeString;
        public string UniqueId;
        public string IconPath;
        public string ItemEffect;
        public string ItemDescription;
    }
}
