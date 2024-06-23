using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using static UMA.UMAData;

public class CharacterInitialization : MonoBehaviour
{
    private UMAData umaData;

    private void Start()
    {
        umaData = GetComponent<UMAData>();
        InitializeCharacter();
    }

    public void InitializeCharacter()
    {
        if (umaData != null)
        {
            var umaRecipe = umaData.umaRecipe;
            if (umaRecipe == null)
            {
                umaRecipe = new UMARecipe();
                umaData.umaRecipe = umaRecipe;
            }

            if (umaRecipe.GetDna<UMADnaHumanoid>() == null)
            {
                umaRecipe.AddDna(new UMADnaHumanoid());
                Debug.Log("ÈÞ¸Ó³ë");
            }

            umaData.Dirty();
        }
    }
}
