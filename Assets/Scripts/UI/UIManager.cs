using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialScreen;
    [SerializeField] GameObject fadeScreen;
    [SerializeField] GameObject endGameStats;

    [SerializeField] Button preSelectEndButton;

    [SerializeField] float fadeDuration = 1f;

    Camera cam;

    private void OnEnable()
    {
        Timer.endGame += FadeOut;
    }
    private void OnDisable()
    {
        Timer.endGame -= FadeOut;
    }

    void Start()
    {
        Time.timeScale = 0f;
        cam = Camera.main;
        endGameStats.SetActive(false);
    }


    void EndGame()
    {
        endGameStats.SetActive(true);
        preSelectEndButton.Select();
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

    public void CloseTutorial()
    {
        GetComponent<PlayerHUD>().player.SwitchToPlayerMap();
        tutorialScreen.SetActive(false);
        Time.timeScale = 1f;
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
