using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get; private set; }

    public Character MyCharacter;
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
        if (MyCharacter == null)
        {
            return;
        }

        HealthSliderUI.value = (float)MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
        MentalSliderUI.value = MyCharacter.Stat.Mental / MyCharacter.Stat.MaxMental;
        HungerSliderUI.value = MyCharacter.Stat.Hunger / MyCharacter.Stat.MaxHunger;

        HealthTextUI.text = $"{(int)(HealthSliderUI.value * 100)}%";
        MentalTextUI.text = $"{(int)(MentalSliderUI.value * 100)}%";
        HungerTextUI.text = $"{(int)(HungerSliderUI.value * 100)}%";
    }
}
