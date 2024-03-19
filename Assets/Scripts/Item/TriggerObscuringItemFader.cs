using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 当进入触发器的物体与当前物体发生碰撞时，获取碰撞物体上及其子物体上的所有 ObscuringItemFader 组件，并触发淡出效果

        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 当离开触发器的物体与当前物体发生碰撞时，获取碰撞物体上及其子物体上的所有 ObscuringItemFader 组件，并触发淡入效果

        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeIn();
            }
        }

    }
}