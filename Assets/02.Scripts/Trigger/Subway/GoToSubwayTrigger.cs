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
    private float speed = 2f; // �̵� �ӵ�

    void Start()
    {
        originalPosition = SubwayEntrance.transform.position; // ���� ��ġ ����
        targetPosition = originalPosition + new Vector3(10, 0, 0); // ��ǥ ��ġ ����

        gameTime.OnTimeTypeChanged += HandleTimeTypeChanged; // �ð� Ÿ�� ���� �̺�Ʈ ����
    }

    void OnDestroy()
    {
        gameTime.OnTimeTypeChanged -= HandleTimeTypeChanged; // �̺�Ʈ ���� ����
    }

    private void HandleTimeTypeChanged(GameTime.TimeType newTimeType)
    {
        if (newTimeType == GameTime.TimeType.Mystery)
        {
            if (!isMoving)
            {
                StartCoroutine(MoveSubwayEntrance(targetPosition)); // ��ǥ ��ġ�� �̵�
            }
        }
        else
        {
            if (!isMoving)
            {
                StartCoroutine(MoveSubwayEntrance(originalPosition)); // ���� ��ġ�� �ǵ�����
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
        SubwayEntrance.transform.position = target; // ���� ��ġ ����
        isMoving = false;
    }
}
