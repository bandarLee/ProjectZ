using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSubwayTrigger : MonoBehaviour
{
    public GameTime gameTime;
    public GameObject SubwayEntrance;
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float speed = 2f; // 이동 속도

    void Start()
    {
        originalPosition = SubwayEntrance.transform.position; // 원래 위치 저장
        targetPosition = originalPosition + new Vector3(10, 0, 0); // 목표 위치 설정

        gameTime.OnTimeTypeChanged += HandleTimeTypeChanged; // 시간 타입 변경 이벤트 구독
    }

    void OnDestroy()
    {
        gameTime.OnTimeTypeChanged -= HandleTimeTypeChanged; // 이벤트 구독 해제
    }

    private void HandleTimeTypeChanged(GameTime.TimeType newTimeType)
    {
        if (newTimeType == GameTime.TimeType.Mystery)
        {
            if (!isMoving)
            {
                StartCoroutine(MoveSubwayEntrance(targetPosition)); // 목표 위치로 이동
            }
        }
        else
        {
            if (!isMoving)
            {
                StartCoroutine(MoveSubwayEntrance(originalPosition)); // 원래 위치로 되돌리기
            }
        }
    }

    private IEnumerator MoveSubwayEntrance(Vector3 target)
    {
        isMoving = true;
        while (Vector3.Distance(SubwayEntrance.transform.position, target) > 0.01f)
        {
            SubwayEntrance.transform.position = Vector3.MoveTowards(SubwayEntrance.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        SubwayEntrance.transform.position = target; // 최종 위치 고정
        isMoving = false;
    }
}
