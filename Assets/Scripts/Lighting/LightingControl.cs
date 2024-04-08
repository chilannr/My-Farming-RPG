using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingControl : MonoBehaviour
{
    [SerializeField] private LightingSchedule lightingSchedule; // �洢��ͬ���ں�ʱ���Ӧ�Ĺ�Դ��������
    [SerializeField] private bool isLightFlicker = false; // �����Ƿ����ù�Դ��˸Ч��
    [SerializeField][Range(0f, 1f)] private float lightFlickerIntensity; // ���ƹ�Դ��˸��ǿ��
    [SerializeField][Range(0f, 0.2f)] private float lightFlickerTimeMin; // ���ƹ�Դ��˸��ʱ������Сֵ
    [SerializeField][Range(0f, 0.2f)] private float lightFlickerTimeMax; // ���ƹ�Դ��˸��ʱ�������ֵ

    private Light2D light2D; // �����е� Light2D �������
    private Dictionary<string, float> lightingBrightnessDictionary = new Dictionary<string, float>(); // �洢 LightingSchedule �е���������,�������
    private float currentLightIntensity; // ��ǰ��Դ������ֵ
    private float lightFlickerTimer = 0f; // ���ڼ�ʱ��Դ��˸�ļ��ʱ��
    private Coroutine fadeInLightCoroutine; // ���ڿ��ƹ�Դ�������Ч����Э��

    private void Awake()
    {
        light2D = GetComponentInChildren<Light2D>(); // ��ȡ Light2D �������

        if (light2D == null)
            enabled = false; // ���û�� Light2D ���,����øýű�

        // ���� lightingBrightnessDictionary
        foreach (LightingBrightness lightingBrightness in lightingSchedule.lightingBrightnessArray)
        {
            string Key = lightingBrightness.season.ToString() + lightingBrightness.hour.ToString();
            lightingBrightnessDictionary.Add(Key, lightingBrightness.lightIntensity);
        }
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameHourEvent += EventHandler_AdvanceGameHourEvent; // ������Ϸʱ���ƽ�һСʱ�¼�
        EventHandler.AfterSceneLoadEvent += EventHandler_AfterSceneLoadEvent; // ���ĳ�����������¼�
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameHourEvent -= EventHandler_AdvanceGameHourEvent; // ȡ��������Ϸʱ���ƽ�һСʱ�¼�
        EventHandler.AfterSceneLoadEvent -= EventHandler_AfterSceneLoadEvent; // ȡ�����ĳ�����������¼�
    }

    private void EventHandler_AdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        SetLightingIntensity(gameSeason, gameHour, true); // ���ݵ�ǰ���ں�ʱ�����ù�Դ����,���ý���Ч��
    }

    private void EventHandler_AfterSceneLoadEvent()
    {
        SetLightingAfterSceneLoaded(); // �ڳ������غ�,���ݵ�ǰ���ں�ʱ�����ù�Դ����
    }

    private void Update()
    {
        if (isLightFlicker)
            lightFlickerTimer -= Time.deltaTime; // ��������˹�Դ��˸Ч��,�������˸��ʱ��
    }

    private void LateUpdate()
    {
        if (isLightFlicker && lightFlickerTimer <= 0f)
        {
            LightFlicker(); // �����˸��ʱ����ʱ,��ִ�й�Դ��˸Ч��
        }
        else
        {
            light2D.intensity = currentLightIntensity; // ����,ֱ�����ù�Դ����
        }
    }

    private void LightFlicker()
    {
        // ʵ�ֹ�Դ��˸Ч���ľ����߼�
        light2D.intensity = Random.Range(currentLightIntensity, currentLightIntensity + (currentLightIntensity * lightFlickerIntensity));
        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax); // ����������˸��ʱ��
    }

    private void SetLightingAfterSceneLoaded()
    {
        Season gameSeason = TimeManager.Instance.GetGameSeason(); // ��ȡ��ǰ��Ϸ����
        int gameHour = TimeManager.Instance.GetGameTime().Hours; // ��ȡ��ǰ��Ϸʱ��(Сʱ)

        SetLightingIntensity(gameSeason, gameHour, false); // ���ù�Դ����,�����ý���Ч��
    }

    private void SetLightingIntensity(Season gameSeason, int gameHour, bool fadein)
    {
        int i = 0;
        while (i < 23) // ѭ�������������Ч����ֵ
        {
            string key = gameSeason.ToString() + (gameHour).ToString(); // �����ֵ

            if (lightingBrightnessDictionary.TryGetValue(key, out float targetLightingIntensity)) // ���Ҷ�Ӧ������ֵ
            {
                if (fadein)
                {
                    if (fadeInLightCoroutine != null) StopCoroutine(fadeInLightCoroutine); // ������������еĽ���Э��,��ֹͣ

                    fadeInLightCoroutine = StartCoroutine(FadeInLightRoutine(targetLightingIntensity)); // �����������Э��
                }
                else
                {
                    currentLightIntensity = targetLightingIntensity; // ֱ�����ù�Դ����
                }
                break;
            }
            i++;

            gameHour--; // ��������ǰһСʱ������ֵ
            if (gameHour < 0)
            {
                gameHour = 23;
            }
        }
    }

    private IEnumerator FadeInLightRoutine(float targetIntensity)
    {
        float fadeDuration = 5f; // �������ʱ��
        float fadeSpeed = Mathf.Abs(currentLightIntensity - targetIntensity) / fadeDuration; // ���㽥���ٶ�

        while (!Mathf.Approximately(currentLightIntensity, targetIntensity)) // ѭ�����¹�Դ����,ʵ�ֽ���Ч��
        {
            currentLightIntensity = Mathf.MoveTowards(currentLightIntensity, targetIntensity, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        currentLightIntensity = targetIntensity; // ȷ����������ֵ׼ȷ
    }
}