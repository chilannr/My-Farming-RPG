using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// ͨ�� [ExecuteAlways] ���ԣ������ڱ༭��ģʽ�����иýű�
[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap tilemap; // Tilemap ���
    [SerializeField] private SO_GridProperties gridProperties = null; // �������Ե� ScriptableObject
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable; // ���񲼶�����

    private void OnEnable()
    {
        // ���ڱ༭��ģʽ��ִ��
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
        // ���ڱ༭��ģʽ��ִ��
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null)
            {
                // ȷ���ڱ�����Ϸʱ���µ� gridproperties ��Ϸ����õ����� - �������ǲ��ᱻ���档
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        // ѹ�� Tilemap �߽�
        tilemap.CompressBounds();

        // ���ڱ༭��ģʽ��ִ��
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
        // ���ڱ༭��ģʽ��ִ��
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
#endif
}