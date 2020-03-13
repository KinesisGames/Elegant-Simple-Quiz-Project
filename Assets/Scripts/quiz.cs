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

    public Text scoreText;
    public Text highScoreText;

    int score;

    float countdown;
    public float countdownTime = 10;
    float startTime;

    public Slider countdownSlider;
    bool isPlaying = false;

    public Transform nextScreen;
    int state = 0;

    Color32 baseImageColor;

    List<int> posTaken = new List<int>();
    Vector3[] possiblePositions = {new Vector3(-250, -200, 0), 
                                   new Vector3( 250, -200, 0), 
                                   new Vector3(-250, -700, 0), 
                                   new Vector3( 250, -700, 0)};

    public void Start() {
        currentRound = 0;
        baseImageColor = answerBoxes[0].transform.parent.GetComponent<Image>().color;
    }

    public void PlayRound(int rounds) {
        currentRound += rounds;
        makeQuiz();
        Reset();
        Time.timeScale = 1.0f;
        isPlaying = true;
        startTime = Time.time;
    }

    void Update() {
        if (isPlaying) {
            countdown = countdownTime - (Time.time - startTime);
            countdownSlider.value = countdown / countdownTime;

            if (countdown < 0) {
                CheckAnswer(-1);
                Debug.Log("Over");
            }
        } else {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (score > highScore)
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
            highScoreText.text = "High Score: " + highScore.ToString();
        }

        currentRoundText.text = "Round " + (currentRound + 1).ToString();

        if (score > 0)
            scoreText.text = "Score: " + score.ToString("F0");
        else
            scoreText.text = "Score: 0";
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
        if (answerIndex <= 0) {
            questionBox.gameObject.SetActive(false);
            countdownSlider.gameObject.SetActive(false);
            nextScreen.gameObject.SetActive(true);    
            isPlaying = false;
            Time.timeScale = 0.0f;
        }

        if (answerIndex < 0) { // Countdown Over 
            nextScreen.GetChild(0).GetComponent<Text>().text = "You Lost!";
            nextScreen.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "Try Again";
            nextScreen.GetChild(0).GetComponent<Text>().color = new Color32(255, 92, 84, 255);
            answerBoxes[0].transform.parent.GetChild(0).gameObject.SetActive(true);
            state = 0;
        } else if (answerIndex == 0) { // Correct Answer
            nextScreen.GetChild(0).GetComponent<Text>().text = "You Won!";
            nextScreen.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "Continue";
            nextScreen.GetChild(0).GetComponent<Text>().color = new Color32(92, 255, 84, 255);
            score += 20;
            state = 1;

            answerBoxes[answerIndex].transform.parent.GetChild(0).gameObject.SetActive(true);

            if (currentRound >= pairs.Length - 1) {
                nextScreen.GetChild(1).gameObject.SetActive(false);
                nextScreen.GetChild(2).GetComponent<RectTransform>().transform.localPosition = 
                nextScreen.GetChild(1).GetComponent<RectTransform>().transform.localPosition;
            } else {
                nextScreen.GetChild(1).gameObject.SetActive(true);
                nextScreen.GetChild(2).GetComponent<RectTransform>().transform.localPosition = 
                new Vector3(0, 100, 0);
            }
        } else { // Wrong Answer
            answerBoxes[answerIndex].transform.parent.GetComponent<Button>().enabled = false;
            baseImageColor.a = 150;
            answerBoxes[answerIndex].transform.parent.GetComponent<Image>().color = baseImageColor;
            answerBoxes[answerIndex].transform.parent.GetChild(0).gameObject.SetActive(true);
            startTime -= (countdownTime / 3);
            score -= 4;
        }
    }

    void Reset() {
        questionBox.gameObject.SetActive(true);
        countdownSlider.gameObject.SetActive(true);
        nextScreen.gameObject.SetActive(false);

        foreach (Text answerBox in answerBoxes) {
            answerBox.transform.parent.GetComponent<Button>().enabled = true;
            baseImageColor.a = 255;
            answerBox.transform.parent.GetComponent<Image>().color = baseImageColor;
            answerBox.transform.parent.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void NextStage() {
        if (state == 0)
            currentRound = 0;

        PlayRound(state);
    }

    public void Exit() {
        Application.Quit();
    }
}
