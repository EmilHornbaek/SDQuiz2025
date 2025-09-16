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
            LinearMovement lm = Camera.main.GetComponent<LinearMovement>();
            lm.GoTo(StartButtonDestination);
        };
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
