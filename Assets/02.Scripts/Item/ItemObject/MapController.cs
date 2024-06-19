using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RectTransform mapRect; // �̴ϸ� UI �̹��� RectTransform
    public RectTransform playerIcon;  // �÷��̾� ������ RectTransform
    public Transform playerTransform; // �÷��̾� Transform

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

        // ��ȯ ������ �������� ����մϴ�.
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
        // �÷��̾��� ���� ��ǥ�� �����ɴϴ�.
        Vector3 playerWorldPos = playerTransform.localPosition;

        // ���� ��ǥ�� �̴ϸ� ��ǥ�� ��ȯ�մϴ�.
        float x = playerWorldPos.x * scaleX + offsetX;
        float y = playerWorldPos.z * scaleY + offsetY;

        // �̴ϸ� ��ǥ�� �����մϴ�.
        playerIcon.anchoredPosition = new Vector2(x, y);

        Debug.Log($"Player World Pos: {playerWorldPos} -> MiniMap Pos: ({x}, {y})");
    }

    // �÷��̾� Transform�� �����ϴ� �޼��� �߰�
    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        Debug.Log("���� �÷��̾� ��ġ: " + playerTransform.position);
    }
}
