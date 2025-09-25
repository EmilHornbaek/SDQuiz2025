using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrapper : MonoBehaviour
{
    [Header("Scenes to load (Must be in Build Settings)")]
    [SerializeField] private SceneReference mainScene;
    [SerializeField] private SceneReference uiScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync(mainScene.SceneName, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(uiScene.SceneName, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }

    private void Awake()
    {
        Screen.fullScreen = true;
    }
}
