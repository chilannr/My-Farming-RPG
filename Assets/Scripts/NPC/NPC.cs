using System;
using System.Collections.Generic;
using UnityEngine;

// 要求该脚本附加到的游戏对象上也有 NPCMovement 和 GenerateGUID 组件
[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour, ISaveable
{
    private string _iSaveableUniqueID; // 用于存储 NPC 对象的唯一 ID
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave; // 用于存储 NPC 对象的保存数据
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    private NPCMovement npcMovement; // 对 NPC 对象上的 NPCMovement 组件的引用

    private void OnEnable()
    {
        ISaveableRegister(); // 在 NPC 对象启用时将其注册到可保存对象列表中
    }

    private void OnDisable()
    {
        ISaveableDeregister(); // 在 NPC 对象禁用时将其从可保存对象列表中移除
    }

    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID; // 从 GenerateGUID 组件获取唯一 ID
        GameObjectSave = new GameObjectSave(); // 创建一个新的 GameObjectSave 实例
    }

    private void Start()
    {
        // 获取 NPC 对象上的 NPCMovement 组件
        npcMovement = GetComponent<NPCMovement>();
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this); // 将 NPC 对象从可保存对象列表中移除
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        // 从保存数据中获取 NPC 对象的 GameObjectSave 实例
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // 从 GameObjectSave 实例中获取场景保存数据
            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // 如果字典不为空
                if (sceneSave.vector3Dictionary != null && sceneSave.stringDictionary != null)
                {
                    // 加载目标网格位置
                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetGridPosition", out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    // 加载目标世界位置
                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPosition", out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3(savedNPCTargetWorldPosition.x, savedNPCTargetWorldPosition.y, savedNPCTargetWorldPosition.z);
                        transform.position = npcMovement.npcTargetWorldPosition;
                    }

                    // 加载目标场景
                    if (sceneSave.stringDictionary.TryGetValue("npcTargetScene", out string savedTargetScene))
                    {
                        if (Enum.TryParse<SceneName>(savedTargetScene, out SceneName sceneName))
                        {
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;
                        }
                    }

                    // 清除任何当前的 NPC 移动
                    npcMovement.CancelNPCMovement();
                }
            }
        }
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this); // 将 NPC 对象添加到可保存对象列表中
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        // 由于在持久场景中，因此这里不需要执行任何操作
    }

    public GameObjectSave ISaveableSave()
    {
        // 移除当前场景的保存数据
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // 创建一个新的 SceneSave 实例
        SceneSave sceneSave = new SceneSave();

        // 创建一个新的 Vector3Serializable 字典
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        // 创建一个新的字符串字典
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // 存储目标网格位置、目标世界位置和目标场景
        sceneSave.vector3Dictionary.Add("npcTargetGridPosition", new Vector3Serializable(npcMovement.npcTargetGridPosition.x, npcMovement.npcTargetGridPosition.y, npcMovement.npcTargetGridPosition.z));
        sceneSave.vector3Dictionary.Add("npcTargetWorldPosition", new Vector3Serializable(npcMovement.npcTargetWorldPosition.x, npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));
        sceneSave.stringDictionary.Add("npcTargetScene", npcMovement.npcTargetScene.ToString());

        // 将场景保存数据添加到 GameObjectSave 实例中
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave; // 返回 GameObjectSave 实例
    }

    public void ISaveableStoreScene(string sceneName)
    {
        // 由于在持久场景中，因此这里不需要执行任何操作
    }
}