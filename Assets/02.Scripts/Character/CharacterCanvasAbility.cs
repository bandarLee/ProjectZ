using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterCanvasAbility : CharacterAbility
{
    public Canvas MyCanvas;
    public TextMeshProUGUI NicknameTextUI;


    private void Start()
    {
        NicknameTextUI.text = Owner.PhotonView.Controller.NickName;
    }

    private void Update()
    {
        // ºôº¸µå ±¸Çö
        MyCanvas.transform.forward = Camera.main.transform.forward;

    }
}
