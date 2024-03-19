using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }

    /// <summary>
    /// Switch the collider that cinemachine uses to define the edges of the screen
    /// </summary>
    private void SwitchBoundingShape()
    {
        // 获取'boundsconfiner'游戏对象上的多边形碰撞器，该碰撞器用于限制Cinemachine相机在屏幕边缘之外
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        // 将CinemachineConfiner的m_BoundingShape2D属性设置为多边形碰撞器
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // 由于限制器边界已更改，需要调用此方法来清除缓存
        cinemachineConfiner.InvalidatePathCache();
    }
}