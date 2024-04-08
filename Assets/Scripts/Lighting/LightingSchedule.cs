using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightingSchedule_", menuName = "Scriptable Objects/Lighting/LightingSchedule")]
public class LightingSchedule : ScriptableObject
{
    public LightingBrightness[] lightingBrightnessArray;//这里定义了一个数组，用来存储光照强度变化的时间和强度值。
}


[System.Serializable]
public struct LightingBrightness
{
    public Season season;//季节。
    public int hour;//时间。
    public float lightIntensity;//光照强度值。
}