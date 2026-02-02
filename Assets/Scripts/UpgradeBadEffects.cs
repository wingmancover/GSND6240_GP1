using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBadEffects : MonoBehaviour
{
    [Header("Flash Overlay")]
    public Image flashOverlay;
    public Color flashColor = new Color(0.5f, 0.0f, 0.0f, 0.0f); // dark red
    public float flashInTime = 0.08f;
    public float flashOutTime = 0.6f;
    [Range(0f, 1f)] public float flashMaxAlpha = 0.65f;

    [Header("Camera Shake")]
    public Camera targetCamera;
    public float shakeDuration = 1.5f;
    public float shakeStrength = 0.25f;
    public float shakeFrequency = 35f;

    [Header("UI Shake")]
    public RectTransform uiShakeTarget;

    [Header("BGM Pause")]
    public AudioSource bgmSource;
    public float bgmPauseSeconds = 3f;

    [Header("Cow Visual (choose one)")]
    public Image cowUIImage;
    [Range(0f, 1f)] public float cowDesaturateAmount = 0.6f; // 0 = no change, 1 = gray
    [Range(0f, 1f)] public float cowDarkenAmount = 0.25f;    // 0 = no change, 1 = black

    [Header("Upgrade Visual Swap")]
    public GameObject cowImageToSwap;
    public GameObject cowAfterSprite;
    public GameObject bgBefore;
    public GameObject bgAfter;

    [Header("Popup suppression")]
    public IdlePopupController idlePopupController;


    private Vector3 camOriginalPos;
    private Vector2 uiOriginalAnchoredPos;
    private bool hasApplied = false;

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            camOriginalPos = targetCamera.transform.localPosition;

        if (uiShakeTarget != null)
            uiOriginalAnchoredPos = uiShakeTarget.anchoredPosition;

        if (flashOverlay != null)
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);

        if (bgBefore != null) bgBefore.SetActive(true);
        if (bgAfter != null) bgAfter.SetActive(false);

        if (cowImageToSwap != null) cowImageToSwap.SetActive(true);
        if (cowAfterSprite != null) cowAfterSprite.SetActive(false);
    }

    public void OnGameOver()
    {
        // prevents any later resume attempts
        StopAllCoroutines();
    }

    // Call this only ONCE from UpgradeManager when upgrade triggers
    public void PlayUpgradeBadEffects()
    {
        if (idlePopupController != null)
            idlePopupController.SuppressPopupsImmediately();

        if (hasApplied) return;
        hasApplied = true;

        ApplyUpgradeVisualSwap();

        StartCoroutine(FlashRoutine());
        StartCoroutine(ShakeRoutine());
        StartCoroutine(PauseBgmRoutine());
        ApplyCowLookPermanent();

        StartCoroutine(ReenablePopupsAfterEffectsRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (flashOverlay == null) yield break;

        // flash in
        float t = 0f;
        while (t < flashInTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, flashMaxAlpha, t / flashInTime);
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, a);
            yield return null;
        }

        // flash out, slow
        t = 0f;
        while (t < flashOutTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(flashMaxAlpha, 0f, t / flashOutTime);
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, a);
            yield return null;
        }

        flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }

    private IEnumerator ShakeRoutine()
    {
        float t = 0f;

        // UI shake
        if (uiShakeTarget != null)
        {
            Vector2 startPos = uiOriginalAnchoredPos;

            while (t < shakeDuration)
            {
                t += Time.unscaledDeltaTime; // unscaled so it still works if timescale changes
                float normalized = t / shakeDuration;

                float strength = Mathf.Lerp(shakeStrength, 0f, normalized);

                // Random, more “violent” than Perlin
                float x = Random.Range(-1f, 1f);
                float y = Random.Range(-1f, 1f);

                // shakeStrength is in "UI units" (pixels-ish), tune it
                uiShakeTarget.anchoredPosition = startPos + new Vector2(x, y) * strength;

                yield return null;
            }

            uiShakeTarget.anchoredPosition = startPos;
            yield break;
        }

        // Camera shake fallback, only affects world objects
        if (targetCamera == null) yield break;

        Vector3 camStartPos = camOriginalPos;

        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / shakeDuration;

            float strength = Mathf.Lerp(shakeStrength, 0f, normalized);

            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            targetCamera.transform.localPosition = camStartPos + new Vector3(x, y, 0f) * strength;

            yield return null;
        }

        targetCamera.transform.localPosition = camStartPos;
    }


    private IEnumerator PauseBgmRoutine()
    {
        if (bgmSource == null) yield break;

        // Pause if playing
        bool wasPlaying = bgmSource.isPlaying;
        if (wasPlaying) bgmSource.Pause();

        // Wait in real time so it still works even if you later change timeScale
        yield return new WaitForSecondsRealtime(bgmPauseSeconds);

        // Resume only if it wasn't stopped by Game Over
        // If Game Over stops it, bgm clip may be null or isPlaying false; UnPause is safe either way
        if (wasPlaying)
            bgmSource.UnPause();
    }

    private void ApplyCowLookPermanent()
    {
        // “desaturate” by blending toward gray

        if (cowUIImage != null)
        {
            Color c = cowUIImage.color;
            cowUIImage.color = ApplyDesatDark(c);
        }
        else
        {
            Debug.LogWarning("UpgradeBadEffects: No cow renderer assigned.");
        }
    }

    private Color ApplyDesatDark(Color original)
    {
        // grayscale luminance approximation
        float gray = original.r * 0.299f + original.g * 0.587f + original.b * 0.114f;
        Color grayColor = new Color(gray, gray, gray, original.a);

        // desaturate blend
        Color desat = Color.Lerp(original, grayColor, cowDesaturateAmount);

        // darken blend toward black (preserve alpha)
        Color dark = Color.Lerp(desat, new Color(0f, 0f, 0f, desat.a), cowDarkenAmount);
        dark.a = original.a;
        return dark;
    }

    private IEnumerator ReenablePopupsAfterEffectsRoutine()
    {
        // Total effect time: flash + shake + bgm pause
        float total = Mathf.Max(
            shakeDuration,
            bgmPauseSeconds,
            flashInTime + flashOutTime
        );

        yield return new WaitForSecondsRealtime(total);

        if (idlePopupController != null)
            idlePopupController.EnablePopupsAfterNextActivity();
    }

    private void ApplyUpgradeVisualSwap()
    {
        // Swap cow sprite
        if (cowImageToSwap != null) cowImageToSwap.SetActive(false);
        if (cowAfterSprite != null) cowAfterSprite.SetActive(true);

        // Swap backgrounds
        if (bgBefore != null) bgBefore.SetActive(false);
        if (bgAfter != null) bgAfter.SetActive(true);
    }
}
