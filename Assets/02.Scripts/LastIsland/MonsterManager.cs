using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public GameObject[] Monsters;
    private int monsterCount = 0;

    public void StartWave()
    {
        Debug.Log("Wave Start");

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartWaveCoroutine(new int[] { 3, 5, 7, 9, 11 }, 20f));
        }
    }

    private IEnumerator StartWaveCoroutine(int[] waveCounts, float delay)
    {
        foreach (int count in waveCounts)
        {
            for (int i = 0; i < count; i++)
            {
                if (monsterCount < Monsters.Length)
                {
                    Monsters[monsterCount].SetActive(true);
                    monsterCount++;
                }
                else
                {
                    Debug.LogWarning("Not enough monsters in the array to activate the requested number of monsters.");
                    yield break;
                }
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
