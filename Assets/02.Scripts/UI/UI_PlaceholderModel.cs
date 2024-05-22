using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterType
{
    Female, // 0
    Male, // 1

}

public class UI_PlaceholderModel : MonoBehaviour
{
    public  CharacterType SelectedCharacterType = CharacterType.Female;

    public GameObject FemaleCharacter;
    public GameObject MaleCharacter;

    private void Start()
    {
        UpdateCharacterVisibility();
    }

    // 왼쪽 화살표 버튼 눌렀을 때 enum 하나 작은 수가 호출되는 함수
    public void OnClickLeftButton()
    {
        SelectedCharacterType = (CharacterType)(((int)SelectedCharacterType - 1 + System.Enum.GetValues(typeof(CharacterType)).Length) % System.Enum.GetValues(typeof(CharacterType)).Length);
        UpdateCharacterVisibility();
    }

    public void OnClickRightButton()
    {
        SelectedCharacterType = (CharacterType)(((int)SelectedCharacterType + 1) % System.Enum.GetValues(typeof(CharacterType)).Length);
        UpdateCharacterVisibility();
    }

    private void UpdateCharacterVisibility()
    {
        FemaleCharacter.SetActive(SelectedCharacterType == CharacterType.Female);
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
    }

   
}
