using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int value)
    {
        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text);
            currentScore += value;
            scoreText.text = currentScore.ToString();
        }
    }
}
