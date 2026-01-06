using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI victoryScoreText;
    public TextMeshProUGUI highScoreText;

    public GameManager gameManager;
    private bool hasUpdatedFinalScore = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        scoreText.text = "0";
        hasUpdatedFinalScore = false;
        highScoreText.text = GetHighScore().ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.gameEnded && !hasUpdatedFinalScore)
        {
            SaveCurrentScore();
            finalScoreText.text = "SCORE: " + GetScore();
            victoryScoreText.text = "SCORE: " + GetScore();
            hasUpdatedFinalScore = true;
        }
    }

    public void AddScore(int value)
    {

        int currentScore = int.Parse(scoreText.text);
        currentScore += value;
        scoreText.text = currentScore.ToString();

    }
    public int GetScore()
    {
        return int.Parse(scoreText.text);
    }

    public void SaveCurrentScore()
    {
        int currentScore = GetScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }
}
