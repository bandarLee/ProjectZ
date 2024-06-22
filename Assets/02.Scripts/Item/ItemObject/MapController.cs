using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RectTransform mapRect; // �̴ϸ� UI �̹��� RectTransform
    public RectTransform playerIcon;  // �÷��̾� ������ RectTransform
    public Transform playerTransform; // �÷��̾� Transform

    public List<RectTransform> otherPlayerIconImages; // �̸� ������ �ٸ� �÷��̾� ������ ����Ʈ
    private List<RectTransform> otherPlayerIcons = new List<RectTransform>();

    private Vector3 positionMargin = new Vector3(1155, 0, 1075); // ������, �� ���� �����Ͽ� �������� ����
    private float scaleX;
    private float scaleY;
    private float offsetX;
    private float offsetY;

    void Start()
    {
        if (Character.LocalPlayerInstance != null)
        {
            playerTransform = Character.LocalPlayerInstance.transform;
        }
        // �ٸ� �÷��̾� ������ ����
        CreateOtherPlayerIcons();

        // ��ȯ ������ ������ ���
        CalculateTransformParameters();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("PlayerTransform is not set.");
            return;
        }

        UpdatePlayerIconPosition();
        UpdateOtherPlayerIconsPosition();
    }

    private void CalculateTransformParameters()
    {
        // �� �����Ǵ� ������ ����Ͽ� ������ ����մϴ�.
        // ���� �����͸� ����մϴ�.
        Vector3[] worldPositions = {
            new Vector3(1157.86f, 0, 1047.64f),
            new Vector3(1183.82f, 0, 1047.64f),
            new Vector3(1192.59f, 0, 1147.31f),
            new Vector3(1157.73f, 0, 1134.59f)
        };

        Vector2[] miniMapPositions = {
            new Vector2(1, -41),
            new Vector2(94, -41),
            new Vector2(147, 322),
            new Vector2(-2, 252)
        };

        // X�� ��ȯ ������ ������ ���
        scaleX = (miniMapPositions[1].x - miniMapPositions[0].x) / (worldPositions[1].x - worldPositions[0].x);
        offsetX = miniMapPositions[0].x - scaleX * worldPositions[0].x;

        // Y�� ��ȯ ������ ������ ��� (z���� y������ ��ȯ)
        scaleY = (miniMapPositions[2].y - miniMapPositions[1].y) / (worldPositions[2].z - worldPositions[1].z);
        offsetY = miniMapPositions[0].y - scaleY * worldPositions[0].z;

        Debug.Log($"ScaleX: {scaleX}, OffsetX: {offsetX}, ScaleY: {scaleY}, OffsetY: {offsetY}");
    }

    private void UpdatePlayerIconPosition()
    {
        // �÷��̾��� ���� ��ǥ
        Vector3 playerWorldPos = playerTransform.localPosition;

        // ���� ��ǥ�� �̴ϸ� ��ǥ�� ��ȯ
        float x = playerWorldPos.x * scaleX + offsetX;
        float y = playerWorldPos.z * scaleY + offsetY;

        // �̴ϸ� ��ǥ�� ����
        playerIcon.anchoredPosition = new Vector2(x, y);
    }

    private void CreateOtherPlayerIcons()
    {
        for (int i = 0; i < otherPlayerIconImages.Count; i++)
        {
            RectTransform otherPlayerIcon = Instantiate(otherPlayerIconImages[i], mapRect);
            otherPlayerIcons.Add(otherPlayerIcon);
        }
    }

    private void UpdateOtherPlayerIconsPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int index = 0;
        foreach (GameObject player in players)
        {
            if (player.transform != playerTransform && index < otherPlayerIcons.Count)
            {
                // �÷��̾��� ���� ��ǥ�� �����ɴϴ�.
                Vector3 playerWorldPos = player.transform.localPosition;

                // ���� ��ǥ�� �̴ϸ� ��ǥ�� ��ȯ�մϴ�.
                float x = playerWorldPos.x * scaleX + offsetX;
                float y = playerWorldPos.z * scaleY + offsetY;

                // �̴ϸ� ��ǥ�� �����մϴ�.
                otherPlayerIcons[index].anchoredPosition = new Vector2(x, y);

                index++;
            }
        }
    }

    // �÷��̾� Transform�� �����ϴ� �޼��� �߰�
    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    // �÷��̾� �������� Ȱ��ȭ ���¸� �����ϴ� �޼��� �߰�
    public void SetPlayerIconActive(bool isActive)
    {
        playerIcon.gameObject.SetActive(isActive);
    }

    // �ٸ� �÷��̾� �������� Ȱ��ȭ ���¸� �����ϴ� �޼��� �߰�
    public void SetOtherPlayerIconsActive(bool isActive)
    {
        foreach (var icon in otherPlayerIcons)
        {
            icon.gameObject.SetActive(isActive);
        }
    }
}
