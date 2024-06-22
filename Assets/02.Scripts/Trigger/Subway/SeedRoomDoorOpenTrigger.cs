using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeedRoomDoorOpenTrigger : MonoBehaviour
{
    public GameObject SeedRoomTrigger;
    public GameObject MCDoor;
    public float openAngle = 45f; // 열릴 각도
    public float duration = 1f;   // 애니메이션 지속 시간

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        // 문이 y축을 기준으로 45도 회전하도록 설정
        MCDoor.transform.DORotate(new Vector3(0, openAngle, 0), duration);
    }
}
