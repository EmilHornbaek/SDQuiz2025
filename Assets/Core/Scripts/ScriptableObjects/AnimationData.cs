using UnityEngine;

[CreateAssetMenu(fileName = "animationData", menuName = "Animatable Object/Mobile Sprite")]
public class AnimationData : ScriptableObject
{
    [Header("Base")]
    public Sprite sprite;

    [Header("Animation Config")]
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    [SerializeField] private float offsetRotation;
    [SerializeField] private float offsetScale;
    [SerializeField] private float duration;

    public enum Trigger
    {
        Play,
        QuizSelect,
        QuizEnd

    }
    public Trigger animationTrigger;
}
