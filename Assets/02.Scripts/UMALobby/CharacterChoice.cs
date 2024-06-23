using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;

public class CharacterChoice : MonoBehaviour
{
    public GameObject[] Avatars;
    public DynamicCharacterAvatar ChoiceAvatar;
    public CharacterDNASliderManager CharacterDNASliderManager;
    public Camera Camera;
    public Vector3 CameraOriginalPosition;
    public int Cameraindex;

    public bool Male = false;
    public bool Ver1 = true;
    public bool Adult = true;

    public LobbyManager LobbyManager;

    void Start()
    {
        CameraOriginalPosition = Camera.gameObject.transform.position;
        Cameraindex = 4;
        ChoiceAvatar = Avatars[Cameraindex].GetComponent<DynamicCharacterAvatar>();
        LobbyManager.characterAvatar = ChoiceAvatar;
        CharacterDNASliderManager.Avatar = ChoiceAvatar;
        CharacterDNASliderManager.ChangeAvatar(ChoiceAvatar);

    }
    public void SetGender(bool isMale)
    {
        Male = isMale;
        ChangeCameraIndex();
    }

    public void SetVersion(bool isVer1)
    {
        Ver1 = isVer1;
        ChangeCameraIndex();
    }

    public void SetAge(bool isAdult)
    {
        Adult = isAdult;
        ChangeCameraIndex();
    }
    public void ChangeCameraIndex()
    {
        int newIndex = GetAvatarIndex(Male, Ver1, Adult);
        if (newIndex >= 0 && newIndex < Avatars.Length)
        {
            Cameraindex = newIndex;
            ChoiceAvatar = Avatars[Cameraindex].GetComponent<DynamicCharacterAvatar>();
            LobbyManager.characterAvatar = ChoiceAvatar;
            CharacterDNASliderManager.ChangeAvatar(ChoiceAvatar);

            Camera.transform.localPosition = new Vector3(0, Cameraindex * 10, 0);
        }
    }

    private int GetAvatarIndex(bool male, bool ver1, bool adult)
    {
        if (male)
        {
            if (adult)
                return ver1 ? 0 : 1;
            else
                return ver1 ? 2 : 3;
        }
        else
        {
            if (adult)
                return ver1 ? 4 : 5;
            else
                return ver1 ? 6 : 7;
        }
    }
}
