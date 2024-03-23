using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    // 游戏保存数据
    public GameSave gameSave;
    // 可保存对象列表
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        // 初始化可保存对象列表
        iSaveableObjectList = new List<ISaveable>();
    }

    // 从文件加载数据
    public void LoadDataFromFile()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            // 打开文件流
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            // 反序列化游戏保存数据
            gameSave = (GameSave)bf.Deserialize(file);

            // 遍历所有可保存对象并应用保存数据
            for (int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                // 如果可保存对象的唯一ID不在游戏对象数据中，则销毁对象
                else
                {
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }

            // 关闭文件流
            file.Close();
        }

        // 禁用暂停菜单
        // UIManager.Instance.DisablePauseMenu();
    }

    // 将数据保存到文件
    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        // 遍历所有可保存对象并生成保存数据
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        BinaryFormatter bf = new BinaryFormatter();

        // 创建文件流
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        // 序列化游戏保存数据并写入文件
        bf.Serialize(file, gameSave);

        // 关闭文件流
        file.Close();

        // 禁用暂停菜单
        // UIManager.Instance.DisablePauseMenu();
    }

    // 存储当前场景数据
    public void StoreCurrentSceneData()
    {
        // 遍历所有可保存对象并触发存储场景数据
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    // 恢复当前场景数据
    public void RestoreCurrentSceneData()
    {
        // 遍历所有可保存对象并触发恢复场景数据
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}