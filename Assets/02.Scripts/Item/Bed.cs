using System.Collections;
using TMPro;
using UnityEngine;

public class Bed : MonoBehaviour
{
    public TextMeshProUGUI UseBedText;
    private CharacterStatAbility playerStatAbility;
    private bool isPlayerInRange = false;
    private bool isUsingBed = false;

    private void Start()
    {
        UseBedText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UseBedText.gameObject.SetActive(true);
            playerStatAbility = other.GetComponent<CharacterStatAbility>();
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UseBedText.gameObject.SetActive(false);
            isPlayerInRange = false;
            playerStatAbility = null;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isUsingBed)
        {
            StartCoroutine(UseBedRoutine());
        }
    }

    private IEnumerator UseBedRoutine()
    {
        isUsingBed = true;
        float useTime = 20f; // 최대 사용 시간

        while (useTime > 0)
        {
            if (playerStatAbility == null || playerStatAbility.State == State.Death)
            {
                break;
            }

            playerStatAbility.Stat.Health += 5;
            playerStatAbility.Stat.Mental += 3;
            playerStatAbility.LimitStat();

            useTime -= 1f;
            yield return new WaitForSeconds(1f);
        }

        isUsingBed = false;
        UseBedText.gameObject.SetActive(false); // 침대 사용 종료 후 텍스트 숨기기
    }
}
