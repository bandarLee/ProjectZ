using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RectTransform playerIcon;  // 플레이어 아이콘 RectTransform
    public Transform playerTransform; // 플레이어 Transform

    public RectTransform[] otherPlayerIcons;

    private Vector3 positionMargin = new Vector3(1155, 0, 1075); // 기준점, 이 값을 조정하여 기준점을 설정
    private float scaleX;
    private float scaleY;
    private float offsetX;
    private float offsetY;

    public bool IsMapActive = false;
    private void OnEnable()
    {
        if (Character.LocalPlayerInstance != null && playerTransform == null)
        {
            playerTransform = Character.LocalPlayerInstance.gameObject.transform;
            CalculateTransformParameters();

        }
    }
    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = Character.LocalPlayerInstance.gameObject.transform;
            CalculateTransformParameters();
        }
        if (IsMapActive)
        {
            UpdatePlayerIconPosition();
            UpdateOtherPlayerIconsPosition();
        }

    }
    public void IconInactive(bool isactive)
    {
        playerIcon.gameObject.SetActive(isactive);
        foreach (RectTransform otherplayerIcon in otherPlayerIcons)
        {
            otherplayerIcon.gameObject.SetActive(isactive);
        }
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
        // 플레이어의 월드 좌표
        Vector3 playerWorldPos = playerTransform.localPosition;

        // 월드 좌표를 미니맵 좌표로 변환
        float x = playerWorldPos.x * scaleX + offsetX;
        float y = playerWorldPos.z * scaleY + offsetY;

        // 미니맵 좌표를 설정
        playerIcon.anchoredPosition = new Vector2(x, y);
    }




    private void UpdateOtherPlayerIconsPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int index = 0;
        foreach (GameObject player in players)
        {
            if (player.transform != playerTransform && index < otherPlayerIcons.Length)
            {
                // 플레이어의 월드 좌표를 가져옵니다.
                Vector3 playerWorldPos = player.transform.localPosition;

                // 월드 좌표를 미니맵 좌표로 변환합니다.
                float x = playerWorldPos.x * scaleX + offsetX;
                float y = playerWorldPos.z * scaleY + offsetY;

                // 미니맵 좌표를 설정합니다.
                otherPlayerIcons[index].anchoredPosition = new Vector2(x, y);

                index++;
            }
        }
    }





}
