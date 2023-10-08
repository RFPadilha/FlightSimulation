using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public delegate void EndGame();
    public static event EndGame endGame;

    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] float timeRemaining = 10;
    bool timerIsRunning = false;

    void Start()
    {
        timerIsRunning = true;
    }
    
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimer(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                endGame?.Invoke();
            }
        }
    }
    void UpdateTimer(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
