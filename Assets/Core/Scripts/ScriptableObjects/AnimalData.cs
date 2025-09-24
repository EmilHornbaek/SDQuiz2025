using Nenn.InspectorEnhancements.Runtime.Attributes.Conditional;
using Nenn.InspectorEnhancements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Nenn.InspectorEnhancements.Runtime.Attributes;

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
public class QuizQuestion
{
    public string Question;
    public QuizAnswer[] Answers;
    [NonSerialized]
    public bool guessedCorrectly;
}
[Serializable]
public class QuizAnswer
{
    public string Answer;

    [Tooltip("This is not implemented yet! but if true will allow for images instead of text")]
    public bool UsesImage;
    public Texture2D Image;

    [Tooltip("If this is true, the answer is correct")]
    public bool Allowed;

}
[Serializable]
public struct Fact
{
    public string fact;
}