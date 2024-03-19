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
        // ��ȡ'boundsconfiner'��Ϸ�����ϵĶ������ײ��������ײ����������Cinemachine�������Ļ��Ե֮��
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        // ��CinemachineConfiner��m_BoundingShape2D��������Ϊ�������ײ��
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // �����������߽��Ѹ��ģ���Ҫ���ô˷������������
        cinemachineConfiner.InvalidatePathCache();
    }
}