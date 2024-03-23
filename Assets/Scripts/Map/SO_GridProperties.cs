using System.Collections.Generic;
using UnityEngine;

// ͨ�� CreateAssetMenu ���ԣ������� Unity �༭���� Assets/Create �˵��´��������͵�ʵ��
[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    public SceneName sceneName; // ��������
    public int gridWidth; // ������
    public int gridHeight; // ����߶�
    public int originX; // ԭ��� x ����
    public int originY; // ԭ��� y ����

    [SerializeField]
    public List<GridProperty> gridPropertyList; // ���������б�
}