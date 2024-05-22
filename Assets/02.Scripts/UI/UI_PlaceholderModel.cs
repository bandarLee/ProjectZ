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
    public static CharacterType SelectedCharacterType = CharacterType.Female;

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

    // ������ ȭ��ǥ ��ư ������ �� enum �ϳ� ū ���� ȣ��Ǵ� �Լ�
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

    // �游��� ��ư�� ������ �� ȣ��Ǵ� �Լ�
    public void OnClickMakeRoomButton()
    {
        string roomID = "11";


        // [�� �ɼ� ����]
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // ���� ������ �ִ� �÷��̾� ��
        roomOptions.IsVisible = true;   // �κ񿡼� �� ��Ͽ� ������ ���ΰ�?
        roomOptions.IsOpen = true;      // �濡 �ٸ� �÷��̾ ���� �� �ִ°�?
        roomOptions.EmptyRoomTtl = 1000 * 20; // ��� �ִ� ���� �󸶳� Ÿ�� �� ����(Time to live, TTL): ��ǻ�ͳ� ��Ʈ��ũ���� �������� ��ȿ �Ⱓ�� ��Ÿ���� ���� ���
        
        // �κ񿡼� ���������� ǥ�õ� �� Ŀ���� ������Ƽ�� Ű�� ����
        // -> ���� �˻��ϰų� ������ �� ����ڿ��� ������ ������ �����ϱ� ���� ���
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "MasterNickname" };

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default); // ���� �ִٸ� �����ϰ� ���ٸ� ����� ��
    }
}
