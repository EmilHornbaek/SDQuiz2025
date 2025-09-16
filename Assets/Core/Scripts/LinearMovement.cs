using System;
using UnityEngine;

[Serializable]
public enum MovementType
{
    Linear,
    BiLinear,
    Cubed
}
public class LinearMovement : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private MovementType lerpMethod = MovementType.Linear;
    
    private bool active = false;
    private float timer = 0f;

    private Transform currentDestination;
    private Transform activeDestination;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentDestination = transform;        
    }

    // Update is called once per frame
    void Update()
    {
        if (active && activeDestination is not null)
        {
            transform.position = Vector3.Lerp(currentDestination.position, activeDestination.position, Math.Min(timer / duration, 1f));

            if (timer >= duration)
            {
                currentDestination = activeDestination;
                activeDestination = null;
                active = false;
            }


            timer += Time.deltaTime;
        }
    }

    public void Activate()
    {
        active = true;
    }

    public void GoTo(Transform destination)
    {
        activeDestination = destination;
        active = true;
    }
}
