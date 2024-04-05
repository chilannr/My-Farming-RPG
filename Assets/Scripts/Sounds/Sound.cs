using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    private AudioSource audioSource; // ����Դ���

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // ��ȡ����Դ���
    }

    // ��������������
    public void SetSound(SoundItem soundItem)
    {
        audioSource.pitch = Random.Range(soundItem.soundPitchRandomVariationMin, soundItem.soundPitchRandomVariationMax); // ��������������
        audioSource.volume = soundItem.soundVolume; // ��������������
        audioSource.clip = soundItem.soundClip; // ���������ļ���
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play(); // ��������
        }
    }

    private void OnDisable()
    {
        audioSource.Stop(); // ֹͣ����
    }
}