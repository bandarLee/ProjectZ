using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Bat_Test : MonoBehaviour
{
    public float speed = 5f; // 몬스터의 이동 속도
    public float changeDirectionInterval = 2f; // 방향을 변경하는 간격
    public Vector3 areaSize = new Vector3(50f, 20f, 50f); // 몬스터가 날아다닐 영역의 크기

    private Vector3 targetPosition;

    void Start()
    {
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            SetRandomTargetPosition();
            yield return new WaitForSeconds(changeDirectionInterval);
        }
    }

    private void SetRandomTargetPosition()
    {
        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float y = Random.Range(0, areaSize.y);
        float z = Random.Range(-areaSize.z / 2, areaSize.z / 2);
        targetPosition = new Vector3(x, y, z);
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
