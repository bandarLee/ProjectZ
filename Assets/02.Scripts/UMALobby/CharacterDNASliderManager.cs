using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.UI;
using UMA;

public class CharacterDNASliderManager : MonoBehaviour
{
    public DynamicCharacterAvatar Avatar;
    public Slider WeightSlider;
    public Slider MuscleSlider;

    public void ChangeAvatar(DynamicCharacterAvatar avatar)
    {
        Avatar = avatar;
        SetInitialSliderValues();

        var dna = Avatar.GetAllDNA();
        Debug.Log(dna.ToString());
    }
    void SetInitialSliderValues()
    {
        if (Avatar != null)
        {
            var umaDna = Avatar.GetDNA();

            if (umaDna.ContainsKey("upperWeight"))
                WeightSlider.value = umaDna["upperWeight"].Get();
            else
            {
                Debug.Log("NoWeight");
            }

            if (umaDna.ContainsKey("upperMuscle"))
                MuscleSlider.value = umaDna["upperMuscle"].Get();
            else
            {
                Debug.Log("NoWeight");
            }
        }
    }
    public void OnUpperWeightSliderChanged()
    {
        float value = WeightSlider.value;
        
        var umaDna = Avatar.GetDNA();
        umaDna["upperWeight"].Set(value);
        umaDna["lowerWeight"].Set(value);
        Avatar.BuildCharacter();

    }
    public void OnUpperMuscleSliderChanged()
    {
        float value = MuscleSlider.value;

        var umaDna = Avatar.GetDNA();
        umaDna["upperMuscle"].Set(value);
        umaDna["lowerMuscle"].Set(value);
        Avatar.BuildCharacter();

    }
    void Update()
    {
        
    }
}
