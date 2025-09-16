using UnityEngine;
using UnityEngine.UI;

public class LerpMotionButton : MonoBehaviour
{
    private LerpHandler lerpHandler;
    [SerializeField] private LerpState lerpSwitch;
    [SerializeField] private bool inverse;
    [SerializeField] private Transform newCameraTarget;
    void Start()
    {
        // Subscribes the button to the on-click event.
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        // Finds the lerp handler in the scene and makes a reference to it.
        if (lerpHandler == null)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("LerpHandler"))
            {
                LerpHandler handler = go.GetComponent<LerpHandler>();
                if (handler != null)
                {
                    lerpHandler = handler;
                }
            }
        }
    }

    /// <summary>
    /// Calls MoveObjects in the lerp handler when clicked.
    /// </summary>
    private void OnClick()
    {
        lerpHandler.MoveObjects(lerpSwitch, inverse, newCameraTarget);
    }
}
