using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//  ��Ƶ�������࣬�̳���SingletonMonobehaviour��ʵ�ֵ���ģʽ
public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null; // ����Ԥ����

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSoundAudioSource = null; // ������Դ���
    [SerializeField] private AudioSource gameMusicAudioSource = null; // ��Ϸ������Դ���

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer gameAudioMixer = null; // ��Ϸ��Ƶ�����

    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot gameMusicSnapshot = null; // ��Ϸ���ֿ���
    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot = null; // ��Ϸ��������

    [Header("Other")]
    [SerializeField] private SO_SoundList so_soundList = null; // �����б�
    [SerializeField] private SO_SceneSoundsList so_sceneSoundsList = null; // ���������б�
    [SerializeField] private float defaultSceneMusicPlayTimeSeconds = 120f; // Ĭ�ϳ������ֲ���ʱ�䣨�룩
    [SerializeField] private float sceneMusicStartMinSecs = 20f; // �������ֿ�ʼ����С����
    [SerializeField] private float sceneMusicStartMaxSecs = 40f; // �������ֿ�ʼ���������
    [SerializeField] private float musicTransitionSecs = 8f; // ���ֹ���ʱ�䣨�룩

    private Dictionary<SoundName, SoundItem> soundDictionary; // �����ֵ�
    private Dictionary<SceneName, SceneSoundsItem> sceneSoundsDictionary; // ���������ֵ�

    private Coroutine playSceneSoundsCoroutine; // ���ų���������Э��

    protected override void Awake()
    {
        base.Awake();

        // ��ʼ�������ֵ�
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        // ���������ֵ�
        foreach (SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }

        // ��ʼ�����������ֵ�
        sceneSoundsDictionary = new Dictionary<SceneName, SceneSoundsItem>();

        // ���س��������ֵ�
        foreach (SceneSoundsItem sceneSoundsItem in so_sceneSoundsList.sceneSoundsDetails)
        {
            sceneSoundsDictionary.Add(sceneSoundsItem.sceneName, sceneSoundsItem);
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlaySceneSounds;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlaySceneSounds;
    }

    private void PlaySceneSounds()
    {
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        float musicPlayTime = defaultSceneMusicPlayTimeSeconds;

        // ���Ի�ȡ��ǰ����
        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, true, out SceneName currentSceneName))
        {
            // ��ȡ���������ֺͻ�������
            if (sceneSoundsDictionary.TryGetValue(currentSceneName, out SceneSoundsItem sceneSoundsItem))
            {
                soundDictionary.TryGetValue(sceneSoundsItem.musicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundsItem.ambientSoundForScene, out ambientSoundItem);
            }
            else
            {
                return;
            }

            // ֹͣ���ڲ��ŵĳ�������
            if (playSceneSoundsCoroutine != null)
            {
                StopCoroutine(playSceneSoundsCoroutine);
            }

            // ���ų����Ļ�������������
            playSceneSoundsCoroutine = StartCoroutine(PlaySceneSoundsRoutine(musicPlayTime, musicSoundItem, ambientSoundItem));
        }
    }

    private IEnumerator PlaySceneSoundsRoutine(float musicPlaySeconds, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {
        if (musicSoundItem != null && ambientSoundItem != null)
        {
            // �Ȳ��Ż�������
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            // �ȴ������Χ�������󲥷�����
            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));

            // ��������
            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            // �ȴ����ֲ����������л�����������
            yield return new WaitForSeconds(musicPlaySeconds);

            // ���Ż�������
            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        // ��������
        gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.soundVolume));

        // �������ּ���������
        gameMusicAudioSource.clip = musicSoundItem.soundClip;
        gameMusicAudioSource.Play();

        // �л������ֿ���
        gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }

    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        // ��������
        gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.soundVolume));

        // ���û�����������������
        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        // �л�����������
        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);
    }

    /// <summary>
    /// ��������С������ת��Ϊ-80��+20�ֱ���Χ
    /// </summary>
    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        // ��������С������ת��Ϊ-80��+20�ֱ���Χ
        return (volumeDecimalFraction * 100f - 80f);
    }

    public void PlaySound(SoundName soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPrefab != null)
        {
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();

            sound.SetSound(soundItem);
            soundGameObject.SetActive(true);
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));
        }
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }
}