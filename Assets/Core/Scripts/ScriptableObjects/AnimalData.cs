using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "animalData", menuName = "Animals/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("Base")]
    public string Name;
    public Texture2D Sprite;
    public AudioClip Sound;

    [Header("Information")]
    [SerializeField] public Fact[] Facts;

    [SerializeField] public QuizQuestion[] QuizQuestions;

}

[Serializable]
public struct QuizQuestion
{
    public string Question;
    public QuizAnswer[] Answers;
}
[Serializable]
public struct QuizAnswer
{
    public string Answer;
    public bool Allowed;
}
[Serializable]
public struct Fact
{
    public string fact;
}