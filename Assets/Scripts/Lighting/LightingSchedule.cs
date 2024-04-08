using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightingSchedule_", menuName = "Scriptable Objects/Lighting/LightingSchedule")]
public class LightingSchedule : ScriptableObject
{
    public LightingBrightness[] lightingBrightnessArray;//���ﶨ����һ�����飬�����洢����ǿ�ȱ仯��ʱ���ǿ��ֵ��
}


[System.Serializable]
public struct LightingBrightness
{
    public Season season;//���ڡ�
    public int hour;//ʱ�䡣
    public float lightIntensity;//����ǿ��ֵ��
}