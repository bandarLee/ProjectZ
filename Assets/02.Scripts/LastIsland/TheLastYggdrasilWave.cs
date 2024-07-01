using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.UI.GridLayoutGroup;

public class TheLastYggdrasilWave : MonoBehaviour
{
    public int Health = 1000;
    public Slider TheLastYggdrasilHPBar;


    PhotonView PV;


    private void Start()
    {
        PV = GetComponent<PhotonView>(); 

        // Slider 초기 설정
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
        // 개방 폐쇄 원칙 + 인터페이스 // 수정에는 닫혀있고, 확장에는 열려있다.
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
                    Health-=50;
                    Debug.Log("세계수 체력 --");
                    // 체력 감소 후 Slider 업데이트
                    if (TheLastYggdrasilHPBar != null)
                    {
                        TheLastYggdrasilHPBar.value = Health;
                    }
                }
            }
        }
    }

}
