using UnityEngine;

public class LerpMotion : MonoBehaviour
{
    public Transform targetTransform;
    [SerializeField] private float duration;
    public LerpState lerpCondition;
    private Vector3 originalPosition;
    private float originalRotation;
    private Vector2 originalScale;
    private float elapsedTime;
    private float timeCompletion;
    private bool active;
    private bool inverse;

    void Start()
    {
        // Sets the original transforms of the object.
        originalPosition = transform.position;
        originalRotation = transform.rotation.z;
        originalScale = transform.localScale;
    }

    void Update()
    {
        MoveUpdate();
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
        if (active)
        {
            elapsedTime += Time.deltaTime;
            timeCompletion = elapsedTime / duration;
            if (!inverse)
            {
                transform.position = Vector3.Lerp(originalPosition, targetTransform.position, timeCompletion);
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(originalRotation, targetTransform.rotation.eulerAngles.z, timeCompletion));
                transform.localScale = Vector2.Lerp(originalScale, targetTransform.localScale, timeCompletion);
            }
            else if (inverse)
            {
                transform.position = Vector3.Lerp(targetTransform.position, originalPosition, timeCompletion);
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(targetTransform.rotation.eulerAngles.z, originalRotation, timeCompletion));
                transform.localScale = Vector2.Lerp(targetTransform.localScale, originalScale, timeCompletion);
            }
            if (elapsedTime >= duration)
            {
                elapsedTime = 0;
                active = false;
                inverse = false;
            }
        }
    }

    /// <summary>
    /// Replaces the object's target transform with the given target, and sets the object's current transform to the origin.
    /// </summary>
    /// <param name="transform">The transform to replace the target transform with.</param>
    public void OverrideTarget(Transform transform)
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation.z;
        originalScale = transform.localScale;

        targetTransform = transform;
    }
}
