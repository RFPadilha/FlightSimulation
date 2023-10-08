using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI multiplierText;

    public int score { get; private set; } = 0;
    public int ringCount { get; private set; } = 0;
    public int multiplier { get; private set; } = 1;

    private void Start()
    {
        scoreText.text = "0";
        multiplierText.text = "x1";
    }
    private void OnEnable()
    {
        RingScript.increaseScore += IncreaseScore;
    }
    private void OnDisable()
    {
        RingScript.increaseScore -= IncreaseScore;
    }

    void IncreaseScore()
    {
        score += 1 * multiplier;
        ringCount += 1;
        if (ringCount % 3 == 0 && ringCount > 0)
        {
            multiplier += 1;
        }
        scoreText.text = score.ToString();
        multiplierText.text = "x" + multiplier.ToString();
    }
    
}
