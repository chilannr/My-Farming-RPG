using System.Collections.Generic;
using UnityEngine;

// 创建ScriptableObject的菜单选项，用于创建和管理声音列表
[CreateAssetMenu(fileName = "so_SoundList", menuName = "Scriptable Objects/Sounds/Sound List")]
public class SO_SoundList : ScriptableObject
{
    [SerializeField]
    public List<SoundItem> soundDetails; // 声音详情列表
}