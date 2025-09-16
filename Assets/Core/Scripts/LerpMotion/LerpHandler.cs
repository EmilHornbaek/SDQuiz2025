using UnityEngine;
using System.Collections.Generic;

public enum LerpState
{
    Play,
    QuizSelect,
    QuizEnd
}

public class LerpHandler : MonoBehaviour
{
    private List<LerpMotion> lerpMotionObjects = new List<LerpMotion>();
    private LerpMotion cam;

    void Start()
    {
        // Ensures reference to a camera in the scene.
        // For multiple cameras, serialize the cam field and apply a camera in the inspector window.
        if (cam == null)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("MainCamera"))
            {
                LerpMotion targetCamera = go.GetComponent<LerpMotion>();
                if (targetCamera != null)
                {
                    cam = targetCamera;
                }
            }
        }
        UpdateList();
    }

    /// <summary>
    /// Clears and repopulates the handler's list of lerp objects. Uses object tags to automatically populate the list.
    /// </summary>
    void UpdateList()
    {
        lerpMotionObjects.Clear();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MovingElement"))
        {
            LerpMotion lerpObj = go.GetComponent<LerpMotion>();
            if (lerpObj != null)
            {
                lerpMotionObjects.Add(lerpObj);
            }
        }
    }

    /// <summary>
    /// Loops through and calls the Move function on all lerp objects that match the given lerpState parameter.
    /// </summary>
    /// <param name="lerpState">The target LerpState.</param>
    /// <param name="inverse">If true, all activated lerp objects will run in reverse. Defaults to false.</param>
    /// <param name="cameraTarget">If given, sets a new transform target for the camera. Defaults to null.</param>
    public void MoveObjects(LerpState lerpState, bool inverse = false, Transform cameraTarget = null)
    {
        if (cameraTarget != null) { cam.targetTransform = cameraTarget; }
        cam.Move(inverse);
        foreach (LerpMotion lm in lerpMotionObjects)
        {
            if (lm.lerpCondition == lerpState)
            {
                lm.Move(inverse);
            }
        }
    }
}
