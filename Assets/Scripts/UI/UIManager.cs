using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI Objects")]
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI multiplierText;

    [SerializeField] TextMeshProUGUI speed;
    [SerializeField] TextMeshProUGUI throttle;
    [SerializeField] TextMeshProUGUI altitude;
    [SerializeField] float timeRemaining = 10;

    [SerializeField] PlayerControls player;

    [Header("EndGame UI Objects")]
    [SerializeField] GameObject fadeScreen;
    [SerializeField] GameObject endGameStats;

    [SerializeField] TextMeshProUGUI ringsCollected;
    [SerializeField] TextMeshProUGUI finalScore;
    [SerializeField] TextMeshProUGUI finalMultiplier;

    [SerializeField] TextMeshProUGUI deathsByHighG;
    [SerializeField] TextMeshProUGUI deathsByLowG;

    [SerializeField] float fadeDuration = 1f;

    bool timerIsRunning = false;

    float prevVelocity = 0f;
    float prevThrottle = 0f;
    float prevAltitude = 0f;

    int score = 0;
    int ringCount = 0;
    int multiplier = 1;



    private void OnEnable()
    {
        RingScript.increaseScore += IncreaseScore;
    }
    private void OnDisable()
    {
        RingScript.increaseScore -= IncreaseScore;
    }



    void Start()
    {
        speed.text = "0km/h";
        throttle.text = "0%";
        timerIsRunning = true;
        scoreText.text = score.ToString();
        multiplierText.text = "x" + multiplier.ToString();
        altitude.text = "0m";
        endGameStats.SetActive(false);
    }

    void Update()
    {
        UpdatePlayerInfo();

        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimer(timeRemaining);
            }
            else
            {
                FadeOut();
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    void UpdatePlayerInfo()
    {
        if(player.planeBody.velocity.magnitude != prevVelocity)
        {
            speed.text = (Mathf.Abs(player.planeBody.velocity.magnitude) * 3.6f).ToString("F0") + "km/h";
            prevVelocity = Mathf.Abs(player.planeBody.velocity.magnitude);
        }
        if (player.throttle != prevThrottle)
        {
            throttle.text = player.throttle.ToString("F0") + "%";
            prevThrottle = player.throttle;
        }
        if(player.transform.position.y != prevAltitude)
        {
            altitude.text = player.transform.position.y.ToString("F0") + "m";
            prevAltitude = player.transform.position.y;
        }
    }

    void UpdateTimer(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    void UpdateUI()
    {
        scoreText.text = score.ToString();
        multiplierText.text = "x" + multiplier.ToString();
    }

    void IncreaseScore()
    {
        score += 1*multiplier;
        ringCount += 1;
        if(ringCount%3==0 && ringCount > 0)
        {
            multiplier += 1;
        }
        UpdateUI();
    }

    void EndGame()
    {
        ringsCollected.text = "Rings: " + ringCount.ToString();
        finalScore.text = "Final Score: " + scoreText.text;
        finalMultiplier.text = "Final Multiplier: " + multiplierText.text;
        endGameStats.SetActive(true);
        Time.timeScale = 0;
    }
    void FadeOut()
    {
        StartCoroutine(Fade());
    }
    private IEnumerator Fade()
    {
        Image rend = fadeScreen.transform.GetComponent<Image>();
        Color initialColor = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
        Color targetColor = new Color(rend.color.r, rend.color.g, rend.color.b, 1f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            rend.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }
        EndGame();
    }
    public void PlayAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Debug.Log("Application Quit.");
        Application.Quit();
    }
}
