using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrapper : MonoBehaviour
{
    [Header("Scenes to load (Must be in Build Settings)")]
    [SerializeField] private string mainSceneName;
    [SerializeField] private string uiSceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync(mainSceneName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(uiSceneName, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
