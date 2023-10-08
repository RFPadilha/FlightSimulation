using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ScoreCounter))]
public class EndGameStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ringsCollected;
    [SerializeField] TextMeshProUGUI finalScore;
    [SerializeField] TextMeshProUGUI finalMultiplier;
    ScoreCounter stats;
    private void Start()
    {
        stats = GetComponent<ScoreCounter>();
    }
    private void OnEnable()
    {
        Timer.endGame += SetEndStats;
    }
    private void OnDisable()
    {
        Timer.endGame -= SetEndStats;
    }
    void SetEndStats()
    {
        ringsCollected.text = "Rings: " + stats.ringCount.ToString();
        finalScore.text = "Final Score: " + stats.score.ToString();
        finalMultiplier.text = "Final Multiplier: x" + stats.multiplier.ToString();
        
    }
}
