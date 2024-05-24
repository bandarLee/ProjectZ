using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get; private set; }

    public CharacterStatAbility MyCharacterAbility;

    public Slider HealthSliderUI;
    public Slider MentalSliderUI;
    public Slider HungerSliderUI;

    public TextMeshProUGUI HealthTextUI;
    public TextMeshProUGUI MentalTextUI;
    public TextMeshProUGUI HungerTextUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (MyCharacterAbility == null)
        {
            return;
        }

        UpdateUI(MyCharacterAbility.Stat);

    }

    private void UpdateUI(Stat stat)
    {
        HealthSliderUI.value = (float)stat.Health / (float)stat.MaxHealth;
        MentalSliderUI.value = (float)stat.Mental / (float)stat.MaxMental;
        HungerSliderUI.value = (float)stat.Hunger / (float)stat.MaxHunger;
        Debug.Log(HungerSliderUI.value);

        HealthTextUI.text = $"{(int)(HealthSliderUI.value * 100)}%";
        MentalTextUI.text = $"{(int)(MentalSliderUI.value * 100)}%";
        HungerTextUI.text = $"{(int)(HungerSliderUI.value * 100)}%";
    }
}
