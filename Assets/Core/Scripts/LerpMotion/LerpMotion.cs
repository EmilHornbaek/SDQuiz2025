using System;
using UnityEngine;

[Serializable]
public enum MovementType
{
    Linear,
    Cubic
}

public class LerpMotion : MonoBehaviour
{
    public Transform targetTransform;
    [SerializeField] private bool lockZPosition;
    [SerializeField] private float duration;
    [SerializeField] private MovementType lerpMethod = MovementType.Linear;
    public LerpState lerpCondition;
    private TransformSnapshot originalTransform;
    private float elapsedTime;
    private float timeCompletion;
    private bool active;
    private bool inverse;

    void Start()
    {
        // Sets the original transforms of the object.
        originalTransform = transform.Snapshot();
    }

    void Update()
    {
        if (active)
        {
            elapsedTime += Time.deltaTime;
            if (lerpMethod == MovementType.Linear) { timeCompletion = elapsedTime / duration; }
            else { timeCompletion = Cubic(elapsedTime / duration); }
            MoveUpdate();
        }
    }

    /// <summary>
    /// Starts the lerp motion if the object has a valid target transform.
    /// </summary>
    /// <param name="inverse">If true, runs the lerp motion in reverse. Defaults to false.</param>
    public void Move(bool inverse = false)
    {
        if (targetTransform != null)
        {
            active = true;
            this.inverse = inverse;
        }
    }

    /// <summary>
    /// Handles the lerp movement logic and moves the object over time.
    /// </summary>
    private void MoveUpdate()
    {
        Vector3 targetPos;
        if (lockZPosition) { targetPos = new Vector3(targetTransform.position.x, targetTransform.position.y, originalTransform.position.z); }
        else { targetPos = targetTransform.position; }

        if (!inverse)
        {
            transform.position = Vector3.Lerp(originalTransform.position, targetPos, timeCompletion);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(originalTransform.rotation.z, targetTransform.rotation.eulerAngles.z, timeCompletion));
            transform.localScale = Vector2.Lerp(originalTransform.localScale, targetTransform.localScale, timeCompletion);
        }
        else if (inverse)
        {
            transform.position = Vector3.Lerp(targetPos, originalTransform.position, timeCompletion);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(targetTransform.rotation.eulerAngles.z, originalTransform.rotation.z, timeCompletion));
            transform.localScale = Vector2.Lerp(targetTransform.localScale, originalTransform.localScale, timeCompletion);
        }
        if (elapsedTime >= duration)
        {
            elapsedTime = 0;
            active = false;
            inverse = false;
        }
    }

    /// <summary>
    /// Replaces the object's target transform with the given target, and sets the object's current transform to the origin.
    /// </summary>
    /// <param name="transform">The transform to replace the target transform with.</param>
    public void OverrideTarget(Transform transform)
    {
        originalTransform = transform.Snapshot();
        targetTransform = transform;
    }

    private float Cubic(float t)
    {
        return t < .5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
