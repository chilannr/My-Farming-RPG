using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;  // �������ʱ��
    [SerializeField] private CanvasGroup faderCanvasGroup = null;  // ���ڽ���Ч����CanvasGroup���
    [SerializeField] private Image faderImage = null;  // ����Ч���ı���ͼƬ
    public SceneName startingSceneName;  // ��ʼ��������

    // ����ʵ�ֽ���Ч����Э�̺���
    private IEnumerator Fade(float finalAlpha)
    {
        // �������־����Ϊtrue���Է�ֹFadeAndSwitchScenesЭ�̱��ٴε���
        isFading = true;

        // ȷ��CanvasGroup�����ֹ���߽��볡�����Է�ֹ���ܸ��������
        faderCanvasGroup.blocksRaycasts = true;

        // ���ݵ�ǰalphaֵ��Ŀ��alphaֵ�ͽ������ʱ�����CanvasGroup�Ľ����ٶ�
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // ��CanvasGroup��alphaֵ��δ�ﵽĿ��alphaֵʱ...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // ...��alphaֵ����Ŀ��alphaֵ�ƶ�
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // �ȴ�һ֡Ȼ�����
            yield return null;
        }

        // ������ɺ󽫽����־����Ϊfalse
        isFading = false;

        // ֹͣCanvasGroup��ֹ���ߣ��Ա㲻�ٺ�������
        faderCanvasGroup.blocksRaycasts = false;
    }

    // ����ʵ�ֳ����л��͹���Ч����Э�̺���
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // ���ó���ж��ǰ�Ľ��䵭���¼�
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // ��ʼ�������������ȴ��������
        yield return StartCoroutine(Fade(1f));

        // �洢��������
        SaveLoadManager.Instance.StoreCurrentSceneData();

        // �������λ��
        Player.Instance.gameObject.transform.position = spawnPosition;

        // ���ó���ж��ǰ���¼�
        EventHandler.CallBeforeSceneUnloadEvent();

        // ж�ص�ǰ�����
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // ��ʼ����ָ���������ȴ��������
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // ���ó������غ���¼�
        EventHandler.CallAfterSceneLoadEvent();

        // �ָ��³���������
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        // ��ʼ���䵭�벢�ȴ��������
        yield return StartCoroutine(Fade(0f));

        // ���ó������غ�Ľ��䵭���¼�
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    // ���ڼ��س���������Ϊ�������Э�̺���
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // ������������ڶ�֡�м��أ���������ӵ��Ѽ��صĳ����У���ʱ������Persistent������
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // �ҵ�������صĳ������Ѽ��س����е����һ��������
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // �����¼��صĳ�������Ϊ����������Ϊ��һ��Ҫж�صĳ�����
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    // ��ʼ��������������Э�̺���
    private IEnumerator Start()
    {
        // ����ʼalphaֵ����Ϊ����
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // ��ʼ���ص�һ���������ȴ��������
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // ������¼��ж����ߣ�����ø��¼�
        EventHandler.CallAfterSceneLoadEvent();
        
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        // ����������ɺ�ʼ���䵭��
        StartCoroutine(Fade(0f));
    }

    // ��������Ŀ���������ֽ�����ϵ��Ӱ�����Ҫ�ⲿ�ӿ�
    // �������Ҫ�л�����ʱ�������ô˺���
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        // ���û�н��н��䣬��ʼ���䲢�л�����
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}