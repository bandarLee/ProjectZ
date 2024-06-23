using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeedRoomDoorOpenTrigger : MonoBehaviour
{
    public GameObject SeedRoomTrigger;
    public GameObject MCDoor;
    public float openAngle = 45f;
    public float duration = 1f;

    // 힌지(노란색 부분)를 기준으로 회전하는 부모 객체
    public GameObject hingePivot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Trigger the DoorTrigger");

            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        hingePivot.transform.DORotate(new Vector3(0, 0, openAngle), duration);
    }
}
