using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 需要BoxCollider2D组件
[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    // 用于指定要传送到的场景名称，初始值为Scene1_Farm
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;

    // 用于指定在新场景中的位置，初始值为(0,0,0)
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();


    // 当其他碰撞器停留在该物体的碰撞器时，每帧调用一次
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 尝试获取碰撞器所在物体的Player组件
        Player player = collision.GetComponent<Player>();

        // 如果获取到Player组件
        if (player != null)
        {
            // 计算玩家在新场景中的位置

            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

            float zPosition = 0f;

            // 传送到新场景
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));

        }

    }
}