using UnityEngine;
using UnityEngine.UIElements;
using Nullzone.Unity.Attributes;


public class StartScreen : MonoBehaviour
{
    [SerializeField] private LerpState lerpSwitch;
    [SerializeField, FieldName("Send Camera To:")] private Transform newCameraTarget;
    [SerializeField] private float speedMultiplier = 1;

    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += () =>
        {
            LerpHandler lh = LerpHandler.Instance;
            lh.MoveObjects(lerpSwitch, false, newCameraTarget, speedMultiplier);
        };
    }
}