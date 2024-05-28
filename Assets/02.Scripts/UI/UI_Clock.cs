using DigitalRuby.WeatherMaker;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Clock : MonoBehaviourPun, IPunObservable
{
    public Slider ClockSlider;
    public Image Sun;
    public Image Moon;
    public Image Mystery;

    private WeatherMakerDayNightCycleManagerScript dayNightCycleManager;
    private GameTime gameTimeScript;

    private void Start()
    {
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;
        gameTimeScript = FindObjectOfType<GameTime>();

        // �����̴� �ִ밪
        ClockSlider.maxValue = 86400f;

        Sun.rectTransform.anchoredPosition = Vector2.zero;
        Moon.rectTransform.anchoredPosition = Vector2.zero;

        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RequestTimeFromMaster", RpcTarget.MasterClient);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateSunAndMoonVisibility();
        }
    }

    private void UpdateSunAndMoonVisibility()           // ���� �ð��� �������� �����̴��� UI�� ������Ʈ
    {
        // �����̴� ���� ���� �ð����� ���� (�Ϸ��� �߰� �ð��� 43200�ʸ� ��������)
        float timeOfDay = dayNightCycleManager.TimeOfDay - 31600;

        // ���� ���� �����ϰ� 0-86400 ������ ������ ����
        if (timeOfDay < 0)
        {
            timeOfDay += 86400f;
        }

        ClockSlider.value = timeOfDay % 86400f;

        // ���� �ð��� ���� gameTimeScript�� CurrentTimeType ������Ʈ
        if (dayNightCycleManager.TimeOfDay >= 42240f && dayNightCycleManager.TimeOfDay < 44160f)
        {
            gameTimeScript.CurrentTimeType = GameTime.TimeType.Mystery;
        }
        else if (dayNightCycleManager.TimeOfDayCategory.HasFlag(WeatherMakerTimeOfDayCategory.Day))
        {
            gameTimeScript.CurrentTimeType = GameTime.TimeType.Day;
        }
        else
        {
            gameTimeScript.CurrentTimeType = GameTime.TimeType.Night;
        }

        // CurrentTimeType�� ���� UI ������Ʈ
        switch (gameTimeScript.CurrentTimeType)
        {
            case GameTime.TimeType.Day:
                Sun.gameObject.SetActive(true);
                Moon.gameObject.SetActive(false);
                Mystery.gameObject.SetActive(false);
                ClockSlider.fillRect.GetComponentInChildren<Image>().color = new Color32(255, 145, 0, 255); // ��Ȳ��
                break;

            case GameTime.TimeType.Night:
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(true);
                Mystery.gameObject.SetActive(false);
                ClockSlider.fillRect.GetComponentInChildren<Image>().color = new Color32(30, 100, 255, 255); // �Ķ���
                break;

            case GameTime.TimeType.Mystery:
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(false);
                Mystery.gameObject.SetActive(true);
                ClockSlider.fillRect.GetComponentInChildren<Image>().color = new Color32(174, 0, 128, 255); // �����
                break;
        }
    }

    [PunRPC]
    private void RequestTimeFromMaster()        // ������ Ŭ���̾�Ʈ���� ���� �ð��� ��û
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTime", RpcTarget.Others, dayNightCycleManager.TimeOfDay);
        }
    }

    [PunRPC]
    private void SyncTime(float masterTimeOfDay)        // ������ Ŭ���̾�Ʈ���� �ð��� �޾ƿ� ����ȭ
    {
        dayNightCycleManager.TimeOfDay = masterTimeOfDay;
    }

    // ������ IPunObservable �������̽� ����� ������ Ŭ���̾�Ʈ�� ���� �ð��� �����ϰ�, �ٸ� Ŭ���̾�Ʈ�� �̰��� �޾ƿ��� ��
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)     
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)
        {
            stream.SendNext(dayNightCycleManager.TimeOfDay);
        }
        else if (stream.IsReading)
        {
            dayNightCycleManager.TimeOfDay = (float)stream.ReceiveNext();
        }
    }
}
