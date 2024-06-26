using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Effect : MonoBehaviour
{
    public Image DamageEffectImage;
    public Image TemperatureEffectImage;
    public float flashSpeed = 1f;

    public Color DamageColor = RGB(92, 3, 3, 208);
    public Color HotColor = RGB(250, 156, 79, 37);
    public Color VeryHotColor = RGB(255, 141, 47, 183);
    public Color ColdColor = RGB(28, 72, 171, 183);
    public Color VeryColdColor = RGB(28, 32, 171, 242);

    private void Start()
    {
        DamageEffectImage.gameObject.SetActive(false);
        TemperatureEffectImage.gameObject.SetActive(false);
    }

    public void ShowDamageEffect()
    {
        StartCoroutine(FlashDamageEffect(DamageEffectImage, DamageColor));
    }

    private IEnumerator FlashDamageEffect(Image effectImage, Color effectColor)
    {
            effectImage.gameObject.SetActive(true);
            effectImage.color = effectColor;
            yield return new WaitForSeconds(0.1f);

            while (effectImage.color.a > 0)
            {
                effectImage.color = Color.Lerp(DamageEffectImage.color, Color.clear, flashSpeed * Time.deltaTime); // 점점 투명하게
                yield return null;
            }
            effectImage.gameObject.SetActive(false);
    }

    public void ShowHotEffect()
    {
        StartCoroutine(ChangeTemperatureEffectColor(HotColor));
    }

    public void ShowVeryHotEffect()
    {
        StartCoroutine(ChangeTemperatureEffectColor(VeryHotColor));
    }

    public void ShowColdEffect()
    {
        StartCoroutine(ChangeTemperatureEffectColor(ColdColor));
    }

    public void ShowVeryColdEffect()
    {
        StartCoroutine(ChangeTemperatureEffectColor(VeryColdColor));
    }

    public void HideTemperatureEffects()
    {
        if (TemperatureEffectImage != null)
        {
            TemperatureEffectImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator ChangeTemperatureEffectColor(Color targetColor)
    {
        if (TemperatureEffectImage != null)
        {
            TemperatureEffectImage.gameObject.SetActive(true);

            Color initialColor = TemperatureEffectImage.color;
            float duration = 2.0f; 
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                TemperatureEffectImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            TemperatureEffectImage.color = targetColor;
        }
    }

    private static Color RGB(float r, float g, float b, float a = 255)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
