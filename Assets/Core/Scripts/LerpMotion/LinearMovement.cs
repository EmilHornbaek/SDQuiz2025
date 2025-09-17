using System;
using UnityEngine;
using Nullzone.Unity.Attributes;
using Unity.VisualScripting;

[Serializable]
public enum WorldSpaceType
{
    [InspectorName("2D")]
    SecondDimension,
    [InspectorName("3D")]
    ThirdDimension
}

public class LinearMovement : MonoBehaviour
{
    private const int WEIRD_MAGIC_DURATION_MULTIPLIER = 5;
    [SerializeField, FieldName("Duration (seconds)")] private float duration;
    [SerializeField] private MovementType lerpMethod = MovementType.Linear;
    [SerializeField] private WorldSpaceType worldSpaceType;
    
    private bool active = false;
    private float timer = 0f;

    private TransformSnapshot currentDestination;
    private TransformSnapshot activeDestination;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentDestination = transform.Snapshot();
    }

    // Update is called once per frame
    void Update()
    {
        if (active && !activeDestination.Equals(default))
        {
            timer += Time.deltaTime;

            float t = HandleInterpolation(timer / duration);
            HandleTransform(t);


            if (timer >= duration)
            {
                currentDestination = activeDestination;
                activeDestination = default;
                timer = 0;
                active = false;
            }


        }
    }

    private void HandleTransform(float t)
    {
        switch (worldSpaceType)
        {
            case WorldSpaceType.SecondDimension:
                Vector3 activeVector = activeDestination.position;
                transform.position = Vector3.Lerp(currentDestination.position, new Vector3(activeVector.x, activeVector.y, currentDestination.position.z), t);
                break;
            case WorldSpaceType.ThirdDimension:
                transform.position = Vector3.Lerp(currentDestination.position, activeDestination.position, t);
                break;
            default:
                break;
        }
    }

    private float HandleInterpolation(float v)
    {
        switch (lerpMethod)
        {
            case MovementType.Cubic:
                v = Cubic(v);
                break;
        }
        return Mathf.Clamp01(v);
    }

    public void Activate()
    {
        active = true;
    }

    public void GoTo(Transform destination)
    {
        activeDestination = destination.Snapshot();
        active = true;
    }

    private float Cubic(float t)
    {
        return t < .5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
