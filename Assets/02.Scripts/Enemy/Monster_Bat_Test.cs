using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Bat_Test : MonoBehaviour
{
    public float speed = 5f; // ������ �̵� �ӵ�
    public float changeDirectionInterval = 2f; // ������ �����ϴ� ����
    public Vector3 areaSize = new Vector3(50f, 20f, 50f); // ���Ͱ� ���ƴٴ� ������ ũ��

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
