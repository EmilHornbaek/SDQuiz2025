using Unity.VisualScripting;
using UnityEngine;

public static class TransformExtension
{
    public static TransformSnapshot Snapshot(this Transform transform)
    {
        return new TransformSnapshot
        {
            position = transform.position,
            rotation = transform.rotation,
            localScale = transform.localScale,
        };
    }
}

public struct TransformSnapshot
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
}
