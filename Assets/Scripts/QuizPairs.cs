using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Quiz Pairs")]
public class QuizPairs : ScriptableObject {
    public string question;
    public string[] answers = new string[4];
}
