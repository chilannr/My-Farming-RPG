using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    // 定义需要用到的游戏对象和动画类型数组
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;

    // 定义两个字典，用于存储动画类型
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        // 初始化动画类型字典，以动画剪辑为键
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        // 初始化动画类型字典，以字符串为键
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    // 应用角色定制参数
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {
        // 遍历所有角色属性并为每个属性设置动画覆盖控制器
        foreach (CharacterAttribute characterAttribute in characterAttributesList)
        {
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            // 在场景中找到匹配的动画器类型
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            // 获取动画器的当前基础动画
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach (AnimationClip animationClip in animationsList)
            {
                // 在字典中找到动画
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() + characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    // 如果找到了要替换的动画，将其添加到列表中
                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;

                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            // 将动画更新应用到动画覆盖控制器，然后用新的控制器更新动画器
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
