using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// 通过 [ExecuteAlways] 特性，可以在编辑器模式下运行该脚本
[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap tilemap; // Tilemap 组件
    [SerializeField] private SO_GridProperties gridProperties = null; // 网格属性的 ScriptableObject
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable; // 网格布尔属性

    private void OnEnable()
    {
        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null)
            {
                // 确保在保存游戏时更新的 gridproperties 游戏对象得到保存 - 否则它们不会被保存。
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        // 压缩 Tilemap 边界
        tilemap.CompressBounds();

        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        // 仅在编辑器模式下执行
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
#endif
}