using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatText : MonoBehaviour
{
    public static UI_StatText Instance { get; private set; }

    public TextMeshProUGUI StatTextUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StatTextUpdate();
    }
    public void StatTextUpdate()
    {
        if (Character.LocalPlayerInstance == null || Character.LocalPlayerInstance.Stat == null)
        {
            Debug.LogError("Character or Stat is null");
            return;
        }
        string statMessage = "";
        statMessage += $"체력: {Character.LocalPlayerInstance.Stat.MaxHealth}\n\n";
        statMessage += $"공격력: {Character.LocalPlayerInstance.Stat.Damage}\n\n";
        statMessage += $"점프력: {Character.LocalPlayerInstance.Stat.JumpPower}\n\n";
        statMessage += $"이동속도: {Character.LocalPlayerInstance.Stat.MoveSpeed}\n\n\n";

        statMessage += $"정신력: {Character.LocalPlayerInstance.Stat.MaxMental}\n\n";
        statMessage += $"포만감: {Character.LocalPlayerInstance.Stat.MaxHunger}\n\n";
        statMessage += $"체감온도: {Character.LocalPlayerInstance.Stat.Temperature}\n\n";
        statMessage += $"공격쿨타임: {Character.LocalPlayerInstance.Stat.AttackCoolTime}\n\n";
        statMessage += $"회전속도: {Character.LocalPlayerInstance.Stat.RotationSpeed}\n\n";

        StatTextUI.text = statMessage.TrimEnd();
    }

    
}
