using DigitalRuby.WeatherMaker;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    private void Start()
    {
        dayNightCycleManager = WeatherMakerDayNightCycleManagerScript.Instance;
        gameTimeScript = FindObjectOfType<GameTime>();
        previousTimeType = gameTimeScript.CurrentTimeType;

        // 슬라이더 최대값 설정
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

    private void UpdateSunAndMoonVisibility() // 현재 시간을 기준으로 슬라이더와 UI를 업데이트
    {
        // 슬라이더 값을 현재 시간으로 설정 (하루의 중간 시간인 43200초를 기준으로)
        float timeOfDay = dayNightCycleManager.TimeOfDay - 31600;

        // 음수 값을 방지하고 0-86400 사이의 값으로 조정
        if (timeOfDay < 0)
        {
            timeOfDay += 86400f;
        }

        ClockSlider.value = timeOfDay % 86400f;

        // 현재 시간에 따라 gameTimeScript의 CurrentTimeType 업데이트
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

        // CurrentTimeType에 따라 UI 업데이트
        switch (gameTimeScript.CurrentTimeType)
        {
            case GameTime.TimeType.Day:
                Sun.gameObject.SetActive(true);
                Moon.gameObject.SetActive(false);
                Mystery.gameObject.SetActive(false);
                ClockSlider.fillRect.GetComponentInChildren<Image>().color = new Color32(255, 145, 0, 255); // 주황색
                break;

            case GameTime.TimeType.Night:
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(true);
                Mystery.gameObject.SetActive(false);
                ClockSlider.fillRect.GetComponentInChildren<Image>().color = new Color32(30, 100, 255, 255); // 파란색
                break;

            case GameTime.TimeType.Mystery:
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(false);
                Mystery.gameObject.SetActive(true);
                ClockSlider.fillRect.GetComponentInChildren<Image>().color = new Color32(174, 0, 128, 255); // 보라색
                break;
        }
    }

    [PunRPC]
    private void RequestTimeFromMaster() // 마스터 클라이언트에게 현재 시간을 요청
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTime", RpcTarget.Others, dayNightCycleManager.TimeOfDay, (int)gameTimeScript.CurrentTimeType);
        }
    }

    [PunRPC]
    private void SyncTime(float masterTimeOfDay, int masterTimeType) // 마스터 클라이언트에서 시간을 받아와 동기화
    {
        dayNightCycleManager.TimeOfDay = masterTimeOfDay;
        gameTimeScript.CurrentTimeType = (GameTime.TimeType)masterTimeType;
        UpdateSunAndMoonVisibility(); // 받은 시간으로 UI 업데이트
    }

    // 포톤의 IPunObservable 인터페이스 사용해 마스터 클라이언트가 현재 시간을 전송하고, 다른 클라이언트가 이것을 받아오게 함
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
            UpdateSunAndMoonVisibility(); // 받은 시간으로 UI 업데이트
        }
    }
}
