using UnityEngine;

[System.Serializable]
public class SoundItem
{
    public SoundName soundName; // 声音名称
    public AudioClip soundClip; // 声音剪辑
    public string soundDescription; // 声音描述

    [Range(0.1f, 1.5f)] public float soundPitchRandomVariationMin = 0.8f; // 声音音调的最小随机变化值
    [Range(0.1f, 1.5f)] public float soundPitchRandomVariationMax = 1.2f; // 声音音调的最大随机变化值
    [Range(0f, 1f)] public float soundVolume = 1f; // 声音音量
}