using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoToSubwayTrigger : MonoBehaviour
{
    public GameTime gameTime;
    public GameObject SubwayEntrance;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        originalPosition = SubwayEntrance.transform.position; // 원래 위치 저장
        targetPosition = originalPosition + new Vector3(0, 0, 5); // 목표 위치 설정
        ManageSubwayEntrance(GameTime.TimeType.Mystery);

    }


    public void ManageSubwayEntrance(GameTime.TimeType newTimeType)
    {
        if (newTimeType == GameTime.TimeType.Mystery)
        {
            this.gameObject.transform.DOMove(targetPosition, 2f);
        }
        else
        {
            this.gameObject.transform.DOMove(originalPosition, 2f);
        }
    }
}
