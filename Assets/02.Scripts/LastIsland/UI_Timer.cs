using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UI_Timer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI TimerTextUI;
    public Color TimerTextColor;
    private int _totalTime = 0;
    public PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        TimerTextUI.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TimerTextUI.gameObject.SetActive(true);
            _totalTime = 100;
            StartCoroutine(Timer_Coroutine());
        }
    }

    private IEnumerator Timer_Coroutine()
    {
        var wait = new WaitForSeconds(1f);

        while (true)
        {
            yield return wait;
            if (_totalTime == 30)
            {
                PV.RPC("AnimationPlay", RpcTarget.All);
            }
            if (_totalTime > 0)
            {
                _totalTime -= 1;
                PV.RPC("ShowTimer", RpcTarget.All, _totalTime); //1초 마다 방 모두에게 전달
            }
            if (_totalTime <= 0)
            {
                PV.RPC("ShowTimer", RpcTarget.All, _totalTime); //1초 마다 방 모두에게 전달
                PV.RPC("TimerEnded", RpcTarget.All);
                break;
            }
        }
    }

    [PunRPC]
    void ShowTimer(int number)
    {
        int minutes = number / 60;
        int seconds = number % 60;
        TimerTextUI.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    // 타이머 끝 => 엔딩
    [PunRPC]
    void TimerEnded()
    {
        Debug.Log("TimerEnded 함수 실행");
    }
}
