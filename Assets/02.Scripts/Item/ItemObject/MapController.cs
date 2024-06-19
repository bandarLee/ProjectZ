using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RectTransform MapRect; // �̴ϸ� UI �̹��� RectTransform
    public RectTransform PlayerIcon;  // �÷��̾� ������ RectTransform
    public Transform PlayerTransform; // �÷��̾� Transform

   /* public float mapWidth;  // �̴ϸ��� ���� ��
    public float mapHeight; // �̴ϸ��� ���� ����*/

    private Vector2 MapOrigin; // �̴ϸ��� ���� ��ǥ

    void Start()
    {
        // �̴ϸ��� ���� ��ǥ ����
        MapOrigin = new Vector2(MapRect.rect.width / 2, MapRect.rect.height / 2);
        PlayerTransform = Character.LocalPlayerInstance.transform;
    }

    void Update()
    {
        if (PlayerTransform == null)
        { 
            Debug.LogWarning("PlayerTransform is not set.");
            return; 
        }

        Vector3 playerWorldPos = PlayerTransform.localPosition;
        Vector3 PositionMargin = new Vector3(1155, 0, 1075);
        PlayerIcon.anchoredPosition = new Vector2((playerWorldPos.x - PositionMargin.x), (playerWorldPos.z - PositionMargin.z));

        Debug.Log("playerWorldPos: " + playerWorldPos);

    }

    // �÷��̾� Transform�� �����ϴ� �޼��� �߰�
    public void SetPlayerTransform(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
        Debug.Log("���� �÷��̾� ��ġ: " + playerTransform.position);
    }
}
