using UnityEngine;
using UnityEngine.EventSystems;

public class IdlePopupController : MonoBehaviour
{
    [Header("Idle Settings")]
    [SerializeField] private float idleSeconds = 3f;

    [Header("References")]
    [SerializeField] private GameObject idlePopup;     // the full-screen panel
    [SerializeField] private GameObject gameOverPanel; // existing game over panel
    [SerializeField] private AudioClip popupSound;
    [SerializeField] private AudioSource bgmSource;

    private float lastActivityTime;
    private bool isGameOver;
    private bool popupsSuppressed = false;
    private bool enableAfterNextActivity = false;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        lastActivityTime = Time.time;

        if (idlePopup != null)
            idlePopup.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        if (popupsSuppressed) return;

        // If popup is visible, allow to dismiss by clicking anywhere
        if (idlePopup != null && idlePopup.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // ignore clicks on UI buttons behind it
                HidePopupAndResetTimer();
            }
            return;
        }

        // If player has been idle for a certain time, show popup
        if (Time.time - lastActivityTime >= idleSeconds)
        {
            ShowPopup();
        }
    }

    // Call this from the 3 bar buttons OnClick
    public void RegisterActivity()
    {
        if (isGameOver) return;

        // If waiting for the first click after effects,
        // re-enable popups and restart the idle timer
        if (enableAfterNextActivity)
        {
            popupsSuppressed = false;
            enableAfterNextActivity = false;
            lastActivityTime = Time.time;

            if (idlePopup != null) idlePopup.SetActive(false);
            return;
        }

        lastActivityTime = Time.time;

        // If popup is up and the player clicks one of the buttons (or call this), close it
        if (idlePopup != null && idlePopup.activeSelf)
        {
            idlePopup.SetActive(false);
        }
    }

    public void SetGameOver()
    {
        isGameOver = true;

        // Hide popup if game ends
        if (idlePopup != null) 
            idlePopup.SetActive(false);

        // Hard stop BGM on game over
        if (bgmSource != null)
            bgmSource.Stop();
    }

    private void ShowPopup()
    {
        if (idlePopup == null) return;

        idlePopup.SetActive(true);

        // Pause BGM
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Pause();

        if (popupSound != null)
        {
            audioSource.Stop();
            audioSource.clip = popupSound;
            audioSource.Play();
        }

        // don’t keep re-triggering every frame while idle
        // reset timer so it won't immediately pop up again after closing unless idle again
        lastActivityTime = Time.time;
    }

    private void HidePopupAndResetTimer()
    {
        if (idlePopup != null)
            idlePopup.SetActive(false);

        // Resume BGM
        if (bgmSource != null)
            bgmSource.UnPause();

        lastActivityTime = Time.time;
    }

    public void SuppressPopupsImmediately()
    {
        // Hide if currently visible and prevent showing again
        // For Upgrade effect purpose
        popupsSuppressed = true;
        enableAfterNextActivity = false;

        if (idlePopup != null)
            idlePopup.SetActive(false);
    }

    public void EnablePopupsAfterNextActivity()
    {
        // Still suppressed, but once the player clicks a bar button,
        // re-enable popups and restart the idle timer
        popupsSuppressed = true;
        enableAfterNextActivity = true;

        if (idlePopup != null)
            idlePopup.SetActive(false);
    }

}
