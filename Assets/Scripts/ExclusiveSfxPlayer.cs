using UnityEngine;

public class ExclusiveSfxPlayer : MonoBehaviour
{
    [Header("Assign clips")]
    public AudioClip milkClip;
    public AudioClip laborClip;
    public AudioClip nutritionClip;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D
    }

    // Call these from button OnClick
    public void PlayMilk() => PlayExclusive(milkClip);
    public void PlayLabor() => PlayExclusive(laborClip);
    public void PlayNutrition() => PlayExclusive(nutritionClip);

    private void PlayExclusive(AudioClip clip)
    {
        if (clip == null) return;

        // Stop whatever is currently playing on this channel
        if (source.isPlaying)
            source.Stop();

        source.clip = clip;
        source.Play();
    }
}