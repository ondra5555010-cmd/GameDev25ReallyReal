using UnityEngine;

public class DungeonMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioClip loop;

    [Header("Volumes")]
    [SerializeField] private float normalVolume = 0.25f;
    [SerializeField] private float pausedVolume = 0.15f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.spatialBlend = 0f; // 2D
        audioSource.loop = false;
        audioSource.volume = normalVolume;
        audioSource.clip = intro;
        audioSource.Play();

        Invoke(nameof(PlayLoop), intro.length);
    }

    void PlayLoop()
    {
        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Volá PauseManager
    public void OnPause()
    {
        audioSource.volume = pausedVolume;
    }

    // Volá PauseManager
    public void OnResume()
    {
        audioSource.volume = normalVolume;
    }
}
