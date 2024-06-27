using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatText : MonoBehaviour
{
    public TextMeshProUGUI StatTextUI;

    private Stat _stat;

    private void Start()
    {
        _stat = Character.LocalPlayerInstance.Stat;
        //UpdateUI(); 
    }

    
}
