using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ӵ�����Ԥ������,���������������Դʵ��е�ֵ
/// </summary>
public class CropInstantiator : MonoBehaviour
{
    private Grid grid; // �������
    [SerializeField] private int daysSinceDug = -1; // �����ھ������
    [SerializeField] private int daysSinceWatered = -1; // ���뽽ˮ������
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0; // ������Ʒ����
    [SerializeField] private int growthDays = 0; // ��������

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs; // ȡ���¼�����
    }

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs; // �����¼�
    }

    private void InstantiateCropPrefabs()
    {
        // ��ȡ������Ϸ����
        grid = GameObject.FindObjectOfType<Grid>();

        // ��ȡ�����������е�λ��
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        // ����������������
        SetCropGridProperties(cropGridPosition);

        // ���������Ϸ����
        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        if (seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

            if (gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;

            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
}