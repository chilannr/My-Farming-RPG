using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 附加到作物预制体上,用于设置网格属性词典中的值
/// </summary>
public class CropInstantiator : MonoBehaviour
{
    private Grid grid; // 网格组件
    [SerializeField] private int daysSinceDug = -1; // 距离挖掘的天数
    [SerializeField] private int daysSinceWatered = -1; // 距离浇水的天数
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0; // 种子物品代码
    [SerializeField] private int growthDays = 0; // 生长天数

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs; // 取消事件订阅
    }

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs; // 订阅事件
    }

    private void InstantiateCropPrefabs()
    {
        // 获取网格游戏对象
        grid = GameObject.FindObjectOfType<Grid>();

        // 获取作物在网格中的位置
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        // 设置作物网格属性
        SetCropGridProperties(cropGridPosition);

        // 销毁这个游戏对象
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