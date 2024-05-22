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

    // 왼쪽 화살표 버튼 눌렀을 때 enum 하나 작은 수가 호출되는 함수
    public void OnClickLeftButton()
    {
        SelectedCharacterType = (CharacterType)(((int)SelectedCharacterType - 1 + System.Enum.GetValues(typeof(CharacterType)).Length) % System.Enum.GetValues(typeof(CharacterType)).Length);
        UpdateCharacterVisibility();
    }

    // 오른쪽 화살표 버튼 눌렀을 때 enum 하나 큰 수가 호출되는 함수
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

    // 방만들기 버튼을 눌렀을 때 호출되는 함수
    public void OnClickMakeRoomButton()
    {
        string roomID = "11";


        // [룸 옵션 설정]
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;    // 입장 가능한 최대 플레이어 수
        roomOptions.IsVisible = true;   // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;      // 방에 다른 플레이어가 들어올 수 있는가?
        roomOptions.EmptyRoomTtl = 1000 * 20; // 비어 있는 방이 얼마나 타임 투 리브(Time to live, TTL): 컴퓨터나 네트워크에서 데이터의 유효 기간을 나타내기 위한 방법
        
        // 로비에서 공개적으로 표시될 룸 커스텀 프로퍼티의 키를 정의
        // -> 방을 검색하거나 선택할 때 사용자에게 유용한 정보를 제공하기 위해 사용
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "MasterNickname" };

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default); // 방이 있다면 입장하고 없다면 만드는 것
    }
}
