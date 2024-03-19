using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    // 淡出效果
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    // 淡入效果
    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    // 实现淡入效果的协程
    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1f - currentAlpha;

        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / Settings.fadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    // 实现淡出效果的协程
    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = currentAlpha - Settings.targetAlpha;

        while (currentAlpha - Settings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / Settings.fadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha);
    }
}
