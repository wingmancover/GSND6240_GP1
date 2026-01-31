using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ProgressBarView milk;
    public ProgressBarView labor;
    public ProgressBarView nutrition;

    public GameObject gameOverPanel;
    public IdlePopupController idlePopup;
    public AudioSource bgmSource;
    public UpgradeBadEffects badEffects;

    bool isGameOver = false;

    void Start()
    {
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

        if (badEffects != null)  // Stop upgrade effects
            badEffects.OnGameOver();

        // stop BGM
        if (bgmSource != null)
            bgmSource.Stop();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // freeze time, it's optional
        Time.timeScale = 0f;
    }
}
