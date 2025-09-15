using UnityEngine;

public enum Trigger
{
    Play,
    QuizSelect,
    QuizEnd
}

public class MobileSprite : MonoBehaviour
{
    public Transform targetTransform;
    [SerializeField] private float duration;
    public Trigger trigger;
    private Vector2 originalPosition;
    //private Quaternion originalQuaternion;   DISABLED
    private float originalRotation;
    private Vector2 originalScale;
    private float elapsedTime;
    private float timeCompletion;
    private bool active;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation.z;
        originalScale = transform.localScale;
        //originalQuaternion = transform.rotation;   DISABLED
        Trigger();
    }
    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            elapsedTime += Time.deltaTime;
            timeCompletion = elapsedTime / duration;
            transform.position = Vector2.Lerp(originalPosition, targetTransform.position, timeCompletion);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(originalRotation, targetTransform.rotation.eulerAngles.z, timeCompletion));
            //transform.rotation = Quaternion.LerpUnclamped(originalQuaternion, targetTransform.rotation, timeCompletion);   DISABLED
            transform.localScale = Vector2.Lerp(originalScale, targetTransform.localScale, timeCompletion);
            if (elapsedTime >= duration)
            {
                elapsedTime = 0;
                active = false;
            }
        }
    }

    private void Trigger(bool inverse = false)
    {
        active = true;
    }
}
