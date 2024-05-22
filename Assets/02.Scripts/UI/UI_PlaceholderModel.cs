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

    // ���� ȭ��ǥ ��ư ������ �� enum �ϳ� ���� ���� ȣ��Ǵ� �Լ�
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
