using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int seedItemCode; // ��Ӧ���ӵ���Ʒ����
    public int[] growthDays; // ÿ�������׶����������
    public GameObject[] growthPrefab;// ʵ���������׶�ʱʹ�õ�Ԥ����
    public Sprite[] growthSprite; // ��������
    public Season[] seasons; // ��������
    public Sprite harvestedSprite; // �ջ��ʹ�õľ���

    [ItemCodeDescription]
    public int harvestedTransformItemCode; // ����������ջ��ת��Ϊ��һ����Ʒ,������뽫�����
    public bool hideCropBeforeHarvestedAnimation; // �Ƿ����ջ񶯻�֮ǰ��������
    public bool disableCropCollidersBeforeHarvestedAnimation;// �Ƿ����ջ񶯻�֮ǰ�����������ײ��,�Ա����ջ񶯻�Ӱ��������Ϸ����
    public bool isHarvestedAnimation; // ������һ�������׶ε�Ԥ������Ҫ�����ջ񶯻�
    public bool isHarvestActionEffect = false; // ȷ���Ƿ����ջ�Ч��
    public bool spawnCropProducedAtPlayerPosition; // �Ƿ������λ�������ջ���Ʒ
    public HarvestActionEffect harvestActionEffect; // ������ջ�Ч��
    public SoundName harvestSound; // ������ջ���Ч

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // �����ջ������Ĺ�����Ʒ��������,�������Ҫ������Ϊ0
    public int[] requiredHarvestActions; // ��Ӧ����������ջ�������

    [ItemCodeDescription]
    public int[] cropProducedItemCode; // �ջ���������Ʒ��������
    public int[] cropProducedMinQuantity; // �ջ���������Ʒ��С����
    public int[] cropProducedMaxQuantity; // ����������������С����,�������С���������֮���������
    public int daysToRegrow; // �����������������,-1��ʾֻ��һ���ջ�

    /// <summary>
    /// ���ظù����Ƿ���������ջ������,���Է���true,���򷵻�false
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        if (RequiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }

    }


    /// <summary>
    /// ����ù��߲��������ջ������,����-1,���򷵻ظù���������ջ�������
    /// </summary>
    public int RequiredHarvestActionsForTool(int toolItemCode)
    {
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}