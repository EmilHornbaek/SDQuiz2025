using UnityEngine;
using UnityEngine.UIElements;
using Nullzone.Unity.Attributes;


public class StartScreen : MonoBehaviour
{
    [SerializeField] private StyleSheet style;
    private LerpState lerpSwitch = LerpState.Play;
    [SerializeField, Tooltip("Speeds up or slows down all LerpMotion happening upon pressing play.")] private float speedMultiplier = 1;
    [SerializeField, FieldName("Send Camera To:"), Tooltip("Do not touch. The transform which the camera is sent to upon pressing play.")] private Transform newCameraTarget;

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
        root.styleSheets.Clear();
        if (style is not null) root.styleSheets.Add(style);
    }
}