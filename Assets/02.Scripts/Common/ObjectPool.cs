using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool               // Pool 클래스: 오브젝트 풀의 설정을 저장
    {
        public string tag;          // 오브젝트 풀의 태그
        public GameObject prefab;   // 생성할 프리팹
        public int size;            // 풀의 크기
    }

    public List<Pool> pools;        // 여러 오브젝트 풀을 담을 리스트
    public Dictionary<string, Queue<GameObject>> poolDictionary; // 각 풀을 관리할 딕셔너리

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>(); // 딕셔너리 초기화

        foreach (Pool pool in pools)    // 각 풀 설정에 따라 오브젝트 생성
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab); // 프리팹 인스턴스화
                obj.SetActive(false); // 비활성화하여 초기화
                objectPool.Enqueue(obj); // 큐에 오브젝트 추가
            }

            poolDictionary.Add(pool.tag, objectPool); // 딕셔너리에 풀 추가
        }
    }

    // 오브젝트 풀에서 오브젝트를 가져와 스폰
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) // 해당 태그의 풀 존재 여부 확인
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // 큐에서 오브젝트 가져오기
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true); // 활성화
        objectToSpawn.transform.position = position; // 위치 설정
        objectToSpawn.transform.rotation = rotation; // 회전 설정

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();// IPooledObject 인터페이스를 구현한 스크립트 호출

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn); // 오브젝트를 다시 큐에 추가하여 재사용 가능하게 함

        return objectToSpawn; // 스폰된 오브젝트 반환
    }
}
