using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;  // 渐变持续时间
    [SerializeField] private CanvasGroup faderCanvasGroup = null;  // 用于渐变效果的CanvasGroup组件
    [SerializeField] private Image faderImage = null;  // 渐变效果的背景图片
    public SceneName startingSceneName;  // 起始场景名称

    // 用于实现渐变效果的协程函数
    private IEnumerator Fade(float finalAlpha)
    {
        // 将渐变标志设置为true，以防止FadeAndSwitchScenes协程被再次调用
        isFading = true;

        // 确保CanvasGroup组件阻止射线进入场景，以防止接受更多的输入
        faderCanvasGroup.blocksRaycasts = true;

        // 根据当前alpha值、目标alpha值和渐变持续时间计算CanvasGroup的渐变速度
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // 当CanvasGroup的alpha值尚未达到目标alpha值时...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // ...将alpha值朝向目标alpha值移动
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // 等待一帧然后继续
            yield return null;
        }

        // 渐变完成后将渐变标志设置为false
        isFading = false;

        // 停止CanvasGroup阻止射线，以便不再忽略输入
        faderCanvasGroup.blocksRaycasts = false;
    }

    // 用于实现场景切换和过渡效果的协程函数
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // 调用场景卸载前的渐变淡出事件
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // 开始渐变至黑屏并等待渐变完成
        yield return StartCoroutine(Fade(1f));

        // 存储场景数据
        SaveLoadManager.Instance.StoreCurrentSceneData();

        // 设置玩家位置
        Player.Instance.gameObject.transform.position = spawnPosition;

        // 调用场景卸载前的事件
        EventHandler.CallBeforeSceneUnloadEvent();

        // 卸载当前活动场景
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // 开始加载指定场景并等待加载完成
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // 调用场景加载后的事件
        EventHandler.CallAfterSceneLoadEvent();

        // 恢复新场景的数据
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        // 开始渐变淡入并等待渐变完成
        yield return StartCoroutine(Fade(0f));

        // 调用场景加载后的渐变淡入事件
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    // 用于加载场景并设置为活动场景的协程函数
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // 允许给定场景在多帧中加载，并将其添加到已加载的场景中（此时仅包括Persistent场景）
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 找到最近加载的场景（已加载场景中的最后一个场景）
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // 将最新加载的场景设置为活动场景（标记为下一个要卸载的场景）
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    // 初始化场景控制器的协程函数
    private IEnumerator Start()
    {
        // 将初始alpha值设置为黑屏
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // 开始加载第一个场景并等待加载完成
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // 如果该事件有订阅者，则调用该事件
        EventHandler.CallAfterSceneLoadEvent();
        
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        // 场景加载完成后开始渐变淡入
        StartCoroutine(Fade(0f));
    }

    // 这是与项目的其他部分进行联系和影响的主要外部接口
    // 当玩家想要切换场景时，将调用此函数
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        // 如果没有进行渐变，则开始渐变并切换场景
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}