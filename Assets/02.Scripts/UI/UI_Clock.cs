using DigitalRuby.WeatherMaker;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon; // ExitGames.Client.Photon.Hashtable�� ����ϱ� ���� �߰�

public class UI_Clock : MonoBehaviourPun, IPunObservable
{
    public GameTime GameTime;
    public Slider ClockSlider;
    public Image Sun;
    public Image Moon;
    public Image Mystery;
    public GoToSubwayTrigger gotoSubwayTrigger;

    private WeatherMakerDayNightCycleManagerScript dayNightCycleManager;
    private GameTime gameTimeScript;
    private GameTime.TimeType previousTimeType;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // �� �ڵ� ����ȭ ����
    }

    private void OnEnable()
    {
        // �� ��ȯ �� �ð��� �ε��մϴ�.
        LoadTime();
    }

    private void Start()
    {
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;
        gameTimeScript = FindObjectOfType<GameTime>();
        previousTimeType = gameTimeScript.CurrentTimeType;

        // �����̴� �ִ밪 ����
        ClockSlider.maxValue = 86400f;

        Sun.rectTransform.anchoredPosition = Vector2.zero;
        Moon.rectTransform.anchoredPosition = Vector2.zero;

        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RequestTimeFromMaster", RpcTarget.MasterClient);
        }
        else
        {
            // ������ Ŭ���̾�Ʈ�� �ƴ� ��� ������ �� �ð��� �����մϴ�.
            LoadTime();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ������ Ŭ���̾�Ʈ���� �ð��� ������Ʈ
            dayNightCycleManager.TimeOfDay += Time.deltaTime;
            UpdateSunAndMoonVisibility();
        }
    }

    private void UpdateSunAndMoonVisibility() // ���� �ð��� �������� �����̴��� UI�� ������Ʈ
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

        if (gameTimeScript.CurrentTimeType != previousTimeType)
        {
            previousTimeType = gameTimeScript.CurrentTimeType;
            gotoSubwayTrigger.ManageSubwayEntrance(gameTimeScript.CurrentTimeType);
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
    private void RequestTimeFromMaster() // ������ Ŭ���̾�Ʈ���� ���� �ð��� ��û
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTime", RpcTarget.Others, dayNightCycleManager.TimeOfDay, (int)gameTimeScript.CurrentTimeType);
        }
    }

    [PunRPC]
    private void SyncTime(float masterTimeOfDay, int masterTimeType) // ������ Ŭ���̾�Ʈ���� �ð��� �޾ƿ� ����ȭ
    {
        dayNightCycleManager.TimeOfDay = masterTimeOfDay;
        gameTimeScript.CurrentTimeType = (GameTime.TimeType)masterTimeType;
        UpdateSunAndMoonVisibility(); // ���� �ð����� UI ������Ʈ
    }

    // ������ IPunObservable �������̽� ����� ������ Ŭ���̾�Ʈ�� ���� �ð��� �����ϰ�, �ٸ� Ŭ���̾�Ʈ�� �̰��� �޾ƿ��� ��
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)
        {
            stream.SendNext(dayNightCycleManager.TimeOfDay);
            stream.SendNext((int)gameTimeScript.CurrentTimeType);
        }
        else if (stream.IsReading)
        {
            dayNightCycleManager.TimeOfDay = (float)stream.ReceiveNext();
            gameTimeScript.CurrentTimeType = (GameTime.TimeType)stream.ReceiveNext();
            UpdateSunAndMoonVisibility(); // ���� �ð����� UI ������Ʈ
        }
    }

    // �ð� ������ �����ϴ� �޼���
    private void SaveTime()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "TimeOfDay", dayNightCycleManager.TimeOfDay },
            { "TimeType", (int)gameTimeScript.CurrentTimeType }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // �ð� ������ �ε��ϴ� �޼���
    private void LoadTime()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("TimeOfDay", out object timeOfDay) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("TimeType", out object timeType))
        {
            dayNightCycleManager.TimeOfDay = (float)timeOfDay;
            gameTimeScript.CurrentTimeType = (GameTime.TimeType)(int)timeType;
            UpdateSunAndMoonVisibility();
        }
    }

    private void OnDisable()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SaveTime();
        }
    }
}
