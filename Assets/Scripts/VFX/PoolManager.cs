using System.Collections.Generic; // ����ʹ���ֵ�Ͷ������ݽṹ

using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>(); // ������ֵ�,��ΪԤ����ʵ��ID,ֵΪ���д洢��Ԥ�������
    [SerializeField] private Pool[] pool = null; // ���������
    [SerializeField] private Transform objectPoolTransform = null; // ����ظ���Transform


    [System.Serializable]
    public struct Pool // ����ؽṹ��
    {
        public int poolSize; // ����ش�С
        public GameObject prefab; // ��Ӧ��Ԥ����
    }

    private void Start()
    {
        // �ڿ�ʼʱ���������
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }


    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID(); // ��ȡԤ����ʵ��ID��Ϊ��ֵ
        string prefabName = prefab.name; // ��ȡԤ��������

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // ����������Ϸ����,���ڸ��Ӳ㼶��ϵ

        parentGameObject.transform.SetParent(objectPoolTransform); // ���ø���Transform


        if (!poolDictionary.ContainsKey(poolKey)) // ����ֵ��в������ü�ֵ
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>()); // ����µļ�ֵ��,ֵΪ�ն���

            for (int i = 0; i < poolSize; i++) // ʵ����������ӵ�������
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false); // ������Ϸ����

                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey)) // ����ֵ��а����ü�ֵ
        {
            // �Ӷ���ض����л�ȡ����
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab); // ���ö���λ�ú���ת

            return objectToReuse;
        }
        else
        {
            Debug.Log("No object pool for " + prefab); // ����ֵ��в������ü�ֵ,�����־
            return null;
        }
    }


    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue(); // �Ӷ�����ȡ��һ������
        poolDictionary[poolKey].Enqueue(objectToReuse); // ��ȡ���Ķ������¼������

        if (objectToReuse.activeSelf == true) // ��������Ѿ�����
        {
            objectToReuse.SetActive(false); // ���ö���
        }

        return objectToReuse;
    }

    // ���ö���λ�á���ת������
    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position; // ����λ��
        objectToReuse.transform.rotation = rotation; // ������ת
        objectToReuse.transform.localScale = prefab.transform.localScale; // ��������
    }
}