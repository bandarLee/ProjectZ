using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위해 추가
using DG.Tweening; // DOTween을 사용하기 위해 추가

public class GoToLastIslandTrigger : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI GotoLastIslandText;
    public Image FadeImage; 
    private bool isPlayerInTrigger = false;
    public PhotonView pv;
    private void Start()
    {
        GotoLastIslandText.gameObject.SetActive(false);
        FadeImage.gameObject.SetActive(false);
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Character>().PhotonView.IsMine)
        {
            Debug.Log("Player triggered GoToLastIslandTrigger");
            isPlayerInTrigger = true;
            GotoLastIslandText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            GotoLastIslandText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        FadeImage.gameObject.SetActive(true); 
        FadeImage.color = new Color(0, 0, 0, 0); // 초기 색상 투명
        Debug.Log("1");

        FadeImage.DOFade(1, 1.5f);
        Debug.Log("1");
        yield return new WaitForSeconds(1.5f);
        Debug.Log("1");
        LoadLastIslandScene();
    }

    private void LoadLastIslandScene()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LastIsLandScene");
        }
        else
        {
            pv.RPC("RequestLoadLevel", RpcTarget.MasterClient, "LastIsLandScene");
        }
        Debug.Log("4");
    }

    [PunRPC]
    private void RequestLoadLevel(string levelName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(levelName);
        }
    }
}
