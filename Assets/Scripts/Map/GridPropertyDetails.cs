[System.Serializable]
public class GridPropertyDetails
{
    public int gridX; // �����X����
    public int gridY; // �����Y����
    public bool isDiggable = false; // �Ƿ���ھ�
    public bool canDropItem = false; // �Ƿ���Է�����Ʒ
    public bool canPlaceFurniture = false; // �Ƿ���Է��üҾ�
    public bool isPath = false; // �Ƿ�Ϊ·��
    public bool isNPCObstacle = false; // �Ƿ�ΪNPC�ϰ���
    public int daysSinceDug = -1; // �ھ�������
    public int daysSinceWatered = -1; // ��ˮ�������
    public int seedItemCode = -1; // ������Ʒ�Ĵ���
    public int growthDays = -1; // ������Ҫ������
    public int daysSinceLastHarvest = -1; // �ϴ��ջ�������

    public GridPropertyDetails()
    {
    }
}