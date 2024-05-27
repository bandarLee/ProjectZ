using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class ItemGenerateManager : MonoBehaviourPunCallbacks
{
    public ItemPresets itemPresetsContainer; // ItemPresets 스크립트를 참조합니다.
    public List<BoxInventory> allBoxInventories;
    public List<BoxTypeConfig> boxTypeConfigs; // 각 BoxType에 대한 설정 리스트

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
            Debug.Log("PhotonView가 추가되었습니다.");
        }
    }

    private void Start()
    {
        Debug.Log("ItemGenerateManager Start 호출됨");

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("현재 클라이언트는 마스터 클라이언트가 아닙니다. 아이템을 생성하지 않습니다.");
            return;
        }

        if (itemPresetsContainer == null)
        {
            Debug.LogError("ItemPresetsContainer가 설정되지 않았습니다.");
            return;
        }

        allBoxInventories = FindObjectsOfType<BoxInventory>().ToList();
        Debug.Log("발견된 BoxInventory 개수: " + allBoxInventories.Count);

        foreach (var box in allBoxInventories)
        {
            GenerateItemsForBox(box);
        }
    }

    public void GenerateItemsForBox(BoxInventory box)
    {
        var config = boxTypeConfigs.FirstOrDefault(c => c.boxType == box.boxType);
        if (config.Equals(default(BoxTypeConfig)))
        {
            Debug.LogWarning($"박스 타입에 대한 설정을 찾을 수 없습니다: {box.boxType}");
            return;
        }

        PhotonView boxPhotonView = box.GetComponent<PhotonView>();
        if (boxPhotonView == null)
        {
            Debug.LogError("BoxInventory에 PhotonView가 없습니다. BoxInventory 이름: " + box.gameObject.name);
            return;
        }

        Debug.Log("PhotonView가 " + box.gameObject.name + "에 있습니다. ViewID: " + boxPhotonView.ViewID);

        for (int i = 0; i < config.itemCount; i++)
        {
            Item randomItem = GetRandomItem(config);
            if (randomItem != null)
            {
                Debug.Log("아이템 생성: " + randomItem.itemName);
                if (photonView != null)
                {
                    Debug.Log("photonView가 null이 아닙니다. RPC를 호출합니다.");
                    photonView.RPC("RPC_AddItemToBox", RpcTarget.AllBuffered, boxPhotonView.ViewID, randomItem.itemName, randomItem.itemType.ToString());
                }
                else
                {
                    Debug.LogError("photonView가 null입니다. RPC를 호출할 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("Item preset이 비어 있거나 null입니다. 상자에 아이템을 추가할 수 없습니다.");
            }
        }

        // 아이템 생성 후 로그 추가
        Debug.Log($"총 {config.itemCount}개의 아이템이 {box.gameObject.name}에 추가되었습니다.");
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

        Debug.Log("랜덤 아이템 타입 선택됨: " + selectedType.ToString());
        return itemPresetsContainer.GenerateRandomItem(selectedType);
    }

    [PunRPC]
    private void RPC_AddItemToBox(int boxViewID, string itemName, string itemTypeString)
    {
        PhotonView boxView = PhotonView.Find(boxViewID);
        if (boxView != null)
        {
            BoxInventory boxInventory = boxView.GetComponent<BoxInventory>();
            if (boxInventory != null)
            {
                ItemType itemType = (ItemType)System.Enum.Parse(typeof(ItemType), itemTypeString);
                Item item = new Item { itemName = itemName, itemType = itemType, uniqueId = System.Guid.NewGuid().ToString() };
                boxInventory.AddItem(item);
                Debug.Log($"아이템 {itemName} 타입 {itemType}가 ViewID {boxViewID}를 가진 상자에 추가되었습니다.");
            }
            else
            {
                Debug.LogWarning("BoxInventory 컴포넌트를 PhotonView에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("PhotonView를 ViewID로 찾을 수 없습니다: " + boxViewID);
        }
    }
}
