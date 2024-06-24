using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool               // Pool Ŭ����: ������Ʈ Ǯ�� ������ ����
    {
        public string tag;          // ������Ʈ Ǯ�� �±�
        public GameObject prefab;   // ������ ������
        public int size;            // Ǯ�� ũ��
    }

    public List<Pool> pools;        // ���� ������Ʈ Ǯ�� ���� ����Ʈ
    public Dictionary<string, Queue<GameObject>> poolDictionary; // �� Ǯ�� ������ ��ųʸ�

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

        poolDictionary = new Dictionary<string, Queue<GameObject>>(); // ��ųʸ� �ʱ�ȭ

        foreach (Pool pool in pools)    // �� Ǯ ������ ���� ������Ʈ ����
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab); // ������ �ν��Ͻ�ȭ
                obj.SetActive(false); // ��Ȱ��ȭ�Ͽ� �ʱ�ȭ
                objectPool.Enqueue(obj); // ť�� ������Ʈ �߰�
            }

            poolDictionary.Add(pool.tag, objectPool); // ��ųʸ��� Ǯ �߰�
        }
    }

    // ������Ʈ Ǯ���� ������Ʈ�� ������ ����
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) // �ش� �±��� Ǯ ���� ���� Ȯ��
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // ť���� ������Ʈ ��������
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true); // Ȱ��ȭ
        objectToSpawn.transform.position = position; // ��ġ ����
        objectToSpawn.transform.rotation = rotation; // ȸ�� ����

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();// IPooledObject �������̽��� ������ ��ũ��Ʈ ȣ��

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn); // ������Ʈ�� �ٽ� ť�� �߰��Ͽ� ���� �����ϰ� ��

        return objectToSpawn; // ������ ������Ʈ ��ȯ
    }
}
