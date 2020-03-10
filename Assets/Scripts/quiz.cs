using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class quiz : MonoBehaviour {
    public QuizPairs[] pairs;
    int currentRound = 0;

    public Text currentRoundText;
    
    public Text questionBox; 
    public Text[] answerBoxes;

    float countdown;
    float countdownTime = 60;
    float startTime;

    public Text countdownText;
    bool isPlaying = false;

    public Transform nextScreen;
    int state = 0;

    List<int> posTaken = new List<int>();
    Vector3[] possiblePositions = {new Vector3(-250, -200, 0), 
                                   new Vector3( 250, -200, 0), 
                                   new Vector3(-250, -700, 0), 
                                   new Vector3( 250, -700, 0)};

    public void Start() {
        currentRound = 0;
    }

    public void PlayRound(int rounds) {
        currentRound += rounds;
        makeQuiz();
        Time.timeScale = 1.0f;
        isPlaying = true;
        startTime = Time.time;
    }

    void Update() {
        if (isPlaying) {
            countdown = countdownTime - (Time.time - startTime);
            if (countdown > 0)
                countdownText.text = countdown.ToString("F0");
            else {
                countdownText.text = "0";
                CheckAnswer(-1);
            }
        }

        currentRoundText.text = "Round " + (currentRound + 1).ToString();
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

    public void CheckAnswer(int answerIndex) {
        questionBox.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        nextScreen.gameObject.SetActive(true);
        
        isPlaying = false;
        Time.timeScale = 0.0f;

        if (answerIndex != 0) {
            nextScreen.GetChild(0).GetComponent<Text>().text = "You Lost!";
            nextScreen.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "Try Again";
            nextScreen.GetComponent<Image>().color = new Color32(150, 40, 35, 180);
            state = 0;
        } else {
            nextScreen.GetChild(0).GetComponent<Text>().text = "You Won!";
            nextScreen.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "Continue";
            nextScreen.GetComponent<Image>().color = new Color32(40, 150, 35, 180);
            state = 1;

            if (currentRound >= pairs.Length - 1) {
                nextScreen.GetChild(1).gameObject.SetActive(false);
                nextScreen.GetChild(2).GetComponent<RectTransform>().transform.localPosition = 
                nextScreen.GetChild(1).GetComponent<RectTransform>().transform.localPosition;
            } else {
                nextScreen.GetChild(1).gameObject.SetActive(true);
                nextScreen.GetChild(2).GetComponent<RectTransform>().transform.localPosition = 
                new Vector3(0, 100, 0);
            }
        }
    }

    public void NextStage() {
        PlayRound(state);

        questionBox.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);
        nextScreen.gameObject.SetActive(false);
    }

    public void Exit() {
        Application.Quit();
    }
}
