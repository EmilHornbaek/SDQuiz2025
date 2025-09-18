using UnityEngine;

public class UpdateProbe : MonoBehaviour
{
    void OnEnable() { Debug.Log("[Probe] OnEnable"); }
    void Start() { Debug.Log("[Probe] Start"); }
    void Update()
    {
        if ((Time.frameCount & 0x1F) == 0)
            Debug.Log($"[Probe] Update tick {Time.frameCount}");
    }

}
