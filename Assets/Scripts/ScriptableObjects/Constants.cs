using UnityEngine;

[CreateAssetMenu(fileName ="ConstantsSO", menuName = "SO/ConstantsSO")]
public class Constants : ScriptableObject
{
    public float blockBreakTime;
    public float treasureInteractionTime;
    [Range(0f, 1f)] public float interactSaturationPoint;
    public float initialInteractDivider, saturatedInteractDivider;
    public float dividerChangeTime;

    public float Initial_TR_SliderWidth, target_TR_SliderWidth;

    public float blocktitleRevealTime;
}