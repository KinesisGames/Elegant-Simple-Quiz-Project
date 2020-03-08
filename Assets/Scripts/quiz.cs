using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class quiz : MonoBehaviour {
    public _pairs[] pairs;
    int currentRound = 0;

    public Text currentRoundText;
    
    public Text questionBox; 
    public Text[] answerBoxes;

    float countdownTime = 60;
    public Text countdownText;
    bool isPlaying = false;
    float startTime;

    List<int> posTaken = new List<int>();
    Vector3[] possiblePositions = {new Vector3(-250, -200, 0), 
                                   new Vector3( 250, -200, 0), 
                                   new Vector3(-250, -700, 0), 
                                   new Vector3( 250, -700, 0)};

    void Start() {
        currentRound = 0;
    }

    public void Play(int rounds) {
        currentRound += rounds;
        makeQuiz();
        isPlaying = true;
        startTime = Time.time;
    }

    void Update() {
        float countdown = countdownTime - startTime;
        if (isPlaying && (countdown > 0))
            countdownText.text = countdown.ToString("F0");
        else
            countdownText.text = "0";
    }

    void makeQuiz() {
        questionBox.text = pairs[currentRound].question;
        posTaken = new List<int>();

        for (int i = 0; i < answerBoxes.Length; i++) {
            answerBoxes[i].transform.parent.GetComponent<RectTransform>().transform.localPosition = RandomPosition();
            answerBoxes[i].text = pairs[currentRound].answers[i];
        }
    }

    Vector3 RandomPosition() {
        int randomPos = (int)(Random.Range(0, 4));
        while (posTaken.Contains(randomPos))
            randomPos = (int)(Random.Range(0, 4));
        posTaken.Add(randomPos);
        
        return possiblePositions[randomPos];
    }
}

[System.Serializable]
public class _pairs {
    public string question;
    public string[] answers = new string[4];
}
