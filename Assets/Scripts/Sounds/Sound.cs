using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    private AudioSource audioSource; // 声音源组件

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // 获取声音源组件
    }

    // 设置声音的属性
    public void SetSound(SoundItem soundItem)
    {
        audioSource.pitch = Random.Range(soundItem.soundPitchRandomVariationMin, soundItem.soundPitchRandomVariationMax); // 设置声音的音调
        audioSource.volume = soundItem.soundVolume; // 设置声音的音量
        audioSource.clip = soundItem.soundClip; // 设置声音的剪辑
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play(); // 播放声音
        }
    }

    private void OnDisable()
    {
        audioSource.Stop(); // 停止声音
    }
}