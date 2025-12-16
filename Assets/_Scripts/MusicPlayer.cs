using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip intro;
    public AudioClip loop;

    void Start()
    {
        audioSource.clip = intro;
        audioSource.loop = false;
        audioSource.Play();
        Invoke(nameof(PlayLoop), intro.length);
    }

    void PlayLoop()
    {
        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
