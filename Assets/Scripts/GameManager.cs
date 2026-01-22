using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ProgressBarView milk;
    public ProgressBarView labor;
    public ProgressBarView nutrition;

    public GameObject gameOverPanel;

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

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // freeze time, it's optional
        Time.timeScale = 0f;
    }
}
