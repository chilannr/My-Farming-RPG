using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//  音频管理器类，继承自SingletonMonobehaviour以实现单例模式
public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null; // 声音预制体

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSoundAudioSource = null; // 环境音源组件
    [SerializeField] private AudioSource gameMusicAudioSource = null; // 游戏音乐音源组件

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer gameAudioMixer = null; // 游戏音频混合器

    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot gameMusicSnapshot = null; // 游戏音乐快照
    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot = null; // 游戏环境快照

    [Header("Other")]
    [SerializeField] private SO_SoundList so_soundList = null; // 声音列表
    [SerializeField] private SO_SceneSoundsList so_sceneSoundsList = null; // 场景声音列表
    [SerializeField] private float defaultSceneMusicPlayTimeSeconds = 120f; // 默认场景音乐播放时间（秒）
    [SerializeField] private float sceneMusicStartMinSecs = 20f; // 场景音乐开始的最小秒数
    [SerializeField] private float sceneMusicStartMaxSecs = 40f; // 场景音乐开始的最大秒数
    [SerializeField] private float musicTransitionSecs = 8f; // 音乐过渡时间（秒）

    private Dictionary<SoundName, SoundItem> soundDictionary; // 声音字典
    private Dictionary<SceneName, SceneSoundsItem> sceneSoundsDictionary; // 场景声音字典

    private Coroutine playSceneSoundsCoroutine; // 播放场景声音的协程

    protected override void Awake()
    {
        base.Awake();

        // 初始化声音字典
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        // 加载声音字典
        foreach (SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }

        // 初始化场景声音字典
        sceneSoundsDictionary = new Dictionary<SceneName, SceneSoundsItem>();

        // 加载场景声音字典
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

        // 尝试获取当前场景
        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, true, out SceneName currentSceneName))
        {
            // 获取场景的音乐和环境声音
            if (sceneSoundsDictionary.TryGetValue(currentSceneName, out SceneSoundsItem sceneSoundsItem))
            {
                soundDictionary.TryGetValue(sceneSoundsItem.musicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundsItem.ambientSoundForScene, out ambientSoundItem);
            }
            else
            {
                return;
            }

            // 停止正在播放的场景声音
            if (playSceneSoundsCoroutine != null)
            {
                StopCoroutine(playSceneSoundsCoroutine);
            }

            // 播放场景的环境声音和音乐
            playSceneSoundsCoroutine = StartCoroutine(PlaySceneSoundsRoutine(musicPlayTime, musicSoundItem, ambientSoundItem));
        }
    }

    private IEnumerator PlaySceneSoundsRoutine(float musicPlaySeconds, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {
        if (musicSoundItem != null && ambientSoundItem != null)
        {
            // 先播放环境声音
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            // 等待随机范围的秒数后播放音乐
            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));

            // 播放音乐
            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            // 等待音乐播放秒数后切换到环境声音
            yield return new WaitForSeconds(musicPlaySeconds);

            // 播放环境声音
            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        // 设置音量
        gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.soundVolume));

        // 设置音乐剪辑并播放
        gameMusicAudioSource.clip = musicSoundItem.soundClip;
        gameMusicAudioSource.Play();

        // 切换到音乐快照
        gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }

    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        // 设置音量
        gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.soundVolume));

        // 设置环境声音剪辑并播放
        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        // 切换到环境快照
        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);
    }

    /// <summary>
    /// 将音量的小数分数转换为-80到+20分贝范围
    /// </summary>
    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        // 将音量从小数分数转换为-80到+20分贝范围
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