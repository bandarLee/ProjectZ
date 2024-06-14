using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MicroWaveUI : MonoBehaviour
{
    public Slider ArrowImageSlider;

    private void Start()
    {
/*        StartCoroutine(FillSliderOverTime(5f)); */
    }

    private IEnumerator FillSliderOverTime(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            ArrowImageSlider.value = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
        ArrowImageSlider.value = 1f; 
    }
}
