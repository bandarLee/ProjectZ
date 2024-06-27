using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using UnityEngine.UI;

public class TheLastYggdrasilTrigger : MonoBehaviourPunCallbacks
{
    public GameObject LastYggdrasilTrigger;

    public TextMeshProUGUI UseSeedText;
    public TextMeshProUGUI NoSeedItemText;
    public TextMeshProUGUI StartWaveText;
    public TextMeshProUGUI ProtectLastYggdrasilText;

    public GameObject TheLastYggdrasilPrefab;
    public Slider TheLastYggdrasilHPSlider;
    public UI_Timer uiTimer;


    private Inventory playerInventory;
    private QuickSlotManager quickSlotManager;
    private InventoryUI inventoryUI;
    private InventoryManager inventoryManager;
    private ItemUseManager itemUseManager;

    public bool IsPlayerTrigger = false;
    private Vector3 initialPosition; // ������ �ʱ� ��ġ ����
    private bool treePlanted = false; // ������ �ɾ������� ���θ� ����

    public PhotonView PV;

    private void Start()
    {
        UseSeedText.gameObject.SetActive(false);
        NoSeedItemText.gameObject.SetActive(false);
        TheLastYggdrasilHPSlider.gameObject.SetActive(false);
        StartWaveText.gameObject.SetActive(false);
        ProtectLastYggdrasilText.gameObject.SetActive(false);

        initialPosition = new Vector3(-45.53f, 10.7f, -54.78f); // �ʱ� ��ġ ����
        TheLastYggdrasilPrefab.transform.position = initialPosition;
        TheLastYggdrasilPrefab.SetActive(false);

        uiTimer = FindObjectOfType<UI_Timer>();

        StartCoroutine(InitializingInventory());

        PV = GetComponent<PhotonView>();    
    }

    private IEnumerator InitializingInventory()
    {
        yield return new WaitForSeconds(1.1f);

        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager�� ã�� �� �����ϴ�. ���� InventoryManager�� �ִ��� Ȯ���ϼ���.");
        }
        playerInventory = Inventory.Instance;
        quickSlotManager = FindObjectOfType<QuickSlotManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        itemUseManager = FindObjectOfType<ItemUseManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!treePlanted && other.CompareTag("Player")) // ������ �ɾ����� �ʾ��� ���� Ʈ���� �۵�
        {
            Debug.Log("Player Triggered on LastYggdrasilTrigger");

            IsPlayerTrigger = true;
            UseSeedText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerTrigger = false;
            UseSeedText.gameObject.SetActive(false);
        }
    }

    // �÷��̾ EŰ�� ���� ������� �ɴ´�
    private void Update()
    {
        if (!treePlanted && IsPlayerTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Item seedItem = GetSeedItem();
            if (seedItem != null)
            {
                itemUseManager.ApplyEffect(seedItem);
                UseSeedText.gameObject.SetActive(false); // UseSeedText ��Ȱ��ȭ
                NoSeedItemText.gameObject.SetActive(false); // NoSeedItemText ��Ȱ��ȭ
                treePlanted = true; // ������ �ɾ������� ǥ��

                // Ÿ�̸� ����
                if (PhotonNetwork.IsMasterClient && uiTimer != null)
                {
                    uiTimer.StartTimer(100);
                }
                StartCoroutine(ShowWaveTexts());

                // ���� ���� ���� ��� �÷��̾�� ����ȭ
                PV.RPC("PlantTree", RpcTarget.AllBuffered);
            }
            else
            {
                NoSeedItemText.gameObject.SetActive(true);
                Debug.Log("No Seed Item in Quick Slot");
                StartCoroutine(HideNoSeedTextAfterDelay());
            }
        } 
    }

    [PunRPC]
    private void PlantTree()
    {
        if (TheLastYggdrasilPrefab != null)
        {
            TheLastYggdrasilPrefab.SetActive(true);
            TheLastYggdrasilHPSlider.gameObject.SetActive(true); // ü�¹� Ȱ��ȭ

            TheLastYggdrasilPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            TheLastYggdrasilPrefab.transform.position = new Vector3(-45.53f, 10.7f, -54.78f);
            TheLastYggdrasilPrefab.transform.DOScale(new Vector3(1f, 1f, 1f), 60f).OnUpdate(() =>
            {
                // ������ ���濡 ���� ��ġ ����
                Vector3 newPosition = CalculatePositionBasedOnScale(TheLastYggdrasilPrefab.transform.localScale);
                TheLastYggdrasilPrefab.transform.position = newPosition;
            });
        }
    }
      

    private IEnumerator ShowWaveTexts()
    {
        yield return DisplayText(StartWaveText, "���͵��� ������ ���۵˴ϴ�", 0.07f);
        yield return DisplayText(ProtectLastYggdrasilText, "������ ������ ���ѳ�����", 0.07f);
    }

    private IEnumerator DisplayText(TextMeshProUGUI textMeshProUGUI, string text, float typingSpeed)
    {
        textMeshProUGUI.gameObject.SetActive(true);
        textMeshProUGUI.text = text;

        int totalVisibleCharacters = text.Length;
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            textMeshProUGUI.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(1.5f);
        textMeshProUGUI.gameObject.SetActive(false);
    }

    private Vector3 CalculatePositionBasedOnScale(Vector3 scale)
    {
        float x = scale.x;
        if (x <= 0.1f) return new Vector3(-45.53f, 10.7f, -54.78f);
        if (x <= 0.3f) return Vector3.Lerp(new Vector3(-45.53f, 10.7f, -54.78f), new Vector3(-45.23f, 11.1f, -55.37f), (x - 0.1f) / 0.2f);
        if (x <= 0.5f) return Vector3.Lerp(new Vector3(-45.23f, 11.1f, -55.37f), new Vector3(-44.96f, 11.55f, -55.46f), (x - 0.3f) / 0.2f);
        if (x <= 0.7f) return Vector3.Lerp(new Vector3(-44.96f, 11.55f, -55.46f), new Vector3(-44.96f, 11.77f, -55.46f), (x - 0.5f) / 0.2f);
        if (x <= 0.9f) return Vector3.Lerp(new Vector3(-44.96f, 11.77f, -55.46f), new Vector3(-44.96f, 12.17f, -55.48f), (x - 0.7f) / 0.2f);
        return Vector3.Lerp(new Vector3(-44.96f, 12.17f, -55.48f), new Vector3(-44.96f, 12.34f, -55.46f), (x - 0.9f) / 0.1f);
    }

    private IEnumerator HideNoSeedTextAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        if (!treePlanted) // ������ �ɾ����� �ʾ��� ���� �ؽ�Ʈ ��Ȱ��ȭ
        {
            NoSeedItemText.gameObject.SetActive(false);
        }
    }

    private Item GetSeedItem()
    {
        if (quickSlotManager == null || quickSlotManager.quickSlotItems == null)
        {
            Debug.LogError("QuickSlotManager or quickSlotItems is null");
            return null;
        }

        // ������ �˻�
        foreach (var slotItem in quickSlotManager.quickSlotItems)
        {
            if (slotItem != null && slotItem.itemType == ItemType.Special && slotItem.itemName == "���������")
            {
                return slotItem;
            }
        }
        return null;
    }
}
