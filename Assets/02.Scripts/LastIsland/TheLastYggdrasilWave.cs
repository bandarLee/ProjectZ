using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheLastYggdrasilWave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
          // 세계수 체력 --
          // 세계수 체력bar 동기화
          // 몬스터 삭제
        }
    }
}
