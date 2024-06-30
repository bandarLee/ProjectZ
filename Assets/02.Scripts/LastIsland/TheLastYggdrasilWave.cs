using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.UI.GridLayoutGroup;

public class TheLastYggdrasilWave : MonoBehaviour
{
    public int Health = 10;
    public Slider TheLastYggdrasilHPBar;


    PhotonView PV;


    private void Start()
    {
        PV = GetComponent<PhotonView>();

        // Slider �ʱ� ����
        if (TheLastYggdrasilHPBar != null)
        {
            TheLastYggdrasilHPBar.maxValue = Health; 
            TheLastYggdrasilHPBar.value = Health;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine == false || other.transform == transform)
        {
            return;
        }
        // ���� ��� ��Ģ + �������̽� // �������� �����ְ�, Ȯ�忡�� �����ִ�.
        IDamaged damagedAbleObject = other.GetComponent<IDamaged>();

        if (damagedAbleObject != null)
        {

            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                if (other.CompareTag("Monster") && !other.GetComponent<Monster_Final>().IsMonsterTrigger)
                {
                    other.GetComponent<Monster_Final>().IsMonsterTrigger = true;
                    float damage = 1000;
                    photonView.RPC("Damaged", RpcTarget.All, damage, PV.OwnerActorNr);
                    Health--;
                    Debug.Log("����� ü�� --");
                    // ü�� ���� �� Slider ������Ʈ
                    if (TheLastYggdrasilHPBar != null)
                    {
                        TheLastYggdrasilHPBar.value = Health;
                    }
                }
            }
        }
    }

/*    [PunRPC]
    private void LoadBadEndingScene()
    {
        // Bad Ending Scene���� ��ȯ
        PhotonNetwork.LoadLevel("BadEndingScene");
    }
    [PunRPC]
    private void LoadHappyEndingScene()
    {
        // Bad Ending Scene���� ��ȯ
        PhotonNetwork.LoadLevel("HappyEndingScene");
    }*/
}
