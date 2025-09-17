using UnityEngine;
using UnityEngine.UIElements;
using Nullzone.Unity.Attributes;


public class StartScreen : MonoBehaviour
{
    [SerializeField] private LerpState lerpSwitch;
    [SerializeField, FieldName("Send Camera To:")] private Transform newCameraTarget;

    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += () =>
        {
            LerpHandler lh = FindLerpHandler();
            lh.MoveObjects(lerpSwitch, false, newCameraTarget);
        };
    }

    private LerpHandler FindLerpHandler()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("LerpHandler"))
        {
            LerpHandler handler = go.GetComponent<LerpHandler>();
            if (handler != null) { return handler; }
        }
        return null;
    }
}