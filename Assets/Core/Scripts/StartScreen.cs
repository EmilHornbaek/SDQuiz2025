using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private Transform StartButtonDestination;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root =  document.rootVisualElement;
        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += () => { 
            Camera.main.transform.position = new Vector3(StartButtonDestination.position.x,StartButtonDestination.position.y, -10); 
        };
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
