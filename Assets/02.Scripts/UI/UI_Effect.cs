using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Effect : MonoBehaviour
{
    public Image DamageEffectImage;
    public Image HotEffectImage;
    public Image ColdEffectImage;
    public float flashSpeed = 2.5f;

    public Color DamageColor = RGB(67, 0, 0, 208);
    public Color HotColor = RGB(253, 80, 0, 99);
    public Color ColdColor = RGB(28, 32, 171, 125);

    private void Start()
    {
        DamageEffectImage.gameObject.SetActive(false);
        HotEffectImage.gameObject.SetActive(false);
        ColdEffectImage.gameObject.SetActive(false);
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

    private static Color RGB(float r, float g, float b, float a = 255)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
