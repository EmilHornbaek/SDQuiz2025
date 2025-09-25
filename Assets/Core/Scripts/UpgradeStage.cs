using UnityEngine;


public class UpgradeStage : MonoBehaviour
{
    [SerializeField, Tooltip("This object will be shown once the player's total score is equal to or greater than this value.")] private int showCondition;
    private SpriteRenderer sr;

    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (PlayerStats.Instance.TotalPoints < showCondition) { sr.enabled = false; }
        PlayerStats.Instance.OnPointChange += OnPointChange;
    }

    private void OnPointChange(object sender, PlayerStats.PointChangeArgs args)
    {
        if (args.NewAmount >= showCondition)
        {
            sr.enabled = true;
        }
        else { sr.enabled = false; }
    }
}
