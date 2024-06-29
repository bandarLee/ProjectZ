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
        statMessage += $"ü��: {Character.LocalPlayerInstance.Stat.MaxHealth}\n\n";
        statMessage += $"���ݷ�: {Character.LocalPlayerInstance.Stat.Damage}\n\n";
        statMessage += $"������: {Character.LocalPlayerInstance.Stat.JumpPower}\n\n";
        statMessage += $"�̵��ӵ�: {Character.LocalPlayerInstance.Stat.MoveSpeed}\n\n\n";

        statMessage += $"���ŷ�: {Character.LocalPlayerInstance.Stat.MaxMental}\n\n";
        statMessage += $"������: {Character.LocalPlayerInstance.Stat.MaxHunger}\n\n";
        statMessage += $"ü���µ�: {Character.LocalPlayerInstance.Stat.Temperature}\n\n";
        statMessage += $"������Ÿ��: {Character.LocalPlayerInstance.Stat.AttackCoolTime}\n\n";
        statMessage += $"ȸ���ӵ�: {Character.LocalPlayerInstance.Stat.RotationSpeed}\n\n";

        StatTextUI.text = statMessage.TrimEnd();
    }

    
}
