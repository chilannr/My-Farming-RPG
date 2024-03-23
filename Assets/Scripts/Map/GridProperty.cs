[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate; // ��������
    public GridBoolProperty gridBoolProperty; // ���񲼶�����
    public bool gridBoolValue = false; // ���񲼶�ֵ��Ĭ��Ϊ false

    // ���캯�������ڳ�ʼ����������
    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}