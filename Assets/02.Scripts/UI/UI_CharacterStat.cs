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

    public TextMeshProUGUI WarningText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (MyCharacterAbility == null || MyCharacterAbility.Stat == null)
        {
            return;
        }

        UpdateUI(MyCharacterAbility.Stat);
        UpdateWarnings(MyCharacterAbility.Stat);
    }

    private void UpdateUI(Stat stat)
    {
        HealthSliderUI.value = (float)stat.Health / (float)stat.MaxHealth;
        MentalSliderUI.value = (float)stat.Mental / (float)stat.MaxMental;
        HungerSliderUI.value = (float)stat.Hunger / (float)stat.MaxHunger;

        HealthTextUI.text = $"{(int)(HealthSliderUI.value * 100)}%";
        MentalTextUI.text = $"{(int)(MentalSliderUI.value * 100)}%";
        HungerTextUI.text = $"{(int)(HungerSliderUI.value * 100)}%";
    }

    private void UpdateWarnings(Stat stat)
    {
        string warningMessage = "";

        if (stat.Health <= 20)
        {
            warningMessage += "체력 부족\n";
        }

        if (stat.Mental <= 20)
        {
            warningMessage += "정신력 부족\n";
        }

        if (stat.Hunger <= 20)
        {
            warningMessage += "공복\n";
        }

        WarningText.text = warningMessage.TrimEnd(); 
    }

    public void SetMental(int mental)
    {
        if (MyCharacterAbility != null && MyCharacterAbility.Stat != null)
        {
            MyCharacterAbility.Stat.Mental = mental;
            UpdateUI(MyCharacterAbility.Stat); 
        }
    }
}
