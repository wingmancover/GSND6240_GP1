using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    [Header("Bars")]
    public ProgressBarView milk;
    public ProgressBarView labor;
    public ProgressBarView nutrition;

    [Header("Upgrade condition")]
    [Range(0f, 99f)]
    public float threshold = 90f;

    [Header("One-shot")]
    public bool hasUpgraded = false;

    [Header("Placeholder effects")]
    public GameObject upgradePlaceholderUI;   // turn on a badge/panel/effect later
    public AudioClip upgradeSound;            // upgrade sound

    [Header("Optional hook for later")]
    public UnityEvent onUpgrade;              // plug effects in Inspector later

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (upgradePlaceholderUI != null)
            upgradePlaceholderUI.SetActive(false);
    }

    void Update()
    {
        if (hasUpgraded) return;
        if (milk == null || labor == null || nutrition == null) return;

        if (milk.model.currentValue >= threshold &&
            labor.model.currentValue >= threshold &&
            nutrition.model.currentValue >= threshold)
        {
            TriggerUpgrade();
        }
    }

    private void TriggerUpgrade()
    {
        hasUpgraded = true;

        // Placeholder visual
        if (upgradePlaceholderUI != null)
            upgradePlaceholderUI.SetActive(true);

        // Sound
        if (upgradeSound != null)
        {
            audioSource.Stop();
            audioSource.clip = upgradeSound;
            audioSource.Play();
        }

        // Placeholder hook for later upgrades (swap assets, change UI, etc.)
        onUpgrade?.Invoke();

        Debug.Log($"Upgrade triggered! All bars >= {threshold}");
    }
}
