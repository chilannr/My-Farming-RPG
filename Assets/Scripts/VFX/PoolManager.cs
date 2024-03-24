using System.Collections.Generic; // 用于使用字典和队列数据结构

using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>(); // 对象池字典,键为预制体实例ID,值为队列存储该预制体对象
    [SerializeField] private Pool[] pool = null; // 对象池数组
    [SerializeField] private Transform objectPoolTransform = null; // 对象池父级Transform


    [System.Serializable]
    public struct Pool // 对象池结构体
    {
        public int poolSize; // 对象池大小
        public GameObject prefab; // 对应的预制体
    }

    private void Start()
    {
        // 在开始时创建对象池
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }


    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID(); // 获取预制体实例ID作为键值
        string prefabName = prefab.name; // 获取预制体名称

        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); // 创建父级游戏对象,用于父子层级关系

        parentGameObject.transform.SetParent(objectPoolTransform); // 设置父级Transform


        if (!poolDictionary.ContainsKey(poolKey)) // 如果字典中不包含该键值
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>()); // 添加新的键值对,值为空队列

            for (int i = 0; i < poolSize; i++) // 实例化对象添加到队列中
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false); // 禁用游戏对象

                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey)) // 如果字典中包含该键值
        {
            // 从对象池队列中获取对象
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab); // 重置对象位置和旋转

            return objectToReuse;
        }
        else
        {
            Debug.Log("No object pool for " + prefab); // 如果字典中不包含该键值,输出日志
            return null;
        }
    }


    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue(); // 从队列中取出一个对象
        poolDictionary[poolKey].Enqueue(objectToReuse); // 将取出的对象重新加入队列

        if (objectToReuse.activeSelf == true) // 如果对象已经激活
        {
            objectToReuse.SetActive(false); // 禁用对象
        }

        return objectToReuse;
    }

    // 重置对象位置、旋转和缩放
    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position; // 设置位置
        objectToReuse.transform.rotation = rotation; // 设置旋转
        objectToReuse.transform.localScale = prefab.transform.localScale; // 设置缩放
    }
}