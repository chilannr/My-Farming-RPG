using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 要求该脚本附加到的游戏对象上也有 AStar 组件
[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    [SerializeField] private SO_SceneRouteList so_SceneRouteList = null; // 用于存储场景路径列表的 ScriptableObject 资源
    private Dictionary<string, SceneRoute> sceneRouteDictionary; // 用于存储场景路径数据的字典

    [HideInInspector]
    public NPC[] npcArray; // 用于存储场景中所有 NPC 对象的数组

    private AStar aStar; // 对 AStar 组件的引用,用于寻路计算

    protected override void Awake()
    {
        base.Awake();

        // 创建场景路径字典
        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        // 从 ScriptableObject 资源加载场景路径数据并存储在字典中
        if (so_SceneRouteList.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute so_sceneRoute in so_SceneRouteList.sceneRouteList)
            {
                // 检查字典中是否已存在相同的路径
                if (sceneRouteDictionary.ContainsKey(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("** Duplicate Scene Route Key Found ** Check for duplicate routes in the scriptable object scene route list");
                    continue;
                }

                // 将场景路径数据添加到字典中
                sceneRouteDictionary.Add(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString(), so_sceneRoute);
            }
        }

        // 获取 AStar 组件的引用
        aStar = GetComponent<AStar>();

        // 获取场景中所有 NPC 对象并存储在数组中
        npcArray = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad; // 订阅场景加载完成事件
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad; // 取消订阅场景加载完成事件
    }

    private void AfterSceneLoad()
    {
        SetNPCsActiveStatus(); // 在场景加载完成后调用该方法
    }

    private void SetNPCsActiveStatus()
    {
        // 遍历所有 NPC 对象
        foreach (NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            // 如果 NPC 当前所在场景与活动场景匹配,则激活该 NPC
            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            // 否则禁用该 NPC
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;

        // 从字典中获取指定场景之间的路径数据
        if (sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        // 调用 AStar 组件的 BuildPath 方法计算路径
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}