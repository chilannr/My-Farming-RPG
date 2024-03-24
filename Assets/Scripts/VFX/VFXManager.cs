using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    // 等待两秒的时间间隔
    private WaitForSeconds twoSeconds;

    // 不同特效的预制体
    [SerializeField] private GameObject deciduousLeavesFallingPrefab = null;
    [SerializeField] private GameObject pineConesFallingPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;
    [SerializeField] private GameObject breakingStonePrefab = null;
    [SerializeField] private GameObject reapingPrefab = null;

    protected override void Awake()
    {
        base.Awake();

        // 初始化等待两秒的时间间隔
        twoSeconds = new WaitForSeconds(2f);
    }

    private void OnDisable()
    {
        // 取消订阅收获行为特效事件
        EventHandler.HarvestActionEffectEvent -= displayHarvestActionEffect;
    }

    private void OnEnable()
    {
        // 订阅收获行为特效事件
        EventHandler.HarvestActionEffectEvent += displayHarvestActionEffect;
    }

    // 禁用收获行为特效的协程
    private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    // 显示收获行为特效
    private void displayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.deciduousLeavesFalling:
                // 重用预制体并显示特效
                GameObject deciduousLeaveFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                deciduousLeaveFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeaveFalling, twoSeconds));
                break;

            case HarvestActionEffect.pineConesFalling:
                // 重用预制体并显示特效
                GameObject pineConesFalling = PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);
                pineConesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, twoSeconds));
                break;

            case HarvestActionEffect.choppingTreeTrunk:
                // 重用预制体并显示特效
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));
                break;

            case HarvestActionEffect.breakingStone:
                // 重用预制体并显示特效
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePrefab, effectPosition, Quaternion.identity);
                breakingStone.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));
                break;

            case HarvestActionEffect.reaping:
                // 重用预制体并显示特效
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;

            case HarvestActionEffect.none:
                // 无特效
                break;

            default:
                break;
        }
    }
}