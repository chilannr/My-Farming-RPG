using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ҪBoxCollider2D���
[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    // ����ָ��Ҫ���͵��ĳ������ƣ���ʼֵΪScene1_Farm
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;

    // ����ָ�����³����е�λ�ã���ʼֵΪ(0,0,0)
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();


    // ��������ײ��ͣ���ڸ��������ײ��ʱ��ÿ֡����һ��
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ���Ի�ȡ��ײ�����������Player���
        Player player = collision.GetComponent<Player>();

        // �����ȡ��Player���
        if (player != null)
        {
            // ����������³����е�λ��

            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;

            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

            float zPosition = 0f;

            // ���͵��³���
            SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));

        }

    }
}