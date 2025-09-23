using UnityEngine;


public class UpgradeStage : MonoBehaviour
{
    [SerializeField] private int showCondition;
    private SpriteRenderer sr;

    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (PlayerStats.Instance.TotalPoints < showCondition) { sr.enabled = false; }
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
