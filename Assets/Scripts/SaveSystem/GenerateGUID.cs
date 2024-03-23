using UnityEngine;

// 使用ExecuteAlways属性确保脚本在编辑器模式和播放模式下都执行
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = ""; // 存储GUID的字符串值

    public string GUID { get => _gUID; set => _gUID = value; } // 公共属性用于获取和设置GUID的值

    private void Awake()
    {
        // 仅在编辑器中填充
        if (!Application.IsPlaying(gameObject))
        {
            // 确保对象具有保证唯一ID
            if (_gUID == "")
            {
                // 分配GUID
                _gUID = System.Guid.NewGuid().ToString(); // 生成新的GUID字符串并分配给_gUID
            }
        }
    }
}