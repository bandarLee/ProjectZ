using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Bed : MonoBehaviour
{
    public TextMeshProUGUI UseBedText;
    public TextMeshProUGUI StopUsingBedText;
    public TextMeshProUGUI CantUseBedText;
    public TextMeshProUGUI AllRecoveriesText;

    public Slider UsingTimeSlider;

    private bool isPlayerInRange = false;
    private bool isUsingBed = false;
    private float useTime = 20f;
    private Coroutine useBedCoroutine;

    private void Start()
    {
        UsingTimeSlider.gameObject.SetActive(false);
        UseBedText.gameObject.SetActive(false);
        StopUsingBedText.gameObject.SetActive(false);
        CantUseBedText.gameObject.SetActive(false);
        AllRecoveriesText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Character>().PhotonView.IsMine)
        {
            Character character = other.GetComponent<Character>();
            if (character.Stat.Health >= character.Stat.MaxHealth && character.Stat.Mental >= character.Stat.MaxMental)
            {
                StartCoroutine(ShowCantUseBedText());
            }
            else
            {
                UseBedText.gameObject.SetActive(true);
                isPlayerInRange = true;
                
                Character.LocalPlayerInstance._animator.SetBool("DoSleep", true);
            }
        }
    }

    private IEnumerator ShowCantUseBedText()
    {
        CantUseBedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        CantUseBedText.gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UseBedText.gameObject.SetActive(false);
            isPlayerInRange = false;
            
            Character.LocalPlayerInstance._animator.SetBool("DoSleep", false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isUsingBed)
        {
            useBedCoroutine = StartCoroutine(UseBed());
        }
        else if (isUsingBed && Input.GetKeyDown(KeyCode.E))
        {
            StopUsingBed();
        }
    }
    private void StopUsingBed()
    {
        if (useBedCoroutine != null)
        {
            StopCoroutine(useBedCoroutine);
            useBedCoroutine = null;
        }

        isUsingBed = false;
        StopUsingBedText.gameObject.SetActive(false);
        UsingTimeSlider.gameObject.SetActive(false);
    }

    private IEnumerator UseBed()
    {
        isUsingBed = true;
        UseBedText.gameObject.SetActive(false);
        StopUsingBedText.gameObject.SetActive(true);
        UsingTimeSlider.gameObject.SetActive(true);
        UsingTimeSlider.maxValue = useTime;
        UsingTimeSlider.value = 0;

        float elapsedTime = 0;
        Character character = Character.LocalPlayerInstance;

        while (elapsedTime < useTime)
        {
            elapsedTime += Time.deltaTime;
            UsingTimeSlider.value = elapsedTime;

            if (elapsedTime % 1f < Time.deltaTime)
            {
                character.Stat.Health = Mathf.Min(character.Stat.Health + 5, character.Stat.MaxHealth);
                character.Stat.Mental = Mathf.Min(character.Stat.Mental + 3, character.Stat.MaxMental);
            }
            if (character.Stat.Health >= character.Stat.MaxHealth && character.Stat.Mental >= character.Stat.MaxMental)
            {
                StartCoroutine(ShowAllRecoveriesText());
                StopUsingBed();
                yield break;
            }
            yield return null;
        }

        StopUsingBedText.gameObject.SetActive(false);
        UsingTimeSlider.gameObject.SetActive(false);
        isUsingBed = false;
    }

    private IEnumerator ShowAllRecoveriesText()
    {
        AllRecoveriesText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        AllRecoveriesText.gameObject.SetActive(false);
    }
}
