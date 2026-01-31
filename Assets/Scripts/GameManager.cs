using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ProgressBarView milk;
    public ProgressBarView labor;
    public ProgressBarView nutrition;

    public GameObject gameOverPanel;
    public IdlePopupController idlePopup;
    public AudioSource bgmSource;
    public UpgradeBadEffects badEffects;

    [Header("Game Over Audio")]
    public AudioSource sfxSource;
    public AudioSource gameOverSource;
    public AudioClip gameOverClip;

    bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        if (milk.model.IsEmpty() && labor.model.IsEmpty() && nutrition.model.IsEmpty())
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;

        // hide popup + stop its logic
        if (idlePopup != null)
            idlePopup.SetGameOver();

        // Stop upgrade effects, also stops coroutines
        if (badEffects != null)
            badEffects.OnGameOver();

        // Stop BGM
        if (bgmSource != null)
            bgmSource.Stop();

        // Stop other SFX
        if (sfxSource != null)
            sfxSource.Stop();

        // Play game over sound
        if (gameOverSource != null && gameOverClip != null)
        {
            gameOverSource.Stop();
            gameOverSource.clip = gameOverClip;
            gameOverSource.loop = false;
            gameOverSource.Play();
        }

        // Show panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Freeze time, and UI still works
        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
