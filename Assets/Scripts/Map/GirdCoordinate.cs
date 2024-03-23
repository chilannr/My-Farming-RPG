using UnityEngine;

// Serializable ����ʹ�ø�������� Unity �༭���н������л�����ʾ
[System.Serializable]
public class GridCoordinate
{
    public int x; // x ����
    public int y; // y ����

    // ���캯�������ڳ�ʼ������
    public GridCoordinate(int p1, int p2)
    {
        x = p1;
        y = p2;
    }

    // �� GridCoordinate ����ת��Ϊ Vector2 ����
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    // �� GridCoordinate ����ת��Ϊ Vector2Int ����
    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    // �� GridCoordinate ����ת��Ϊ Vector3 ����
    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }

    // �� GridCoordinate ����ת��Ϊ Vector3Int ����
    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}