using System.Collections;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause; // 用于控制动画暂停的等待时间
    private bool isAnimating = false; // 标记是否正在播放动画

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f); // 初始化等待时间为0.04秒
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimating == false) // 如果没有正在播放动画
        {
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock()); // 播放逆时针旋转动画
            }
            else
            {
                StartCoroutine(RotateClock()); // 播放顺时针旋转动画
            }

            // 如果碰撞对象的标签是"Player"，则播放rustle声音
            //if (collision.gameObject.tag == "Player")
            //{
            //    AudioManager.Instance.PlaySound(SoundName.effectRustle);
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAnimating == false) // 如果没有正在播放动画
        {
            if (gameObject.transform.position.x > collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock()); // 播放逆时针旋转动画
            }
            else
            {
                StartCoroutine(RotateClock()); // 播放顺时针旋转动画
            }

            // 如果碰撞对象的标签是"Player"，则播放rustle声音
            //if (collision.gameObject.tag == "Player")
            //{
            //    AudioManager.Instance.PlaySound(SoundName.effectRustle);
            //}
        }
    }

    private IEnumerator RotateAntiClock()
    {
        isAnimating = true; // 标记为正在播放动画

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f); // 逆时针旋转2度

            yield return pause; // 等待一段时间
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f); // 逆时针旋转-2度

            yield return pause; // 等待一段时间
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f); // 逆时针旋转2度

        yield return pause; // 等待一段时间

        isAnimating = false; // 标记为动画播放结束
    }

    private IEnumerator RotateClock()
    {
        isAnimating = true; // 标记为正在播放动画

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f); // 顺时针旋转-2度

            yield return pause; // 等待一段时间
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f); // 顺时针旋转2度

            yield return pause; // 等待一段时间
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f); // 顺时针旋转-2度

        yield return pause; // 等待一段时间

        isAnimating = false; // 标记为动画播放结束
    }
}