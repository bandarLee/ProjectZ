using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class UI_Info : MonoBehaviour
{
    public GameObject InfoObject;
    public Canvas canvas;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI[] Stat;

    public GameObject[] ProfileImages;

    public void AssignCharacter(GameObject Character)
    {
        foreach(GameObject profile in ProfileImages)
        {
            profile.SetActive(false);
        }
        InfoObject = Character;

        if (InfoObject.CompareTag("Monster"))
        {
            Name.text = "Monster";
            Stat detectstat = null;
            if (InfoObject.TryGetComponent<Monster_Bat>(out Monster_Bat monsterBat))
            {
                detectstat = monsterBat.stat;
                ProfileImages[2].SetActive(true);
            }
            else if (InfoObject.TryGetComponent<Monster_Lev>(out Monster_Lev monsterLev))
            {
                detectstat = monsterLev.stat;
                ProfileImages[1].SetActive(true);

            }
            else if (InfoObject.TryGetComponent<Monster_Final>(out Monster_Final monsterFinal))
            {
                detectstat = monsterFinal.stat;
                ProfileImages[2].SetActive(true);

            }

            if (detectstat != null)
            {
                Stat[0].text = $"생명력 : {detectstat.Health} / {detectstat.MaxHealth}";
                Stat[1].text = $"공격 : {detectstat.Damage}   민첩력 : {detectstat.MoveSpeed}";
                Stat[2].text = $"공격범위 : {detectstat.attackRange}";
                Stat[3].text = $"감지범위 : {detectstat.detectRange}";
            }
            else
            {
                Stat[0].text = "???";
                Stat[1].text = "";
                Stat[2].text = "";
                Stat[3].text = "";
            }
        }
        else if (InfoObject.CompareTag("Player"))
        {
            Player photonPlayer = InfoObject.GetComponent<PhotonView>().Owner;
            Stat detectstat = InfoObject.GetComponent<Character>().Stat;
            ProfileImages[0].SetActive(true);

            Name.text = photonPlayer.NickName;
            Stat[0].text = $"생명력 : {detectstat.Health} / {detectstat.MaxHealth}";
            Stat[1].text = $"배고픔 : {detectstat.Hunger} / {detectstat.MaxHunger}";
            Stat[2].text = $"정신력 : {detectstat.Mental} / {detectstat.MaxMental}";
            Stat[3].text = $"공격 : {detectstat.Damage}   속도 : {detectstat.MoveSpeed}";

        }

    }
    void Update()
    {
        if (InfoObject != null)
        {
            Camera currentCamera = Camera.main;
            Vector3 screenPos = currentCamera.WorldToScreenPoint(InfoObject.transform.position);

            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out anchoredPos);
            RectTransform uiTransform = GetComponent<RectTransform>();
            if (uiTransform != null)
            {
                uiTransform.anchoredPosition = anchoredPos + new Vector2(200, 100);
            }
        }
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
}
