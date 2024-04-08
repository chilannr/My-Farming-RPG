using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingControl : MonoBehaviour
{
    [SerializeField] private LightingSchedule lightingSchedule; // 存储不同季节和时间对应的光源亮度数据
    [SerializeField] private bool isLightFlicker = false; // 控制是否启用光源闪烁效果
    [SerializeField][Range(0f, 1f)] private float lightFlickerIntensity; // 控制光源闪烁的强度
    [SerializeField][Range(0f, 0.2f)] private float lightFlickerTimeMin; // 控制光源闪烁的时间间隔最小值
    [SerializeField][Range(0f, 0.2f)] private float lightFlickerTimeMax; // 控制光源闪烁的时间间隔最大值

    private Light2D light2D; // 场景中的 Light2D 组件引用
    private Dictionary<string, float> lightingBrightnessDictionary = new Dictionary<string, float>(); // 存储 LightingSchedule 中的亮度数据,方便查找
    private float currentLightIntensity; // 当前光源的亮度值
    private float lightFlickerTimer = 0f; // 用于计时光源闪烁的间隔时间
    private Coroutine fadeInLightCoroutine; // 用于控制光源渐变变亮效果的协程

    private void Awake()
    {
        light2D = GetComponentInChildren<Light2D>(); // 获取 Light2D 组件引用

        if (light2D == null)
            enabled = false; // 如果没有 Light2D 组件,则禁用该脚本

        // 构建 lightingBrightnessDictionary
        foreach (LightingBrightness lightingBrightness in lightingSchedule.lightingBrightnessArray)
        {
            string Key = lightingBrightness.season.ToString() + lightingBrightness.hour.ToString();
            lightingBrightnessDictionary.Add(Key, lightingBrightness.lightIntensity);
        }
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameHourEvent += EventHandler_AdvanceGameHourEvent; // 订阅游戏时间推进一小时事件
        EventHandler.AfterSceneLoadEvent += EventHandler_AfterSceneLoadEvent; // 订阅场景加载完成事件
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameHourEvent -= EventHandler_AdvanceGameHourEvent; // 取消订阅游戏时间推进一小时事件
        EventHandler.AfterSceneLoadEvent -= EventHandler_AfterSceneLoadEvent; // 取消订阅场景加载完成事件
    }

    private void EventHandler_AdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        SetLightingIntensity(gameSeason, gameHour, true); // 根据当前季节和时间设置光源亮度,启用渐变效果
    }

    private void EventHandler_AfterSceneLoadEvent()
    {
        SetLightingAfterSceneLoaded(); // 在场景加载后,根据当前季节和时间设置光源亮度
    }

    private void Update()
    {
        if (isLightFlicker)
            lightFlickerTimer -= Time.deltaTime; // 如果启用了光源闪烁效果,则更新闪烁计时器
    }

    private void LateUpdate()
    {
        if (isLightFlicker && lightFlickerTimer <= 0f)
        {
            LightFlicker(); // 如果闪烁计时器到时,则执行光源闪烁效果
        }
        else
        {
            light2D.intensity = currentLightIntensity; // 否则,直接设置光源亮度
        }
    }

    private void LightFlicker()
    {
        // 实现光源闪烁效果的具体逻辑
        light2D.intensity = Random.Range(currentLightIntensity, currentLightIntensity + (currentLightIntensity * lightFlickerIntensity));
        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax); // 重新设置闪烁计时器
    }

    private void SetLightingAfterSceneLoaded()
    {
        Season gameSeason = TimeManager.Instance.GetGameSeason(); // 获取当前游戏季节
        int gameHour = TimeManager.Instance.GetGameTime().Hours; // 获取当前游戏时间(小时)

        SetLightingIntensity(gameSeason, gameHour, false); // 设置光源亮度,不启用渐变效果
    }

    private void SetLightingIntensity(Season gameSeason, int gameHour, bool fadein)
    {
        int i = 0;
        while (i < 23) // 循环查找最近的有效亮度值
        {
            string key = gameSeason.ToString() + (gameHour).ToString(); // 构造键值

            if (lightingBrightnessDictionary.TryGetValue(key, out float targetLightingIntensity)) // 查找对应的亮度值
            {
                if (fadein)
                {
                    if (fadeInLightCoroutine != null) StopCoroutine(fadeInLightCoroutine); // 如果有正在运行的渐变协程,则停止

                    fadeInLightCoroutine = StartCoroutine(FadeInLightRoutine(targetLightingIntensity)); // 启动渐变变亮协程
                }
                else
                {
                    currentLightIntensity = targetLightingIntensity; // 直接设置光源亮度
                }
                break;
            }
            i++;

            gameHour--; // 继续查找前一小时的亮度值
            if (gameHour < 0)
            {
                gameHour = 23;
            }
        }
    }

    private IEnumerator FadeInLightRoutine(float targetIntensity)
    {
        float fadeDuration = 5f; // 渐变持续时间
        float fadeSpeed = Mathf.Abs(currentLightIntensity - targetIntensity) / fadeDuration; // 计算渐变速度

        while (!Mathf.Approximately(currentLightIntensity, targetIntensity)) // 循环更新光源亮度,实现渐变效果
        {
            currentLightIntensity = Mathf.MoveTowards(currentLightIntensity, targetIntensity, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        currentLightIntensity = targetIntensity; // 确保最终亮度值准确
    }
}