using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RectTransform MapRect; // 미니맵 UI 이미지 RectTransform
    public RectTransform PlayerIcon;  // 플레이어 아이콘 RectTransform
    public Transform PlayerTransform; // 플레이어 Transform

    /* public float mapWidth;  // 미니맵의 실제 폭
     public float mapHeight; // 미니맵의 실제 높이*/


    void Start()
    {
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
        PlayerIcon.anchoredPosition = new Vector2((playerWorldPos.x - PositionMargin.x)*4f, (playerWorldPos.z - PositionMargin.z)*6f);

        Debug.Log("플레이어 마진 값" + PlayerTransform.localPosition);



    }

    // 플레이어 Transform을 설정하는 메서드 추가
    public void SetPlayerTransform(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
        Debug.Log("현재 플레이어 위치: " + playerTransform.position);
    }
}
