using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RectTransform mapRect; // 미니맵 UI 이미지 RectTransform
    public RectTransform playerIcon;  // 플레이어 아이콘 RectTransform
    public Transform playerTransform; // 플레이어 Transform

    private Vector3 positionMargin = new Vector3(1155, 0, 1075); // 기준점, 이 값을 조정하여 기준점을 설정
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

        // 변환 비율과 오프셋을 계산합니다.
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
        // 각 대응되는 점들을 사용하여 비율을 계산합니다.
        // 예시 데이터를 사용합니다.
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

        // X축 변환 비율과 오프셋 계산
        scaleX = (miniMapPositions[1].x - miniMapPositions[0].x) / (worldPositions[1].x - worldPositions[0].x);
        offsetX = miniMapPositions[0].x - scaleX * worldPositions[0].x;

        // Y축 변환 비율과 오프셋 계산 (z축을 y축으로 변환)
        scaleY = (miniMapPositions[2].y - miniMapPositions[1].y) / (worldPositions[2].z - worldPositions[1].z);
        offsetY = miniMapPositions[0].y - scaleY * worldPositions[0].z;

        Debug.Log($"ScaleX: {scaleX}, OffsetX: {offsetX}, ScaleY: {scaleY}, OffsetY: {offsetY}");
    }

    private void UpdatePlayerIconPosition()
    {
        // 플레이어의 월드 좌표를 가져옵니다.
        Vector3 playerWorldPos = playerTransform.localPosition;

        // 월드 좌표를 미니맵 좌표로 변환합니다.
        float x = playerWorldPos.x * scaleX + offsetX;
        float y = playerWorldPos.z * scaleY + offsetY;

        // 미니맵 좌표를 설정합니다.
        playerIcon.anchoredPosition = new Vector2(x, y);

        Debug.Log($"Player World Pos: {playerWorldPos} -> MiniMap Pos: ({x}, {y})");
    }

    // 플레이어 Transform을 설정하는 메서드 추가
    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        Debug.Log("현재 플레이어 위치: " + playerTransform.position);
    }
}
